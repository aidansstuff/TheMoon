using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Networking.Transport.Error;
using Unity.Networking.Transport.Protocols;
using Unity.Networking.Transport.Relay;
using Unity.Networking.Transport.TLS;
using Unity.Networking.Transport.Utilities;
using UnityEngine;

namespace Unity.Networking.Transport
{
	public struct NetworkDriver : IDisposable
	{
		public struct Concurrent
		{
			private struct PendingSend
			{
				public NetworkPipeline Pipeline;

				public NetworkConnection Connection;

				public NetworkInterfaceSendHandle SendHandle;

				public int headerSize;
			}

			internal NetworkSendInterface m_NetworkSendInterface;

			internal NetworkProtocol m_NetworkProtocolInterface;

			internal NetworkEventQueue.Concurrent m_EventQueue;

			internal NativeArray<byte> m_DisconnectReasons;

			[ReadOnly]
			internal NativeList<Connection> m_ConnectionList;

			[ReadOnly]
			internal NativeList<byte> m_DataStream;

			internal NetworkPipelineProcessor.Concurrent m_PipelineProcessor;

			internal UdpCHeader.HeaderFlags m_DefaultHeaderFlags;

			internal NativeQueue<QueuedSendMessage>.ParallelWriter m_ConcurrentParallelSendQueue;

			internal int m_MaxMessageSize;

			public NetworkEvent.Type PopEventForConnection(NetworkConnection connectionId, out DataStreamReader reader)
			{
				NetworkPipeline pipeline;
				return PopEventForConnection(connectionId, out reader, out pipeline);
			}

			public NetworkEvent.Type PopEventForConnection(NetworkConnection connectionId, out DataStreamReader reader, out NetworkPipeline pipeline)
			{
				pipeline = default(NetworkPipeline);
				reader = default(DataStreamReader);
				if (connectionId.m_NetworkId < 0 || connectionId.m_NetworkId >= m_ConnectionList.Length || m_ConnectionList[connectionId.m_NetworkId].Version != connectionId.m_NetworkVersion)
				{
					return NetworkEvent.Type.Empty;
				}
				int offset;
				int size;
				int pipelineId;
				NetworkEvent.Type num = m_EventQueue.PopEventForConnection(connectionId.m_NetworkId, out offset, out size, out pipelineId);
				pipeline = new NetworkPipeline
				{
					Id = pipelineId
				};
				if (num == NetworkEvent.Type.Disconnect && offset < 0)
				{
					reader = new DataStreamReader(m_DisconnectReasons.GetSubArray(math.abs(offset), 1));
					return num;
				}
				if (size > 0)
				{
					reader = new DataStreamReader(((NativeArray<byte>)m_DataStream).GetSubArray(offset, size));
				}
				return num;
			}

			public int MaxHeaderSize(NetworkPipeline pipe)
			{
				int num = m_NetworkProtocolInterface.PaddingSize;
				if (pipe.Id > 0)
				{
					num += m_PipelineProcessor.SendHeaderCapacity(pipe) + 1;
				}
				return num;
			}

			internal int MaxProtocolHeaderSize()
			{
				return m_NetworkProtocolInterface.PaddingSize;
			}

			public int BeginSend(NetworkConnection id, out DataStreamWriter writer, int requiredPayloadSize = 0)
			{
				return BeginSend(NetworkPipeline.Null, id, out writer, requiredPayloadSize);
			}

			public unsafe int BeginSend(NetworkPipeline pipe, NetworkConnection id, out DataStreamWriter writer, int requiredPayloadSize = 0)
			{
				writer = default(DataStreamWriter);
				if (id.m_NetworkId < 0 || id.m_NetworkId >= m_ConnectionList.Length)
				{
					return -1;
				}
				Connection connection = m_ConnectionList[id.m_NetworkId];
				if (connection.Version != id.m_NetworkVersion)
				{
					return -2;
				}
				if (connection.State != NetworkConnection.State.Connected)
				{
					return -3;
				}
				int num = ((pipe.Id > 0) ? (m_PipelineProcessor.SendHeaderCapacity(pipe) + 1) : 0);
				int num2 = m_PipelineProcessor.PayloadCapacity(pipe);
				int payloadOffset;
				int num3 = m_NetworkProtocolInterface.ComputePacketOverhead.Ptr.Invoke(ref connection, out payloadOffset);
				int num4 = ((num2 == 0) ? (m_MaxMessageSize - num3 - num) : num2);
				int num5 = ((num2 == 0) ? m_MaxMessageSize : (num2 + num3 + num));
				if (num4 < requiredPayloadSize)
				{
					return -4;
				}
				if (requiredPayloadSize > 0 && num4 > requiredPayloadSize)
				{
					int num6 = num4 - requiredPayloadSize;
					num4 -= num6;
					num5 -= num6;
				}
				NetworkInterfaceSendHandle handle = default(NetworkInterfaceSendHandle);
				if (num5 > m_MaxMessageSize)
				{
					handle.data = (IntPtr)UnsafeUtility.Malloc(num5, 8, Allocator.Temp);
					handle.capacity = num5;
					handle.id = 0;
					handle.size = 0;
					handle.flags = SendHandleFlags.AllocatedByDriver;
				}
				else
				{
					int num7 = m_NetworkSendInterface.BeginSendMessage.Ptr.Invoke(out handle, m_NetworkSendInterface.UserData, num5);
					if (num7 != 0)
					{
						return num7;
					}
				}
				if (handle.capacity < num5)
				{
					return -4;
				}
				NativeArray<byte> data = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>((byte*)(void*)handle.data + payloadOffset + num, num4, Allocator.Invalid);
				writer = new DataStreamWriter(data);
				writer.m_SendHandleData = (IntPtr)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<PendingSend>(), UnsafeUtility.AlignOf<PendingSend>(), Allocator.Temp);
				*(PendingSend*)(void*)writer.m_SendHandleData = new PendingSend
				{
					Pipeline = pipe,
					Connection = id,
					SendHandle = handle,
					headerSize = payloadOffset
				};
				return 0;
			}

			public unsafe int EndSend(DataStreamWriter writer)
			{
				PendingSend* ptr = (PendingSend*)(void*)writer.m_SendHandleData;
				if (ptr == null || ptr->Connection == default(NetworkConnection))
				{
					return -8;
				}
				if (m_ConnectionList[ptr->Connection.m_NetworkId].Version != ptr->Connection.m_NetworkVersion)
				{
					return -2;
				}
				if (writer.HasFailedWrites)
				{
					AbortSend(writer);
					return -4;
				}
				PendingSend pendingSend = *(PendingSend*)(void*)writer.m_SendHandleData;
				ptr->Connection = default(NetworkConnection);
				pendingSend.SendHandle.size = pendingSend.headerSize + writer.Length;
				int num = 0;
				if (pendingSend.Pipeline.Id > 0)
				{
					pendingSend.SendHandle.size += m_PipelineProcessor.SendHeaderCapacity(pendingSend.Pipeline) + 1;
					UdpCHeader.HeaderFlags defaultHeaderFlags = m_DefaultHeaderFlags;
					m_DefaultHeaderFlags = UdpCHeader.HeaderFlags.HasPipeline;
					num = m_PipelineProcessor.Send(this, pendingSend.Pipeline, pendingSend.Connection, pendingSend.SendHandle, pendingSend.headerSize);
					m_DefaultHeaderFlags = defaultHeaderFlags;
				}
				else
				{
					num = CompleteSend(pendingSend.Connection, pendingSend.SendHandle, (m_DefaultHeaderFlags & UdpCHeader.HeaderFlags.HasPipeline) != 0);
				}
				if (num <= 0)
				{
					return num;
				}
				return writer.Length;
			}

			public unsafe void AbortSend(DataStreamWriter writer)
			{
				PendingSend* ptr = (PendingSend*)(void*)writer.m_SendHandleData;
				if (ptr == null || ptr->Connection == default(NetworkConnection))
				{
					UnityEngine.Debug.LogError("AbortSend without matching BeginSend");
					return;
				}
				PendingSend pendingSend = *(PendingSend*)(void*)writer.m_SendHandleData;
				ptr->Connection = default(NetworkConnection);
				AbortSend(pendingSend.SendHandle);
			}

			internal unsafe int CompleteSend(NetworkConnection sendConnection, NetworkInterfaceSendHandle sendHandle, bool hasPipeline)
			{
				if ((sendHandle.flags & SendHandleFlags.AllocatedByDriver) != 0)
				{
					int num = 0;
					NetworkInterfaceSendHandle networkInterfaceSendHandle = sendHandle;
					if ((num = m_NetworkSendInterface.BeginSendMessage.Ptr.Invoke(out sendHandle, m_NetworkSendInterface.UserData, 1472)) != 0)
					{
						return num;
					}
					UnsafeUtility.MemCpy((void*)sendHandle.data, (void*)networkInterfaceSendHandle.data, networkInterfaceSendHandle.size);
					sendHandle.size = networkInterfaceSendHandle.size;
				}
				Connection connection = m_ConnectionList[sendConnection.m_NetworkId];
				NetworkSendQueueHandle queueHandle = NetworkSendQueueHandle.ToTempHandle(m_ConcurrentParallelSendQueue);
				return m_NetworkProtocolInterface.ProcessSend.Ptr.Invoke(ref connection, hasPipeline, ref m_NetworkSendInterface, ref sendHandle, ref queueHandle, m_NetworkProtocolInterface.UserData);
			}

			internal void AbortSend(NetworkInterfaceSendHandle sendHandle)
			{
				if ((sendHandle.flags & SendHandleFlags.AllocatedByDriver) == 0)
				{
					m_NetworkSendInterface.AbortSendMessage.Ptr.Invoke(ref sendHandle, m_NetworkSendInterface.UserData);
				}
			}

			public NetworkConnection.State GetConnectionState(NetworkConnection id)
			{
				if (id.m_NetworkId < 0 || id.m_NetworkId >= m_ConnectionList.Length)
				{
					return NetworkConnection.State.Disconnected;
				}
				Connection connection = m_ConnectionList[id.m_NetworkId];
				if (connection.Version != id.m_NetworkVersion)
				{
					return NetworkConnection.State.Disconnected;
				}
				return connection.State;
			}
		}

		internal struct Connection
		{
			public NetworkInterfaceEndPoint Address;

			public long LastNonDataSend;

			public long LastReceive;

			public int Id;

			public int Version;

			public int ConnectAttempts;

			public NetworkConnection.State State;

			public SessionIdToken ReceiveToken;

			public SessionIdToken SendToken;

			public byte DidReceiveData;

			public byte IsAccepted;

			public static Connection Null
			{
				get
				{
					Connection result = default(Connection);
					result.Id = 0;
					result.Version = 0;
					return result;
				}
			}

			public static bool operator ==(Connection lhs, Connection rhs)
			{
				if (lhs.Id == rhs.Id && lhs.Version == rhs.Version)
				{
					return lhs.Address == rhs.Address;
				}
				return false;
			}

			public static bool operator !=(Connection lhs, Connection rhs)
			{
				if (lhs.Id == rhs.Id && lhs.Version == rhs.Version)
				{
					return lhs.Address != rhs.Address;
				}
				return true;
			}

			public override bool Equals(object compare)
			{
				return this == (Connection)compare;
			}

			public override int GetHashCode()
			{
				return Id;
			}

			public bool Equals(Connection connection)
			{
				if (connection.Id == Id && connection.Version == Version)
				{
					return connection.Address == Address;
				}
				return false;
			}
		}

		private enum ErrorCodeType
		{
			ReceiveError = 0,
			SendError = 1,
			NumErrorCodes = 2
		}

		private struct Parameters
		{
			public NetworkDataStreamParameter dataStream;

			public NetworkConfigParameter config;

			public Parameters(NetworkSettings settings)
			{
				dataStream = settings.GetDataStreamParameters();
				config = settings.GetNetworkConfigParameters();
			}
		}

		[BurstCompile]
		private struct UpdateJob : IJob
		{
			public NetworkDriver driver;

			public void Execute()
			{
				driver.InternalUpdate();
			}
		}

		[BurstCompile]
		private struct ClearEventQueue : IJob
		{
			public NativeList<byte> dataStream;

			public NativeArray<int> dataStreamHead;

			public NetworkEventQueue eventQueue;

			public void Execute()
			{
				eventQueue.Clear();
				dataStreamHead[0] = 0;
			}
		}

		private static List<INetworkInterface> s_NetworkInterfaces = new List<INetworkInterface>();

		private static List<INetworkProtocol> s_NetworkProtocols = new List<INetworkProtocol>();

		private int m_NetworkInterfaceIndex;

		private NetworkSendInterface m_NetworkSendInterface;

		private int m_NetworkProtocolIndex;

		private NetworkProtocol m_NetworkProtocolInterface;

		private NativeQueue<QueuedSendMessage> m_ParallelSendQueue;

		private NetworkEventQueue m_EventQueue;

		private NativeArray<byte> m_DisconnectReasons;

		private NativeQueue<int> m_FreeList;

		private NativeQueue<int> m_NetworkAcceptQueue;

		private NativeList<Connection> m_ConnectionList;

		[NativeDisableContainerSafetyRestriction]
		private NativeArray<int> m_InternalState;

		private NativeReference<int> m_ProtocolStatus;

		private NativeQueue<int> m_PendingFree;

		private NativeArray<int> m_ErrorCodes;

		private Parameters m_NetworkParams;

		private NativeList<byte> m_DataStream;

		private NativeArray<int> m_DataStreamHead;

		private NetworkPipelineProcessor m_PipelineProcessor;

		private UdpCHeader.HeaderFlags m_DefaultHeaderFlags;

		private long m_UpdateTime;

		private long m_UpdateTimeAdjustment;

		private Unity.Mathematics.Random m_Rand;

		private const int InternalStateListening = 0;

		private const int InternalStateBound = 1;

		internal INetworkInterface NetworkInterface => s_NetworkInterfaces[m_NetworkInterfaceIndex];

		internal INetworkProtocol NetworkProtocol => s_NetworkProtocols[m_NetworkProtocolIndex];

		internal int ProtocolStatus => m_ProtocolStatus.Value;

		public long LastUpdateTime => m_UpdateTime;

		public bool Listening
		{
			get
			{
				return m_InternalState[0] != 0;
			}
			private set
			{
				m_InternalState[0] = (value ? 1 : 0);
			}
		}

		public bool Bound => m_InternalState[1] == 1;

		public bool IsCreated => m_InternalState.IsCreated;

		public int ReceiveErrorCode
		{
			get
			{
				return m_ErrorCodes[0];
			}
			internal set
			{
				if (value != 0)
				{
					UnityEngine.Debug.LogError(FixedString.Format("Error on receive, errorCode = {0}", value));
				}
				m_ErrorCodes[0] = value;
			}
		}

		public Concurrent ToConcurrent()
		{
			Concurrent result = default(Concurrent);
			result.m_NetworkSendInterface = m_NetworkSendInterface;
			result.m_NetworkProtocolInterface = m_NetworkProtocolInterface;
			result.m_EventQueue = m_EventQueue.ToConcurrent();
			result.m_ConnectionList = m_ConnectionList;
			result.m_DataStream = m_DataStream;
			result.m_DisconnectReasons = m_DisconnectReasons;
			result.m_PipelineProcessor = m_PipelineProcessor.ToConcurrent();
			result.m_DefaultHeaderFlags = m_DefaultHeaderFlags;
			result.m_ConcurrentParallelSendQueue = m_ParallelSendQueue.AsParallelWriter();
			result.m_MaxMessageSize = m_NetworkParams.config.maxMessageSize;
			return result;
		}

		private Concurrent ToConcurrentSendOnly()
		{
			Concurrent result = default(Concurrent);
			result.m_NetworkSendInterface = m_NetworkSendInterface;
			result.m_NetworkProtocolInterface = m_NetworkProtocolInterface;
			result.m_EventQueue = default(NetworkEventQueue.Concurrent);
			result.m_ConnectionList = m_ConnectionList;
			result.m_DataStream = m_DataStream;
			result.m_DisconnectReasons = m_DisconnectReasons;
			result.m_PipelineProcessor = m_PipelineProcessor.ToConcurrent();
			result.m_DefaultHeaderFlags = m_DefaultHeaderFlags;
			result.m_ConcurrentParallelSendQueue = m_ParallelSendQueue.AsParallelWriter();
			result.m_MaxMessageSize = m_NetworkParams.config.maxMessageSize;
			return result;
		}

		public static NetworkDriver Create(NetworkSettings settings)
		{
			return new NetworkDriver(default(BaselibNetworkInterface), settings);
		}

		public static NetworkDriver Create()
		{
			return Create(new NetworkSettings(Allocator.Temp));
		}

		public static NetworkDriver Create<N>(N networkInterface) where N : INetworkInterface
		{
			return Create(networkInterface, new NetworkSettings(Allocator.Temp));
		}

		public static NetworkDriver Create<N>(N networkInterface, NetworkSettings settings) where N : INetworkInterface
		{
			return new NetworkDriver(networkInterface, settings);
		}

		public NetworkDriver(INetworkInterface netIf)
			: this(netIf, default(NetworkSettings))
		{
		}

		[Obsolete("Use Create(NetworkSettings) instead", false)]
		public static NetworkDriver Create(params INetworkParameter[] param)
		{
			return Create(NetworkSettings.FromArray(param));
		}

		[Obsolete("Use NetworkDriver(INetworkInterface, NetworkSettings) instead", false)]
		public NetworkDriver(INetworkInterface netIf, params INetworkParameter[] param)
			: this(netIf, NetworkSettings.FromArray(param))
		{
		}

		[Obsolete("Use NetworkDriver(INetworkInterface, NetworkSettings) instead", false)]
		internal NetworkDriver(INetworkInterface netIf, INetworkProtocol netProtocol, params INetworkParameter[] param)
			: this(netIf, netProtocol, NetworkSettings.FromArray(param))
		{
		}

		private static int InsertInAvailableIndex<T>(List<T> list, T element)
		{
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				if (list[i] == null)
				{
					list[i] = element;
					return i;
				}
			}
			list.Add(element);
			return count;
		}

		private static INetworkProtocol GetProtocolForParameters(NetworkSettings settings)
		{
			if (settings.TryGet<RelayNetworkParameter>(out var _))
			{
				return default(RelayNetworkProtocol);
			}
			if (settings.TryGet<SecureNetworkProtocolParameter>(out var _))
			{
				return default(SecureNetworkProtocol);
			}
			return default(UnityTransportProtocol);
		}

		public NetworkDriver(INetworkInterface netIf, NetworkSettings settings)
			: this(netIf, GetProtocolForParameters(settings), settings)
		{
		}

		internal NetworkDriver(INetworkInterface netIf, INetworkProtocol netProtocol, NetworkSettings settings)
		{
			m_NetworkParams = new Parameters(settings);
			netProtocol.Initialize(settings);
			m_NetworkProtocolIndex = InsertInAvailableIndex(s_NetworkProtocols, netProtocol);
			m_NetworkProtocolInterface = netProtocol.CreateProtocolInterface();
			m_NetworkInterfaceIndex = InsertInAvailableIndex(s_NetworkInterfaces, netIf);
			int num = netIf.Initialize(settings);
			if (num != 0)
			{
				UnityEngine.Debug.LogError($"Failed to initialize the NetworkInterface. Error Code: {num}.");
			}
			m_NetworkSendInterface = netIf.CreateSendInterface();
			m_PipelineProcessor = new NetworkPipelineProcessor(settings);
			m_ParallelSendQueue = new NativeQueue<QueuedSendMessage>(Allocator.Persistent);
			long timestamp = Stopwatch.GetTimestamp();
			long num2 = timestamp / (Stopwatch.Frequency / 1000);
			m_UpdateTime = ((m_NetworkParams.config.fixedFrameTimeMS > 0) ? 1 : num2);
			m_UpdateTimeAdjustment = 0L;
			m_Rand = new Unity.Mathematics.Random((uint)timestamp);
			int num3 = m_NetworkParams.dataStream.size;
			if (num3 == 0)
			{
				num3 = 65536;
			}
			m_DataStream = new NativeList<byte>(num3, Allocator.Persistent);
			m_DataStream.ResizeUninitialized(num3);
			m_DataStreamHead = new NativeArray<int>(1, Allocator.Persistent);
			m_DefaultHeaderFlags = (UdpCHeader.HeaderFlags)0;
			m_NetworkAcceptQueue = new NativeQueue<int>(Allocator.Persistent);
			m_ConnectionList = new NativeList<Connection>(1, Allocator.Persistent);
			m_FreeList = new NativeQueue<int>(Allocator.Persistent);
			m_EventQueue = new NetworkEventQueue(100);
			m_DisconnectReasons = new NativeArray<byte>(4, Allocator.Persistent);
			for (int i = 0; i < 4; i++)
			{
				m_DisconnectReasons[i] = (byte)i;
			}
			m_InternalState = new NativeArray<int>(2, Allocator.Persistent);
			m_PendingFree = new NativeQueue<int>(Allocator.Persistent);
			m_ProtocolStatus = new NativeReference<int>(Allocator.Persistent);
			m_ProtocolStatus.Value = 0;
			m_ErrorCodes = new NativeArray<int>(2, Allocator.Persistent);
			Listening = false;
		}

		public void Dispose()
		{
			if (IsCreated)
			{
				s_NetworkProtocols[m_NetworkProtocolIndex].Dispose();
				s_NetworkProtocols[m_NetworkProtocolIndex] = null;
				s_NetworkInterfaces[m_NetworkInterfaceIndex].Dispose();
				s_NetworkInterfaces[m_NetworkInterfaceIndex] = null;
				m_NetworkProtocolIndex = -1;
				m_NetworkInterfaceIndex = -1;
				m_DataStream.Dispose();
				m_DataStreamHead.Dispose();
				m_PipelineProcessor.Dispose();
				m_EventQueue.Dispose();
				m_DisconnectReasons.Dispose();
				m_NetworkAcceptQueue.Dispose();
				m_ConnectionList.Dispose();
				m_FreeList.Dispose();
				m_InternalState.Dispose();
				m_PendingFree.Dispose();
				m_ProtocolStatus.Dispose();
				m_ErrorCodes.Dispose();
				m_ParallelSendQueue.Dispose();
			}
		}

		private unsafe SessionIdToken GenerateRandomSessionIdToken(ref SessionIdToken token)
		{
			for (uint num = 0u; num < 8; num++)
			{
				token.Value[num] = (byte)(m_Rand.NextUInt() & 0xFFu);
			}
			return token;
		}

		private void UpdateLastUpdateTime()
		{
			long timestamp = Stopwatch.GetTimestamp();
			long num = ((m_NetworkParams.config.fixedFrameTimeMS > 0) ? (m_UpdateTime + m_NetworkParams.config.fixedFrameTimeMS) : (timestamp / (Stopwatch.Frequency / 1000) - m_UpdateTimeAdjustment));
			m_Rand.InitState((uint)timestamp);
			long num2 = num - m_UpdateTime;
			if (m_NetworkParams.config.maxFrameTimeMS > 0 && num2 > m_NetworkParams.config.maxFrameTimeMS)
			{
				m_UpdateTimeAdjustment += num2 - m_NetworkParams.config.maxFrameTimeMS;
				num = m_UpdateTime + m_NetworkParams.config.maxFrameTimeMS;
			}
			m_UpdateTime = num;
		}

		public JobHandle ScheduleUpdate(JobHandle dep = default(JobHandle))
		{
			UpdateLastUpdateTime();
			UpdateJob updateJob = default(UpdateJob);
			updateJob.driver = this;
			UpdateJob jobData = updateJob;
			if (Bound)
			{
				ClearEventQueue jobData2 = default(ClearEventQueue);
				jobData2.dataStream = m_DataStream;
				jobData2.dataStreamHead = m_DataStreamHead;
				jobData2.eventQueue = m_EventQueue;
				JobHandle dependsOn = IJobExtensions.Schedule(jobData2, dep);
				dependsOn = IJobExtensions.Schedule(jobData, dependsOn);
				dependsOn = s_NetworkInterfaces[m_NetworkInterfaceIndex].ScheduleReceive(new NetworkPacketReceiver
				{
					m_Driver = this
				}, dependsOn);
				return s_NetworkInterfaces[m_NetworkInterfaceIndex].ScheduleSend(m_ParallelSendQueue, dependsOn);
			}
			return IJobExtensions.Schedule(jobData, dep);
		}

		public JobHandle ScheduleFlushSend(JobHandle dep)
		{
			if (Bound)
			{
				return s_NetworkInterfaces[m_NetworkInterfaceIndex].ScheduleSend(m_ParallelSendQueue, dep);
			}
			return dep;
		}

		private void InternalUpdate()
		{
			m_PipelineProcessor.Timestamp = m_UpdateTime;
			int item;
			while (m_PendingFree.TryDequeue(out item))
			{
				int num = m_ConnectionList[item].Version + 1;
				if (num == 0)
				{
					num = 1;
				}
				m_ConnectionList[item] = new Connection
				{
					Id = item,
					Version = num,
					IsAccepted = 0
				};
				m_FreeList.Enqueue(item);
			}
			CheckTimeouts();
			if (m_NetworkProtocolInterface.NeedsUpdate)
			{
				NetworkSendQueueHandle queueHandle = NetworkSendQueueHandle.ToTempHandle(m_ParallelSendQueue.AsParallelWriter());
				m_NetworkProtocolInterface.Update.Ptr.Invoke(m_UpdateTime, ref m_NetworkSendInterface, ref queueHandle, m_NetworkProtocolInterface.UserData);
			}
			m_PipelineProcessor.UpdateReceive(this, out var updateCount);
			int num2 = math.max(0, (m_ConnectionList.Length - m_FreeList.Count) * 64);
			if (updateCount > num2)
			{
				UnityEngine.Debug.LogWarning(FixedString.Format("A lot of pipeline updates have been queued, possibly too many being scheduled in pipeline logic, queue count: {0}", updateCount));
			}
			m_DefaultHeaderFlags = UdpCHeader.HeaderFlags.HasPipeline;
			m_PipelineProcessor.UpdateSend(ToConcurrentSendOnly(), out updateCount);
			if (updateCount > num2)
			{
				UnityEngine.Debug.LogWarning(FixedString.Format("A lot of pipeline updates have been queued, possibly too many being scheduled in pipeline logic, queue count: {0}", updateCount));
			}
			m_DefaultHeaderFlags = (UdpCHeader.HeaderFlags)0;
		}

		public NetworkPipeline CreatePipeline(params Type[] stages)
		{
			return m_PipelineProcessor.CreatePipeline(stages);
		}

		public int Bind(NetworkEndPoint endpoint)
		{
			if (s_NetworkInterfaces[m_NetworkInterfaceIndex].CreateInterfaceEndPoint(endpoint, out var endpoint2) != 0)
			{
				return -1;
			}
			int num = s_NetworkProtocols[m_NetworkProtocolIndex].Bind(s_NetworkInterfaces[m_NetworkInterfaceIndex], ref endpoint2);
			m_InternalState[1] = ((num == 0) ? 1 : 0);
			return num;
		}

		public int Listen()
		{
			if (!Bound)
			{
				return -1;
			}
			int num = s_NetworkInterfaces[m_NetworkInterfaceIndex].Listen();
			if (num == 0)
			{
				Listening = true;
			}
			return num;
		}

		public NetworkConnection Accept()
		{
			if (!Listening)
			{
				return default(NetworkConnection);
			}
			if (!m_NetworkAcceptQueue.TryDequeue(out var item))
			{
				return default(NetworkConnection);
			}
			Connection connection = m_ConnectionList[item];
			connection.State = NetworkConnection.State.Connected;
			connection.IsAccepted = 1;
			SetConnection(connection);
			NetworkConnection result = default(NetworkConnection);
			result.m_NetworkId = item;
			result.m_NetworkVersion = m_ConnectionList[item].Version;
			return result;
		}

		public NetworkConnection Connect(NetworkEndPoint endpoint)
		{
			if (!Bound)
			{
				NetworkEndPoint endpoint2 = ((endpoint.Family == NetworkFamily.Ipv6) ? NetworkEndPoint.AnyIpv6 : NetworkEndPoint.AnyIpv4);
				if (Bind(endpoint2) != 0)
				{
					return default(NetworkConnection);
				}
			}
			if (s_NetworkProtocols[m_NetworkProtocolIndex].CreateConnectionAddress(s_NetworkInterfaces[m_NetworkInterfaceIndex], endpoint, out var address) != 0)
			{
				return default(NetworkConnection);
			}
			Connection value;
			if (!m_FreeList.TryDequeue(out var item))
			{
				item = m_ConnectionList.Length;
				ref NativeList<Connection> connectionList = ref m_ConnectionList;
				value = new Connection
				{
					Id = item,
					Version = 1
				};
				connectionList.Add(in value);
			}
			int version = m_ConnectionList[item].Version;
			SessionIdToken token = default(SessionIdToken);
			GenerateRandomSessionIdToken(ref token);
			value = default(Connection);
			value.Id = item;
			value.Version = version;
			value.State = NetworkConnection.State.Connecting;
			value.Address = address;
			value.ConnectAttempts = 1;
			value.LastNonDataSend = m_UpdateTime;
			value.LastReceive = 0L;
			value.SendToken = default(SessionIdToken);
			value.ReceiveToken = token;
			value.IsAccepted = 0;
			Connection connection = value;
			SetConnection(connection);
			NetworkConnection networkConnection = default(NetworkConnection);
			networkConnection.m_NetworkId = item;
			networkConnection.m_NetworkVersion = version;
			NetworkConnection networkConnection2 = networkConnection;
			NetworkSendQueueHandle queueHandle = NetworkSendQueueHandle.ToTempHandle(m_ParallelSendQueue.AsParallelWriter());
			m_NetworkProtocolInterface.Connect.Ptr.Invoke(ref connection, ref m_NetworkSendInterface, ref queueHandle, m_NetworkProtocolInterface.UserData);
			m_PipelineProcessor.initializeConnection(networkConnection2);
			return networkConnection2;
		}

		public int Disconnect(NetworkConnection id)
		{
			Connection connection;
			if ((connection = GetConnection(id)) == Connection.Null)
			{
				return 0;
			}
			if (connection.State == NetworkConnection.State.Connected)
			{
				NetworkSendQueueHandle queueHandle = NetworkSendQueueHandle.ToTempHandle(m_ParallelSendQueue.AsParallelWriter());
				m_NetworkProtocolInterface.Disconnect.Ptr.Invoke(ref connection, ref m_NetworkSendInterface, ref queueHandle, m_NetworkProtocolInterface.UserData);
			}
			RemoveConnection(connection);
			return 0;
		}

		public void GetPipelineBuffers(NetworkPipeline pipeline, NetworkPipelineStageId stageId, NetworkConnection connection, out NativeArray<byte> readProcessingBuffer, out NativeArray<byte> writeProcessingBuffer, out NativeArray<byte> sharedBuffer)
		{
			if (connection.m_NetworkId < 0 || connection.m_NetworkId >= m_ConnectionList.Length || m_ConnectionList[connection.m_NetworkId].Version != connection.m_NetworkVersion)
			{
				UnityEngine.Debug.LogError("Trying to get pipeline buffers for invalid connection.");
				readProcessingBuffer = default(NativeArray<byte>);
				writeProcessingBuffer = default(NativeArray<byte>);
				sharedBuffer = default(NativeArray<byte>);
			}
			else
			{
				m_PipelineProcessor.GetPipelineBuffers(pipeline, stageId, connection, out readProcessingBuffer, out writeProcessingBuffer, out sharedBuffer);
			}
		}

		public NetworkConnection.State GetConnectionState(NetworkConnection con)
		{
			Connection connection;
			if ((connection = GetConnection(con)) == Connection.Null)
			{
				return NetworkConnection.State.Disconnected;
			}
			return connection.State;
		}

		public NetworkEndPoint RemoteEndPoint(NetworkConnection id)
		{
			if (id == default(NetworkConnection))
			{
				return default(NetworkEndPoint);
			}
			Connection connection;
			if ((connection = GetConnection(id)) == Connection.Null)
			{
				return default(NetworkEndPoint);
			}
			return s_NetworkProtocols[m_NetworkProtocolIndex].GetRemoteEndPoint(s_NetworkInterfaces[m_NetworkInterfaceIndex], connection.Address);
		}

		public NetworkEndPoint LocalEndPoint()
		{
			NetworkInterfaceEndPoint localEndPoint = s_NetworkInterfaces[m_NetworkInterfaceIndex].LocalEndPoint;
			return s_NetworkInterfaces[m_NetworkInterfaceIndex].GetGenericEndPoint(localEndPoint);
		}

		public int MaxHeaderSize(NetworkPipeline pipe)
		{
			return ToConcurrentSendOnly().MaxHeaderSize(pipe);
		}

		internal int MaxProtocolHeaderSize()
		{
			return m_NetworkProtocolInterface.PaddingSize;
		}

		public int BeginSend(NetworkPipeline pipe, NetworkConnection id, out DataStreamWriter writer, int requiredPayloadSize = 0)
		{
			return ToConcurrentSendOnly().BeginSend(pipe, id, out writer, requiredPayloadSize);
		}

		public int BeginSend(NetworkConnection id, out DataStreamWriter writer, int requiredPayloadSize = 0)
		{
			return ToConcurrentSendOnly().BeginSend(NetworkPipeline.Null, id, out writer, requiredPayloadSize);
		}

		public int EndSend(DataStreamWriter writer)
		{
			return ToConcurrentSendOnly().EndSend(writer);
		}

		public void AbortSend(DataStreamWriter writer)
		{
			ToConcurrentSendOnly().AbortSend(writer);
		}

		public NetworkEvent.Type PopEvent(out NetworkConnection con, out DataStreamReader reader)
		{
			NetworkPipeline pipeline;
			return PopEvent(out con, out reader, out pipeline);
		}

		public NetworkEvent.Type PopEvent(out NetworkConnection con, out DataStreamReader reader, out NetworkPipeline pipeline)
		{
			reader = default(DataStreamReader);
			NetworkEvent.Type type = NetworkEvent.Type.Empty;
			int id = 0;
			int offset = 0;
			int size = 0;
			int pipelineId = 0;
			while (true)
			{
				type = m_EventQueue.PopEvent(out id, out offset, out size, out pipelineId);
				if (id < 0 || type != NetworkEvent.Type.Data || m_ConnectionList[id].IsAccepted != 0)
				{
					break;
				}
				UnityEngine.Debug.LogWarning("A NetworkEvent.Data event was discarded for a connection that had not been accepted yet. To avoid this, consider calling Accept() prior to PopEvent() in your project's network update loop, or only use PopEventForConnection() in conjunction with Accept().");
			}
			pipeline = new NetworkPipeline
			{
				Id = pipelineId
			};
			if (type == NetworkEvent.Type.Disconnect && offset < 0)
			{
				reader = new DataStreamReader(m_DisconnectReasons.GetSubArray(math.abs(offset), 1));
			}
			else if (size > 0)
			{
				reader = new DataStreamReader(((NativeArray<byte>)m_DataStream).GetSubArray(offset, size));
			}
			con = ((id < 0) ? default(NetworkConnection) : new NetworkConnection
			{
				m_NetworkId = id,
				m_NetworkVersion = m_ConnectionList[id].Version
			});
			return type;
		}

		public NetworkEvent.Type PopEventForConnection(NetworkConnection connectionId, out DataStreamReader reader)
		{
			NetworkPipeline pipeline;
			return PopEventForConnection(connectionId, out reader, out pipeline);
		}

		public NetworkEvent.Type PopEventForConnection(NetworkConnection connectionId, out DataStreamReader reader, out NetworkPipeline pipeline)
		{
			reader = default(DataStreamReader);
			pipeline = default(NetworkPipeline);
			if (connectionId.m_NetworkId < 0 || connectionId.m_NetworkId >= m_ConnectionList.Length || m_ConnectionList[connectionId.m_NetworkId].Version != connectionId.m_NetworkVersion)
			{
				return NetworkEvent.Type.Empty;
			}
			int offset;
			int size;
			int pipelineId;
			NetworkEvent.Type num = m_EventQueue.PopEventForConnection(connectionId.m_NetworkId, out offset, out size, out pipelineId);
			pipeline = new NetworkPipeline
			{
				Id = pipelineId
			};
			if (num == NetworkEvent.Type.Disconnect && offset < 0)
			{
				reader = new DataStreamReader(m_DisconnectReasons.GetSubArray(math.abs(offset), 1));
				return num;
			}
			if (size > 0)
			{
				reader = new DataStreamReader(((NativeArray<byte>)m_DataStream).GetSubArray(offset, size));
			}
			return num;
		}

		public int GetEventQueueSizeForConnection(NetworkConnection connectionId)
		{
			if (connectionId.m_NetworkId < 0 || connectionId.m_NetworkId >= m_ConnectionList.Length || m_ConnectionList[connectionId.m_NetworkId].Version != connectionId.m_NetworkVersion)
			{
				return 0;
			}
			return m_EventQueue.GetCountForConnection(connectionId.m_NetworkId);
		}

		private void AddConnectEvent(int id)
		{
			m_EventQueue.PushEvent(new NetworkEvent
			{
				connectionId = id,
				type = NetworkEvent.Type.Connect
			});
		}

		private void AddDisconnectEvent(int id, DisconnectReason reason = DisconnectReason.Default)
		{
			m_EventQueue.PushEvent(new NetworkEvent
			{
				connectionId = id,
				type = NetworkEvent.Type.Disconnect,
				status = (int)reason
			});
		}

		private Connection GetConnection(NetworkConnection id)
		{
			if (id.m_NetworkId < 0 || id.m_NetworkId >= m_ConnectionList.Length)
			{
				return Connection.Null;
			}
			Connection result = m_ConnectionList[id.m_NetworkId];
			if (result.Version != id.m_NetworkVersion)
			{
				return Connection.Null;
			}
			return result;
		}

		private Connection GetConnection(NetworkInterfaceEndPoint address, SessionIdToken sessionId)
		{
			for (int i = 0; i < m_ConnectionList.Length; i++)
			{
				if (address == m_ConnectionList[i].Address && m_ConnectionList[i].ReceiveToken == sessionId)
				{
					return m_ConnectionList[i];
				}
			}
			return Connection.Null;
		}

		private Connection GetNewConnection(NetworkInterfaceEndPoint address, SessionIdToken sessionId)
		{
			for (int i = 0; i < m_ConnectionList.Length; i++)
			{
				if (address == m_ConnectionList[i].Address && m_ConnectionList[i].SendToken == sessionId)
				{
					return m_ConnectionList[i];
				}
			}
			return Connection.Null;
		}

		private void SetConnection(Connection connection)
		{
			m_ConnectionList[connection.Id] = connection;
		}

		private bool RemoveConnection(Connection connection)
		{
			if (connection.State != 0 && connection == m_ConnectionList[connection.Id])
			{
				connection.State = NetworkConnection.State.Disconnected;
				m_ConnectionList[connection.Id] = connection;
				m_PendingFree.Enqueue(connection.Id);
				return true;
			}
			return false;
		}

		private void UpdateConnection(Connection connection)
		{
			if (connection == m_ConnectionList[connection.Id])
			{
				SetConnection(connection);
			}
		}

		private void CheckTimeouts()
		{
			for (int i = 0; i < m_ConnectionList.Length; i++)
			{
				Connection connection = m_ConnectionList[i];
				if (connection == Connection.Null)
				{
					continue;
				}
				long updateTime = m_UpdateTime;
				NetworkConnection networkConnection = default(NetworkConnection);
				networkConnection.m_NetworkId = connection.Id;
				networkConnection.m_NetworkVersion = connection.Version;
				NetworkConnection id = networkConnection;
				if (connection.State == NetworkConnection.State.Connecting && updateTime - connection.LastNonDataSend > m_NetworkParams.config.connectTimeoutMS)
				{
					if (connection.ConnectAttempts >= m_NetworkParams.config.maxConnectAttempts)
					{
						Disconnect(id);
						AddDisconnectEvent(connection.Id, DisconnectReason.MaxConnectionAttempts);
						continue;
					}
					connection.ConnectAttempts = ++connection.ConnectAttempts;
					connection.LastNonDataSend = updateTime;
					SetConnection(connection);
					NetworkSendQueueHandle queueHandle = NetworkSendQueueHandle.ToTempHandle(m_ParallelSendQueue.AsParallelWriter());
					m_NetworkProtocolInterface.Connect.Ptr.Invoke(ref connection, ref m_NetworkSendInterface, ref queueHandle, m_NetworkProtocolInterface.UserData);
				}
				if (connection.State == NetworkConnection.State.Connected && updateTime - connection.LastReceive > m_NetworkParams.config.disconnectTimeoutMS)
				{
					Disconnect(id);
					AddDisconnectEvent(connection.Id, DisconnectReason.Timeout);
					connection = m_ConnectionList[i];
				}
				if (connection.State == NetworkConnection.State.Connected && connection.DidReceiveData != 0 && m_NetworkParams.config.heartbeatTimeoutMS > 0 && updateTime - connection.LastReceive > m_NetworkParams.config.heartbeatTimeoutMS && updateTime - connection.LastNonDataSend > m_NetworkParams.config.heartbeatTimeoutMS)
				{
					connection.LastNonDataSend = updateTime;
					SetConnection(connection);
					NetworkSendQueueHandle queueHandle2 = NetworkSendQueueHandle.ToTempHandle(m_ParallelSendQueue.AsParallelWriter());
					m_NetworkProtocolInterface.ProcessSendPing.Ptr.Invoke(ref connection, ref m_NetworkSendInterface, ref queueHandle2, m_NetworkProtocolInterface.UserData);
				}
			}
		}

		internal bool IsAddressUsed(NetworkInterfaceEndPoint address)
		{
			for (int i = 0; i < m_ConnectionList.Length; i++)
			{
				if (address == m_ConnectionList[i].Address)
				{
					return true;
				}
			}
			return false;
		}

		internal void AppendPacket(IntPtr dataStream, ref NetworkInterfaceEndPoint endpoint, int dataLen)
		{
			ProcessPacketCommand command = default(ProcessPacketCommand);
			NetworkSendQueueHandle queueHandle = NetworkSendQueueHandle.ToTempHandle(m_ParallelSendQueue.AsParallelWriter());
			m_NetworkProtocolInterface.ProcessReceive.Ptr.Invoke(dataStream, ref endpoint, dataLen, ref m_NetworkSendInterface, ref queueHandle, m_NetworkProtocolInterface.UserData, ref command);
			switch (command.Type)
			{
			case ProcessPacketCommandType.AddressUpdate:
			{
				for (int i = 0; i < m_ConnectionList.Length; i++)
				{
					if (command.Address == m_ConnectionList[i].Address && command.SessionId == m_ConnectionList[i].ReceiveToken)
					{
						m_ConnectionList.ElementAt(i).Address = command.As.AddressUpdate.NewAddress;
					}
				}
				break;
			}
			case ProcessPacketCommandType.ConnectionAccept:
			{
				Connection connection3 = GetConnection(command.Address, command.SessionId);
				if (connection3 != Connection.Null)
				{
					connection3.DidReceiveData = 1;
					connection3.LastReceive = m_UpdateTime;
					SetConnection(connection3);
					if (connection3.State == NetworkConnection.State.Connecting)
					{
						connection3.SendToken = command.As.ConnectionAccept.ConnectionToken;
						connection3.State = NetworkConnection.State.Connected;
						connection3.IsAccepted = 1;
						UpdateConnection(connection3);
						AddConnectEvent(connection3.Id);
					}
				}
				break;
			}
			case ProcessPacketCommandType.ConnectionRequest:
			{
				if (!Listening)
				{
					break;
				}
				Connection connection4 = GetNewConnection(command.Address, command.SessionId);
				if (connection4 == Connection.Null || connection4.State == NetworkConnection.State.Disconnected)
				{
					SessionIdToken token = default(SessionIdToken);
					GenerateRandomSessionIdToken(ref token);
					Connection value;
					if (!m_FreeList.TryDequeue(out var item))
					{
						item = m_ConnectionList.Length;
						ref NativeList<Connection> connectionList = ref m_ConnectionList;
						value = new Connection
						{
							Id = item,
							Version = 1
						};
						connectionList.Add(in value);
					}
					int version = m_ConnectionList[item].Version;
					value = default(Connection);
					value.Id = item;
					value.Version = version;
					value.ReceiveToken = token;
					value.SendToken = command.SessionId;
					value.State = NetworkConnection.State.Connected;
					value.Address = command.Address;
					value.ConnectAttempts = 1;
					value.LastReceive = m_UpdateTime;
					value.IsAccepted = 0;
					connection4 = value;
					m_PipelineProcessor.initializeConnection(new NetworkConnection
					{
						m_NetworkId = item,
						m_NetworkVersion = connection4.Version
					});
					m_NetworkAcceptQueue.Enqueue(item);
				}
				connection4.LastNonDataSend = m_UpdateTime;
				SetConnection(connection4);
				m_NetworkProtocolInterface.ProcessSendConnectionAccept.Ptr.Invoke(ref connection4, ref m_NetworkSendInterface, ref queueHandle, m_NetworkProtocolInterface.UserData);
				break;
			}
			case ProcessPacketCommandType.Disconnect:
			{
				Connection connection5 = GetConnection(command.Address, command.SessionId);
				if (connection5 != Connection.Null && RemoveConnection(connection5))
				{
					AddDisconnectEvent(connection5.Id, DisconnectReason.ClosedByRemote);
				}
				break;
			}
			case ProcessPacketCommandType.Ping:
			{
				Connection connection9 = GetConnection(command.Address, command.SessionId);
				if (!(connection9 == Connection.Null) && connection9.State == NetworkConnection.State.Connected)
				{
					connection9.DidReceiveData = 1;
					connection9.LastReceive = m_UpdateTime;
					connection9.LastNonDataSend = m_UpdateTime;
					UpdateConnection(connection9);
					m_NetworkProtocolInterface.ProcessSendPong.Ptr.Invoke(ref connection9, ref m_NetworkSendInterface, ref queueHandle, m_NetworkProtocolInterface.UserData);
				}
				break;
			}
			case ProcessPacketCommandType.Pong:
			{
				Connection connection8 = GetConnection(command.Address, command.SessionId);
				if (connection8 != Connection.Null)
				{
					connection8.DidReceiveData = 1;
					connection8.LastReceive = m_UpdateTime;
					UpdateConnection(connection8);
				}
				break;
			}
			case ProcessPacketCommandType.DataWithImplicitConnectionAccept:
			{
				Connection connection6 = GetConnection(command.Address, command.SessionId);
				if (connection6 == Connection.Null)
				{
					break;
				}
				connection6.DidReceiveData = 1;
				connection6.LastReceive = m_UpdateTime;
				UpdateConnection(connection6);
				if (connection6.State == NetworkConnection.State.Connecting)
				{
					connection6.SendToken = command.As.DataWithImplicitConnectionAccept.ConnectionToken;
					connection6.State = NetworkConnection.State.Connected;
					UpdateConnection(connection6);
					AddConnectEvent(connection6.Id);
				}
				if (connection6.State == NetworkConnection.State.Connected)
				{
					int num2 = PinMemoryTillUpdate(command.As.DataWithImplicitConnectionAccept.Offset + command.As.DataWithImplicitConnectionAccept.Length) + command.As.DataWithImplicitConnectionAccept.Offset;
					if (command.As.DataWithImplicitConnectionAccept.HasPipeline)
					{
						NetworkConnection networkConnection = default(NetworkConnection);
						networkConnection.m_NetworkId = connection6.Id;
						networkConnection.m_NetworkVersion = connection6.Version;
						NetworkConnection connection7 = networkConnection;
						m_PipelineProcessor.Receive(this, connection7, ((NativeArray<byte>)m_DataStream).GetSubArray(num2, command.As.DataWithImplicitConnectionAccept.Length));
					}
					else
					{
						m_EventQueue.PushEvent(new NetworkEvent
						{
							connectionId = connection6.Id,
							type = NetworkEvent.Type.Data,
							offset = num2,
							size = command.As.DataWithImplicitConnectionAccept.Length
						});
					}
				}
				break;
			}
			case ProcessPacketCommandType.Data:
			{
				Connection connection = GetConnection(command.Address, command.SessionId);
				if (connection == Connection.Null)
				{
					break;
				}
				connection.DidReceiveData = 1;
				connection.LastReceive = m_UpdateTime;
				UpdateConnection(connection);
				if (connection.State == NetworkConnection.State.Connected)
				{
					int num = PinMemoryTillUpdate(command.As.Data.Offset + command.As.Data.Length) + command.As.Data.Offset;
					if (command.As.Data.HasPipeline)
					{
						NetworkConnection networkConnection = default(NetworkConnection);
						networkConnection.m_NetworkId = connection.Id;
						networkConnection.m_NetworkVersion = connection.Version;
						NetworkConnection connection2 = networkConnection;
						m_PipelineProcessor.Receive(this, connection2, ((NativeArray<byte>)m_DataStream).GetSubArray(num, command.As.Data.Length));
					}
					else
					{
						m_EventQueue.PushEvent(new NetworkEvent
						{
							connectionId = connection.Id,
							type = NetworkEvent.Type.Data,
							offset = num,
							size = command.As.Data.Length
						});
					}
				}
				break;
			}
			case ProcessPacketCommandType.ProtocolStatusUpdate:
				m_ProtocolStatus.Value = command.As.ProtocolStatusUpdate.Status;
				break;
			case ProcessPacketCommandType.Drop:
			case ProcessPacketCommandType.ConnectionReject:
				break;
			}
		}

		internal unsafe void PushDataEvent(NetworkConnection con, int pipelineId, byte* dataPtr, int dataLength)
		{
			if (!IsPointerInsideDataStream(dataPtr, dataLength, out var sliceOffset))
			{
				int dataLen = dataLength;
				IntPtr intPtr = AllocateMemory(ref dataLen);
				if (intPtr == IntPtr.Zero || dataLen < dataLength)
				{
					return;
				}
				UnsafeUtility.MemCpy(intPtr.ToPointer(), dataPtr, dataLength);
				sliceOffset = PinMemoryTillUpdate(dataLength);
			}
			m_EventQueue.PushEvent(new NetworkEvent
			{
				pipelineId = (short)pipelineId,
				connectionId = con.m_NetworkId,
				type = NetworkEvent.Type.Data,
				offset = sliceOffset,
				size = dataLength
			});
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal int PinMemoryTillUpdate(int length)
		{
			int num = m_DataStreamHead[0];
			m_DataStreamHead[0] = num + length;
			return num;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe bool IsPointerInsideDataStream(byte* dataPtr, int dataLength, out int sliceOffset)
		{
			sliceOffset = 0;
			byte* unsafePtr = (byte*)m_DataStream.GetUnsafePtr();
			int num;
			if (dataPtr >= unsafePtr)
			{
				num = ((dataPtr + dataLength <= unsafePtr + m_DataStreamHead[0]) ? 1 : 0);
				if (num != 0)
				{
					sliceOffset = (int)(dataPtr - unsafePtr);
				}
			}
			else
			{
				num = 0;
			}
			return (byte)num != 0;
		}

		internal unsafe IntPtr AllocateMemory(ref int dataLen)
		{
			NativeList<byte> dataStream = m_DataStream;
			int num = m_DataStreamHead[0];
			if (m_NetworkParams.dataStream.size == 0)
			{
				dataStream.ResizeUninitializedTillPowerOf2(num + dataLen);
			}
			else if (num + dataLen > dataStream.Length)
			{
				dataLen = dataStream.Length - num;
				if (dataLen <= 0)
				{
					dataLen = 0;
					return IntPtr.Zero;
				}
			}
			return new IntPtr((byte*)dataStream.GetUnsafePtr() + num);
		}
	}
}
