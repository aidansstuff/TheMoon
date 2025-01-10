using System;
using System.Diagnostics;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe
{
	[DebuggerDisplay("Length = {Length}, Capacity = {Capacity}, IsCreated = {IsCreated}, IsEmpty = {IsEmpty}")]
	[DebuggerTypeProxy(typeof(UnsafeRingQueueDebugView<>))]
	[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
	public struct UnsafeRingQueue<T> : INativeDisposable, IDisposable where T : unmanaged
	{
		[NativeDisableUnsafePtrRestriction]
		public unsafe T* Ptr;

		public AllocatorManager.AllocatorHandle Allocator;

		internal RingControl Control;

		public bool IsEmpty
		{
			get
			{
				if (IsCreated)
				{
					return Length == 0;
				}
				return true;
			}
		}

		public int Length => Control.Length;

		public int Capacity => Control.Capacity;

		public unsafe bool IsCreated => Ptr != null;

		public unsafe UnsafeRingQueue(T* ptr, int capacity)
		{
			Ptr = ptr;
			Allocator = AllocatorManager.None;
			Control = new RingControl(capacity);
		}

		public unsafe UnsafeRingQueue(int capacity, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.ClearMemory)
		{
			capacity++;
			Allocator = allocator;
			Control = new RingControl(capacity);
			int num = capacity * UnsafeUtility.SizeOf<T>();
			Ptr = (T*)Memory.Unmanaged.Allocate(num, 16, allocator);
			if (options == NativeArrayOptions.ClearMemory)
			{
				UnsafeUtility.MemClear(Ptr, num);
			}
		}

		public unsafe void Dispose()
		{
			if (CollectionHelper.ShouldDeallocate(Allocator))
			{
				Memory.Unmanaged.Free(Ptr, Allocator);
				Allocator = AllocatorManager.Invalid;
			}
			Ptr = null;
		}

		[NotBurstCompatible]
		public unsafe JobHandle Dispose(JobHandle inputDeps)
		{
			if (CollectionHelper.ShouldDeallocate(Allocator))
			{
				UnsafeDisposeJob jobData = default(UnsafeDisposeJob);
				jobData.Ptr = Ptr;
				jobData.Allocator = Allocator;
				JobHandle result = IJobExtensions.Schedule(jobData, inputDeps);
				Ptr = null;
				Allocator = AllocatorManager.Invalid;
				return result;
			}
			Ptr = null;
			return inputDeps;
		}

		public unsafe bool TryEnqueue(T value)
		{
			if (1 != Control.Reserve(1))
			{
				return false;
			}
			Ptr[Control.Current] = value;
			Control.Commit(1);
			return true;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void ThrowQueueFull()
		{
			throw new InvalidOperationException("Trying to enqueue into full queue.");
		}

		public void Enqueue(T value)
		{
			TryEnqueue(value);
		}

		public unsafe bool TryDequeue(out T item)
		{
			item = Ptr[Control.Read];
			return 1 == Control.Consume(1);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void ThrowQueueEmpty()
		{
			throw new InvalidOperationException("Trying to dequeue from an empty queue");
		}

		public T Dequeue()
		{
			TryDequeue(out var item);
			return item;
		}
	}
}
