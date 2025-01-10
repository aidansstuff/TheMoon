using System;
using System.Diagnostics;
using Unity.Collections.LowLevel.Unsafe.NotBurstCompatible;
using Unity.Jobs;
using Unity.Mathematics;

namespace Unity.Collections.LowLevel.Unsafe
{
	[BurstCompatible]
	public struct UnsafeAppendBuffer : INativeDisposable, IDisposable
	{
		[BurstCompatible]
		public struct Reader
		{
			public unsafe readonly byte* Ptr;

			public readonly int Size;

			public int Offset;

			public bool EndOfBuffer => Offset == Size;

			public unsafe Reader(ref UnsafeAppendBuffer buffer)
			{
				Ptr = buffer.Ptr;
				Size = buffer.Length;
				Offset = 0;
			}

			public unsafe Reader(void* ptr, int length)
			{
				Ptr = (byte*)ptr;
				Size = length;
				Offset = 0;
			}

			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe void ReadNext<T>(out T value) where T : struct
			{
				int num = UnsafeUtility.SizeOf<T>();
				UnsafeUtility.CopyPtrToStructure<T>(Ptr + Offset, out value);
				Offset += num;
			}

			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe T ReadNext<T>() where T : struct
			{
				int num = UnsafeUtility.SizeOf<T>();
				T result = UnsafeUtility.ReadArrayElement<T>(Ptr + Offset, 0);
				Offset += num;
				return result;
			}

			public unsafe void* ReadNext(int structSize)
			{
				void* result = (void*)((IntPtr)Ptr + Offset);
				Offset += structSize;
				return result;
			}

			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe void ReadNext<T>(out NativeArray<T> value, AllocatorManager.AllocatorHandle allocator) where T : struct
			{
				int num = ReadNext<int>();
				value = CollectionHelper.CreateNativeArray<T>(num, allocator);
				int num2 = num * UnsafeUtility.SizeOf<T>();
				if (num2 > 0)
				{
					void* source = ReadNext(num2);
					UnsafeUtility.MemCpy(value.GetUnsafePtr(), source, num2);
				}
			}

			[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
			public unsafe void* ReadNextArray<T>(out int length) where T : struct
			{
				length = ReadNext<int>();
				if (length != 0)
				{
					return ReadNext(length * UnsafeUtility.SizeOf<T>());
				}
				return null;
			}

			[NotBurstCompatible]
			[Obsolete("Please use `ReadNextNBC` from `Unity.Collections.LowLevel.Unsafe.NotBurstCompatible` namespace instead. (RemovedAfter 2021-06-22)", false)]
			public void ReadNext(out string value)
			{
				this.ReadNextNBC(out value);
			}

			[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
			private void CheckBounds(int structSize)
			{
				if (Offset + structSize > Size)
				{
					throw new ArgumentException($"Requested value outside bounds of UnsafeAppendOnlyBuffer. Remaining bytes: {Size - Offset} Requested: {structSize}");
				}
			}
		}

		[NativeDisableUnsafePtrRestriction]
		public unsafe byte* Ptr;

		public int Length;

		public int Capacity;

		public AllocatorManager.AllocatorHandle Allocator;

		public readonly int Alignment;

		public bool IsEmpty => Length == 0;

		public unsafe bool IsCreated => Ptr != null;

		public unsafe UnsafeAppendBuffer(int initialCapacity, int alignment, AllocatorManager.AllocatorHandle allocator)
		{
			Alignment = alignment;
			Allocator = allocator;
			Ptr = null;
			Length = 0;
			Capacity = 0;
			SetCapacity(initialCapacity);
		}

		public unsafe UnsafeAppendBuffer(void* ptr, int length)
		{
			Alignment = 0;
			Allocator = AllocatorManager.None;
			Ptr = (byte*)ptr;
			Length = 0;
			Capacity = length;
		}

		public unsafe void Dispose()
		{
			if (CollectionHelper.ShouldDeallocate(Allocator))
			{
				Memory.Unmanaged.Free(Ptr, Allocator);
				Allocator = AllocatorManager.Invalid;
			}
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
				jobData.Allocator = Allocator;
				JobHandle result = IJobExtensions.Schedule(jobData, inputDeps);
				Ptr = null;
				Allocator = AllocatorManager.Invalid;
				return result;
			}
			Ptr = null;
			return inputDeps;
		}

		public void Reset()
		{
			Length = 0;
		}

		public unsafe void SetCapacity(int capacity)
		{
			if (capacity > Capacity)
			{
				capacity = math.max(64, math.ceilpow2(capacity));
				byte* ptr = (byte*)Memory.Unmanaged.Allocate(capacity, Alignment, Allocator);
				if (Ptr != null)
				{
					UnsafeUtility.MemCpy(ptr, Ptr, Length);
					Memory.Unmanaged.Free(Ptr, Allocator);
				}
				Ptr = ptr;
				Capacity = capacity;
			}
		}

		public void ResizeUninitialized(int length)
		{
			SetCapacity(length);
			Length = length;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe void Add<T>(T value) where T : struct
		{
			int num = UnsafeUtility.SizeOf<T>();
			SetCapacity(Length + num);
			UnsafeUtility.CopyStructureToPtr(ref value, Ptr + Length);
			Length += num;
		}

		public unsafe void Add(void* ptr, int structSize)
		{
			SetCapacity(Length + structSize);
			UnsafeUtility.MemCpy(Ptr + Length, ptr, structSize);
			Length += structSize;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe void AddArray<T>(void* ptr, int length) where T : struct
		{
			Add(length);
			if (length != 0)
			{
				Add(ptr, length * UnsafeUtility.SizeOf<T>());
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe void Add<T>(NativeArray<T> value) where T : struct
		{
			Add(value.Length);
			Add(value.GetUnsafeReadOnlyPtr(), UnsafeUtility.SizeOf<T>() * value.Length);
		}

		[NotBurstCompatible]
		[Obsolete("Please use `AddNBC` from `Unity.Collections.LowLevel.Unsafe.NotBurstCompatible` namespace instead. (RemovedAfter 2021-06-22)", false)]
		public void Add(string value)
		{
			this.AddNBC(value);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		public unsafe T Pop<T>() where T : struct
		{
			int num = UnsafeUtility.SizeOf<T>();
			long num2 = (long)Ptr;
			long num3 = Length;
			T result = UnsafeUtility.ReadArrayElement<T>((void*)(num2 + num3 - num), 0);
			Length -= num;
			return result;
		}

		public unsafe void Pop(void* ptr, int structSize)
		{
			long num = (long)Ptr;
			long num2 = Length;
			long num3 = num + num2 - structSize;
			UnsafeUtility.MemCpy(ptr, (void*)num3, structSize);
			Length -= structSize;
		}

		[NotBurstCompatible]
		[Obsolete("Please use `ToBytesNBC` from `Unity.Collections.LowLevel.Unsafe.NotBurstCompatible` namespace instead. (RemovedAfter 2021-06-22)", false)]
		public byte[] ToBytes()
		{
			return this.ToBytesNBC();
		}

		public Reader AsReader()
		{
			return new Reader(ref this);
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckAlignment(int alignment)
		{
			bool num = alignment == 0;
			bool flag = ((alignment - 1) & alignment) == 0;
			if (!(!num && flag))
			{
				throw new ArgumentException($"Specified alignment must be non-zero positive power of two. Requested: {alignment}");
			}
		}
	}
}
