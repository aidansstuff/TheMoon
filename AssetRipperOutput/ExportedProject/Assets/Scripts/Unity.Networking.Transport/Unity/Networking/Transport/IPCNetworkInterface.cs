using System;
using AOT;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Networking.Transport
{
	[BurstCompile]
	public struct IPCNetworkInterface : INetworkInterface, IDisposable
	{
		[BurstCompile]
		private struct SendUpdate : IJob
		{
			public IPCManager ipcManager;

			public NativeQueue<QueuedSendMessage> ipcQueue;

			[ReadOnly]
			public NativeArray<NetworkInterfaceEndPoint> localEndPoint;

			public void Execute()
			{
				ipcManager.Update(localEndPoint[0], ipcQueue);
			}
		}

		[BurstCompile]
		private struct ReceiveJob : IJob
		{
			public NetworkPacketReceiver receiver;

			public IPCManager ipcManager;

			public NetworkInterfaceEndPoint localEndPoint;

			public unsafe void Execute()
			{
				receiver.ReceiveErrorCode = 0;
				IntPtr intPtr;
				NetworkInterfaceEndPoint address;
				int num;
				do
				{
					int dataLen = 1472;
					intPtr = receiver.AllocateMemory(ref dataLen);
					if (intPtr == IntPtr.Zero)
					{
						break;
					}
					address = default(NetworkInterfaceEndPoint);
					num = NativeReceive(intPtr.ToPointer(), dataLen, ref address);
					if (num <= 0)
					{
						if (num != 0)
						{
							receiver.ReceiveErrorCode = -num;
						}
						break;
					}
				}
				while (receiver.AppendPacket(intPtr, ref address, num, NetworkPacketReceiver.AppendPacketMode.NoCopyNeeded));
			}

			private unsafe int NativeReceive(void* data, int length, ref NetworkInterfaceEndPoint address)
			{
				return ipcManager.ReceiveMessageEx(localEndPoint, data, length, ref address);
			}
		}

		[ReadOnly]
		private NativeArray<NetworkInterfaceEndPoint> m_LocalEndPoint;

		private static TransportFunctionPointer<NetworkSendInterface.BeginSendMessageDelegate> BeginSendMessageFunctionPointer = new TransportFunctionPointer<NetworkSendInterface.BeginSendMessageDelegate>(BeginSendMessage);

		private static TransportFunctionPointer<NetworkSendInterface.EndSendMessageDelegate> EndSendMessageFunctionPointer = new TransportFunctionPointer<NetworkSendInterface.EndSendMessageDelegate>(EndSendMessage);

		private static TransportFunctionPointer<NetworkSendInterface.AbortSendMessageDelegate> AbortSendMessageFunctionPointer = new TransportFunctionPointer<NetworkSendInterface.AbortSendMessageDelegate>(AbortSendMessage);

		public NetworkInterfaceEndPoint LocalEndPoint => m_LocalEndPoint[0];

		public int CreateInterfaceEndPoint(NetworkEndPoint address, out NetworkInterfaceEndPoint endpoint)
		{
			if (!address.IsLoopback && !address.IsAny)
			{
				endpoint = default(NetworkInterfaceEndPoint);
				return -9;
			}
			endpoint = IPCManager.Instance.CreateEndPoint(address.Port);
			return 0;
		}

		public NetworkEndPoint GetGenericEndPoint(NetworkInterfaceEndPoint endpoint)
		{
			if (!IPCManager.Instance.GetEndPointPort(endpoint, out var port))
			{
				return default(NetworkEndPoint);
			}
			return NetworkEndPoint.LoopbackIpv4.WithPort(port);
		}

		public int Initialize(NetworkSettings settings)
		{
			IPCManager.Instance.AddRef();
			m_LocalEndPoint = new NativeArray<NetworkInterfaceEndPoint>(1, Allocator.Persistent);
			NetworkInterfaceEndPoint endpoint = default(NetworkInterfaceEndPoint);
			int num = 0;
			if ((num = CreateInterfaceEndPoint(NetworkEndPoint.LoopbackIpv4, out endpoint)) != 0)
			{
				return num;
			}
			m_LocalEndPoint[0] = endpoint;
			return 0;
		}

		public void Dispose()
		{
			m_LocalEndPoint.Dispose();
			IPCManager.Instance.Release();
		}

		public JobHandle ScheduleReceive(NetworkPacketReceiver receiver, JobHandle dep)
		{
			ReceiveJob jobData = default(ReceiveJob);
			jobData.receiver = receiver;
			jobData.ipcManager = IPCManager.Instance;
			jobData.localEndPoint = LocalEndPoint;
			dep = IJobExtensions.Schedule(jobData, JobHandle.CombineDependencies(dep, IPCManager.ManagerAccessHandle));
			IPCManager.ManagerAccessHandle = dep;
			return dep;
		}

		public JobHandle ScheduleSend(NativeQueue<QueuedSendMessage> sendQueue, JobHandle dep)
		{
			SendUpdate jobData = default(SendUpdate);
			jobData.ipcManager = IPCManager.Instance;
			jobData.ipcQueue = sendQueue;
			jobData.localEndPoint = m_LocalEndPoint;
			dep = IJobExtensions.Schedule(jobData, JobHandle.CombineDependencies(dep, IPCManager.ManagerAccessHandle));
			IPCManager.ManagerAccessHandle = dep;
			return dep;
		}

		public int Bind(NetworkInterfaceEndPoint endpoint)
		{
			m_LocalEndPoint[0] = endpoint;
			return 0;
		}

		public int Listen()
		{
			return 0;
		}

		public NetworkSendInterface CreateSendInterface()
		{
			NetworkSendInterface result = default(NetworkSendInterface);
			result.BeginSendMessage = BeginSendMessageFunctionPointer;
			result.EndSendMessage = EndSendMessageFunctionPointer;
			result.AbortSendMessage = AbortSendMessageFunctionPointer;
			return result;
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkSendInterface.BeginSendMessageDelegate))]
		private unsafe static int BeginSendMessage(out NetworkInterfaceSendHandle handle, IntPtr userData, int requiredPayloadSize)
		{
			handle.id = 0;
			handle.size = 0;
			handle.capacity = requiredPayloadSize;
			handle.data = (IntPtr)UnsafeUtility.Malloc(handle.capacity, 8, Allocator.Temp);
			handle.flags = (SendHandleFlags)0;
			return 0;
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkSendInterface.EndSendMessageDelegate))]
		private unsafe static int EndSendMessage(ref NetworkInterfaceSendHandle handle, ref NetworkInterfaceEndPoint address, IntPtr userData, ref NetworkSendQueueHandle sendQueueHandle)
		{
			NativeQueue<QueuedSendMessage>.ParallelWriter parallelWriter = sendQueueHandle.FromHandle();
			QueuedSendMessage value = default(QueuedSendMessage);
			value.Dest = address;
			value.DataLength = handle.size;
			UnsafeUtility.MemCpy(value.Data, (void*)handle.data, handle.size);
			parallelWriter.Enqueue(value);
			return handle.size;
		}

		[BurstCompile(DisableDirectCall = true)]
		[MonoPInvokeCallback(typeof(NetworkSendInterface.AbortSendMessageDelegate))]
		private static void AbortSendMessage(ref NetworkInterfaceSendHandle handle, IntPtr userData)
		{
		}
	}
}
