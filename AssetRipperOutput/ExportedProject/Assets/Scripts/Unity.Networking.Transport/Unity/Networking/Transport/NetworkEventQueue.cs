using System;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Networking.Transport
{
	internal struct NetworkEventQueue : IDisposable
	{
		private struct SubQueueItem
		{
			public int connection;

			public int idx;
		}

		public struct Concurrent
		{
			[NativeContainer]
			[NativeContainerIsAtomicWriteOnly]
			internal struct ConcurrentConnectionQueue
			{
				[NativeDisableUnsafePtrRestriction]
				private unsafe UnsafeList<int>* m_ConnectionEventHeadTail;

				public unsafe int Length => m_ConnectionEventHeadTail->Length;

				public unsafe ConcurrentConnectionQueue(NativeList<int> queue)
				{
					m_ConnectionEventHeadTail = (UnsafeList<int>*)NativeListUnsafeUtility.GetInternalListDataPtrUnchecked(ref queue);
				}

				public unsafe int Dequeue(int connectionId)
				{
					int num = -1;
					if (connectionId < 0 || connectionId >= m_ConnectionEventHeadTail->Length / 2)
					{
						return -1;
					}
					while (num < 0)
					{
						num = m_ConnectionEventHeadTail->Ptr[connectionId * 2];
						if (num >= m_ConnectionEventHeadTail->Ptr[connectionId * 2 + 1])
						{
							return -1;
						}
						if (Interlocked.CompareExchange(ref m_ConnectionEventHeadTail->Ptr[connectionId * 2], num + 1, num) != num)
						{
							num = -1;
						}
					}
					return num;
				}
			}

			[ReadOnly]
			internal NativeList<NetworkEvent> m_ConnectionEventQ;

			internal ConcurrentConnectionQueue m_ConnectionEventHeadTail;

			private int MaxEvents => m_ConnectionEventQ.Length / (m_ConnectionEventHeadTail.Length / 2);

			public NetworkEvent.Type PopEventForConnection(int connectionId, out int offset, out int size)
			{
				int pipelineId;
				return PopEventForConnection(connectionId, out offset, out size, out pipelineId);
			}

			public NetworkEvent.Type PopEventForConnection(int connectionId, out int offset, out int size, out int pipelineId)
			{
				offset = 0;
				size = 0;
				pipelineId = 0;
				int num = m_ConnectionEventHeadTail.Dequeue(connectionId);
				if (num < 0)
				{
					return NetworkEvent.Type.Empty;
				}
				NetworkEvent networkEvent = m_ConnectionEventQ[connectionId * MaxEvents + num];
				pipelineId = networkEvent.pipelineId;
				if (networkEvent.type == NetworkEvent.Type.Data)
				{
					offset = networkEvent.offset;
					size = networkEvent.size;
				}
				else if (networkEvent.type == NetworkEvent.Type.Disconnect && networkEvent.status != 0)
				{
					offset = -networkEvent.status;
				}
				return networkEvent.type;
			}
		}

		private NativeQueue<SubQueueItem> m_MasterEventQ;

		private NativeList<NetworkEvent> m_ConnectionEventQ;

		private NativeList<int> m_ConnectionEventHeadTail;

		private int MaxEvents => m_ConnectionEventQ.Length / (m_ConnectionEventHeadTail.Length / 2);

		public NetworkEventQueue(int queueSizePerConnection)
		{
			m_MasterEventQ = new NativeQueue<SubQueueItem>(Allocator.Persistent);
			m_ConnectionEventQ = new NativeList<NetworkEvent>(queueSizePerConnection, Allocator.Persistent);
			m_ConnectionEventHeadTail = new NativeList<int>(2, Allocator.Persistent);
			m_ConnectionEventQ.ResizeUninitialized(queueSizePerConnection);
			ref NativeList<int> connectionEventHeadTail = ref m_ConnectionEventHeadTail;
			int value = 0;
			connectionEventHeadTail.Add(in value);
			ref NativeList<int> connectionEventHeadTail2 = ref m_ConnectionEventHeadTail;
			value = 0;
			connectionEventHeadTail2.Add(in value);
		}

		public void Dispose()
		{
			m_MasterEventQ.Dispose();
			m_ConnectionEventQ.Dispose();
			m_ConnectionEventHeadTail.Dispose();
		}

		public NetworkEvent.Type PopEvent(out int id, out int offset, out int size)
		{
			int pipelineId;
			return PopEvent(out id, out offset, out size, out pipelineId);
		}

		public NetworkEvent.Type PopEvent(out int id, out int offset, out int size, out int pipelineId)
		{
			offset = 0;
			size = 0;
			id = -1;
			pipelineId = 0;
			SubQueueItem item;
			do
			{
				if (!m_MasterEventQ.TryDequeue(out item))
				{
					return NetworkEvent.Type.Empty;
				}
			}
			while (m_ConnectionEventHeadTail[item.connection * 2] != item.idx);
			id = item.connection;
			return PopEventForConnection(item.connection, out offset, out size, out pipelineId);
		}

		public NetworkEvent.Type PopEventForConnection(int connectionId, out int offset, out int size)
		{
			int pipelineId;
			return PopEventForConnection(connectionId, out offset, out size, out pipelineId);
		}

		public NetworkEvent.Type PopEventForConnection(int connectionId, out int offset, out int size, out int pipelineId)
		{
			offset = 0;
			size = 0;
			pipelineId = 0;
			if (connectionId < 0 || connectionId >= m_ConnectionEventHeadTail.Length / 2)
			{
				return NetworkEvent.Type.Empty;
			}
			int num = m_ConnectionEventHeadTail[connectionId * 2];
			if (num >= m_ConnectionEventHeadTail[connectionId * 2 + 1])
			{
				return NetworkEvent.Type.Empty;
			}
			m_ConnectionEventHeadTail[connectionId * 2] = num + 1;
			NetworkEvent networkEvent = m_ConnectionEventQ[connectionId * MaxEvents + num];
			pipelineId = networkEvent.pipelineId;
			if (networkEvent.type == NetworkEvent.Type.Data)
			{
				offset = networkEvent.offset;
				size = networkEvent.size;
			}
			else if (networkEvent.type == NetworkEvent.Type.Disconnect && networkEvent.status != 0)
			{
				offset = -networkEvent.status;
			}
			return networkEvent.type;
		}

		public int GetCountForConnection(int connectionId)
		{
			if (connectionId < 0 || connectionId >= m_ConnectionEventHeadTail.Length / 2)
			{
				return 0;
			}
			return m_ConnectionEventHeadTail[connectionId * 2 + 1] - m_ConnectionEventHeadTail[connectionId * 2];
		}

		public void PushEvent(NetworkEvent ev)
		{
			int num = MaxEvents;
			if (ev.connectionId >= m_ConnectionEventHeadTail.Length / 2)
			{
				int i = m_ConnectionEventHeadTail.Length;
				m_ConnectionEventHeadTail.ResizeUninitialized((ev.connectionId + 1) * 2);
				for (; i < m_ConnectionEventHeadTail.Length; i++)
				{
					m_ConnectionEventHeadTail[i] = 0;
				}
				m_ConnectionEventQ.ResizeUninitialized(m_ConnectionEventHeadTail.Length / 2 * num);
			}
			int num2 = m_ConnectionEventHeadTail[ev.connectionId * 2 + 1];
			if (num2 >= num)
			{
				int num3 = num;
				while (num2 >= num)
				{
					num *= 2;
				}
				int num4 = m_ConnectionEventHeadTail.Length / 2;
				m_ConnectionEventQ.ResizeUninitialized(num4 * num);
				for (int num5 = num4 - 1; num5 >= 0; num5--)
				{
					for (int num6 = m_ConnectionEventHeadTail[num5 * 2 + 1] - 1; num6 >= m_ConnectionEventHeadTail[num5 * 2]; num6--)
					{
						m_ConnectionEventQ[num5 * num + num6] = m_ConnectionEventQ[num5 * num3 + num6];
					}
				}
			}
			m_ConnectionEventQ[ev.connectionId * num + num2] = ev;
			m_ConnectionEventHeadTail[ev.connectionId * 2 + 1] = num2 + 1;
			m_MasterEventQ.Enqueue(new SubQueueItem
			{
				connection = ev.connectionId,
				idx = num2
			});
		}

		internal void Clear()
		{
			m_MasterEventQ.Clear();
			for (int i = 0; i < m_ConnectionEventHeadTail.Length; i++)
			{
				m_ConnectionEventHeadTail[i] = 0;
			}
		}

		public Concurrent ToConcurrent()
		{
			Concurrent result = default(Concurrent);
			result.m_ConnectionEventQ = m_ConnectionEventQ;
			result.m_ConnectionEventHeadTail = new Concurrent.ConcurrentConnectionQueue(m_ConnectionEventHeadTail);
			return result;
		}
	}
}
