using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Networking.Transport.Utilities;

namespace Unity.Networking.Transport
{
	internal struct IPCManager
	{
		[StructLayout(LayoutKind.Explicit)]
		internal struct IPCData
		{
			[FieldOffset(0)]
			public int from;

			[FieldOffset(4)]
			public int length;

			[FieldOffset(8)]
			public unsafe fixed byte data[1472];
		}

		public static IPCManager Instance;

		private NativeMultiQueue<IPCData> m_IPCQueue;

		private NativeHashMap<ushort, int> m_IPCChannels;

		internal static JobHandle ManagerAccessHandle;

		private int m_RefCount;

		public bool IsCreated => m_IPCQueue.IsCreated;

		public void AddRef()
		{
			if (m_RefCount == 0)
			{
				m_IPCQueue = new NativeMultiQueue<IPCData>(128);
				m_IPCChannels = new NativeHashMap<ushort, int>(64, Allocator.Persistent);
			}
			m_RefCount++;
		}

		public void Release()
		{
			m_RefCount--;
			if (m_RefCount == 0)
			{
				ManagerAccessHandle.Complete();
				m_IPCQueue.Dispose();
				m_IPCChannels.Dispose();
			}
		}

		internal unsafe void Update(NetworkInterfaceEndPoint local, NativeQueue<QueuedSendMessage> queue)
		{
			QueuedSendMessage item;
			while (queue.TryDequeue(out item))
			{
				IPCData value = default(IPCData);
				UnsafeUtility.MemCpy(value.data, item.Data, item.DataLength);
				value.length = item.DataLength;
				value.from = *(int*)local.data;
				m_IPCQueue.Enqueue(*(int*)item.Dest.data, value);
			}
		}

		public unsafe NetworkInterfaceEndPoint CreateEndPoint(ushort port)
		{
			ManagerAccessHandle.Complete();
			int item = 0;
			if (port == 0)
			{
				while (item == 0)
				{
					port = RandomHelpers.GetRandomUShort();
					if (!m_IPCChannels.TryGetValue(port, out var _))
					{
						item = m_IPCChannels.Count() + 1;
						m_IPCChannels.TryAdd(port, item);
					}
				}
			}
			else if (!m_IPCChannels.TryGetValue(port, out item))
			{
				item = m_IPCChannels.Count() + 1;
				m_IPCChannels.TryAdd(port, item);
			}
			NetworkInterfaceEndPoint result = default(NetworkInterfaceEndPoint);
			result.dataLength = 4;
			*(int*)result.data = item;
			return result;
		}

		public unsafe bool GetEndPointPort(NetworkInterfaceEndPoint ep, out ushort port)
		{
			ManagerAccessHandle.Complete();
			int num = *(int*)ep.data;
			NativeArray<int> valueArray = m_IPCChannels.GetValueArray(Allocator.Temp);
			NativeArray<ushort> keyArray = m_IPCChannels.GetKeyArray(Allocator.Temp);
			port = 0;
			for (int i = 0; i < m_IPCChannels.Count(); i++)
			{
				if (valueArray[i] == num)
				{
					port = keyArray[i];
					return true;
				}
			}
			return false;
		}

		public unsafe int PeekNext(NetworkInterfaceEndPoint local, void* slice, out int length, out NetworkInterfaceEndPoint from)
		{
			ManagerAccessHandle.Complete();
			from = default(NetworkInterfaceEndPoint);
			length = 0;
			if (m_IPCQueue.Peek(*(int*)local.data, out var value))
			{
				UnsafeUtility.MemCpy(slice, value.data, value.length);
				length = value.length;
			}
			GetEndPointByHandle(value.from, out from);
			return length;
		}

		public unsafe int ReceiveMessageEx(NetworkInterfaceEndPoint local, void* payloadData, int payloadLen, ref NetworkInterfaceEndPoint remote)
		{
			if (!m_IPCQueue.Peek(*(int*)local.data, out var value))
			{
				return 0;
			}
			GetEndPointByHandle(value.from, out remote);
			int num = Math.Min(payloadLen, value.length);
			UnsafeUtility.MemCpy(payloadData, value.data, num);
			if (num < value.length)
			{
				return -10040;
			}
			m_IPCQueue.Dequeue(*(int*)local.data, out value);
			return num;
		}

		private unsafe void GetEndPointByHandle(int handle, out NetworkInterfaceEndPoint endpoint)
		{
			NetworkInterfaceEndPoint networkInterfaceEndPoint = default(NetworkInterfaceEndPoint);
			networkInterfaceEndPoint.dataLength = 4;
			*(int*)networkInterfaceEndPoint.data = handle;
			endpoint = networkInterfaceEndPoint;
		}
	}
}
