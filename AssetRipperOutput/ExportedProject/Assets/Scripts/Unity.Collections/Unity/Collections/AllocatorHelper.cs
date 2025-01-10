using System;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[BurstCompatible(GenericTypeArguments = new Type[] { typeof(AllocatorManager.AllocatorHandle) })]
	public struct AllocatorHelper<T> : IDisposable where T : unmanaged, AllocatorManager.IAllocator
	{
		private unsafe readonly T* m_allocator;

		private AllocatorManager.AllocatorHandle m_backingAllocator;

		public unsafe ref T Allocator => ref UnsafeUtility.AsRef<T>(m_allocator);

		[NotBurstCompatible]
		public unsafe AllocatorHelper(AllocatorManager.AllocatorHandle backingAllocator)
		{
			m_allocator = (T*)UnsafeUtility.AddressOf(ref AllocatorManager.CreateAllocator<T>(backingAllocator));
			m_backingAllocator = backingAllocator;
		}

		[NotBurstCompatible]
		public unsafe void Dispose()
		{
			AllocatorManager.DestroyAllocator(ref UnsafeUtility.AsRef<T>(m_allocator), m_backingAllocator);
		}
	}
}
