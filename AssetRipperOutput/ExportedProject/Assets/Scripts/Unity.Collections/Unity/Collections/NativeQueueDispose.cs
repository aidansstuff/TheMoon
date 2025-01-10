using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[NativeContainer]
	[BurstCompatible]
	internal struct NativeQueueDispose
	{
		[NativeDisableUnsafePtrRestriction]
		internal unsafe NativeQueueData* m_Buffer;

		[NativeDisableUnsafePtrRestriction]
		internal unsafe NativeQueueBlockPoolData* m_QueuePool;

		internal AllocatorManager.AllocatorHandle m_AllocatorLabel;

		public unsafe void Dispose()
		{
			NativeQueueData.DeallocateQueue(m_Buffer, m_QueuePool, m_AllocatorLabel);
		}
	}
}
