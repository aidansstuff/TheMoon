using System;
using AOT;
using Unity.Baselib.LowLevel;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Networking.Transport.Utilities.LowLevel.Unsafe;
using UnityEngine;

namespace Unity.Networking.Transport
{
	[BurstCompile]
	public struct BaselibNetworkInterface : INetworkInterface, IDisposable
	{
		internal struct Payloads : IDisposable
		{
			public UnsafeAtomicFreeList m_Handles;

			public UnsafeBaselibNetworkArray m_PayloadArray;

			public UnsafeBaselibNetworkArray m_EndpointArray;

			private uint m_PayloadSize;

			public int InUse => m_Handles.InUse;

			public int Capacity => m_Handles.Capacity;

			public bool IsCreated => m_Handles.IsCreated;

			public Payloads(int capacity, uint maxPayloadSize)
			{
				m_PayloadSize = maxPayloadSize;
				m_Handles = new UnsafeAtomicFreeList(capacity, Allocator.Persistent);
				m_PayloadArray = new UnsafeBaselibNetworkArray(capacity, (int)maxPayloadSize);
				m_EndpointArray = new UnsafeBaselibNetworkArray(capacity, 28);
			}

			public void Dispose()
			{
				m_Handles.Dispose();
				m_PayloadArray.Dispose();
				m_EndpointArray.Dispose();
			}

			public Binding.Baselib_RegisteredNetwork_Request GetRequestFromHandle(int handle)
			{
				Binding.Baselib_RegisteredNetwork_Request result = default(Binding.Baselib_RegisteredNetwork_Request);
				result.payload = m_PayloadArray.AtIndexAsSlice(handle, m_PayloadSize);
				result.remoteEndpoint = new Binding.Baselib_RegisteredNetwork_Endpoint
				{
					slice = m_EndpointArray.AtIndexAsSlice(handle, 28u)
				};
				return result;
			}

			public int AcquireHandle()
			{
				return m_Handles.Pop();
			}

			public void ReleaseHandle(int handle)
			{
				m_Handles.Push(handle);
			}
		}

		internal enum SocketStatus
		{
			SocketNormal = 0,
			SocketNeedsRecreate = 1,
			SocketFailed = 2
		}

		internal struct BaselibData
		{
			public Binding.Baselib_RegisteredNetwork_Socket_UDP m_Socket;

			public SocketStatus m_SocketStatus;

			public Payloads m_PayloadsTx;

			public NetworkInterfaceEndPoint m_LocalEndpoint;

			public long m_LastUpdateTime;

			public long m_LastSocketRecreateTime;

			public uint m_NumSocketRecreate;
		}

		[BurstCompile]
		private struct FlushSendJob : IJob
		{
			public Payloads Tx;

			[NativeDisableContainerSafetyRestriction]
			public NativeArray<BaselibData> Baselib;

			public unsafe void Execute()
			{
				Binding.Baselib_RegisteredNetwork_CompletionResult* ptr = stackalloc Binding.Baselib_RegisteredNetwork_CompletionResult[64];
				Binding.Baselib_ErrorState baselib_ErrorState = default(Binding.Baselib_ErrorState);
				for (int i = 0; i < Tx.Capacity; i++)
				{
					Binding.Baselib_RegisteredNetwork_ProcessStatus baselib_RegisteredNetwork_ProcessStatus = Binding.Baselib_RegisteredNetwork_Socket_UDP_ProcessSend(Baselib[0].m_Socket, &baselib_ErrorState);
					if (baselib_ErrorState.code != 0)
					{
						Debug.LogError($"Error on baselib processing send ({baselib_ErrorState.code})");
						MarkSocketAsNeedingRecreate(Baselib);
						return;
					}
					if (baselib_RegisteredNetwork_ProcessStatus != Binding.Baselib_RegisteredNetwork_ProcessStatus.Pending)
					{
						break;
					}
				}
				int num = 0;
				long num2 = (long)Tx.Capacity / 64L + 1;
				while ((num = (int)Binding.Baselib_RegisteredNetwork_Socket_UDP_DequeueSend(Baselib[0].m_Socket, ptr, 64u, &baselib_ErrorState)) > 0)
				{
					if (baselib_ErrorState.code != 0)
					{
						MarkSocketAsNeedingRecreate(Baselib);
						break;
					}
					for (int j = 0; j < num; j++)
					{
						Tx.ReleaseHandle((int)ptr[j].requestUserdata - 1);
					}
					if (num2-- < 0)
					{
						break;
					}
				}
			}
		}

		[BurstCompile]
		private struct ReceiveJob : IJob
		{
			public NetworkPacketReceiver Receiver;

			public Payloads Rx;

			[NativeDisableContainerSafetyRestriction]
			public NativeArray<BaselibData> Baselib;

			public unsafe void Execute()
			{
				Binding.Baselib_ErrorState baselib_ErrorState = default(Binding.Baselib_ErrorState);
				BaselibData value = Baselib[0];
				value.m_LastUpdateTime = Receiver.LastUpdateTime;
				Baselib[0] = value;
				int num = 0;
				while (Binding.Baselib_RegisteredNetwork_Socket_UDP_ProcessRecv(Baselib[0].m_Socket, &baselib_ErrorState) == Binding.Baselib_RegisteredNetwork_ProcessStatus.Pending && num++ < Rx.Capacity)
				{
				}
				Binding.Baselib_RegisteredNetwork_CompletionResult* ptr = stackalloc Binding.Baselib_RegisteredNetwork_CompletionResult[64];
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				do
				{
					num4 = (int)Binding.Baselib_RegisteredNetwork_Socket_UDP_DequeueRecv(Baselib[0].m_Socket, ptr, 64u, &baselib_ErrorState);
					if (baselib_ErrorState.code != 0)
					{
						Receiver.ReceiveErrorCode = -10;
						return;
					}
					num2 += num4;
					for (int i = 0; i < num4; i++)
					{
						if (ptr[i].status == Binding.Baselib_RegisteredNetwork_CompletionStatus.Failed)
						{
							num3++;
							continue;
						}
						int bytesTransferred = (int)ptr[i].bytesTransferred;
						if (bytesTransferred > 0)
						{
							int handle = (int)ptr[i].requestUserdata - 1;
							Binding.Baselib_RegisteredNetwork_Request requestFromHandle = Rx.GetRequestFromHandle(handle);
							Binding.Baselib_RegisteredNetwork_BufferSlice slice = requestFromHandle.remoteEndpoint.slice;
							NetworkInterfaceEndPoint address = default(NetworkInterfaceEndPoint);
							address.dataLength = (int)slice.size;
							UnsafeUtility.MemCpy(address.data, (void*)slice.data, (int)slice.size);
							Receiver.AppendPacket(requestFromHandle.payload.data, ref address, bytesTransferred);
							Rx.ReleaseHandle(handle);
						}
					}
				}
				while ((long)num4 == 64);
				if (num2 > 0 && num3 == num2)
				{
					MarkSocketAsNeedingRecreate(Baselib);
				}
				int num5 = ScheduleAllReceives(Baselib[0].m_Socket, ref Rx);
				if (num5 < 0)
				{
					Receiver.ReceiveErrorCode = num5;
					MarkSocketAsNeedingRecreate(Baselib);
				}
			}
		}

		public static BaselibNetworkParameter DefaultParameters = new BaselibNetworkParameter
		{
			receiveQueueCapacity = 64,
			sendQueueCapacity = 64,
			maximumPayloadSize = 2000u
		};

		private BaselibNetworkParameter configuration;

		private const int k_defaultRxQueueSize = 64;

		private const int k_defaultTxQueueSize = 64;

		private const int k_defaultMaximumPayloadSize = 2000;

		private const uint k_MaxNumSocketRecreate = 1000u;

		private const uint k_RequestsBatchSize = 64u;

		[ReadOnly]
		internal NativeArray<BaselibData> m_Baselib;

		[NativeDisableContainerSafetyRestriction]
		private Payloads m_PayloadsRx;

		[NativeDisableContainerSafetyRestriction]
		private Payloads m_PayloadsTx;

		private UnsafeBaselibNetworkArray m_LocalAndTempEndpoint;

		private static TransportFunctionPointer<NetworkSendInterface.BeginSendMessageDelegate> BeginSendMessageFunctionPointer = new TransportFunctionPointer<NetworkSendInterface.BeginSendMessageDelegate>(BeginSendMessage);

		private static TransportFunctionPointer<NetworkSendInterface.EndSendMessageDelegate> EndSendMessageFunctionPointer = new TransportFunctionPointer<NetworkSendInterface.EndSendMessageDelegate>(EndSendMessage);

		private static TransportFunctionPointer<NetworkSendInterface.AbortSendMessageDelegate> AbortSendMessageFunctionPointer = new TransportFunctionPointer<NetworkSendInterface.AbortSendMessageDelegate>(AbortSendMessage);

		public NetworkInterfaceEndPoint LocalEndPoint => m_Baselib[0].m_LocalEndpoint;

		public bool IsCreated => m_Baselib.IsCreated;

		public int CreateInterfaceEndPoint(NetworkEndPoint address, out NetworkInterfaceEndPoint endpoint)
		{
			return CreateInterfaceEndPoint(address.rawNetworkAddress, out endpoint);
		}

		private unsafe int CreateInterfaceEndPoint(Binding.Baselib_NetworkAddress address, out NetworkInterfaceEndPoint endpoint)
		{
			Binding.Baselib_RegisteredNetwork_BufferSlice dstSlice = m_LocalAndTempEndpoint.AtIndexAsSlice(0, 28u);
			Binding.Baselib_ErrorState baselib_ErrorState = default(Binding.Baselib_ErrorState);
			endpoint = default(NetworkInterfaceEndPoint);
			Binding.Baselib_RegisteredNetwork_Endpoint baselib_RegisteredNetwork_Endpoint = Binding.Baselib_RegisteredNetwork_Endpoint_Create(&address, dstSlice, &baselib_ErrorState);
			if (baselib_ErrorState.code != 0)
			{
				return (int)baselib_ErrorState.code;
			}
			endpoint.dataLength = (int)baselib_RegisteredNetwork_Endpoint.slice.size;
			fixed (byte* ptr = endpoint.data)
			{
				void* destination = ptr;
				UnsafeUtility.MemCpy(destination, (void*)baselib_RegisteredNetwork_Endpoint.slice.data, endpoint.dataLength);
			}
			return 0;
		}

		private unsafe NetworkInterfaceEndPoint GetLocalEndPoint(Binding.Baselib_RegisteredNetwork_Socket_UDP socket)
		{
			Binding.Baselib_ErrorState baselib_ErrorState = default(Binding.Baselib_ErrorState);
			Binding.Baselib_NetworkAddress address = default(Binding.Baselib_NetworkAddress);
			Binding.Baselib_RegisteredNetwork_Socket_UDP_GetNetworkAddress(socket, &address, &baselib_ErrorState);
			NetworkInterfaceEndPoint endpoint = default(NetworkInterfaceEndPoint);
			if (baselib_ErrorState.code != 0)
			{
				return endpoint;
			}
			CreateInterfaceEndPoint(address, out endpoint);
			return endpoint;
		}

		public unsafe NetworkEndPoint GetGenericEndPoint(NetworkInterfaceEndPoint endpoint)
		{
			NetworkEndPoint result = default(NetworkEndPoint);
			Binding.Baselib_ErrorState baselib_ErrorState = default(Binding.Baselib_ErrorState);
			Binding.Baselib_RegisteredNetwork_BufferSlice slice = m_LocalAndTempEndpoint.AtIndexAsSlice(0, 28u);
			Binding.Baselib_RegisteredNetwork_Endpoint endpoint2 = default(Binding.Baselib_RegisteredNetwork_Endpoint);
			endpoint2.slice = slice;
			endpoint2.slice.size = (uint)endpoint.dataLength;
			UnsafeUtility.MemCpy((void*)endpoint2.slice.data, endpoint.data, endpoint.dataLength);
			Binding.Baselib_RegisteredNetwork_Endpoint_GetNetworkAddress(endpoint2, &result.rawNetworkAddress, &baselib_ErrorState);
			if (baselib_ErrorState.code != 0)
			{
				return default(NetworkEndPoint);
			}
			return result;
		}

		public int Initialize(NetworkSettings settings)
		{
			configuration = settings.GetBaselibNetworkInterfaceParameters();
			m_Baselib = new NativeArray<BaselibData>(1, Allocator.Persistent);
			BaselibData value = default(BaselibData);
			m_PayloadsTx = new Payloads(configuration.sendQueueCapacity, configuration.maximumPayloadSize);
			m_PayloadsRx = new Payloads(configuration.receiveQueueCapacity, configuration.maximumPayloadSize);
			m_LocalAndTempEndpoint = new UnsafeBaselibNetworkArray(2, 28);
			value.m_PayloadsTx = m_PayloadsTx;
			m_Baselib[0] = value;
			return 0;
		}

		public void Dispose()
		{
			if (m_Baselib[0].m_Socket.handle != IntPtr.Zero)
			{
				Binding.Baselib_RegisteredNetwork_Socket_UDP_Close(m_Baselib[0].m_Socket);
			}
			m_LocalAndTempEndpoint.Dispose();
			if (m_PayloadsTx.IsCreated)
			{
				m_PayloadsTx.Dispose();
			}
			if (m_PayloadsRx.IsCreated)
			{
				m_PayloadsRx.Dispose();
			}
			m_Baselib.Dispose();
		}

		private static void MarkSocketAsNeedingRecreate(NativeArray<BaselibData> baselib)
		{
			BaselibData value = baselib[0];
			value.m_SocketStatus = SocketStatus.SocketNeedsRecreate;
			baselib[0] = value;
		}

		private void RecreateSocket(long updateTime)
		{
			BaselibData value = m_Baselib[0];
			if (value.m_LastSocketRecreateTime == value.m_LastUpdateTime || value.m_NumSocketRecreate >= 1000)
			{
				Debug.LogError("Unrecoverable socket failure. An unknown condition is preventing the application from reliably creating sockets.");
				value.m_SocketStatus = SocketStatus.SocketFailed;
				m_Baselib[0] = value;
				return;
			}
			Debug.LogWarning("Socket error encountered; attempting recovery by creating a new one.");
			Bind(value.m_LocalEndpoint);
			value = m_Baselib[0];
			value.m_LastSocketRecreateTime = updateTime;
			value.m_NumSocketRecreate++;
			m_Baselib[0] = value;
		}

		public JobHandle ScheduleReceive(NetworkPacketReceiver receiver, JobHandle dep)
		{
			if (m_Baselib[0].m_SocketStatus == SocketStatus.SocketNeedsRecreate)
			{
				RecreateSocket(receiver.LastUpdateTime);
			}
			if (m_Baselib[0].m_SocketStatus == SocketStatus.SocketFailed)
			{
				receiver.ReceiveErrorCode = -10;
				return dep;
			}
			ReceiveJob jobData = default(ReceiveJob);
			jobData.Baselib = m_Baselib;
			jobData.Rx = m_PayloadsRx;
			jobData.Receiver = receiver;
			return IJobExtensions.Schedule(jobData, dep);
		}

		public JobHandle ScheduleSend(NativeQueue<QueuedSendMessage> sendQueue, JobHandle dep)
		{
			if (m_Baselib[0].m_SocketStatus != 0)
			{
				return dep;
			}
			FlushSendJob jobData = default(FlushSendJob);
			jobData.Baselib = m_Baselib;
			jobData.Tx = m_PayloadsTx;
			return IJobExtensions.Schedule(jobData, dep);
		}

		public unsafe int Bind(NetworkInterfaceEndPoint endpoint)
		{
			BaselibData value = m_Baselib[0];
			Binding.Baselib_RegisteredNetwork_BufferSlice slice = m_LocalAndTempEndpoint.AtIndexAsSlice(0, 28u);
			UnsafeUtility.MemCpy((void*)slice.data, endpoint.data, endpoint.dataLength);
			Binding.Baselib_ErrorState baselib_ErrorState = default(Binding.Baselib_ErrorState);
			Binding.Baselib_RegisteredNetwork_Endpoint endpoint2 = default(Binding.Baselib_RegisteredNetwork_Endpoint);
			endpoint2.slice = slice;
			Binding.Baselib_NetworkAddress address = default(Binding.Baselib_NetworkAddress);
			Binding.Baselib_RegisteredNetwork_Endpoint_GetNetworkAddress(endpoint2, &address, &baselib_ErrorState);
			bool flag = WouldBindFailWithoutAddressReuse(address);
			Binding.Baselib_RegisteredNetwork_Socket_UDP socket = checked(Binding.Baselib_RegisteredNetwork_Socket_UDP_Create(&address, Binding.Baselib_NetworkAddress_AddressReuse.Allow, (uint)configuration.sendQueueCapacity, (uint)configuration.receiveQueueCapacity, &baselib_ErrorState));
			if (baselib_ErrorState.code != 0)
			{
				if (baselib_ErrorState.code == Binding.Baselib_ErrorCode.AddressInUse)
				{
					Debug.LogError("Failed to bind the socket because address is already in use. It is likely that another process is already listening on the same port.");
				}
				if (baselib_ErrorState.code != Binding.Baselib_ErrorCode.UnexpectedError)
				{
					return 0 - baselib_ErrorState.code;
				}
				return -10;
			}
			if (m_Baselib[0].m_Socket.handle != IntPtr.Zero)
			{
				Binding.Baselib_RegisteredNetwork_Socket_UDP_Close(m_Baselib[0].m_Socket);
				m_PayloadsRx.Dispose();
				m_PayloadsRx = new Payloads(configuration.receiveQueueCapacity, configuration.maximumPayloadSize);
			}
			int num = ScheduleAllReceives(socket, ref m_PayloadsRx);
			if (num < 0)
			{
				return num;
			}
			if (value.m_SocketStatus != SocketStatus.SocketNeedsRecreate && flag)
			{
				ushort port = GetGenericEndPoint(endpoint).Port;
				Debug.LogWarning($"Port {port} is likely already in use by another application. " + "Socket was still created, but expect erroneous behavior. This condition will become a failure starting in version 2.0 of Unity Transport.");
			}
			value.m_Socket = socket;
			value.m_SocketStatus = SocketStatus.SocketNormal;
			value.m_LocalEndpoint = GetLocalEndPoint(socket);
			m_Baselib[0] = value;
			return 0;
		}

		private unsafe bool WouldBindFailWithoutAddressReuse(Binding.Baselib_NetworkAddress address)
		{
			Binding.Baselib_ErrorState baselib_ErrorState = default(Binding.Baselib_ErrorState);
			Binding.Baselib_RegisteredNetwork_Socket_UDP socket = checked(Binding.Baselib_RegisteredNetwork_Socket_UDP_Create(&address, Binding.Baselib_NetworkAddress_AddressReuse.DoNotAllow, (uint)configuration.sendQueueCapacity, (uint)configuration.receiveQueueCapacity, &baselib_ErrorState));
			if (baselib_ErrorState.code == Binding.Baselib_ErrorCode.Success)
			{
				Binding.Baselib_RegisteredNetwork_Socket_UDP_Close(socket);
			}
			return baselib_ErrorState.code == Binding.Baselib_ErrorCode.AddressInUse;
		}

		public int Listen()
		{
			return 0;
		}

		public unsafe NetworkSendInterface CreateSendInterface()
		{
			NetworkSendInterface result = default(NetworkSendInterface);
			result.BeginSendMessage = BeginSendMessageFunctionPointer;
			result.EndSendMessage = EndSendMessageFunctionPointer;
			result.AbortSendMessage = AbortSendMessageFunctionPointer;
			result.UserData = (IntPtr)m_Baselib.GetUnsafePtr();
			return result;
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkSendInterface.BeginSendMessageDelegate))]
		private unsafe static int BeginSendMessage(out NetworkInterfaceSendHandle handle, IntPtr userData, int requiredPayloadSize)
		{
			BaselibData* ptr = (BaselibData*)(void*)userData;
			handle = default(NetworkInterfaceSendHandle);
			int num = ptr->m_PayloadsTx.AcquireHandle();
			if (num < 0)
			{
				return -5;
			}
			Binding.Baselib_RegisteredNetwork_Request requestFromHandle = ptr->m_PayloadsTx.GetRequestFromHandle(num);
			if ((int)requestFromHandle.payload.size < requiredPayloadSize)
			{
				ptr->m_PayloadsTx.ReleaseHandle(num);
				return -4;
			}
			handle.id = num;
			handle.size = 0;
			handle.data = requestFromHandle.payload.data;
			handle.capacity = (int)requestFromHandle.payload.size;
			return 0;
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkSendInterface.EndSendMessageDelegate))]
		private unsafe static int EndSendMessage(ref NetworkInterfaceSendHandle handle, ref NetworkInterfaceEndPoint address, IntPtr userData, ref NetworkSendQueueHandle sendQueueHandle)
		{
			BaselibData* ptr = (BaselibData*)(void*)userData;
			int id = handle.id;
			Binding.Baselib_RegisteredNetwork_Request requestFromHandle = ptr->m_PayloadsTx.GetRequestFromHandle(id);
			requestFromHandle.requestUserdata = (IntPtr)(id + 1);
			requestFromHandle.payload.size = (uint)handle.size;
			NetworkInterfaceEndPoint networkInterfaceEndPoint = address;
			UnsafeUtility.MemCpy((void*)requestFromHandle.remoteEndpoint.slice.data, networkInterfaceEndPoint.data, address.dataLength);
			Binding.Baselib_RegisteredNetwork_Request* requests = &requestFromHandle;
			Binding.Baselib_ErrorState baselib_ErrorState = default(Binding.Baselib_ErrorState);
			int num = (int)Binding.Baselib_RegisteredNetwork_Socket_UDP_ScheduleSend(ptr->m_Socket, requests, 1u, &baselib_ErrorState);
			if (baselib_ErrorState.code != 0 || (long)num != 1)
			{
				ptr->m_PayloadsTx.ReleaseHandle(id);
				return -5;
			}
			return handle.size;
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkSendInterface.AbortSendMessageDelegate))]
		private unsafe static void AbortSendMessage(ref NetworkInterfaceSendHandle handle, IntPtr userData)
		{
			BaselibData* ptr = (BaselibData*)(void*)userData;
			int id = handle.id;
			ptr->m_PayloadsTx.ReleaseHandle(id);
		}

		private unsafe static int ScheduleAllReceives(Binding.Baselib_RegisteredNetwork_Socket_UDP socket, ref Payloads PayloadsRx)
		{
			Binding.Baselib_ErrorState baselib_ErrorState = default(Binding.Baselib_ErrorState);
			Binding.Baselib_RegisteredNetwork_Request* ptr = stackalloc Binding.Baselib_RegisteredNetwork_Request[64];
			int num = 0;
			do
			{
				for (num = 0; (long)num < 64L; num++)
				{
					if (PayloadsRx.InUse >= PayloadsRx.Capacity)
					{
						break;
					}
					int num2 = PayloadsRx.AcquireHandle();
					ptr[num] = PayloadsRx.GetRequestFromHandle(num2);
					ptr[num].requestUserdata = (IntPtr)num2 + 1;
				}
				if (num <= 0)
				{
					continue;
				}
				Binding.Baselib_RegisteredNetwork_Socket_UDP_ScheduleRecv(socket, ptr, (uint)num, &baselib_ErrorState);
				if (baselib_ErrorState.code != 0)
				{
					if (baselib_ErrorState.code != Binding.Baselib_ErrorCode.UnexpectedError)
					{
						return 0 - baselib_ErrorState.code;
					}
					return -10;
				}
			}
			while ((long)num == 64);
			return 0;
		}

		private bool ValidateParameters(BaselibNetworkParameter param)
		{
			if (param.receiveQueueCapacity <= 0)
			{
				return false;
			}
			if (param.sendQueueCapacity <= 0)
			{
				return false;
			}
			return true;
		}

		private bool TryExtractParameters(out BaselibNetworkParameter config, params INetworkParameter[] param)
		{
			for (int i = 0; i < param.Length; i++)
			{
				if (param[i] is BaselibNetworkParameter && ValidateParameters((BaselibNetworkParameter)(object)param[i]))
				{
					config = (BaselibNetworkParameter)(object)param[i];
					return true;
				}
			}
			config = default(BaselibNetworkParameter);
			return false;
		}
	}
}
