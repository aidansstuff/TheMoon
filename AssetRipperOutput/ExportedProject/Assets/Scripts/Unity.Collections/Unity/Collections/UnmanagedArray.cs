using System;

namespace Unity.Collections
{
	internal struct UnmanagedArray<T> : IDisposable where T : unmanaged
	{
		private IntPtr m_pointer;

		private int m_length;

		private AllocatorManager.AllocatorHandle m_allocator;

		public unsafe ref T this[int index] => ref *(T*)((byte*)(void*)m_pointer + (nint)index * (nint)sizeof(T));

		public unsafe UnmanagedArray(int length, AllocatorManager.AllocatorHandle allocator)
		{
			m_pointer = (IntPtr)Memory.Unmanaged.Array.Allocate<T>(length, allocator);
			m_length = length;
			m_allocator = allocator;
		}

		public unsafe void Dispose()
		{
			Memory.Unmanaged.Free((T*)(void*)m_pointer, Allocator.Persistent);
		}

		public unsafe T* GetUnsafePointer()
		{
			return (T*)(void*)m_pointer;
		}
	}
}
