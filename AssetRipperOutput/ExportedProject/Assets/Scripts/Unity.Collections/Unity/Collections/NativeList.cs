using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Collections.NotBurstCompatible;
using Unity.Jobs;

namespace Unity.Collections
{
	[NativeContainer]
	[DebuggerDisplay("Length = {Length}")]
	[DebuggerTypeProxy(typeof(NativeListDebugView<>))]
	[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
	public struct NativeList<T> : INativeDisposable, IDisposable, INativeList<T>, IIndexable<T>, IEnumerable<T>, IEnumerable where T : unmanaged
	{
		[NativeContainer]
		[NativeContainerIsAtomicWriteOnly]
		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public struct ParallelWriter
		{
			[NativeDisableUnsafePtrRestriction]
			public unsafe UnsafeList<T>* ListData;

			public unsafe readonly void* Ptr => ListData->Ptr;

			internal unsafe ParallelWriter(UnsafeList<T>* listData)
			{
				ListData = listData;
			}

			public unsafe void AddNoResize(T value)
			{
				int index = Interlocked.Increment(ref ListData->m_length) - 1;
				UnsafeUtility.WriteArrayElement(ListData->Ptr, index, value);
			}

			public unsafe void AddRangeNoResize(void* ptr, int count)
			{
				int num = Interlocked.Add(ref ListData->m_length, count) - count;
				int num2 = sizeof(T);
				void* destination = (byte*)ListData->Ptr + num * num2;
				UnsafeUtility.MemCpy(destination, ptr, count * num2);
			}

			public unsafe void AddRangeNoResize(UnsafeList<T> list)
			{
				AddRangeNoResize(list.Ptr, list.Length);
			}

			public unsafe void AddRangeNoResize(NativeList<T> list)
			{
				AddRangeNoResize(*list.m_ListData);
			}
		}

		[NativeDisableUnsafePtrRestriction]
		internal unsafe UnsafeList<T>* m_ListData;

		internal AllocatorManager.AllocatorHandle m_DeprecatedAllocator;

		public unsafe T this[int index]
		{
			get
			{
				return (*m_ListData)[index];
			}
			set
			{
				(*m_ListData)[index] = value;
			}
		}

		public unsafe int Length
		{
			get
			{
				return CollectionHelper.AssumePositive(m_ListData->Length);
			}
			set
			{
				m_ListData->Resize(value, NativeArrayOptions.ClearMemory);
			}
		}

		public unsafe int Capacity
		{
			get
			{
				return m_ListData->Capacity;
			}
			set
			{
				m_ListData->Capacity = value;
			}
		}

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

		public unsafe bool IsCreated => m_ListData != null;

		public NativeList(AllocatorManager.AllocatorHandle allocator)
			: this(1, allocator, 2)
		{
		}

		public NativeList(int initialCapacity, AllocatorManager.AllocatorHandle allocator)
			: this(initialCapacity, allocator, 2)
		{
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(AllocatorManager.AllocatorHandle) })]
		internal unsafe void Initialize<U>(int initialCapacity, ref U allocator, int disposeSentinelStackDepth) where U : unmanaged, AllocatorManager.IAllocator
		{
			m_ListData = UnsafeList<T>.Create(initialCapacity, ref allocator);
			m_DeprecatedAllocator = allocator.Handle;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(AllocatorManager.AllocatorHandle) })]
		internal static NativeList<T> New<U>(int initialCapacity, ref U allocator, int disposeSentinelStackDepth) where U : unmanaged, AllocatorManager.IAllocator
		{
			NativeList<T> result = default(NativeList<T>);
			result.Initialize(initialCapacity, ref allocator, disposeSentinelStackDepth);
			return result;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(AllocatorManager.AllocatorHandle) })]
		internal static NativeList<T> New<U>(int initialCapacity, ref U allocator) where U : unmanaged, AllocatorManager.IAllocator
		{
			return New(initialCapacity, ref allocator, 2);
		}

		private NativeList(int initialCapacity, AllocatorManager.AllocatorHandle allocator, int disposeSentinelStackDepth)
		{
			this = default(NativeList<T>);
			AllocatorManager.AllocatorHandle allocator2 = allocator;
			Initialize(initialCapacity, ref allocator2, disposeSentinelStackDepth);
		}

		public unsafe ref T ElementAt(int index)
		{
			return ref m_ListData->ElementAt(index);
		}

		public unsafe UnsafeList<T>* GetUnsafeList()
		{
			return m_ListData;
		}

		public unsafe void AddNoResize(T value)
		{
			m_ListData->AddNoResize(value);
		}

		public unsafe void AddRangeNoResize(void* ptr, int count)
		{
			m_ListData->AddRangeNoResize(ptr, count);
		}

		public unsafe void AddRangeNoResize(NativeList<T> list)
		{
			m_ListData->AddRangeNoResize(*list.m_ListData);
		}

		public unsafe void Add(in T value)
		{
			m_ListData->Add(in value);
		}

		public unsafe void AddRange(NativeArray<T> array)
		{
			AddRange(array.GetUnsafeReadOnlyPtr(), array.Length);
		}

		public unsafe void AddRange(void* ptr, int count)
		{
			m_ListData->AddRange(ptr, CollectionHelper.AssumePositive(count));
		}

		public unsafe void InsertRangeWithBeginEnd(int begin, int end)
		{
			m_ListData->InsertRangeWithBeginEnd(CollectionHelper.AssumePositive(begin), CollectionHelper.AssumePositive(end));
		}

		public unsafe void RemoveAtSwapBack(int index)
		{
			m_ListData->RemoveAtSwapBack(CollectionHelper.AssumePositive(index));
		}

		public unsafe void RemoveRangeSwapBack(int index, int count)
		{
			m_ListData->RemoveRangeSwapBack(CollectionHelper.AssumePositive(index), CollectionHelper.AssumePositive(count));
		}

		[Obsolete("RemoveRangeSwapBackWithBeginEnd(begin, end) is deprecated, use RemoveRangeSwapBack(index, count) instead. (RemovedAfter 2021-06-02)", false)]
		public unsafe void RemoveRangeSwapBackWithBeginEnd(int begin, int end)
		{
			m_ListData->RemoveRangeSwapBackWithBeginEnd(CollectionHelper.AssumePositive(begin), CollectionHelper.AssumePositive(end));
		}

		public unsafe void RemoveAt(int index)
		{
			m_ListData->RemoveAt(CollectionHelper.AssumePositive(index));
		}

		public unsafe void RemoveRange(int index, int count)
		{
			m_ListData->RemoveRange(index, count);
		}

		[Obsolete("RemoveRangeWithBeginEnd(begin, end) is deprecated, use RemoveRange(index, count) instead. (RemovedAfter 2021-06-02)", false)]
		public unsafe void RemoveRangeWithBeginEnd(int begin, int end)
		{
			m_ListData->RemoveRangeWithBeginEnd(begin, end);
		}

		public unsafe void Dispose()
		{
			UnsafeList<T>.Destroy(m_ListData);
			m_ListData = null;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(AllocatorManager.AllocatorHandle) })]
		internal unsafe void Dispose<U>(ref U allocator) where U : unmanaged, AllocatorManager.IAllocator
		{
			UnsafeList<T>.Destroy(m_ListData, ref allocator);
			m_ListData = null;
		}

		[NotBurstCompatible]
		public unsafe JobHandle Dispose(JobHandle inputDeps)
		{
			NativeListDisposeJob jobData = default(NativeListDisposeJob);
			jobData.Data = new NativeListDispose
			{
				m_ListData = (UntypedUnsafeList*)m_ListData
			};
			JobHandle result = IJobExtensions.Schedule(jobData, inputDeps);
			m_ListData = null;
			return result;
		}

		public unsafe void Clear()
		{
			m_ListData->Clear();
		}

		public static implicit operator NativeArray<T>(NativeList<T> nativeList)
		{
			return nativeList.AsArray();
		}

		public unsafe NativeArray<T> AsArray()
		{
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(m_ListData->Ptr, m_ListData->Length, Allocator.None);
		}

		public unsafe NativeArray<T> AsDeferredJobArray()
		{
			byte* listData = (byte*)m_ListData;
			listData++;
			return NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<T>(listData, 0, Allocator.Invalid);
		}

		[NotBurstCompatible]
		public T[] ToArray()
		{
			return this.ToArrayNBC();
		}

		public NativeArray<T> ToArray(AllocatorManager.AllocatorHandle allocator)
		{
			NativeArray<T> result = CollectionHelper.CreateNativeArray<T>(Length, allocator, NativeArrayOptions.UninitializedMemory);
			result.CopyFrom(this);
			return result;
		}

		public NativeArray<T>.Enumerator GetEnumerator()
		{
			NativeArray<T> array = AsArray();
			return new NativeArray<T>.Enumerator(ref array);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		[NotBurstCompatible]
		[Obsolete("Please use `CopyFromNBC` from `Unity.Collections.NotBurstCompatible` namespace instead. (RemovedAfter 2021-06-22)", false)]
		public void CopyFrom(T[] array)
		{
			this.CopyFromNBC(array);
		}

		public void CopyFrom(NativeArray<T> array)
		{
			Clear();
			Resize(array.Length, NativeArrayOptions.UninitializedMemory);
			AsArray().CopyFrom(array);
		}

		public unsafe void Resize(int length, NativeArrayOptions options)
		{
			m_ListData->Resize(length, options);
		}

		public void ResizeUninitialized(int length)
		{
			Resize(length, NativeArrayOptions.UninitializedMemory);
		}

		public unsafe void SetCapacity(int capacity)
		{
			m_ListData->SetCapacity(capacity);
		}

		public unsafe void TrimExcess()
		{
			m_ListData->TrimExcess();
		}

		public unsafe NativeArray<T>.ReadOnly AsParallelReader()
		{
			return new NativeArray<T>.ReadOnly(m_ListData->Ptr, m_ListData->Length);
		}

		public unsafe ParallelWriter AsParallelWriter()
		{
			return new ParallelWriter(m_ListData);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckInitialCapacity(int initialCapacity)
		{
			if (initialCapacity < 0)
			{
				throw new ArgumentOutOfRangeException("initialCapacity", "Capacity must be >= 0");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckTotalSize(int initialCapacity, long totalSize)
		{
			if (totalSize > int.MaxValue)
			{
				throw new ArgumentOutOfRangeException("initialCapacity", $"Capacity * sizeof(T) cannot exceed {int.MaxValue} bytes");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckSufficientCapacity(int capacity, int length)
		{
			if (capacity < length)
			{
				throw new Exception($"Length {length} exceeds capacity Capacity {capacity}");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckIndexInRange(int value, int length)
		{
			if (value < 0)
			{
				throw new IndexOutOfRangeException($"Value {value} must be positive.");
			}
			if ((uint)value >= (uint)length)
			{
				throw new IndexOutOfRangeException($"Value {value} is out of range in NativeList of '{length}' Length.");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckArgPositive(int value)
		{
			if (value < 0)
			{
				throw new ArgumentOutOfRangeException($"Value {value} must be positive.");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private unsafe void CheckHandleMatches(AllocatorManager.AllocatorHandle handle)
		{
			if (m_ListData == null)
			{
				throw new ArgumentOutOfRangeException($"Allocator handle {handle} can't match because container is not initialized.");
			}
			if (m_ListData->Allocator.Index != handle.Index)
			{
				throw new ArgumentOutOfRangeException($"Allocator handle {handle} can't match because container handle index doesn't match.");
			}
			if (m_ListData->Allocator.Version != handle.Version)
			{
				throw new ArgumentOutOfRangeException($"Allocator handle {handle} matches container handle index, but has different version.");
			}
		}
	}
}
