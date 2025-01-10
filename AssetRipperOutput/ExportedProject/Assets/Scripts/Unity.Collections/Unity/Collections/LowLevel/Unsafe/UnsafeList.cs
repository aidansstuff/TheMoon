using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using Unity.Jobs;
using Unity.Mathematics;

namespace Unity.Collections.LowLevel.Unsafe
{
	[DebuggerDisplay("Length = {Length}, Capacity = {Capacity}, IsCreated = {IsCreated}, IsEmpty = {IsEmpty}")]
	[Obsolete("Untyped UnsafeList is deprecated, please use UnsafeList<T> instead. (RemovedAfter 2021-05-18)", false)]
	public struct UnsafeList : INativeDisposable, IDisposable
	{
		public struct ParallelReader
		{
			[NativeDisableUnsafePtrRestriction]
			public unsafe readonly void* Ptr;

			public readonly int Length;

			internal unsafe ParallelReader(void* ptr, int length)
			{
				Ptr = ptr;
				Length = length;
			}

			public unsafe int IndexOf<T>(T value) where T : struct, IEquatable<T>
			{
				return NativeArrayExtensions.IndexOf<T, T>(Ptr, Length, value);
			}

			public bool Contains<T>(T value) where T : struct, IEquatable<T>
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

			public unsafe void AddNoResize<T>(T value) where T : struct
			{
				int index = Interlocked.Increment(ref ListData->Length) - 1;
				UnsafeUtility.WriteArrayElement(Ptr, index, value);
			}

			private unsafe void AddRangeNoResize(int sizeOf, int alignOf, void* ptr, int length)
			{
				int num = Interlocked.Add(ref ListData->Length, length) - length;
				void* destination = (byte*)Ptr + num * sizeOf;
				UnsafeUtility.MemCpy(destination, ptr, length * sizeOf);
			}

			public unsafe void AddRangeNoResize<T>(void* ptr, int length) where T : struct
			{
				AddRangeNoResize(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), ptr, length);
			}

			public unsafe void AddRangeNoResize<T>(UnsafeList list) where T : struct
			{
				AddRangeNoResize(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), list.Ptr, list.Length);
			}
		}

		[NativeDisableUnsafePtrRestriction]
		public unsafe void* Ptr;

		public int Length;

		public readonly int unused;

		public int Capacity;

		public AllocatorManager.AllocatorHandle Allocator;

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

		public unsafe UnsafeList(Allocator allocator)
		{
			this = default(UnsafeList);
			Ptr = null;
			Length = 0;
			Capacity = 0;
			Allocator = allocator;
		}

		public unsafe UnsafeList(void* ptr, int length)
		{
			this = default(UnsafeList);
			Ptr = ptr;
			Length = length;
			Capacity = length;
			Allocator = Unity.Collections.Allocator.None;
		}

		internal unsafe void Initialize<U>(int sizeOf, int alignOf, int initialCapacity, ref U allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory) where U : unmanaged, AllocatorManager.IAllocator
		{
			Allocator = allocator.Handle;
			Ptr = null;
			Length = 0;
			Capacity = 0;
			if (initialCapacity != 0)
			{
				SetCapacity(ref allocator, sizeOf, alignOf, initialCapacity);
			}
			if (options == NativeArrayOptions.ClearMemory && Ptr != null)
			{
				UnsafeUtility.MemClear(Ptr, Capacity * sizeOf);
			}
		}

		internal static UnsafeList New<U>(int sizeOf, int alignOf, int initialCapacity, ref U allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory) where U : unmanaged, AllocatorManager.IAllocator
		{
			UnsafeList result = default(UnsafeList);
			result.Initialize(sizeOf, alignOf, initialCapacity, ref allocator, options);
			return result;
		}

		public UnsafeList(int sizeOf, int alignOf, int initialCapacity, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			this = default(UnsafeList);
			this = default(UnsafeList);
			Initialize(sizeOf, alignOf, initialCapacity, ref allocator, options);
		}

		public unsafe UnsafeList(int sizeOf, int alignOf, int initialCapacity, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			this = default(UnsafeList);
			Allocator = allocator;
			Ptr = null;
			Length = 0;
			Capacity = 0;
			if (initialCapacity != 0)
			{
				SetCapacity(sizeOf, alignOf, initialCapacity);
			}
			if (options == NativeArrayOptions.ClearMemory && Ptr != null)
			{
				UnsafeUtility.MemClear(Ptr, Capacity * sizeOf);
			}
		}

		public unsafe static UnsafeList* Create(int sizeOf, int alignOf, int initialCapacity, Allocator allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			UnsafeList* ptr = AllocatorManager.Allocate<UnsafeList>(allocator);
			UnsafeUtility.MemClear(ptr, UnsafeUtility.SizeOf<UnsafeList>());
			ptr->Allocator = allocator;
			if (initialCapacity != 0)
			{
				ptr->SetCapacity(sizeOf, alignOf, initialCapacity);
			}
			if (options == NativeArrayOptions.ClearMemory && ptr->Ptr != null)
			{
				UnsafeUtility.MemClear(ptr->Ptr, ptr->Capacity * sizeOf);
			}
			return ptr;
		}

		internal unsafe static UnsafeList* Create<U>(int sizeOf, int alignOf, int initialCapacity, ref U allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory) where U : unmanaged, AllocatorManager.IAllocator
		{
			UnsafeList* ptr = AllocatorManager.Allocate(ref allocator, default(UnsafeList), 1);
			UnsafeUtility.MemClear(ptr, UnsafeUtility.SizeOf<UnsafeList>());
			ptr->Allocator = allocator.Handle;
			if (initialCapacity != 0)
			{
				ptr->SetCapacity(ref allocator, sizeOf, alignOf, initialCapacity);
			}
			if (options == NativeArrayOptions.ClearMemory && ptr->Ptr != null)
			{
				UnsafeUtility.MemClear(ptr->Ptr, ptr->Capacity * sizeOf);
			}
			return ptr;
		}

		internal unsafe static void Destroy<U>(UnsafeList* listData, ref U allocator, int sizeOf, int alignOf) where U : unmanaged, AllocatorManager.IAllocator
		{
			listData->Dispose(ref allocator, sizeOf, alignOf);
			AllocatorManager.Free(ref allocator, listData, UnsafeUtility.SizeOf<UnsafeList>(), UnsafeUtility.AlignOf<UnsafeList>(), 1);
		}

		public unsafe static void Destroy(UnsafeList* listData)
		{
			AllocatorManager.AllocatorHandle allocator = listData->Allocator;
			listData->Dispose();
			AllocatorManager.Free(allocator, listData);
		}

		public unsafe void Dispose()
		{
			if (CollectionHelper.ShouldDeallocate(Allocator))
			{
				AllocatorManager.Free(Allocator, Ptr);
				Allocator = AllocatorManager.Invalid;
			}
			Ptr = null;
			Length = 0;
			Capacity = 0;
		}

		internal unsafe void Dispose<U>(ref U allocator, int sizeOf, int alignOf) where U : unmanaged, AllocatorManager.IAllocator
		{
			AllocatorManager.Free(ref allocator, Ptr, sizeOf, alignOf, Length);
			Ptr = null;
			Length = 0;
			Capacity = 0;
		}

		[NotBurstCompatible]
		public unsafe JobHandle Dispose(JobHandle inputDeps)
		{
			if (CollectionHelper.ShouldDeallocate(Allocator))
			{
				UnsafeDisposeJob jobData = default(UnsafeDisposeJob);
				jobData.Ptr = Ptr;
				jobData.Allocator = (Allocator)Allocator.Value;
				JobHandle result = IJobExtensions.Schedule(jobData, inputDeps);
				Ptr = null;
				Allocator = AllocatorManager.Invalid;
				return result;
			}
			Ptr = null;
			return inputDeps;
		}

		public void Clear()
		{
			Length = 0;
		}

		public unsafe void Resize(int sizeOf, int alignOf, int length, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			int length2 = Length;
			if (length > Capacity)
			{
				SetCapacity(sizeOf, alignOf, length);
			}
			Length = length;
			if (options == NativeArrayOptions.ClearMemory && length2 < length)
			{
				int num = length - length2;
				byte* ptr = (byte*)Ptr;
				UnsafeUtility.MemClear(ptr + length2 * sizeOf, num * sizeOf);
			}
		}

		public void Resize<T>(int length, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory) where T : struct
		{
			Resize(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), length, options);
		}

		private unsafe void Realloc<U>(ref U allocator, int sizeOf, int alignOf, int capacity) where U : unmanaged, AllocatorManager.IAllocator
		{
			void* ptr = null;
			if (capacity > 0)
			{
				ptr = AllocatorManager.Allocate(ref allocator, sizeOf, alignOf, capacity);
				if (Capacity > 0)
				{
					int num = math.min(capacity, Capacity) * sizeOf;
					UnsafeUtility.MemCpy(ptr, Ptr, num);
				}
			}
			AllocatorManager.Free(ref allocator, Ptr, sizeOf, alignOf, Capacity);
			Ptr = ptr;
			Capacity = capacity;
			Length = math.min(Length, capacity);
		}

		private void Realloc(int sizeOf, int alignOf, int capacity)
		{
			Realloc(ref Allocator, sizeOf, alignOf, capacity);
		}

		private void SetCapacity<U>(ref U allocator, int sizeOf, int alignOf, int capacity) where U : unmanaged, AllocatorManager.IAllocator
		{
			int x = math.max(capacity, 64 / sizeOf);
			x = math.ceilpow2(x);
			if (x != Capacity)
			{
				Realloc(ref allocator, sizeOf, alignOf, x);
			}
		}

		private void SetCapacity(int sizeOf, int alignOf, int capacity)
		{
			SetCapacity(ref Allocator, sizeOf, alignOf, capacity);
		}

		public void SetCapacity<T>(int capacity) where T : struct
		{
			SetCapacity(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), capacity);
		}

		public void TrimExcess<T>() where T : struct
		{
			if (Capacity != Length)
			{
				Realloc(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), Length);
			}
		}

		public unsafe int IndexOf<T>(T value) where T : struct, IEquatable<T>
		{
			return NativeArrayExtensions.IndexOf<T, T>(Ptr, Length, value);
		}

		public bool Contains<T>(T value) where T : struct, IEquatable<T>
		{
			return IndexOf(value) != -1;
		}

		public unsafe void AddNoResize<T>(T value) where T : struct
		{
			UnsafeUtility.WriteArrayElement(Ptr, Length, value);
			Length++;
		}

		private unsafe void AddRangeNoResize(int sizeOf, void* ptr, int length)
		{
			void* destination = (byte*)Ptr + Length * sizeOf;
			UnsafeUtility.MemCpy(destination, ptr, length * sizeOf);
			Length += length;
		}

		public unsafe void AddRangeNoResize<T>(void* ptr, int length) where T : struct
		{
			AddRangeNoResize(UnsafeUtility.SizeOf<T>(), ptr, length);
		}

		public unsafe void AddRangeNoResize<T>(UnsafeList list) where T : struct
		{
			AddRangeNoResize(UnsafeUtility.SizeOf<T>(), list.Ptr, CollectionHelper.AssumePositive(list.Length));
		}

		public unsafe void Add<T>(T value) where T : struct
		{
			int length = Length;
			if (Length + 1 > Capacity)
			{
				Resize<T>(length + 1);
			}
			else
			{
				Length++;
			}
			UnsafeUtility.WriteArrayElement(Ptr, length, value);
		}

		private unsafe void AddRange(int sizeOf, int alignOf, void* ptr, int length)
		{
			int length2 = Length;
			if (Length + length > Capacity)
			{
				Resize(sizeOf, alignOf, Length + length);
			}
			else
			{
				Length += length;
			}
			void* destination = (byte*)Ptr + length2 * sizeOf;
			UnsafeUtility.MemCpy(destination, ptr, length * sizeOf);
		}

		public unsafe void AddRange<T>(void* ptr, int length) where T : struct
		{
			AddRange(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), ptr, length);
		}

		public unsafe void AddRange<T>(UnsafeList list) where T : struct
		{
			AddRange(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), list.Ptr, list.Length);
		}

		private unsafe void InsertRangeWithBeginEnd(int sizeOf, int alignOf, int begin, int end)
		{
			int num = end - begin;
			if (num >= 1)
			{
				int length = Length;
				if (Length + num > Capacity)
				{
					Resize(sizeOf, alignOf, Length + num);
				}
				else
				{
					Length += num;
				}
				int num2 = length - begin;
				if (num2 >= 1)
				{
					int num3 = num2 * sizeOf;
					byte* ptr = (byte*)Ptr;
					byte* destination = ptr + end * sizeOf;
					byte* source = ptr + begin * sizeOf;
					UnsafeUtility.MemMove(destination, source, num3);
				}
			}
		}

		public void InsertRangeWithBeginEnd<T>(int begin, int end) where T : struct
		{
			InsertRangeWithBeginEnd(UnsafeUtility.SizeOf<T>(), UnsafeUtility.AlignOf<T>(), begin, end);
		}

		private unsafe void RemoveRangeSwapBackWithBeginEnd(int sizeOf, int begin, int end)
		{
			int num = end - begin;
			if (num > 0)
			{
				int num2 = math.max(Length - num, end);
				void* destination = (byte*)Ptr + begin * sizeOf;
				void* source = (byte*)Ptr + num2 * sizeOf;
				UnsafeUtility.MemCpy(destination, source, (Length - num2) * sizeOf);
				Length -= num;
			}
		}

		public void RemoveAtSwapBack<T>(int index) where T : struct
		{
			RemoveRangeSwapBackWithBeginEnd<T>(index, index + 1);
		}

		public void RemoveRangeSwapBackWithBeginEnd<T>(int begin, int end) where T : struct
		{
			RemoveRangeSwapBackWithBeginEnd(UnsafeUtility.SizeOf<T>(), begin, end);
		}

		private unsafe void RemoveRangeWithBeginEnd(int sizeOf, int begin, int end)
		{
			int num = end - begin;
			if (num > 0)
			{
				int num2 = math.min(begin + num, Length);
				void* destination = (byte*)Ptr + begin * sizeOf;
				void* source = (byte*)Ptr + num2 * sizeOf;
				UnsafeUtility.MemCpy(destination, source, (Length - num2) * sizeOf);
				Length -= num;
			}
		}

		public void RemoveAt<T>(int index) where T : struct
		{
			RemoveRangeWithBeginEnd<T>(index, index + 1);
		}

		public void RemoveRangeWithBeginEnd<T>(int begin, int end) where T : struct
		{
			RemoveRangeWithBeginEnd(UnsafeUtility.SizeOf<T>(), begin, end);
		}

		public unsafe ParallelReader AsParallelReader()
		{
			return new ParallelReader(Ptr, Length);
		}

		public unsafe ParallelWriter AsParallelWriter()
		{
			return new ParallelWriter(Ptr, (UnsafeList*)UnsafeUtility.AddressOf(ref this));
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		internal unsafe static void CheckNull(void* listData)
		{
			if (listData == null)
			{
				throw new Exception("UnsafeList has yet to be created or has been destroyed!");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckAllocator(Allocator a)
		{
			if (!CollectionHelper.ShouldDeallocate(a))
			{
				throw new Exception("UnsafeList is not initialized, it must be initialized with allocator before use.");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckAllocator(AllocatorManager.AllocatorHandle a)
		{
			if (!CollectionHelper.ShouldDeallocate(a))
			{
				throw new Exception("UnsafeList is not initialized, it must be initialized with allocator before use.");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckBeginEnd(int begin, int end)
		{
			if (begin > end)
			{
				throw new ArgumentException($"Value for begin {begin} index must less or equal to end {end}.");
			}
			if (begin < 0)
			{
				throw new ArgumentOutOfRangeException($"Value for begin {begin} must be positive.");
			}
			if (begin > Length)
			{
				throw new ArgumentOutOfRangeException($"Value for begin {begin} is out of bounds.");
			}
			if (end > Length)
			{
				throw new ArgumentOutOfRangeException($"Value for end {end} is out of bounds.");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckNoResizeHasEnoughCapacity(int length)
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckNoResizeHasEnoughCapacity(int length, int index)
		{
			if (Capacity < index + length)
			{
				throw new Exception($"AddNoResize assumes that list capacity is sufficient (Capacity {Capacity}, Length {Length}), requested length {length}!");
			}
		}
	}
	[DebuggerDisplay("Length = {Length}, Capacity = {Capacity}, IsCreated = {IsCreated}, IsEmpty = {IsEmpty}")]
	[DebuggerTypeProxy(typeof(UnsafeListTDebugView<>))]
	[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
	public struct UnsafeList<T> : INativeDisposable, IDisposable, INativeList<T>, IIndexable<T>, IEnumerable<T>, IEnumerable where T : unmanaged
	{
		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public struct ParallelReader
		{
			[NativeDisableUnsafePtrRestriction]
			public unsafe readonly T* Ptr;

			public readonly int Length;

			internal unsafe ParallelReader(T* ptr, int length)
			{
				Ptr = ptr;
				Length = length;
			}
		}

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

			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe void AddNoResize(T value)
			{
				int index = Interlocked.Increment(ref ListData->m_length) - 1;
				UnsafeUtility.WriteArrayElement(ListData->Ptr, index, value);
			}

			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe void AddRangeNoResize(void* ptr, int count)
			{
				int num = Interlocked.Add(ref ListData->m_length, count) - count;
				void* destination = (byte*)ListData->Ptr + num * sizeof(T);
				UnsafeUtility.MemCpy(destination, ptr, count * sizeof(T));
			}

			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe void AddRangeNoResize(UnsafeList<T> list)
			{
				AddRangeNoResize(list.Ptr, list.Length);
			}
		}

		public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
		{
			internal unsafe T* m_Ptr;

			internal int m_Length;

			internal int m_Index;

			public unsafe T Current => m_Ptr[m_Index];

			object IEnumerator.Current => Current;

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				return ++m_Index < m_Length;
			}

			public void Reset()
			{
				m_Index = -1;
			}
		}

		[NativeDisableUnsafePtrRestriction]
		public unsafe T* Ptr;

		public int m_length;

		public int m_capacity;

		public AllocatorManager.AllocatorHandle Allocator;

		[Obsolete("Use Length property (UnityUpgradable) -> Length", true)]
		public int length;

		[Obsolete("Use Capacity property (UnityUpgradable) -> Capacity", true)]
		public int capacity;

		public int Length
		{
			get
			{
				return CollectionHelper.AssumePositive(m_length);
			}
			set
			{
				if (value > Capacity)
				{
					Resize(value);
				}
				else
				{
					m_length = value;
				}
			}
		}

		public int Capacity
		{
			get
			{
				return CollectionHelper.AssumePositive(m_capacity);
			}
			set
			{
				SetCapacity(value);
			}
		}

		public unsafe T this[int index]
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
					return m_length == 0;
				}
				return true;
			}
		}

		public unsafe bool IsCreated => Ptr != null;

		public unsafe ref T ElementAt(int index)
		{
			return ref Ptr[CollectionHelper.AssumePositive(index)];
		}

		public unsafe UnsafeList(T* ptr, int length)
		{
			this = default(UnsafeList<T>);
			Ptr = ptr;
			m_length = length;
			m_capacity = 0;
			Allocator = AllocatorManager.None;
		}

		public unsafe UnsafeList(int initialCapacity, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			this = default(UnsafeList<T>);
			Ptr = null;
			m_length = 0;
			m_capacity = 0;
			Allocator = allocator;
			if (initialCapacity != 0)
			{
				SetCapacity(initialCapacity);
			}
			if (options == NativeArrayOptions.ClearMemory && Ptr != null)
			{
				int num = sizeof(T);
				UnsafeUtility.MemClear(Ptr, Capacity * num);
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(AllocatorManager.AllocatorHandle) })]
		internal unsafe void Initialize<U>(int initialCapacity, ref U allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory) where U : unmanaged, AllocatorManager.IAllocator
		{
			Ptr = null;
			m_length = 0;
			m_capacity = 0;
			Allocator = AllocatorManager.None;
			Initialize(initialCapacity, ref allocator, options);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(AllocatorManager.AllocatorHandle) })]
		internal static UnsafeList<T> New<U>(int initialCapacity, ref U allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory) where U : unmanaged, AllocatorManager.IAllocator
		{
			UnsafeList<T> result = default(UnsafeList<T>);
			result.Initialize(initialCapacity, ref allocator, options);
			return result;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(AllocatorManager.AllocatorHandle) })]
		internal unsafe static UnsafeList<T>* Create<U>(int initialCapacity, ref U allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory) where U : unmanaged, AllocatorManager.IAllocator
		{
			UnsafeList<T>* ptr = AllocatorManager.Allocate(ref allocator, default(UnsafeList<T>), 1);
			UnsafeUtility.MemClear(ptr, sizeof(UnsafeList<T>));
			ptr->Allocator = allocator.Handle;
			if (initialCapacity != 0)
			{
				ptr->SetCapacity(ref allocator, initialCapacity);
			}
			if (options == NativeArrayOptions.ClearMemory && ptr->Ptr != null)
			{
				int num = sizeof(T);
				UnsafeUtility.MemClear(ptr->Ptr, ptr->Capacity * num);
			}
			return ptr;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(AllocatorManager.AllocatorHandle) })]
		internal unsafe static void Destroy<U>(UnsafeList<T>* listData, ref U allocator) where U : unmanaged, AllocatorManager.IAllocator
		{
			listData->Dispose(ref allocator);
			AllocatorManager.Free(ref allocator, listData, sizeof(UnsafeList<T>), UnsafeUtility.AlignOf<UnsafeList<T>>(), 1);
		}

		public unsafe static UnsafeList<T>* Create(int initialCapacity, AllocatorManager.AllocatorHandle allocator, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			UnsafeList<T>* intPtr = AllocatorManager.Allocate<UnsafeList<T>>(allocator);
			*intPtr = new UnsafeList<T>(initialCapacity, allocator, options);
			return intPtr;
		}

		public unsafe static void Destroy(UnsafeList<T>* listData)
		{
			AllocatorManager.AllocatorHandle allocator = listData->Allocator;
			listData->Dispose();
			AllocatorManager.Free(allocator, listData);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(AllocatorManager.AllocatorHandle) })]
		internal unsafe void Dispose<U>(ref U allocator) where U : unmanaged, AllocatorManager.IAllocator
		{
			AllocatorManager.Free(ref allocator, Ptr, m_length);
			Ptr = null;
			m_length = 0;
			m_capacity = 0;
		}

		public unsafe void Dispose()
		{
			if (CollectionHelper.ShouldDeallocate(Allocator))
			{
				AllocatorManager.Free(Allocator, Ptr);
				Allocator = AllocatorManager.Invalid;
			}
			Ptr = null;
			m_length = 0;
			m_capacity = 0;
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

		public void Clear()
		{
			m_length = 0;
		}

		public unsafe void Resize(int length, NativeArrayOptions options = NativeArrayOptions.UninitializedMemory)
		{
			int num = m_length;
			if (length > Capacity)
			{
				SetCapacity(length);
			}
			m_length = length;
			if (options == NativeArrayOptions.ClearMemory && num < length)
			{
				int num2 = length - num;
				byte* ptr = (byte*)Ptr;
				int num3 = sizeof(T);
				UnsafeUtility.MemClear(ptr + num * num3, num2 * num3);
			}
		}

		private unsafe void Realloc<U>(ref U allocator, int newCapacity) where U : unmanaged, AllocatorManager.IAllocator
		{
			T* ptr = null;
			int alignOf = UnsafeUtility.AlignOf<T>();
			int num = sizeof(T);
			if (newCapacity > 0)
			{
				ptr = (T*)AllocatorManager.Allocate(ref allocator, num, alignOf, newCapacity);
				if (m_capacity > 0)
				{
					int num2 = math.min(newCapacity, Capacity) * num;
					UnsafeUtility.MemCpy(ptr, Ptr, num2);
				}
			}
			AllocatorManager.Free(ref allocator, Ptr, Capacity);
			Ptr = ptr;
			m_capacity = newCapacity;
			m_length = math.min(m_length, newCapacity);
		}

		private void Realloc(int capacity)
		{
			Realloc(ref Allocator, capacity);
		}

		private unsafe void SetCapacity<U>(ref U allocator, int capacity) where U : unmanaged, AllocatorManager.IAllocator
		{
			int num = sizeof(T);
			int x = math.max(capacity, 64 / num);
			x = math.ceilpow2(x);
			if (x != Capacity)
			{
				Realloc(ref allocator, x);
			}
		}

		public void SetCapacity(int capacity)
		{
			SetCapacity(ref Allocator, capacity);
		}

		public void TrimExcess()
		{
			if (Capacity != m_length)
			{
				Realloc(m_length);
			}
		}

		public unsafe void AddNoResize(T value)
		{
			UnsafeUtility.WriteArrayElement(Ptr, m_length, value);
			m_length++;
		}

		public unsafe void AddRangeNoResize(void* ptr, int count)
		{
			int num = sizeof(T);
			void* destination = (byte*)Ptr + m_length * num;
			UnsafeUtility.MemCpy(destination, ptr, count * num);
			m_length += count;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe void AddRangeNoResize(UnsafeList<T> list)
		{
			AddRangeNoResize(list.Ptr, CollectionHelper.AssumePositive(list.m_length));
		}

		public unsafe void Add(in T value)
		{
			int num = m_length;
			if (m_length + 1 > Capacity)
			{
				Resize(num + 1);
			}
			else
			{
				m_length++;
			}
			UnsafeUtility.WriteArrayElement(Ptr, num, value);
		}

		public unsafe void AddRange(void* ptr, int count)
		{
			int num = m_length;
			if (m_length + count > Capacity)
			{
				Resize(m_length + count);
			}
			else
			{
				m_length += count;
			}
			int num2 = sizeof(T);
			void* destination = (byte*)Ptr + num * num2;
			UnsafeUtility.MemCpy(destination, ptr, count * num2);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe void AddRange(UnsafeList<T> list)
		{
			AddRange(list.Ptr, list.Length);
		}

		public unsafe void InsertRangeWithBeginEnd(int begin, int end)
		{
			int num = end - begin;
			if (num >= 1)
			{
				int num2 = m_length;
				if (m_length + num > Capacity)
				{
					Resize(m_length + num);
				}
				else
				{
					m_length += num;
				}
				int num3 = num2 - begin;
				if (num3 >= 1)
				{
					int num4 = sizeof(T);
					int num5 = num3 * num4;
					byte* ptr = (byte*)Ptr;
					byte* destination = ptr + end * num4;
					byte* source = ptr + begin * num4;
					UnsafeUtility.MemMove(destination, source, num5);
				}
			}
		}

		public void RemoveAtSwapBack(int index)
		{
			RemoveRangeSwapBack(index, 1);
		}

		public unsafe void RemoveRangeSwapBack(int index, int count)
		{
			if (count > 0)
			{
				int num = math.max(m_length - count, index + count);
				int num2 = sizeof(T);
				void* destination = (byte*)Ptr + index * num2;
				void* source = (byte*)Ptr + num * num2;
				UnsafeUtility.MemCpy(destination, source, (m_length - num) * num2);
				m_length -= count;
			}
		}

		[Obsolete("RemoveRangeSwapBackWithBeginEnd(begin, end) is deprecated, use RemoveRangeSwapBack(index, count) instead. (RemovedAfter 2021-06-02)", false)]
		public unsafe void RemoveRangeSwapBackWithBeginEnd(int begin, int end)
		{
			int num = end - begin;
			if (num > 0)
			{
				int num2 = math.max(m_length - num, end);
				int num3 = sizeof(T);
				void* destination = (byte*)Ptr + begin * num3;
				void* source = (byte*)Ptr + num2 * num3;
				UnsafeUtility.MemCpy(destination, source, (m_length - num2) * num3);
				m_length -= num;
			}
		}

		public void RemoveAt(int index)
		{
			RemoveRange(index, 1);
		}

		public unsafe void RemoveRange(int index, int count)
		{
			if (count > 0)
			{
				int num = math.min(index + count, m_length);
				int num2 = sizeof(T);
				void* destination = (byte*)Ptr + index * num2;
				void* source = (byte*)Ptr + num * num2;
				UnsafeUtility.MemCpy(destination, source, (m_length - num) * num2);
				m_length -= count;
			}
		}

		[Obsolete("RemoveRangeWithBeginEnd(begin, end) is deprecated, use RemoveRange(index, count) instead. (RemovedAfter 2021-06-02)", false)]
		public unsafe void RemoveRangeWithBeginEnd(int begin, int end)
		{
			int num = end - begin;
			if (num > 0)
			{
				int num2 = math.min(begin + num, m_length);
				int num3 = sizeof(T);
				void* destination = (byte*)Ptr + begin * num3;
				void* source = (byte*)Ptr + num2 * num3;
				UnsafeUtility.MemCpy(destination, source, (m_length - num2) * num3);
				m_length -= num;
			}
		}

		public unsafe ParallelReader AsParallelReader()
		{
			return new ParallelReader(Ptr, Length);
		}

		public unsafe ParallelWriter AsParallelWriter()
		{
			return new ParallelWriter((UnsafeList<T>*)UnsafeUtility.AddressOf(ref this));
		}

		public unsafe void CopyFrom(UnsafeList<T> array)
		{
			Resize(array.Length);
			UnsafeUtility.MemCpy(Ptr, array.Ptr, UnsafeUtility.SizeOf<T>() * Length);
		}

		public unsafe Enumerator GetEnumerator()
		{
			Enumerator result = default(Enumerator);
			result.m_Ptr = Ptr;
			result.m_Length = Length;
			result.m_Index = -1;
			return result;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		internal unsafe static void CheckNull(void* listData)
		{
			if (listData == null)
			{
				throw new Exception("UnsafeList has yet to be created or has been destroyed!");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckIndexCount(int index, int count)
		{
			if (count < 0)
			{
				throw new ArgumentOutOfRangeException($"Value for cound {count} must be positive.");
			}
			if (index < 0)
			{
				throw new ArgumentOutOfRangeException($"Value for index {index} must be positive.");
			}
			if (index > Length)
			{
				throw new ArgumentOutOfRangeException($"Value for index {index} is out of bounds.");
			}
			if (index + count > Length)
			{
				throw new ArgumentOutOfRangeException($"Value for count {count} is out of bounds.");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckBeginEnd(int begin, int end)
		{
			if (begin > end)
			{
				throw new ArgumentException($"Value for begin {begin} index must less or equal to end {end}.");
			}
			if (begin < 0)
			{
				throw new ArgumentOutOfRangeException($"Value for begin {begin} must be positive.");
			}
			if (begin > Length)
			{
				throw new ArgumentOutOfRangeException($"Value for begin {begin} is out of bounds.");
			}
			if (end > Length)
			{
				throw new ArgumentOutOfRangeException($"Value for end {end} is out of bounds.");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckNoResizeHasEnoughCapacity(int length)
		{
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckNoResizeHasEnoughCapacity(int length, int index)
		{
			if (Capacity < index + length)
			{
				throw new Exception($"AddNoResize assumes that list capacity is sufficient (Capacity {Capacity}, Length {Length}), requested length {length}!");
			}
		}
	}
}
