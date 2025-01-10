using System;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Networking.Transport.Utilities.LowLevel.Unsafe
{
	internal struct UnsafeAtomicFreeList : IDisposable
	{
		[NativeDisableUnsafePtrRestriction]
		private unsafe int* m_Buffer;

		private int m_Length;

		private Allocator m_Allocator;

		public int Capacity => m_Length;

		public unsafe int InUse => *m_Buffer - m_Buffer[1];

		public unsafe bool IsCreated => m_Buffer != null;

		public unsafe UnsafeAtomicFreeList(int capacity, Allocator allocator)
		{
			m_Allocator = allocator;
			m_Length = capacity;
			int num = UnsafeUtility.SizeOf<int>() * (capacity + 2);
			m_Buffer = (int*)UnsafeUtility.Malloc(num, UnsafeUtility.AlignOf<int>(), allocator);
			UnsafeUtility.MemClear(m_Buffer, num);
		}

		public unsafe void Dispose()
		{
			if (IsCreated)
			{
				UnsafeUtility.Free(m_Buffer, m_Allocator);
			}
		}

		public unsafe void Push(int item)
		{
			int* buffer = m_Buffer;
			int num = Interlocked.Increment(ref buffer[1]) - 1;
			while (Interlocked.CompareExchange(ref buffer[num + 2], item + 1, 0) != 0)
			{
			}
		}

		public unsafe int Pop()
		{
			int* buffer = m_Buffer;
			int num = buffer[1] - 1;
			while (num >= 0 && Interlocked.CompareExchange(ref buffer[1], num, num + 1) != num + 1)
			{
				num = buffer[1] - 1;
			}
			if (num >= 0)
			{
				int num2;
				for (num2 = 0; num2 == 0; num2 = Interlocked.Exchange(ref buffer[2 + num], 0))
				{
				}
				return num2 - 1;
			}
			num = Interlocked.Increment(ref *buffer) - 1;
			if (num >= Capacity)
			{
				Interlocked.Decrement(ref *buffer);
				return -1;
			}
			return num;
		}
	}
}
