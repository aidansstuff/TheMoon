using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Unity.Baselib.LowLevel;
using Unity.Collections;
using Unity.Jobs;
using Unity.Services.Qos.Runner;
using UnityEngine;

namespace Unity.Networking.QoS
{
	internal struct QosJob : IQosJob, IJob
	{
		private struct InternalQosServer
		{
			public readonly NetworkEndPoint RemoteEndpoint;

			public readonly DateTime BackoffUntilUtc;

			public readonly int Idx;

			private int m_FirstIdx;

			private ushort m_RequestIdentifier;

			public int FirstIdx
			{
				get
				{
					return m_FirstIdx;
				}
				set
				{
					m_FirstIdx = value;
				}
			}

			public ushort RequestIdentifier
			{
				get
				{
					return m_RequestIdentifier;
				}
				set
				{
					m_RequestIdentifier = value;
				}
			}

			public bool Duplicate => m_FirstIdx != Idx;

			public string Address => RemoteEndpoint.Address;

			public InternalQosServer(NetworkEndPoint remote, DateTime backoffUntilUtc, int idx)
			{
				RemoteEndpoint = remote;
				BackoffUntilUtc = backoffUntilUtc;
				Idx = idx;
				m_FirstIdx = idx;
				m_RequestIdentifier = 0;
			}
		}

		private uint RequestsPerEndpoint;

		private ulong TimeoutMs;

		private ulong MaxWaitMs;

		private uint RequestsBetweenPause;

		private uint RequestPauseMs;

		private uint ReceiveWaitMs;

		private NativeArray<InternalQosResult> _qosResults;

		[DeallocateOnJobCompletion]
		private NativeArray<InternalQosServer> m_QosServers;

		[DeallocateOnJobCompletion]
		private NativeArray<byte> m_TitleBytesUtf8;

		private NativeHashMap<FixedString64Bytes, int> m_AddressIndexes;

		private DateTime m_JobExpireTimeUtc;

		private int m_Requests;

		private int m_Responses;

		public NativeArray<InternalQosResult> QosResults => _qosResults;

		public JobHandle Schedule<T>(JobHandle dependsOn = default(JobHandle)) where T : struct, IJob
		{
			return IJobExtensions.Schedule(this, dependsOn);
		}

		internal QosJob(IList<UcgQosServer> qosServers, string title, uint requestsPerEndpoint = 5u, ulong timeoutMs = 10000uL, ulong maxWaitMs = 500uL, uint requestsBetweenPause = 10u, uint requestPauseMs = 1u, uint receiveWaitMs = 10u)
		{
			this = default(QosJob);
			RequestsPerEndpoint = requestsPerEndpoint;
			TimeoutMs = timeoutMs;
			MaxWaitMs = maxWaitMs;
			RequestsBetweenPause = requestsBetweenPause;
			RequestPauseMs = requestPauseMs;
			ReceiveWaitMs = receiveWaitMs;
			m_AddressIndexes = new NativeHashMap<FixedString64Bytes, int>(qosServers?.Count ?? 0, Allocator.Persistent);
			m_QosServers = new NativeArray<InternalQosServer>(qosServers?.Count ?? 0, Allocator.Persistent);
			if (qosServers != null)
			{
				int num = 0;
				foreach (UcgQosServer qosServer in qosServers)
				{
					if (!NetworkEndPoint.TryParse(qosServer.ipv4, qosServer.port, out var endpoint))
					{
						Debug.LogError("QosJob: Invalid IP address " + qosServer.ipv4 + " in QoS Servers list");
						continue;
					}
					InternalQosServer server = new InternalQosServer(endpoint, qosServer.BackoffUntilUtc, num);
					if (m_AddressIndexes.ContainsKey(server.Address))
					{
						server.FirstIdx = m_AddressIndexes[server.Address];
					}
					else
					{
						m_AddressIndexes.Add(server.Address, num);
					}
					StoreServer(server);
					num++;
				}
				if (num < m_QosServers.Length)
				{
					NativeArray<InternalQosServer> nativeArray = new NativeArray<InternalQosServer>(num, Allocator.Persistent);
					m_QosServers.GetSubArray(0, nativeArray.Length).CopyTo(nativeArray);
					m_QosServers.Dispose();
					m_QosServers = nativeArray;
				}
			}
			_qosResults = new NativeArray<InternalQosResult>(m_QosServers.Length, Allocator.Persistent);
			byte[] bytes = Encoding.UTF8.GetBytes(title);
			m_TitleBytesUtf8 = new NativeArray<byte>(bytes.Length, Allocator.Persistent);
			m_TitleBytesUtf8.CopyFrom(bytes);
		}

		public void Dispose()
		{
			if (m_AddressIndexes.IsCreated)
			{
				m_AddressIndexes.Dispose();
			}
		}

		public void Execute()
		{
			if (m_QosServers.Length != 0)
			{
				m_Requests = 0;
				m_Responses = 0;
				m_JobExpireTimeUtc = DateTime.UtcNow.AddMilliseconds(TimeoutMs);
				var (baselib_Socket_Handle, baselib_ErrorCode) = CreateAndBindSocket();
				if (baselib_ErrorCode != 0)
				{
					Debug.LogError($"QosJob: failed to create and bind the local socket (errorcode {baselib_ErrorCode})");
					return;
				}
				ProcessServers(baselib_Socket_Handle);
				Binding.Baselib_Socket_Close(baselib_Socket_Handle);
			}
		}

		private void ProcessServers(Binding.Baselib_Socket_Handle socketHandle)
		{
			NetworkEndPoint addr = default(NetworkEndPoint);
			foreach (InternalQosServer qosServer in m_QosServers)
			{
				if (!qosServer.Duplicate)
				{
					ProcessServer(qosServer, socketHandle);
					RecvQosResponsesTimed(addr, m_JobExpireTimeUtc, socketHandle, wait: false);
				}
			}
			DateTime dateTime = DateTime.UtcNow.AddMilliseconds(MaxWaitMs);
			if (m_JobExpireTimeUtc < dateTime)
			{
				dateTime = m_JobExpireTimeUtc;
			}
			string text = EnableReceiveWait();
			if (text != "")
			{
				Debug.LogError(text);
				return;
			}
			RecvQosResponsesTimed(addr, dateTime, socketHandle, wait: true);
			foreach (InternalQosServer qosServer2 in m_QosServers)
			{
				InternalQosResult result = (qosServer2.Duplicate ? _qosResults[qosServer2.FirstIdx] : _qosResults[qosServer2.Idx]);
				StoreResult(qosServer2.Idx, result);
			}
		}

		private void ProcessServer(InternalQosServer server, Binding.Baselib_Socket_Handle socketHandle)
		{
			if (QosHelper.ExpiredUtc(m_JobExpireTimeUtc))
			{
				Debug.LogWarning("QosJob: not enough time to process " + server.Address + ".");
				return;
			}
			if (DateTime.UtcNow < server.BackoffUntilUtc)
			{
				Debug.LogWarning("QosJob: skipping " + server.Address + " due to backoff restrictions");
				return;
			}
			InternalQosResult result = _qosResults[server.Idx];
			Binding.Baselib_ErrorCode baselib_ErrorCode = SendQosRequests(server, socketHandle, ref result);
			if (baselib_ErrorCode != 0)
			{
				Debug.LogError($"QosJob: failed to send to {server.Address} (errorcode {baselib_ErrorCode})");
			}
			StoreResult(server.Idx, result);
		}

		private Binding.Baselib_ErrorCode SendQosRequests(InternalQosServer server, Binding.Baselib_Socket_Handle socketHandle, ref InternalQosResult result)
		{
			QosRequest qosRequest = new QosRequest
			{
				Title = m_TitleBytesUtf8.ToArray(),
				Identifier = (ushort)new System.Random().Next(0, 65535)
			};
			server.RequestIdentifier = qosRequest.Identifier;
			StoreServer(server);
			result.RequestsSent = 0u;
			do
			{
				if (QosHelper.ExpiredUtc(m_JobExpireTimeUtc))
				{
					Debug.LogWarning($"QosJob: not enough time to complete {RequestsPerEndpoint - result.RequestsSent} sends to {server.Address} ");
					return Binding.Baselib_ErrorCode.Timeout;
				}
				qosRequest.Timestamp = (ulong)(DateTime.UtcNow.Ticks / 10000);
				qosRequest.Sequence = (byte)result.RequestsSent;
				var (num, num2) = qosRequest.Send(socketHandle.handle, server.RemoteEndpoint, m_JobExpireTimeUtc);
				if (num2 != 0)
				{
					Debug.LogError($"QosJob: send returned error code {(Binding.Baselib_ErrorCode)num2}, can't continue");
					return (Binding.Baselib_ErrorCode)num2;
				}
				if (num != qosRequest.Length)
				{
					Debug.LogWarning($"QosJob: sent {num} of {qosRequest.Length} bytes, ignoring this request");
					result.InvalidRequests++;
					continue;
				}
				m_Requests++;
				result.RequestsSent++;
				if (RequestsBetweenPause != 0 && RequestPauseMs != 0 && m_Requests % RequestsBetweenPause == 0L)
				{
					Thread.Sleep((int)RequestPauseMs);
				}
			}
			while (result.RequestsSent < RequestsPerEndpoint);
			return Binding.Baselib_ErrorCode.Success;
		}

		private void StoreServer(InternalQosServer server)
		{
			m_QosServers[server.Idx] = server;
		}

		private void StoreResult(int idx, InternalQosResult result)
		{
			_qosResults[idx] = result;
		}

		private void RecvQosResponsesTimed(NetworkEndPoint addr, DateTime deadline, Binding.Baselib_Socket_Handle socketHandle, bool wait)
		{
			RecvQosResponses(addr, deadline, socketHandle, wait);
		}

		private void RecvQosResponses(NetworkEndPoint addr, DateTime deadline, Binding.Baselib_Socket_Handle socketHandle, bool wait)
		{
			if (m_Requests == m_Responses)
			{
				return;
			}
			QosResponse qosResponse = new QosResponse();
			InternalQosResult result = _qosResults[0];
			while (m_Requests > m_Responses && !QosHelper.ExpiredUtc(deadline))
			{
				switch (qosResponse.Recv(socketHandle.handle, wait, deadline, ref addr).received)
				{
				case 0:
					if (!wait)
					{
						return;
					}
					continue;
				case -1:
					continue;
				}
				int num = LookupResult(addr, qosResponse, ref result);
				if (num < 0)
				{
					continue;
				}
				string error = "";
				if (!qosResponse.Verify(result.RequestsSent, ref error))
				{
					Debug.LogWarning("QosJob: ignoring response from " + m_QosServers[num].Address + " verify failed with " + error);
					result.InvalidResponses++;
				}
				else
				{
					m_Responses++;
					result.ResponsesReceived++;
					result.AddAggregateLatency((uint)qosResponse.LatencyMs);
					(FcType, byte) tuple = qosResponse.ParseFlowControl();
					if (tuple.Item1 != 0 && tuple.Item2 > result.FcUnits)
					{
						(result.FcType, result.FcUnits) = tuple;
					}
				}
				StoreResult(num, result);
			}
		}

		private string EnableReceiveWait()
		{
			return "";
		}

		private int LookupResult(NetworkEndPoint endPoint, QosResponse response, ref InternalQosResult result)
		{
			if (m_AddressIndexes.TryGetValue(endPoint.Address, out var item))
			{
				result = _qosResults[item];
				InternalQosServer internalQosServer = m_QosServers[item];
				if (response.Identifier != internalQosServer.RequestIdentifier)
				{
					Debug.LogWarning($"QosJob: invalid identifier from {internalQosServer.Address} 0x{response.Identifier:X4} != 0x{internalQosServer.RequestIdentifier:X4} ignoring");
					result.InvalidResponses++;
					return -1;
				}
				return item;
			}
			Debug.LogWarning("QosJob: ignoring unexpected response from " + endPoint.Address);
			return -1;
		}

		private unsafe static (Binding.Baselib_Socket_Handle, Binding.Baselib_ErrorCode) CreateAndBindSocket()
		{
			Binding.Baselib_ErrorState baselib_ErrorState = default(Binding.Baselib_ErrorState);
			Binding.Baselib_Socket_Handle item = Binding.Baselib_Socket_Create(Binding.Baselib_NetworkAddress_Family.IPv4, Binding.Baselib_Socket_Protocol.UDP, &baselib_ErrorState);
			if (baselib_ErrorState.code != 0)
			{
				Debug.LogError($"QosJob: Unable to create socket {baselib_ErrorState.code}");
			}
			return (item, baselib_ErrorState.code);
		}
	}
}
