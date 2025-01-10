using System;
using System.Threading;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[BurstCompatible]
	internal struct NativeQueueData
	{
		public IntPtr m_FirstBlock;

		public IntPtr m_LastBlock;

		public int m_MaxItems;

		public int m_CurrentRead;

		public unsafe byte* m_CurrentWriteBlockTLS;

		internal unsafe NativeQueueBlockHeader* GetCurrentWriteBlockTLS(int threadIndex)
		{
			NativeQueueBlockHeader** ptr = (NativeQueueBlockHeader**)(m_CurrentWriteBlockTLS + threadIndex * 64);
			return *ptr;
		}

		internal unsafe void SetCurrentWriteBlockTLS(int threadIndex, NativeQueueBlockHeader* currentWriteBlock)
		{
			NativeQueueBlockHeader** ptr = (NativeQueueBlockHeader**)(m_CurrentWriteBlockTLS + threadIndex * 64);
			*ptr = currentWriteBlock;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static NativeQueueBlockHeader* AllocateWriteBlockMT<T>(NativeQueueData* data, NativeQueueBlockPoolData* pool, int threadIndex) where T : struct
		{
			NativeQueueBlockHeader* ptr = data->GetCurrentWriteBlockTLS(threadIndex);
			if (ptr != null && ptr->m_NumItems == data->m_MaxItems)
			{
				ptr = null;
			}
			if (ptr == null)
			{
				ptr = pool->AllocateBlock();
				ptr->m_NextBlock = null;
				ptr->m_NumItems = 0;
				NativeQueueBlockHeader* ptr2 = (NativeQueueBlockHeader*)(void*)Interlocked.Exchange(ref data->m_LastBlock, (IntPtr)ptr);
				if (ptr2 == null)
				{
					data->m_FirstBlock = (IntPtr)ptr;
				}
				else
				{
					ptr2->m_NextBlock = ptr;
				}
				data->SetCurrentWriteBlockTLS(threadIndex, ptr);
			}
			return ptr;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe static void AllocateQueue<T>(AllocatorManager.AllocatorHandle label, out NativeQueueData* outBuf) where T : struct
		{
			int num = CollectionHelper.Align(UnsafeUtility.SizeOf<NativeQueueData>(), 64);
			NativeQueueData* ptr = (NativeQueueData*)Memory.Unmanaged.Allocate(num + 8192, 64, label);
			ptr->m_CurrentWriteBlockTLS = (byte*)ptr + num;
			ptr->m_FirstBlock = IntPtr.Zero;
			ptr->m_LastBlock = IntPtr.Zero;
			ptr->m_MaxItems = (16384 - UnsafeUtility.SizeOf<NativeQueueBlockHeader>()) / UnsafeUtility.SizeOf<T>();
			ptr->m_CurrentRead = 0;
			for (int i = 0; i < 128; i++)
			{
				ptr->SetCurrentWriteBlockTLS(i, null);
			}
			outBuf = ptr;
		}

		public unsafe static void DeallocateQueue(NativeQueueData* data, NativeQueueBlockPoolData* pool, AllocatorManager.AllocatorHandle allocation)
		{
			NativeQueueBlockHeader* ptr = (NativeQueueBlockHeader*)(void*)data->m_FirstBlock;
			while (ptr != null)
			{
				NativeQueueBlockHeader* nextBlock = ptr->m_NextBlock;
				pool->FreeBlock(ptr);
				ptr = nextBlock;
			}
			Memory.Unmanaged.Free(data, allocation);
		}
	}
}
