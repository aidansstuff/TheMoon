using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Jobs;

namespace Unity.Collections.LowLevel.Unsafe
{
	[DebuggerDisplay("Length = {Length}, Capacity = {Capacity}, IsCreated = {IsCreated}, IsEmpty = {IsEmpty}")]
	[DebuggerTypeProxy(typeof(UnsafePtrListDebugView))]
	[Obsolete("Untyped UnsafePtrList is deprecated, please use UnsafePtrList<T> instead. (RemovedAfter 2021-05-18)", false)]
	public struct UnsafePtrList : INativeDisposable, IDisposable, INativeList<IntPtr>, IIndexable<IntPtr>, IEnumerable<IntPtr>, IEnumerable
	{
		public struct ParallelReader
		{
			[NativeDisableUnsafePtrRestriction]
			public unsafe readonly void** Ptr;

			public readonly int Length;

			internal unsafe ParallelReader(void** ptr, int length)
			{
				Ptr = ptr;
				Length = length;
			}

			public unsafe int IndexOf(void* value)
			{
				for (int i = 0; i < Length; i++)
				{
					if (Ptr[i] == value)
					{
						return i;
					}
				}
				return -1;
			}

			public unsafe bool Contains(void* value)
			{
				return IndexOf(value) != -1;
			}
		}

		public struct ParallelWriter
		{
			[NativeDisableUnsafePtrRestriction]
			public unsafe readonly void* Ptr;

			[NativeDisableUnsafePtrRestriction]
			public unsafe UnsafeList* ListData;

			internal unsafe ParallelWriter(void* ptr, UnsafeList* listData)
			{
				Ptr = ptr;
				ListData = listData;
			}

			public unsafe void AddNoResize(void* value)
			{
				ListData->AddNoResize((IntPtr)value);
			}

			public unsafe void AddRangeNoResize(void** ptr, int length)
			{
				ListData->AddRangeNoResize<IntPtr>(ptr, length);
			}

			public unsafe void AddRangeNoResize(UnsafePtrList list)
			{
				ListData->AddRangeNoResize<IntPtr>(list.Ptr, list.Length);
			}
		}

		[NativeDisableUnsafePtrRestriction]
		public unsafe readonly void** Ptr;

		public readonly int length;

		public readonly int unused;

		public readonly int capacity;

		public readonly AllocatorManager.AllocatorHandle Allocator;

		public int Length
		{
			get
			{
				return length;
			}
			set
			{
			}
		}

		public int Capacity
		{
			get
			{
				return capacity;
			}
			set
			{
			}
		}

		public unsafe IntPtr this[int index]
		{
			get
			{
				return new IntPtr(Ptr[index]);
			}
			set
			{
				Ptr[index] = (void*)value;
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

		public unsafe bool IsCreated => Ptr != null;

		public unsafe ref IntPtr ElementAt(int index)
		{
			return ref *(IntPtr*)((byte*)Ptr + (nint)index * (nint)sizeof(IntPtr));
		}

		public unsafe UnsafePtrList(void** ptr, int length)
		{
			this = default(UnsafePtrList);
			Ptr = ptr;
			this.length = length;
			capacity = length;
			Allocator = AllocatorManager.None;
		}

		public unsafe UnsafePtrList(int initialCapacity, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			this = default(UnsafePtrList);
			Ptr = null;
			length = 0;
			capacity = 0;
			Allocator = AllocatorManager.None;
			int size = IntPtr.Size;
			this.ListData() = new UnsafeList(size, size, initialCapacity, allocator, options);
		}

		public unsafe UnsafePtrList(int initialCapacity, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			this = default(UnsafePtrList);
			Ptr = null;
			length = 0;
			capacity = 0;
			Allocator = AllocatorManager.None;
			int size = IntPtr.Size;
			this.ListData() = new UnsafeList(size, size, initialCapacity, allocator, options);
		}

		public unsafe static UnsafePtrList* Create(void** ptr, int length)
		{
			UnsafePtrList* intPtr = AllocatorManager.Allocate<UnsafePtrList>(AllocatorManager.Persistent);
			*intPtr = new UnsafePtrList(ptr, length);
			return intPtr;
		}

		public unsafe static UnsafePtrList* Create(int initialCapacity, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			UnsafePtrList* intPtr = AllocatorManager.Allocate<UnsafePtrList>(allocator);
			*intPtr = new UnsafePtrList(initialCapacity, allocator, options);
			return intPtr;
		}

		public unsafe static void Destroy(UnsafePtrList* listData)
		{
			AllocatorManager.AllocatorHandle handle = (((*listData).ListData().Allocator.Value == AllocatorManager.Invalid.Value) ? AllocatorManager.Persistent : (*listData).ListData().Allocator);
			listData->Dispose();
			AllocatorManager.Free(handle, listData);
		}

		public void Dispose()
		{
			this.ListData().Dispose();
		}

		[NotBurstCompatible]
		public JobHandle Dispose(JobHandle inputDeps)
		{
			return this.ListData().Dispose(inputDeps);
		}

		public void Clear()
		{
			this.ListData().Clear();
		}

		public void Resize(int length, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			this.ListData().Resize<IntPtr>(length, options);
		}

		public void SetCapacity(int capacity)
		{
			this.ListData().SetCapacity<IntPtr>(capacity);
		}

		public void TrimExcess()
		{
			this.ListData().TrimExcess<IntPtr>();
		}

		public unsafe int IndexOf(void* value)
		{
			for (int i = 0; i < Length; i++)
			{
				if (Ptr[i] == value)
				{
					return i;
				}
			}
			return -1;
		}

		public unsafe bool Contains(void* value)
		{
			return IndexOf(value) != -1;
		}

		public unsafe void AddNoResize(void* value)
		{
			this.ListData().AddNoResize((IntPtr)value);
		}

		public unsafe void AddRangeNoResize(void** ptr, int length)
		{
			this.ListData().AddRangeNoResize<IntPtr>(ptr, length);
		}

		public unsafe void AddRangeNoResize(UnsafePtrList list)
		{
			this.ListData().AddRangeNoResize<IntPtr>(list.Ptr, list.Length);
		}

		public void Add(in IntPtr value)
		{
			this.ListData().Add(value);
		}

		public unsafe void Add(void* value)
		{
			this.ListData().Add((IntPtr)value);
		}

		public unsafe void AddRange(void* ptr, int length)
		{
			this.ListData().AddRange<IntPtr>(ptr, length);
		}

		public void AddRange(UnsafePtrList list)
		{
			this.ListData().AddRange<IntPtr>(list.ListData());
		}

		public void InsertRangeWithBeginEnd(int begin, int end)
		{
			this.ListData().InsertRangeWithBeginEnd<IntPtr>(begin, end);
		}

		public void RemoveAtSwapBack(int index)
		{
			this.ListData().RemoveAtSwapBack<IntPtr>(index);
		}

		public void RemoveRangeSwapBackWithBeginEnd(int begin, int end)
		{
			this.ListData().RemoveRangeSwapBackWithBeginEnd<IntPtr>(begin, end);
		}

		public void RemoveAt(int index)
		{
			this.ListData().RemoveAt<IntPtr>(index);
		}

		public void RemoveRangeWithBeginEnd(int begin, int end)
		{
			this.ListData().RemoveRangeWithBeginEnd<IntPtr>(begin, end);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator<IntPtr> IEnumerable<IntPtr>.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public unsafe ParallelReader AsParallelReader()
		{
			return new ParallelReader(Ptr, Length);
		}

		public unsafe ParallelWriter AsParallelWriter()
		{
			return new ParallelWriter(Ptr, (UnsafeList*)UnsafeUtility.AddressOf(ref this));
		}
	}
	[DebuggerDisplay("Length = {Length}, Capacity = {Capacity}, IsCreated = {IsCreated}, IsEmpty = {IsEmpty}")]
	[DebuggerTypeProxy(typeof(UnsafePtrListTDebugView<>))]
	[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
	public struct UnsafePtrList<T> : INativeDisposable, IDisposable, IEnumerable<IntPtr>, IEnumerable where T : unmanaged
	{
		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public struct ParallelReader
		{
			[NativeDisableUnsafePtrRestriction]
			public unsafe readonly T** Ptr;

			public readonly int Length;

			internal unsafe ParallelReader(T** ptr, int length)
			{
				Ptr = ptr;
				Length = length;
			}

			public unsafe int IndexOf(void* ptr)
			{
				for (int i = 0; i < Length; i++)
				{
					if (Ptr[i] == ptr)
					{
						return i;
					}
				}
				return -1;
			}

			public unsafe bool Contains(void* ptr)
			{
				return IndexOf(ptr) != -1;
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public struct ParallelWriter
		{
			[NativeDisableUnsafePtrRestriction]
			public unsafe readonly T** Ptr;

			[NativeDisableUnsafePtrRestriction]
			public unsafe UnsafeList<IntPtr>* ListData;

			internal unsafe ParallelWriter(T** ptr, UnsafeList<IntPtr>* listData)
			{
				Ptr = ptr;
				ListData = listData;
			}

			public unsafe void AddNoResize(T* value)
			{
				ListData->AddNoResize((IntPtr)value);
			}

			public unsafe void AddRangeNoResize(T** ptr, int count)
			{
				ListData->AddRangeNoResize(ptr, count);
			}

			public unsafe void AddRangeNoResize(UnsafePtrList<T> list)
			{
				ListData->AddRangeNoResize(list.Ptr, list.Length);
			}
		}

		[NativeDisableUnsafePtrRestriction]
		public unsafe readonly T** Ptr;

		public readonly int m_length;

		public readonly int m_capacity;

		public readonly AllocatorManager.AllocatorHandle Allocator;

		[Obsolete("Use Length property (UnityUpgradable) -> Length", true)]
		public int length;

		[Obsolete("Use Capacity property (UnityUpgradable) -> Capacity", true)]
		public int capacity;

		public int Length
		{
			get
			{
				return UnsafePtrListTExtensions.ListData(ref this).Length;
			}
			set
			{
				UnsafePtrListTExtensions.ListData(ref this).Length = value;
			}
		}

		public int Capacity
		{
			get
			{
				return UnsafePtrListTExtensions.ListData(ref this).Capacity;
			}
			set
			{
				UnsafePtrListTExtensions.ListData(ref this).Capacity = value;
			}
		}

		public unsafe T* this[int index]
		{
			get
			{
				return Ptr[CollectionHelper.AssumePositive(index)];
			}
			set
			{
				Ptr[CollectionHelper.AssumePositive(index)] = value;
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

		public unsafe bool IsCreated => Ptr != null;

		public unsafe ref T* ElementAt(int index)
		{
			return ref Ptr[CollectionHelper.AssumePositive(index)];
		}

		public unsafe UnsafePtrList(T** ptr, int length)
		{
			this = default(UnsafePtrList<T>);
			Ptr = ptr;
			m_length = length;
			m_capacity = length;
			Allocator = AllocatorManager.None;
		}

		public unsafe UnsafePtrList(int initialCapacity, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			this = default(UnsafePtrList<T>);
			Ptr = null;
			m_length = 0;
			m_capacity = 0;
			Allocator = AllocatorManager.None;
			UnsafePtrListTExtensions.ListData(ref this) = new UnsafeList<IntPtr>(initialCapacity, allocator, options);
		}

		public unsafe static UnsafePtrList<T>* Create(T** ptr, int length)
		{
			UnsafePtrList<T>* intPtr = AllocatorManager.Allocate<UnsafePtrList<T>>(AllocatorManager.Persistent);
			*intPtr = new UnsafePtrList<T>(ptr, length);
			return intPtr;
		}

		public unsafe static UnsafePtrList<T>* Create(int initialCapacity, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			UnsafePtrList<T>* intPtr = AllocatorManager.Allocate<UnsafePtrList<T>>(allocator);
			*intPtr = new UnsafePtrList<T>(initialCapacity, allocator, options);
			return intPtr;
		}

		public unsafe static void Destroy(UnsafePtrList<T>* listData)
		{
			AllocatorManager.AllocatorHandle handle = ((UnsafePtrListTExtensions.ListData(ref *listData).Allocator.Value == AllocatorManager.Invalid.Value) ? AllocatorManager.Persistent : UnsafePtrListTExtensions.ListData(ref *listData).Allocator);
			listData->Dispose();
			AllocatorManager.Free(handle, listData);
		}

		public void Dispose()
		{
			UnsafePtrListTExtensions.ListData(ref this).Dispose();
		}

		[NotBurstCompatible]
		public JobHandle Dispose(JobHandle inputDeps)
		{
			return UnsafePtrListTExtensions.ListData(ref this).Dispose(inputDeps);
		}

		public void Clear()
		{
			UnsafePtrListTExtensions.ListData(ref this).Clear();
		}

		public void Resize(int length, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			UnsafePtrListTExtensions.ListData(ref this).Resize(length, options);
		}

		public void SetCapacity(int capacity)
		{
			UnsafePtrListTExtensions.ListData(ref this).SetCapacity(capacity);
		}

		public void TrimExcess()
		{
			UnsafePtrListTExtensions.ListData(ref this).TrimExcess();
		}

		public unsafe int IndexOf(void* ptr)
		{
			for (int i = 0; i < Length; i++)
			{
				if (Ptr[i] == ptr)
				{
					return i;
				}
			}
			return -1;
		}

		public unsafe bool Contains(void* ptr)
		{
			return IndexOf(ptr) != -1;
		}

		public unsafe void AddNoResize(void* value)
		{
			UnsafePtrListTExtensions.ListData(ref this).AddNoResize((IntPtr)value);
		}

		public unsafe void AddRangeNoResize(void** ptr, int count)
		{
			UnsafePtrListTExtensions.ListData(ref this).AddRangeNoResize(ptr, count);
		}

		public unsafe void AddRangeNoResize(UnsafePtrList<T> list)
		{
			UnsafePtrListTExtensions.ListData(ref this).AddRangeNoResize(list.Ptr, list.Length);
		}

		public void Add(in IntPtr value)
		{
			UnsafePtrListTExtensions.ListData(ref this).Add(in value);
		}

		public unsafe void Add(void* value)
		{
			ref UnsafeList<IntPtr> reference = ref UnsafePtrListTExtensions.ListData(ref this);
			IntPtr value2 = (IntPtr)value;
			reference.Add(in value2);
		}

		public unsafe void AddRange(void* ptr, int length)
		{
			UnsafePtrListTExtensions.ListData(ref this).AddRange(ptr, length);
		}

		public void AddRange(UnsafePtrList<T> list)
		{
			UnsafePtrListTExtensions.ListData(ref this).AddRange(UnsafePtrListTExtensions.ListData(ref list));
		}

		public void InsertRangeWithBeginEnd(int begin, int end)
		{
			UnsafePtrListTExtensions.ListData(ref this).InsertRangeWithBeginEnd(begin, end);
		}

		public void RemoveAtSwapBack(int index)
		{
			UnsafePtrListTExtensions.ListData(ref this).RemoveAtSwapBack(index);
		}

		public void RemoveRangeSwapBack(int index, int count)
		{
			UnsafePtrListTExtensions.ListData(ref this).RemoveRangeSwapBack(index, count);
		}

		[Obsolete("RemoveRangeSwapBackWithBeginEnd(begin, end) is deprecated, use RemoveRangeSwapBack(index, count) instead. (RemovedAfter 2021-06-02)", false)]
		public void RemoveRangeSwapBackWithBeginEnd(int begin, int end)
		{
			UnsafePtrListTExtensions.ListData(ref this).RemoveRangeSwapBackWithBeginEnd(begin, end);
		}

		public void RemoveAt(int index)
		{
			UnsafePtrListTExtensions.ListData(ref this).RemoveAt(index);
		}

		public void RemoveRange(int index, int count)
		{
			UnsafePtrListTExtensions.ListData(ref this).RemoveRange(index, count);
		}

		[Obsolete("RemoveRangeWithBeginEnd(begin, end) is deprecated, use RemoveRange(index, count) instead. (RemovedAfter 2021-06-02)", false)]
		public void RemoveRangeWithBeginEnd(int begin, int end)
		{
			UnsafePtrListTExtensions.ListData(ref this).RemoveRangeWithBeginEnd(begin, end);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator<IntPtr> IEnumerable<IntPtr>.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		public unsafe ParallelReader AsParallelReader()
		{
			return new ParallelReader(Ptr, Length);
		}

		public unsafe ParallelWriter AsParallelWriter()
		{
			return new ParallelWriter(Ptr, (UnsafeList<IntPtr>*)UnsafeUtility.AddressOf(ref this));
		}
	}
}
