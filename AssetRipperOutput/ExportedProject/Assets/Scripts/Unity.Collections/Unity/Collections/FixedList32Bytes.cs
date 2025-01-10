using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;

namespace Unity.Collections
{
	[Serializable]
	[DebuggerTypeProxy(typeof(FixedList32BytesDebugView<>))]
	[BurstCompatible(GenericTypeArguments = new Type[] { typeof(int) })]
	public struct FixedList32Bytes<T> : INativeList<T>, IIndexable<T>, IEnumerable<T>, IEnumerable, IEquatable<FixedList32Bytes<T>>, IComparable<FixedList32Bytes<T>>, IEquatable<FixedList64Bytes<T>>, IComparable<FixedList64Bytes<T>>, IEquatable<FixedList128Bytes<T>>, IComparable<FixedList128Bytes<T>>, IEquatable<FixedList512Bytes<T>>, IComparable<FixedList512Bytes<T>>, IEquatable<FixedList4096Bytes<T>>, IComparable<FixedList4096Bytes<T>> where T : unmanaged
	{
		public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
		{
			private FixedList32Bytes<T> m_List;

			private int m_Index;

			public T Current => m_List[m_Index];

			object IEnumerator.Current => Current;

			public Enumerator(ref FixedList32Bytes<T> list)
			{
				m_List = list;
				m_Index = -1;
			}

			public void Dispose()
			{
			}

			public bool MoveNext()
			{
				m_Index++;
				return m_Index < m_List.Length;
			}

			public void Reset()
			{
				m_Index = -1;
			}
		}

		[SerializeField]
		internal ushort length;

		[SerializeField]
		internal FixedBytes30 buffer;

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
				fixed (byte* ptr = &buffer.offset0000.byte0000)
				{
					return ptr + FixedList.PaddingBytes<T>();
				}
			}
		}

		public int Capacity
		{
			get
			{
				return FixedList.Capacity<FixedBytes30, T>();
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

		public unsafe static bool operator ==(in FixedList32Bytes<T> a, in FixedList32Bytes<T> b)
		{
			if (a.length != b.length)
			{
				return false;
			}
			return UnsafeUtility.MemCmp(a.Buffer, b.Buffer, a.LengthInBytes) == 0;
		}

		public static bool operator !=(in FixedList32Bytes<T> a, in FixedList32Bytes<T> b)
		{
			return !(a == b);
		}

		public unsafe int CompareTo(FixedList32Bytes<T> other)
		{
			fixed (byte* ptr = &buffer.offset0000.byte0000)
			{
				byte* num = &other.buffer.offset0000.byte0000;
				byte* ptr2 = ptr + FixedList.PaddingBytes<T>();
				byte* ptr3 = num + FixedList.PaddingBytes<T>();
				int num2 = math.min(Length, other.Length);
				for (int i = 0; i < num2; i++)
				{
					int num3 = UnsafeUtility.MemCmp(ptr2 + sizeof(T) * i, ptr3 + sizeof(T) * i, sizeof(T));
					if (num3 != 0)
					{
						return num3;
					}
				}
				return Length.CompareTo(other.Length);
			}
		}

		public bool Equals(FixedList32Bytes<T> other)
		{
			return CompareTo(other) == 0;
		}

		public unsafe static bool operator ==(in FixedList32Bytes<T> a, in FixedList64Bytes<T> b)
		{
			if (a.length != b.length)
			{
				return false;
			}
			return UnsafeUtility.MemCmp(a.Buffer, b.Buffer, a.LengthInBytes) == 0;
		}

		public static bool operator !=(in FixedList32Bytes<T> a, in FixedList64Bytes<T> b)
		{
			return !(a == b);
		}

		public unsafe int CompareTo(FixedList64Bytes<T> other)
		{
			fixed (byte* ptr = &buffer.offset0000.byte0000)
			{
				byte* num = &other.buffer.offset0000.byte0000;
				byte* ptr2 = ptr + FixedList.PaddingBytes<T>();
				byte* ptr3 = num + FixedList.PaddingBytes<T>();
				int num2 = math.min(Length, other.Length);
				for (int i = 0; i < num2; i++)
				{
					int num3 = UnsafeUtility.MemCmp(ptr2 + sizeof(T) * i, ptr3 + sizeof(T) * i, sizeof(T));
					if (num3 != 0)
					{
						return num3;
					}
				}
				return Length.CompareTo(other.Length);
			}
		}

		public bool Equals(FixedList64Bytes<T> other)
		{
			return CompareTo(other) == 0;
		}

		public FixedList32Bytes(in FixedList64Bytes<T> other)
		{
			this = default(FixedList32Bytes<T>);
			Initialize(in other);
		}

		internal unsafe int Initialize(in FixedList64Bytes<T> other)
		{
			if (other.Length > Capacity)
			{
				return 1;
			}
			length = other.length;
			buffer = default(FixedBytes30);
			UnsafeUtility.MemCpy(Buffer, other.Buffer, LengthInBytes);
			return 0;
		}

		public static implicit operator FixedList32Bytes<T>(in FixedList64Bytes<T> other)
		{
			return new FixedList32Bytes<T>(in other);
		}

		public unsafe static bool operator ==(in FixedList32Bytes<T> a, in FixedList128Bytes<T> b)
		{
			if (a.length != b.length)
			{
				return false;
			}
			return UnsafeUtility.MemCmp(a.Buffer, b.Buffer, a.LengthInBytes) == 0;
		}

		public static bool operator !=(in FixedList32Bytes<T> a, in FixedList128Bytes<T> b)
		{
			return !(a == b);
		}

		public unsafe int CompareTo(FixedList128Bytes<T> other)
		{
			fixed (byte* ptr = &buffer.offset0000.byte0000)
			{
				byte* num = &other.buffer.offset0000.byte0000;
				byte* ptr2 = ptr + FixedList.PaddingBytes<T>();
				byte* ptr3 = num + FixedList.PaddingBytes<T>();
				int num2 = math.min(Length, other.Length);
				for (int i = 0; i < num2; i++)
				{
					int num3 = UnsafeUtility.MemCmp(ptr2 + sizeof(T) * i, ptr3 + sizeof(T) * i, sizeof(T));
					if (num3 != 0)
					{
						return num3;
					}
				}
				return Length.CompareTo(other.Length);
			}
		}

		public bool Equals(FixedList128Bytes<T> other)
		{
			return CompareTo(other) == 0;
		}

		public FixedList32Bytes(in FixedList128Bytes<T> other)
		{
			this = default(FixedList32Bytes<T>);
			Initialize(in other);
		}

		internal unsafe int Initialize(in FixedList128Bytes<T> other)
		{
			if (other.Length > Capacity)
			{
				return 1;
			}
			length = other.length;
			buffer = default(FixedBytes30);
			UnsafeUtility.MemCpy(Buffer, other.Buffer, LengthInBytes);
			return 0;
		}

		public static implicit operator FixedList32Bytes<T>(in FixedList128Bytes<T> other)
		{
			return new FixedList32Bytes<T>(in other);
		}

		public unsafe static bool operator ==(in FixedList32Bytes<T> a, in FixedList512Bytes<T> b)
		{
			if (a.length != b.length)
			{
				return false;
			}
			return UnsafeUtility.MemCmp(a.Buffer, b.Buffer, a.LengthInBytes) == 0;
		}

		public static bool operator !=(in FixedList32Bytes<T> a, in FixedList512Bytes<T> b)
		{
			return !(a == b);
		}

		public unsafe int CompareTo(FixedList512Bytes<T> other)
		{
			fixed (byte* ptr = &buffer.offset0000.byte0000)
			{
				byte* num = &other.buffer.offset0000.byte0000;
				byte* ptr2 = ptr + FixedList.PaddingBytes<T>();
				byte* ptr3 = num + FixedList.PaddingBytes<T>();
				int num2 = math.min(Length, other.Length);
				for (int i = 0; i < num2; i++)
				{
					int num3 = UnsafeUtility.MemCmp(ptr2 + sizeof(T) * i, ptr3 + sizeof(T) * i, sizeof(T));
					if (num3 != 0)
					{
						return num3;
					}
				}
				return Length.CompareTo(other.Length);
			}
		}

		public bool Equals(FixedList512Bytes<T> other)
		{
			return CompareTo(other) == 0;
		}

		public FixedList32Bytes(in FixedList512Bytes<T> other)
		{
			this = default(FixedList32Bytes<T>);
			Initialize(in other);
		}

		internal unsafe int Initialize(in FixedList512Bytes<T> other)
		{
			if (other.Length > Capacity)
			{
				return 1;
			}
			length = other.length;
			buffer = default(FixedBytes30);
			UnsafeUtility.MemCpy(Buffer, other.Buffer, LengthInBytes);
			return 0;
		}

		public static implicit operator FixedList32Bytes<T>(in FixedList512Bytes<T> other)
		{
			return new FixedList32Bytes<T>(in other);
		}

		public unsafe static bool operator ==(in FixedList32Bytes<T> a, in FixedList4096Bytes<T> b)
		{
			if (a.length != b.length)
			{
				return false;
			}
			return UnsafeUtility.MemCmp(a.Buffer, b.Buffer, a.LengthInBytes) == 0;
		}

		public static bool operator !=(in FixedList32Bytes<T> a, in FixedList4096Bytes<T> b)
		{
			return !(a == b);
		}

		public unsafe int CompareTo(FixedList4096Bytes<T> other)
		{
			fixed (byte* ptr = &buffer.offset0000.byte0000)
			{
				byte* num = &other.buffer.offset0000.byte0000;
				byte* ptr2 = ptr + FixedList.PaddingBytes<T>();
				byte* ptr3 = num + FixedList.PaddingBytes<T>();
				int num2 = math.min(Length, other.Length);
				for (int i = 0; i < num2; i++)
				{
					int num3 = UnsafeUtility.MemCmp(ptr2 + sizeof(T) * i, ptr3 + sizeof(T) * i, sizeof(T));
					if (num3 != 0)
					{
						return num3;
					}
				}
				return Length.CompareTo(other.Length);
			}
		}

		public bool Equals(FixedList4096Bytes<T> other)
		{
			return CompareTo(other) == 0;
		}

		public FixedList32Bytes(in FixedList4096Bytes<T> other)
		{
			this = default(FixedList32Bytes<T>);
			Initialize(in other);
		}

		internal unsafe int Initialize(in FixedList4096Bytes<T> other)
		{
			if (other.Length > Capacity)
			{
				return 1;
			}
			length = other.length;
			buffer = default(FixedBytes30);
			UnsafeUtility.MemCpy(Buffer, other.Buffer, LengthInBytes);
			return 0;
		}

		public static implicit operator FixedList32Bytes<T>(in FixedList4096Bytes<T> other)
		{
			return new FixedList32Bytes<T>(in other);
		}

		[NotBurstCompatible]
		public override bool Equals(object obj)
		{
			if (obj is FixedList32Bytes<T> other)
			{
				return Equals(other);
			}
			if (obj is FixedList64Bytes<T> other2)
			{
				return Equals(other2);
			}
			if (obj is FixedList128Bytes<T> other3)
			{
				return Equals(other3);
			}
			if (obj is FixedList512Bytes<T> other4)
			{
				return Equals(other4);
			}
			if (obj is FixedList4096Bytes<T> other5)
			{
				return Equals(other5);
			}
			return false;
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(ref this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}
}
