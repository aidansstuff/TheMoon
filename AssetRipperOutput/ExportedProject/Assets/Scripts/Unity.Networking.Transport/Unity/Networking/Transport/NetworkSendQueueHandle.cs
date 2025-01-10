using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Networking.Transport
{
	public struct NetworkSendQueueHandle
	{
		private IntPtr handle;

		internal unsafe static NetworkSendQueueHandle ToTempHandle(NativeQueue<QueuedSendMessage>.ParallelWriter sendQueue)
		{
			void* ptr = UnsafeUtility.Malloc(UnsafeUtility.SizeOf<NativeQueue<QueuedSendMessage>.ParallelWriter>(), UnsafeUtility.AlignOf<NativeQueue<QueuedSendMessage>.ParallelWriter>(), Allocator.Temp);
			UnsafeUtility.WriteArrayElement(ptr, 0, sendQueue);
			NetworkSendQueueHandle result = default(NetworkSendQueueHandle);
			result.handle = (IntPtr)ptr;
			return result;
		}

		public unsafe NativeQueue<QueuedSendMessage>.ParallelWriter FromHandle()
		{
			return UnsafeUtility.ReadArrayElement<NativeQueue<QueuedSendMessage>.ParallelWriter>((void*)handle, 0);
		}
	}
}
