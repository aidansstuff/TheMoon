namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompatible]
	internal struct UnsafeStreamBlockData
	{
		internal const int AllocationSize = 4096;

		internal AllocatorManager.AllocatorHandle Allocator;

		internal unsafe UnsafeStreamBlock** Blocks;

		internal int BlockCount;

		internal unsafe UnsafeStreamBlock* Free;

		internal unsafe UnsafeStreamRange* Ranges;

		internal int RangeCount;

		internal unsafe UnsafeStreamBlock* Allocate(UnsafeStreamBlock* oldBlock, int threadIndex)
		{
			UnsafeStreamBlock* ptr = (UnsafeStreamBlock*)Memory.Unmanaged.Allocate(4096L, 16, Allocator);
			ptr->Next = null;
			if (oldBlock == null)
			{
				ptr->Next = Blocks[threadIndex];
				Blocks[threadIndex] = ptr;
			}
			else
			{
				oldBlock->Next = ptr;
			}
			return ptr;
		}
	}
}
