using System;
using System.Diagnostics;
using System.Threading;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;

namespace Unity.Collections
{
	[NativeContainer]
	[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
	public struct NativeQueue<T> : INativeDisposable, IDisposable where T : struct
	{
		[NativeContainer]
		[NativeContainerIsAtomicWriteOnly]
		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public struct ParallelWriter
		{
			[NativeDisableUnsafePtrRestriction]
			internal unsafe NativeQueueData* m_Buffer;

			[NativeDisableUnsafePtrRestriction]
			internal unsafe NativeQueueBlockPoolData* m_QueuePool;

			[NativeSetThreadIndex]
			internal int m_ThreadIndex;

			public unsafe void Enqueue(T value)
			{
				NativeQueueBlockHeader* ptr = NativeQueueData.AllocateWriteBlockMT<T>(m_Buffer, m_QueuePool, m_ThreadIndex);
				UnsafeUtility.WriteArrayElement(ptr + 1, ptr->m_NumItems, value);
				ptr->m_NumItems++;
			}
		}

		[NativeDisableUnsafePtrRestriction]
		private unsafe NativeQueueData* m_Buffer;

		[NativeDisableUnsafePtrRestriction]
		private unsafe NativeQueueBlockPoolData* m_QueuePool;

		private AllocatorManager.AllocatorHandle m_AllocatorLabel;

		public unsafe int Count
		{
			get
			{
				int num = 0;
				for (NativeQueueBlockHeader* ptr = (NativeQueueBlockHeader*)(void*)m_Buffer->m_FirstBlock; ptr != null; ptr = ptr->m_NextBlock)
				{
					num += ptr->m_NumItems;
				}
				return num - m_Buffer->m_CurrentRead;
			}
		}

		internal unsafe static int PersistentMemoryBlockCount
		{
			get
			{
				return NativeQueueBlockPool.GetQueueBlockPool()->m_MaxBlocks;
			}
			set
			{
				Interlocked.Exchange(ref NativeQueueBlockPool.GetQueueBlockPool()->m_MaxBlocks, value);
			}
		}

		internal static int MemoryBlockSize => 16384;

		public unsafe bool IsCreated => m_Buffer != null;

		public unsafe NativeQueue(AllocatorManager.AllocatorHandle allocator)
		{
			m_QueuePool = NativeQueueBlockPool.GetQueueBlockPool();
			m_AllocatorLabel = allocator;
			NativeQueueData.AllocateQueue<T>(allocator, out m_Buffer);
		}

		public unsafe bool IsEmpty()
		{
			if (!IsCreated)
			{
				return true;
			}
			int num = 0;
			int currentRead = m_Buffer->m_CurrentRead;
			for (NativeQueueBlockHeader* ptr = (NativeQueueBlockHeader*)(void*)m_Buffer->m_FirstBlock; ptr != null; ptr = ptr->m_NextBlock)
			{
				num += ptr->m_NumItems;
				if (num > currentRead)
				{
					return false;
				}
			}
			return num == currentRead;
		}

		public unsafe T Peek()
		{
			NativeQueueBlockHeader* ptr = (NativeQueueBlockHeader*)(void*)m_Buffer->m_FirstBlock;
			return UnsafeUtility.ReadArrayElement<T>(ptr + 1, m_Buffer->m_CurrentRead);
		}

		public unsafe void Enqueue(T value)
		{
			NativeQueueBlockHeader* ptr = NativeQueueData.AllocateWriteBlockMT<T>(m_Buffer, m_QueuePool, 0);
			UnsafeUtility.WriteArrayElement(ptr + 1, ptr->m_NumItems, value);
			ptr->m_NumItems++;
		}

		public T Dequeue()
		{
			TryDequeue(out var item);
			return item;
		}

		public unsafe bool TryDequeue(out T item)
		{
			NativeQueueBlockHeader* ptr = (NativeQueueBlockHeader*)(void*)m_Buffer->m_FirstBlock;
			if (ptr == null)
			{
				item = default(T);
				return false;
			}
			item = UnsafeUtility.ReadArrayElement<T>(ptr + 1, m_Buffer->m_CurrentRead++);
			if (m_Buffer->m_CurrentRead >= ptr->m_NumItems)
			{
				m_Buffer->m_CurrentRead = 0;
				m_Buffer->m_FirstBlock = (IntPtr)ptr->m_NextBlock;
				if (m_Buffer->m_FirstBlock == IntPtr.Zero)
				{
					m_Buffer->m_LastBlock = IntPtr.Zero;
				}
				for (int i = 0; i < 128; i++)
				{
					if (m_Buffer->GetCurrentWriteBlockTLS(i) == ptr)
					{
						m_Buffer->SetCurrentWriteBlockTLS(i, null);
					}
				}
				m_QueuePool->FreeBlock(ptr);
			}
			return true;
		}

		public unsafe NativeArray<T> ToArray(AllocatorManager.AllocatorHandle allocator)
		{
			NativeQueueBlockHeader* ptr = (NativeQueueBlockHeader*)(void*)m_Buffer->m_FirstBlock;
			NativeArray<T> nativeArray = CollectionHelper.CreateNativeArray<T>(Count, allocator);
			NativeQueueBlockHeader* ptr2 = ptr;
			byte* unsafePtr = (byte*)nativeArray.GetUnsafePtr();
			int num = UnsafeUtility.SizeOf<T>();
			int num2 = 0;
			int num3 = m_Buffer->m_CurrentRead * num;
			int num4 = m_Buffer->m_CurrentRead;
			while (ptr2 != null)
			{
				int num5 = (ptr2->m_NumItems - num4) * num;
				UnsafeUtility.MemCpy(unsafePtr + num2, (byte*)(ptr2 + 1) + num3, num5);
				num3 = (num4 = 0);
				num2 += num5;
				ptr2 = ptr2->m_NextBlock;
			}
			return nativeArray;
		}

		public unsafe void Clear()
		{
			NativeQueueBlockHeader* ptr = (NativeQueueBlockHeader*)(void*)m_Buffer->m_FirstBlock;
			while (ptr != null)
			{
				NativeQueueBlockHeader* nextBlock = ptr->m_NextBlock;
				m_QueuePool->FreeBlock(ptr);
				ptr = nextBlock;
			}
			m_Buffer->m_FirstBlock = IntPtr.Zero;
			m_Buffer->m_LastBlock = IntPtr.Zero;
			m_Buffer->m_CurrentRead = 0;
			for (int i = 0; i < 128; i++)
			{
				m_Buffer->SetCurrentWriteBlockTLS(i, null);
			}
		}

		public unsafe void Dispose()
		{
			NativeQueueData.DeallocateQueue(m_Buffer, m_QueuePool, m_AllocatorLabel);
			m_Buffer = null;
		}

		[NotBurstCompatible]
		public unsafe JobHandle Dispose(JobHandle inputDeps)
		{
			NativeQueueDisposeJob jobData = default(NativeQueueDisposeJob);
			jobData.Data = new NativeQueueDispose
			{
				m_Buffer = m_Buffer,
				m_QueuePool = m_QueuePool,
				m_AllocatorLabel = m_AllocatorLabel
			};
			JobHandle result = IJobExtensions.Schedule(jobData, inputDeps);
			m_Buffer = null;
			return result;
		}

		public unsafe ParallelWriter AsParallelWriter()
		{
			ParallelWriter result = default(ParallelWriter);
			result.m_Buffer = m_Buffer;
			result.m_QueuePool = m_QueuePool;
			result.m_ThreadIndex = 0;
			return result;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckRead()
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private unsafe void CheckReadNotEmpty()
		{
			_ = m_Buffer->m_FirstBlock == (IntPtr)0;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckWrite()
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void ThrowEmpty()
		{
			throw new InvalidOperationException("Trying to read from an empty queue.");
		}
	}
}
