using System;
using System.Runtime.CompilerServices;
using System.Threading;
using AOT;
using Unity.Burst;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs.LowLevel.Unsafe;
using Unity.Mathematics;

namespace Unity.Collections
{
	[BurstCompile]
	public struct RewindableAllocator : AllocatorManager.IAllocator, IDisposable
	{
		[BurstCompatible]
		internal struct MemoryBlock : IDisposable
		{
			public const int kMaximumAlignment = 16384;

			public unsafe byte* m_pointer;

			public long m_bytes;

			public long m_current;

			public long m_allocations;

			public unsafe MemoryBlock(long bytes)
			{
				m_pointer = (byte*)Memory.Unmanaged.Allocate(bytes, 16384, Allocator.Persistent);
				m_bytes = bytes;
				m_current = 0L;
				m_allocations = 0L;
			}

			public void Rewind()
			{
				m_current = 0L;
				m_allocations = 0L;
			}

			public unsafe void Dispose()
			{
				Memory.Unmanaged.Free(m_pointer, Allocator.Persistent);
				m_pointer = null;
				m_bytes = 0L;
				m_current = 0L;
				m_allocations = 0L;
			}

			public unsafe int TryAllocate(ref AllocatorManager.Block block)
			{
				int num = math.max(64, block.Alignment);
				int num2 = ((num != 64) ? 1 : 0);
				int num3 = 63;
				if (num2 == 1)
				{
					num = (num + num3) & ~num3;
				}
				long num4 = (long)num - 1L;
				long num5 = (block.Bytes + num2 * num + num4) & ~num4;
				long num6 = Interlocked.Read(ref m_current);
				long num7;
				long num8;
				do
				{
					long value = num6 + num5;
					num7 = (num6 + num4) & ~num4;
					if (num7 + block.Bytes > m_bytes)
					{
						return -1;
					}
					num8 = num6;
					num6 = Interlocked.CompareExchange(ref m_current, value, num8);
				}
				while (num6 != num8);
				block.Range.Pointer = (IntPtr)(m_pointer + num7);
				block.AllocatedItems = block.Range.Items;
				Interlocked.Increment(ref m_allocations);
				return 0;
			}

			public unsafe bool Contains(IntPtr ptr)
			{
				void* ptr2 = (void*)ptr;
				if (ptr2 >= m_pointer)
				{
					return ptr2 < m_pointer + m_current;
				}
				return false;
			}
		}

		public delegate int Try_000008B3_0024PostfixBurstDelegate(IntPtr state, ref AllocatorManager.Block block);

		internal static class Try_000008B3_0024BurstDirectCall
		{
			private static IntPtr Pointer;

			private static IntPtr DeferredCompilation;

			[BurstDiscard]
			private unsafe static void GetFunctionPointerDiscard(ref IntPtr P_0)
			{
				if (Pointer == (IntPtr)0)
				{
					Pointer = (nint)BurstCompiler.GetILPPMethodFunctionPointer2(DeferredCompilation, (RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/, typeof(Try_000008B3_0024PostfixBurstDelegate).TypeHandle);
				}
				P_0 = Pointer;
			}

			private static IntPtr GetFunctionPointer()
			{
				nint result = 0;
				GetFunctionPointerDiscard(ref result);
				return result;
			}

			public static void Constructor()
			{
				DeferredCompilation = BurstCompiler.CompileILPPMethod2((RuntimeMethodHandle)/*OpCode not supported: LdMemberToken*/);
			}

			public static void Initialize()
			{
			}

			static Try_000008B3_0024BurstDirectCall()
			{
				Constructor();
			}

			public unsafe static int Invoke(IntPtr state, ref AllocatorManager.Block block)
			{
				if (BurstCompiler.IsEnabled)
				{
					IntPtr functionPointer = GetFunctionPointer();
					if (functionPointer != (IntPtr)0)
					{
						return ((delegate* unmanaged[Cdecl]<IntPtr, ref AllocatorManager.Block, int>)functionPointer)(state, ref block);
					}
				}
				return Try_0024BurstManaged(state, ref block);
			}
		}

		private Spinner m_spinner;

		private AllocatorManager.AllocatorHandle m_handle;

		private UnmanagedArray<MemoryBlock> m_block;

		private int m_best;

		private int m_last;

		private int m_used;

		private bool m_enableBlockFree;

		public bool EnableBlockFree
		{
			get
			{
				return m_enableBlockFree;
			}
			set
			{
				m_enableBlockFree = value;
			}
		}

		public int BlocksAllocated => m_last + 1;

		public int InitialSizeInBytes => (int)m_block[0].m_bytes;

		[NotBurstCompatible]
		public AllocatorManager.TryFunction Function => Try;

		public AllocatorManager.AllocatorHandle Handle
		{
			get
			{
				return m_handle;
			}
			set
			{
				m_handle = value;
			}
		}

		public Allocator ToAllocator => m_handle.ToAllocator;

		public bool IsCustomAllocator => m_handle.IsCustomAllocator;

		public void Initialize(int initialSizeInBytes, bool enableBlockFree = false)
		{
			m_spinner = default(Spinner);
			m_block = new UnmanagedArray<MemoryBlock>(64, Allocator.Persistent);
			m_block[0] = new MemoryBlock(initialSizeInBytes);
			m_last = (m_used = (m_best = 0));
			m_enableBlockFree = enableBlockFree;
		}

		public void Rewind()
		{
			if (JobsUtility.IsExecutingJob)
			{
				throw new InvalidOperationException("You cannot Rewind a RewindableAllocator from a Job.");
			}
			m_handle.Rewind();
			while (m_last > m_used)
			{
				m_block[m_last--].Dispose();
			}
			while (m_used > 0)
			{
				m_block[m_used--].Rewind();
			}
			m_block[0].Rewind();
			m_best = 0;
		}

		public void Dispose()
		{
			if (JobsUtility.IsExecutingJob)
			{
				throw new InvalidOperationException("You cannot Dispose a RewindableAllocator from a Job.");
			}
			m_used = 0;
			Rewind();
			m_block[0].Dispose();
			m_block.Dispose();
			m_last = (m_used = (m_best = 0));
		}

		public int Try(ref AllocatorManager.Block block)
		{
			if (block.Range.Pointer == IntPtr.Zero)
			{
				int num = m_block[m_best].TryAllocate(ref block);
				if (num == 0)
				{
					return num;
				}
				m_spinner.Lock();
				int i;
				for (i = 0; i <= m_last; i++)
				{
					num = m_block[i].TryAllocate(ref block);
					if (num == 0)
					{
						m_used = ((i > m_used) ? i : m_used);
						m_best = i;
						m_spinner.Unlock();
						return num;
					}
				}
				long bytes = math.max(m_block[0].m_bytes << i, math.ceilpow2(block.Bytes));
				m_block[i] = new MemoryBlock(bytes);
				num = m_block[i].TryAllocate(ref block);
				m_best = i;
				m_used = i;
				m_last = i;
				m_spinner.Unlock();
				return num;
			}
			if (block.Range.Items == 0)
			{
				if (m_enableBlockFree)
				{
					m_spinner.Lock();
					if (m_block[m_best].Contains(block.Range.Pointer) && Interlocked.Decrement(ref m_block[m_best].m_allocations) == 0L)
					{
						m_block[m_best].Rewind();
					}
					m_spinner.Unlock();
				}
				return 0;
			}
			return -1;
		}

		[BurstCompile]
		[MonoPInvokeCallback(typeof(AllocatorManager.TryFunction))]
		internal static int Try(IntPtr state, ref AllocatorManager.Block block)
		{
			return Try_000008B3_0024BurstDirectCall.Invoke(state, ref block);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe NativeArray<T> AllocateNativeArray<T>(int length) where T : struct
		{
			NativeArray<T> result = default(NativeArray<T>);
			result.m_Buffer = AllocatorManager.AllocateStruct(ref this, default(T), length);
			result.m_Length = length;
			result.m_AllocatorLabel = Allocator.None;
			return result;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe NativeList<T> AllocateNativeList<T>(int capacity) where T : unmanaged
		{
			NativeList<T> result = default(NativeList<T>);
			result.m_ListData = AllocatorManager.Allocate(ref this, default(UnsafeList<T>), 1);
			result.m_ListData->Ptr = AllocatorManager.Allocate(ref this, default(T), capacity);
			result.m_ListData->m_capacity = capacity;
			result.m_ListData->m_length = 0;
			result.m_ListData->Allocator = Allocator.None;
			result.m_DeprecatedAllocator = Allocator.None;
			return result;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[BurstCompile]
		[MonoPInvokeCallback(typeof(AllocatorManager.TryFunction))]
		public unsafe static int Try_0024BurstManaged(IntPtr state, ref AllocatorManager.Block block)
		{
			return ((RewindableAllocator*)(void*)state)->Try(ref block);
		}
	}
}
