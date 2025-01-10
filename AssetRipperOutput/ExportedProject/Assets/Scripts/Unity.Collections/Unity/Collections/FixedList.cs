using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.Collections
{
	[Serializable]
	[BurstCompatible(GenericTypeArguments = new Type[]
	{
		typeof(int),
		typeof(FixedBytes30)
	})]
	internal struct FixedList<T, U> : INativeList<T>, IIndexable<T> where T : unmanaged where U : unmanaged
	{
		[SerializeField]
		internal ushort length;

		[SerializeField]
		internal U buffer;

		[CreateProperty]
		public int Length
		{
			get
			{
				return length;
			}
			set
			{
				length = (ushort)value;
			}
		}

		[CreateProperty]
		private IEnumerable<T> Elements => ToArray();

		public bool IsEmpty => Length == 0;

		internal int LengthInBytes => Length * UnsafeUtility.SizeOf<T>();

		internal unsafe byte* Buffer
		{
			get
			{
				fixed (U* ptr = &buffer)
				{
					return (byte*)ptr + FixedList.PaddingBytes<T>();
				}
			}
		}

		public int Capacity
		{
			get
			{
				return FixedList.Capacity<U, T>();
			}
			set
			{
			}
		}

		public unsafe T this[int index]
		{
			get
			{
				return UnsafeUtility.ReadArrayElement<T>(Buffer, CollectionHelper.AssumePositive(index));
			}
			set
			{
				UnsafeUtility.WriteArrayElement(Buffer, CollectionHelper.AssumePositive(index), value);
			}
		}

		public unsafe ref T ElementAt(int index)
		{
			return ref UnsafeUtility.ArrayElementAsRef<T>(Buffer, index);
		}

		public unsafe override int GetHashCode()
		{
			return (int)CollectionHelper.Hash(Buffer, LengthInBytes);
		}

		public void Add(in T item)
		{
			this[Length++] = item;
		}

		public unsafe void AddRange(void* ptr, int length)
		{
			for (int i = 0; i < length; i++)
			{
				this[Length++] = *(T*)((byte*)ptr + (nint)i * (nint)sizeof(T));
			}
		}

		public void AddNoResize(in T item)
		{
			Add(in item);
		}

		public unsafe void AddRangeNoResize(void* ptr, int length)
		{
			AddRange(ptr, length);
		}

		public void Clear()
		{
			Length = 0;
		}

		public unsafe void InsertRangeWithBeginEnd(int begin, int end)
		{
			int num = end - begin;
			if (num >= 1)
			{
				int num2 = length - begin;
				Length += num;
				if (num2 >= 1)
				{
					int num3 = num2 * UnsafeUtility.SizeOf<T>();
					byte* num4 = Buffer;
					byte* destination = num4 + end * UnsafeUtility.SizeOf<T>();
					byte* source = num4 + begin * UnsafeUtility.SizeOf<T>();
					UnsafeUtility.MemMove(destination, source, num3);
				}
			}
		}

		public void Insert(int index, in T item)
		{
			InsertRangeWithBeginEnd(index, index + 1);
			this[index] = item;
		}

		public void RemoveAtSwapBack(int index)
		{
			RemoveRangeSwapBack(index, 1);
		}

		public unsafe void RemoveRangeSwapBack(int index, int count)
		{
			if (count > 0)
			{
				int num = math.max(Length - count, index + count);
				int num2 = UnsafeUtility.SizeOf<T>();
				void* destination = Buffer + index * num2;
				void* source = Buffer + num * num2;
				UnsafeUtility.MemCpy(destination, source, (Length - num) * num2);
				Length -= count;
			}
		}

		[Obsolete("RemoveRangeSwapBackWithBeginEnd(begin, end) is deprecated, use RemoveRangeSwapBack(index, count) instead. (RemovedAfter 2021-06-02)", false)]
		public void RemoveRangeSwapBackWithBeginEnd(int begin, int end)
		{
			RemoveRangeSwapBack(begin, end - begin);
		}

		public void RemoveAt(int index)
		{
			RemoveRange(index, 1);
		}

		public unsafe void RemoveRange(int index, int count)
		{
			if (count > 0)
			{
				int num = math.min(index + count, Length);
				int num2 = UnsafeUtility.SizeOf<T>();
				void* destination = Buffer + index * num2;
				void* source = Buffer + num * num2;
				UnsafeUtility.MemCpy(destination, source, (Length - num) * num2);
				Length -= count;
			}
		}

		[Obsolete("RemoveRangeWithBeginEnd(begin, end) is deprecated, use RemoveRange(index, count) instead. (RemovedAfter 2021-06-02)", false)]
		public void RemoveRangeWithBeginEnd(int begin, int end)
		{
			RemoveRange(begin, end - begin);
		}

		[NotBurstCompatible]
		public unsafe T[] ToArray()
		{
			T[] array = new T[Length];
			byte* source = Buffer;
			fixed (T* destination = array)
			{
				UnsafeUtility.MemCpy(destination, source, LengthInBytes);
			}
			return array;
		}

		public unsafe NativeArray<T> ToNativeArray(AllocatorManager.AllocatorHandle allocator)
		{
			NativeArray<T> nativeArray = CollectionHelper.CreateNativeArray<T>(Length, allocator, NativeArrayOptions.UninitializedMemory);
			UnsafeUtility.MemCpy(nativeArray.GetUnsafePtr(), Buffer, LengthInBytes);
			return nativeArray;
		}
	}
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[BurstCompatible]
	internal struct FixedList
	{
		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
		internal static int PaddingBytes<T>() where T : struct
		{
			return math.max(0, math.min(6, (1 << math.tzcnt(UnsafeUtility.SizeOf<T>())) - 2));
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		internal static int StorageBytes<BUFFER, T>() where BUFFER : struct where T : struct
		{
			return UnsafeUtility.SizeOf<BUFFER>() - PaddingBytes<T>();
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		internal static int Capacity<BUFFER, T>() where BUFFER : struct where T : struct
		{
			return StorageBytes<BUFFER, T>() / UnsafeUtility.SizeOf<T>();
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(int),
			typeof(int)
		})]
		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		[Conditional("UNITY_DOTS_DEBUG")]
		internal static void CheckResize<BUFFER, T>(int newLength) where BUFFER : struct where T : struct
		{
			int num = Capacity<BUFFER, T>();
			if (newLength < 0 || newLength > num)
			{
				throw new IndexOutOfRangeException($"NewLength {newLength} is out of range of '{num}' Capacity.");
			}
		}
	}
}
