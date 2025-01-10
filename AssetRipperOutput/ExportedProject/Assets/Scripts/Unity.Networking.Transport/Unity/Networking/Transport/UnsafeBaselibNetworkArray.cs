using System;
using Unity.Baselib.LowLevel;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Unity.Networking.Transport
{
	internal struct UnsafeBaselibNetworkArray : IDisposable
	{
		[NativeDisableUnsafePtrRestriction]
		private UnsafePtrList<Binding.Baselib_RegisteredNetwork_Buffer> m_BufferPool;

		public unsafe UnsafeBaselibNetworkArray(int capacity, int typeSize)
		{
			long num = typeSize;
			m_BufferPool = new UnsafePtrList<Binding.Baselib_RegisteredNetwork_Buffer>(capacity, Allocator.Persistent);
			Binding.Baselib_Memory_PageSizeInfo* ptr = stackalloc Binding.Baselib_Memory_PageSizeInfo[1];
			Binding.Baselib_Memory_GetPageSizeInfo(ptr);
			ulong defaultPageSize = ptr->defaultPageSize;
			for (int i = 0; i < capacity; i++)
			{
				ulong pageCount = 1uL;
				if ((ulong)num > defaultPageSize)
				{
					pageCount = (ulong)math.ceil((double)num / (double)defaultPageSize);
				}
				Binding.Baselib_RegisteredNetwork_Buffer* ptr2 = (Binding.Baselib_RegisteredNetwork_Buffer*)UnsafeUtility.Malloc(UnsafeUtility.SizeOf<Binding.Baselib_RegisteredNetwork_Buffer>(), UnsafeUtility.AlignOf<Binding.Baselib_RegisteredNetwork_Buffer>(), Allocator.Persistent);
				Binding.Baselib_ErrorState baselib_ErrorState = default(Binding.Baselib_ErrorState);
				Binding.Baselib_Memory_PageAllocation pageAllocation = Binding.Baselib_Memory_AllocatePages(ptr->defaultPageSize, pageCount, 1uL, Binding.Baselib_Memory_PageState.ReadWrite, &baselib_ErrorState);
				if (baselib_ErrorState.code != 0)
				{
					break;
				}
				UnsafeUtility.MemSet((void*)pageAllocation.ptr, 0, (long)(pageAllocation.pageCount * pageAllocation.pageSize));
				*ptr2 = Binding.Baselib_RegisteredNetwork_Buffer_Register(pageAllocation, &baselib_ErrorState);
				if (baselib_ErrorState.code != 0)
				{
					Binding.Baselib_Memory_ReleasePages(pageAllocation, &baselib_ErrorState);
					*ptr2 = default(Binding.Baselib_RegisteredNetwork_Buffer);
				}
				m_BufferPool.Add(ptr2);
			}
		}

		public unsafe void Dispose()
		{
			Binding.Baselib_ErrorState baselib_ErrorState = default(Binding.Baselib_ErrorState);
			for (int i = 0; i < m_BufferPool.Length; i++)
			{
				Binding.Baselib_RegisteredNetwork_Buffer* intPtr = m_BufferPool[i];
				Binding.Baselib_Memory_PageAllocation allocation = intPtr->allocation;
				Binding.Baselib_RegisteredNetwork_Buffer_Deregister(*intPtr);
				Binding.Baselib_Memory_ReleasePages(allocation, &baselib_ErrorState);
				UnsafeUtility.Free(intPtr, Allocator.Persistent);
			}
		}

		public unsafe Binding.Baselib_RegisteredNetwork_BufferSlice AtIndexAsSlice(int index, uint elementSize)
		{
			uint offset = 0u;
			Binding.Baselib_RegisteredNetwork_Buffer* ptr = null;
			ptr = m_BufferPool[index];
			IntPtr data = (IntPtr)(void*)ptr->allocation.ptr;
			Binding.Baselib_RegisteredNetwork_BufferSlice result = default(Binding.Baselib_RegisteredNetwork_BufferSlice);
			result.id = ptr->id;
			result.data = data;
			result.offset = offset;
			result.size = elementSize;
			return result;
		}
	}
}
