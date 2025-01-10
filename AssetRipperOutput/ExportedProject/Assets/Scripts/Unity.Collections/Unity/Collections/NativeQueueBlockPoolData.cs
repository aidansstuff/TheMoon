using System;
using System.Threading;

namespace Unity.Collections
{
	[BurstCompatible]
	internal struct NativeQueueBlockPoolData
	{
		internal IntPtr m_FirstBlock;

		internal int m_NumBlocks;

		internal int m_MaxBlocks;

		internal const int m_BlockSize = 16384;

		internal int m_AllocLock;

		public unsafe NativeQueueBlockHeader* AllocateBlock()
		{
			while (Interlocked.CompareExchange(ref m_AllocLock, 1, 0) != 0)
			{
			}
			NativeQueueBlockHeader* ptr = (NativeQueueBlockHeader*)(void*)m_FirstBlock;
			NativeQueueBlockHeader* ptr2;
			do
			{
				ptr2 = ptr;
				if (ptr2 == null)
				{
					Interlocked.Exchange(ref m_AllocLock, 0);
					Interlocked.Increment(ref m_NumBlocks);
					return (NativeQueueBlockHeader*)Memory.Unmanaged.Allocate(16384L, 16, Allocator.Persistent);
				}
				ptr = (NativeQueueBlockHeader*)(void*)Interlocked.CompareExchange(ref m_FirstBlock, (IntPtr)ptr2->m_NextBlock, (IntPtr)ptr2);
			}
			while (ptr != ptr2);
			Interlocked.Exchange(ref m_AllocLock, 0);
			return ptr2;
		}

		public unsafe void FreeBlock(NativeQueueBlockHeader* block)
		{
			if (m_NumBlocks > m_MaxBlocks)
			{
				if (Interlocked.Decrement(ref m_NumBlocks) + 1 > m_MaxBlocks)
				{
					Memory.Unmanaged.Free(block, Allocator.Persistent);
					return;
				}
				Interlocked.Increment(ref m_NumBlocks);
			}
			NativeQueueBlockHeader* ptr = (NativeQueueBlockHeader*)(void*)m_FirstBlock;
			NativeQueueBlockHeader* ptr2;
			do
			{
				ptr2 = ptr;
				block->m_NextBlock = ptr;
				ptr = (NativeQueueBlockHeader*)(void*)Interlocked.CompareExchange(ref m_FirstBlock, (IntPtr)block, (IntPtr)ptr);
			}
			while (ptr != ptr2);
		}
	}
}
