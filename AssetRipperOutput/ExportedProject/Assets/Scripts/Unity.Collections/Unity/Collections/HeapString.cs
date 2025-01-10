using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[BurstCompatible]
	[Obsolete("HeapString has been removed and replaced with NativeText (RemovedAfter 2021-07-21) (UnityUpgradable) -> NativeText", false)]
	public struct HeapString : INativeList<byte>, IIndexable<byte>, IDisposable, IUTF8Bytes, IComparable<string>, IEquatable<string>, IComparable<HeapString>, IEquatable<HeapString>, IComparable<FixedString32Bytes>, IEquatable<FixedString32Bytes>, IComparable<FixedString64Bytes>, IEquatable<FixedString64Bytes>, IComparable<FixedString128Bytes>, IEquatable<FixedString128Bytes>, IComparable<FixedString512Bytes>, IEquatable<FixedString512Bytes>, IComparable<FixedString4096Bytes>, IEquatable<FixedString4096Bytes>
	{
		public struct Enumerator : IEnumerator<Unicode.Rune>, IEnumerator, IDisposable
		{
			private HeapString target;

			private int offset;

			private Unicode.Rune current;

			object IEnumerator.Current => Current;

			public Unicode.Rune Current => current;

			public Enumerator(HeapString source)
			{
				target = source;
				offset = 0;
				current = default(Unicode.Rune);
			}

			public void Dispose()
			{
			}

			public unsafe bool MoveNext()
			{
				if (offset >= target.Length)
				{
					return false;
				}
				Unicode.Utf8ToUcs(out current, target.GetUnsafePtr(), ref offset, target.Length);
				return true;
			}

			public void Reset()
			{
				offset = 0;
				current = default(Unicode.Rune);
			}
		}

		private NativeList<byte> m_Data;

		public int Length
		{
			get
			{
				return m_Data.Length - 1;
			}
			set
			{
				m_Data.Resize(value + 1, NativeArrayOptions.UninitializedMemory);
				m_Data[value] = 0;
			}
		}

		public int Capacity
		{
			get
			{
				return m_Data.Capacity - 1;
			}
			set
			{
				m_Data.Capacity = value + 1;
			}
		}

		public bool IsEmpty => m_Data.Length == 1;

		public bool IsCreated => m_Data.IsCreated;

		public byte this[int index]
		{
			get
			{
				return m_Data[index];
			}
			set
			{
				m_Data[index] = value;
			}
		}

		[CreateProperty]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[NotBurstCompatible]
		public string Value => ToString();

		public bool TryResize(int newLength, NativeArrayOptions clearOptions = NativeArrayOptions.ClearMemory)
		{
			Length = newLength;
			return true;
		}

		public unsafe byte* GetUnsafePtr()
		{
			return (byte*)m_Data.GetUnsafePtr();
		}

		public ref byte ElementAt(int index)
		{
			return ref m_Data.ElementAt(index);
		}

		public void Clear()
		{
			Length = 0;
		}

		public void Add(in byte value)
		{
			this[Length++] = value;
		}

		public int CompareTo(HeapString other)
		{
			return FixedStringMethods.CompareTo(ref this, in other);
		}

		public bool Equals(HeapString other)
		{
			return FixedStringMethods.Equals(ref this, in other);
		}

		public void Dispose()
		{
			m_Data.Dispose();
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		[NotBurstCompatible]
		public int CompareTo(string other)
		{
			return ToString().CompareTo(other);
		}

		[NotBurstCompatible]
		public bool Equals(string other)
		{
			return ToString().Equals(other);
		}

		[NotBurstCompatible]
		public unsafe HeapString(string source, Allocator allocator)
		{
			m_Data = new NativeList<byte>(source.Length * 2 + 1, allocator);
			Length = source.Length * 2;
			fixed (char* src = source)
			{
				if (UTF8ArrayUnsafeUtility.Copy(GetUnsafePtr(), out var destLength, Capacity, src, source.Length) != 0)
				{
					m_Data.Dispose();
					m_Data = default(NativeList<byte>);
				}
				Length = destLength;
			}
		}

		public HeapString(int capacity, Allocator allocator)
		{
			m_Data = new NativeList<byte>(capacity + 1, allocator);
			Length = 0;
		}

		public HeapString(Allocator allocator)
		{
			m_Data = new NativeList<byte>(129, allocator);
			Length = 0;
		}

		public int CompareTo(FixedString32Bytes other)
		{
			return FixedStringMethods.CompareTo(ref this, in other);
		}

		public unsafe HeapString(in FixedString32Bytes source, Allocator allocator)
		{
			m_Data = new NativeList<byte>(source.utf8LengthInBytes + 1, allocator);
			Length = source.utf8LengthInBytes;
			byte* source2 = (byte*)UnsafeUtilityExtensions.AddressOf(in source.bytes);
			byte* unsafePtr = (byte*)m_Data.GetUnsafePtr();
			UnsafeUtility.MemCpy(unsafePtr, source2, source.utf8LengthInBytes);
		}

		public unsafe static bool operator ==(in HeapString a, in FixedString32Bytes b)
		{
			HeapString heapString = UnsafeUtilityExtensions.AsRef(in a);
			int length = heapString.Length;
			int utf8LengthInBytes = b.utf8LengthInBytes;
			byte* unsafePtr = heapString.GetUnsafePtr();
			byte* bBytes = (byte*)UnsafeUtilityExtensions.AddressOf(in b.bytes);
			return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(unsafePtr, length, bBytes, utf8LengthInBytes);
		}

		public static bool operator !=(in HeapString a, in FixedString32Bytes b)
		{
			return !(a == b);
		}

		public bool Equals(FixedString32Bytes other)
		{
			return this == other;
		}

		public int CompareTo(FixedString64Bytes other)
		{
			return FixedStringMethods.CompareTo(ref this, in other);
		}

		public unsafe HeapString(in FixedString64Bytes source, Allocator allocator)
		{
			m_Data = new NativeList<byte>(source.utf8LengthInBytes + 1, allocator);
			Length = source.utf8LengthInBytes;
			byte* source2 = (byte*)UnsafeUtilityExtensions.AddressOf(in source.bytes);
			byte* unsafePtr = (byte*)m_Data.GetUnsafePtr();
			UnsafeUtility.MemCpy(unsafePtr, source2, source.utf8LengthInBytes);
		}

		public unsafe static bool operator ==(in HeapString a, in FixedString64Bytes b)
		{
			HeapString heapString = UnsafeUtilityExtensions.AsRef(in a);
			int length = heapString.Length;
			int utf8LengthInBytes = b.utf8LengthInBytes;
			byte* unsafePtr = heapString.GetUnsafePtr();
			byte* bBytes = (byte*)UnsafeUtilityExtensions.AddressOf(in b.bytes);
			return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(unsafePtr, length, bBytes, utf8LengthInBytes);
		}

		public static bool operator !=(in HeapString a, in FixedString64Bytes b)
		{
			return !(a == b);
		}

		public bool Equals(FixedString64Bytes other)
		{
			return this == other;
		}

		public int CompareTo(FixedString128Bytes other)
		{
			return FixedStringMethods.CompareTo(ref this, in other);
		}

		public unsafe HeapString(in FixedString128Bytes source, Allocator allocator)
		{
			m_Data = new NativeList<byte>(source.utf8LengthInBytes + 1, allocator);
			Length = source.utf8LengthInBytes;
			byte* source2 = (byte*)UnsafeUtilityExtensions.AddressOf(in source.bytes);
			byte* unsafePtr = (byte*)m_Data.GetUnsafePtr();
			UnsafeUtility.MemCpy(unsafePtr, source2, source.utf8LengthInBytes);
		}

		public unsafe static bool operator ==(in HeapString a, in FixedString128Bytes b)
		{
			HeapString heapString = UnsafeUtilityExtensions.AsRef(in a);
			int length = heapString.Length;
			int utf8LengthInBytes = b.utf8LengthInBytes;
			byte* unsafePtr = heapString.GetUnsafePtr();
			byte* bBytes = (byte*)UnsafeUtilityExtensions.AddressOf(in b.bytes);
			return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(unsafePtr, length, bBytes, utf8LengthInBytes);
		}

		public static bool operator !=(in HeapString a, in FixedString128Bytes b)
		{
			return !(a == b);
		}

		public bool Equals(FixedString128Bytes other)
		{
			return this == other;
		}

		public int CompareTo(FixedString512Bytes other)
		{
			return FixedStringMethods.CompareTo(ref this, in other);
		}

		public unsafe HeapString(in FixedString512Bytes source, Allocator allocator)
		{
			m_Data = new NativeList<byte>(source.utf8LengthInBytes + 1, allocator);
			Length = source.utf8LengthInBytes;
			byte* source2 = (byte*)UnsafeUtilityExtensions.AddressOf(in source.bytes);
			byte* unsafePtr = (byte*)m_Data.GetUnsafePtr();
			UnsafeUtility.MemCpy(unsafePtr, source2, source.utf8LengthInBytes);
		}

		public unsafe static bool operator ==(in HeapString a, in FixedString512Bytes b)
		{
			HeapString heapString = UnsafeUtilityExtensions.AsRef(in a);
			int length = heapString.Length;
			int utf8LengthInBytes = b.utf8LengthInBytes;
			byte* unsafePtr = heapString.GetUnsafePtr();
			byte* bBytes = (byte*)UnsafeUtilityExtensions.AddressOf(in b.bytes);
			return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(unsafePtr, length, bBytes, utf8LengthInBytes);
		}

		public static bool operator !=(in HeapString a, in FixedString512Bytes b)
		{
			return !(a == b);
		}

		public bool Equals(FixedString512Bytes other)
		{
			return this == other;
		}

		public int CompareTo(FixedString4096Bytes other)
		{
			return FixedStringMethods.CompareTo(ref this, in other);
		}

		public unsafe HeapString(in FixedString4096Bytes source, Allocator allocator)
		{
			m_Data = new NativeList<byte>(source.utf8LengthInBytes + 1, allocator);
			Length = source.utf8LengthInBytes;
			byte* source2 = (byte*)UnsafeUtilityExtensions.AddressOf(in source.bytes);
			byte* unsafePtr = (byte*)m_Data.GetUnsafePtr();
			UnsafeUtility.MemCpy(unsafePtr, source2, source.utf8LengthInBytes);
		}

		public unsafe static bool operator ==(in HeapString a, in FixedString4096Bytes b)
		{
			HeapString heapString = UnsafeUtilityExtensions.AsRef(in a);
			int length = heapString.Length;
			int utf8LengthInBytes = b.utf8LengthInBytes;
			byte* unsafePtr = heapString.GetUnsafePtr();
			byte* bBytes = (byte*)UnsafeUtilityExtensions.AddressOf(in b.bytes);
			return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(unsafePtr, length, bBytes, utf8LengthInBytes);
		}

		public static bool operator !=(in HeapString a, in FixedString4096Bytes b)
		{
			return !(a == b);
		}

		public bool Equals(FixedString4096Bytes other)
		{
			return this == other;
		}

		[NotBurstCompatible]
		public override string ToString()
		{
			if (!m_Data.IsCreated)
			{
				return "";
			}
			return FixedStringMethods.ConvertToString(ref this);
		}

		public override int GetHashCode()
		{
			return FixedStringMethods.ComputeHashCode(ref this);
		}

		[NotBurstCompatible]
		public override bool Equals(object other)
		{
			if (other == null)
			{
				return false;
			}
			if (other is string other2)
			{
				return Equals(other2);
			}
			if (other is HeapString other3)
			{
				return Equals(other3);
			}
			if (other is FixedString32Bytes other4)
			{
				return Equals(other4);
			}
			if (other is FixedString64Bytes other5)
			{
				return Equals(other5);
			}
			if (other is FixedString128Bytes other6)
			{
				return Equals(other6);
			}
			if (other is FixedString512Bytes other7)
			{
				return Equals(other7);
			}
			if (other is FixedString4096Bytes other8)
			{
				return Equals(other8);
			}
			return false;
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckIndexInRange(int index)
		{
			if (index < 0)
			{
				throw new IndexOutOfRangeException($"Index {index} must be positive.");
			}
			if (index >= Length)
			{
				throw new IndexOutOfRangeException($"Index {index} is out of range in HeapString of {Length} length.");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void ThrowCopyError(CopyError error, string source)
		{
			throw new ArgumentException($"HeapString: {error} while copying \"{source}\"");
		}
	}
}
