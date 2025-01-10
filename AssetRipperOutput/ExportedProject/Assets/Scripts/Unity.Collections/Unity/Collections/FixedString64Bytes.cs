using System;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Unity.Collections
{
	[Serializable]
	[StructLayout(LayoutKind.Sequential, Size = 64)]
	[BurstCompatible]
	public struct FixedString64Bytes : INativeList<byte>, IIndexable<byte>, IUTF8Bytes, IComparable<string>, IEquatable<string>, IComparable<FixedString32Bytes>, IEquatable<FixedString32Bytes>, IComparable<FixedString64Bytes>, IEquatable<FixedString64Bytes>, IComparable<FixedString128Bytes>, IEquatable<FixedString128Bytes>, IComparable<FixedString512Bytes>, IEquatable<FixedString512Bytes>, IComparable<FixedString4096Bytes>, IEquatable<FixedString4096Bytes>
	{
		public struct Enumerator : IEnumerator
		{
			private FixedString64Bytes target;

			private int offset;

			private Unicode.Rune current;

			public Unicode.Rune Current => current;

			object IEnumerator.Current => Current;

			public Enumerator(FixedString64Bytes other)
			{
				target = other;
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

		internal const ushort utf8MaxLengthInBytes = 61;

		[SerializeField]
		internal ushort utf8LengthInBytes;

		[SerializeField]
		internal FixedBytes62 bytes;

		public static int UTF8MaxLengthInBytes => 61;

		[CreateProperty]
		[EditorBrowsable(EditorBrowsableState.Never)]
		[NotBurstCompatible]
		public string Value => ToString();

		public unsafe int Length
		{
			get
			{
				return utf8LengthInBytes;
			}
			set
			{
				utf8LengthInBytes = (ushort)value;
				GetUnsafePtr()[(int)utf8LengthInBytes] = 0;
			}
		}

		public int Capacity
		{
			get
			{
				return 61;
			}
			set
			{
			}
		}

		public bool IsEmpty => utf8LengthInBytes == 0;

		public unsafe byte this[int index]
		{
			get
			{
				return GetUnsafePtr()[index];
			}
			set
			{
				GetUnsafePtr()[index] = value;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe byte* GetUnsafePtr()
		{
			return (byte*)UnsafeUtility.AddressOf(ref bytes);
		}

		public unsafe bool TryResize(int newLength, NativeArrayOptions clearOptions = NativeArrayOptions.ClearMemory)
		{
			if (newLength < 0 || newLength > 61)
			{
				return false;
			}
			if (newLength == utf8LengthInBytes)
			{
				return true;
			}
			if (clearOptions == NativeArrayOptions.ClearMemory)
			{
				if (newLength > utf8LengthInBytes)
				{
					UnsafeUtility.MemClear(GetUnsafePtr() + (int)utf8LengthInBytes, newLength - utf8LengthInBytes);
				}
				else
				{
					UnsafeUtility.MemClear(GetUnsafePtr() + newLength, utf8LengthInBytes - newLength);
				}
			}
			utf8LengthInBytes = (ushort)newLength;
			GetUnsafePtr()[(int)utf8LengthInBytes] = 0;
			return true;
		}

		public unsafe ref byte ElementAt(int index)
		{
			return ref GetUnsafePtr()[index];
		}

		public void Clear()
		{
			Length = 0;
		}

		public void Add(in byte value)
		{
			this[Length++] = value;
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
		public unsafe bool Equals(string other)
		{
			int num = utf8LengthInBytes;
			int length = other.Length;
			byte* utf8Buffer = (byte*)UnsafeUtilityExtensions.AddressOf(in bytes);
			fixed (char* utf16Buffer = other)
			{
				return UTF8ArrayUnsafeUtility.StrCmp(utf8Buffer, num, utf16Buffer, length) == 0;
			}
		}

		public unsafe ref FixedList64Bytes<byte> AsFixedList()
		{
			return ref UnsafeUtility.AsRef<FixedList64Bytes<byte>>(UnsafeUtility.AddressOf(ref this));
		}

		[NotBurstCompatible]
		public FixedString64Bytes(string source)
		{
			this = default(FixedString64Bytes);
			Initialize(source);
		}

		[NotBurstCompatible]
		internal unsafe int Initialize(string source)
		{
			bytes = default(FixedBytes62);
			utf8LengthInBytes = 0;
			fixed (char* src = source)
			{
				CopyError copyError = UTF8ArrayUnsafeUtility.Copy(GetUnsafePtr(), out utf8LengthInBytes, (ushort)61, src, source.Length);
				if (copyError != 0)
				{
					return (int)copyError;
				}
				Length = utf8LengthInBytes;
			}
			return 0;
		}

		public FixedString64Bytes(Unicode.Rune rune, int count = 1)
		{
			this = default(FixedString64Bytes);
			Initialize(rune, count);
		}

		internal int Initialize(Unicode.Rune rune, int count = 1)
		{
			this = default(FixedString64Bytes);
			return (int)FixedStringMethods.Append(ref this, rune, count);
		}

		public int CompareTo(FixedString32Bytes other)
		{
			return FixedStringMethods.CompareTo(ref this, in other);
		}

		public FixedString64Bytes(in FixedString32Bytes other)
		{
			this = default(FixedString64Bytes);
			Initialize(in other);
		}

		internal unsafe int Initialize(in FixedString32Bytes other)
		{
			bytes = default(FixedBytes62);
			utf8LengthInBytes = 0;
			int destLength = 0;
			byte* unsafePtr = GetUnsafePtr();
			byte* src = (byte*)UnsafeUtilityExtensions.AddressOf(in other.bytes);
			ushort srcLength = other.utf8LengthInBytes;
			FormatError formatError = UTF8ArrayUnsafeUtility.AppendUTF8Bytes(unsafePtr, ref destLength, 61, src, srcLength);
			if (formatError != 0)
			{
				return (int)formatError;
			}
			Length = destLength;
			return 0;
		}

		public unsafe static bool operator ==(in FixedString64Bytes a, in FixedString32Bytes b)
		{
			int aLength = a.utf8LengthInBytes;
			int bLength = b.utf8LengthInBytes;
			byte* aBytes = (byte*)UnsafeUtilityExtensions.AddressOf(in a.bytes);
			byte* bBytes = (byte*)UnsafeUtilityExtensions.AddressOf(in b.bytes);
			return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(aBytes, aLength, bBytes, bLength);
		}

		public static bool operator !=(in FixedString64Bytes a, in FixedString32Bytes b)
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

		public FixedString64Bytes(in FixedString64Bytes other)
		{
			this = default(FixedString64Bytes);
			Initialize(in other);
		}

		internal unsafe int Initialize(in FixedString64Bytes other)
		{
			bytes = default(FixedBytes62);
			utf8LengthInBytes = 0;
			int destLength = 0;
			byte* unsafePtr = GetUnsafePtr();
			byte* src = (byte*)UnsafeUtilityExtensions.AddressOf(in other.bytes);
			ushort srcLength = other.utf8LengthInBytes;
			FormatError formatError = UTF8ArrayUnsafeUtility.AppendUTF8Bytes(unsafePtr, ref destLength, 61, src, srcLength);
			if (formatError != 0)
			{
				return (int)formatError;
			}
			Length = destLength;
			return 0;
		}

		public unsafe static bool operator ==(in FixedString64Bytes a, in FixedString64Bytes b)
		{
			int aLength = a.utf8LengthInBytes;
			int bLength = b.utf8LengthInBytes;
			byte* aBytes = (byte*)UnsafeUtilityExtensions.AddressOf(in a.bytes);
			byte* bBytes = (byte*)UnsafeUtilityExtensions.AddressOf(in b.bytes);
			return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(aBytes, aLength, bBytes, bLength);
		}

		public static bool operator !=(in FixedString64Bytes a, in FixedString64Bytes b)
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

		public FixedString64Bytes(in FixedString128Bytes other)
		{
			this = default(FixedString64Bytes);
			Initialize(in other);
		}

		internal unsafe int Initialize(in FixedString128Bytes other)
		{
			bytes = default(FixedBytes62);
			utf8LengthInBytes = 0;
			int destLength = 0;
			byte* unsafePtr = GetUnsafePtr();
			byte* src = (byte*)UnsafeUtilityExtensions.AddressOf(in other.bytes);
			ushort srcLength = other.utf8LengthInBytes;
			FormatError formatError = UTF8ArrayUnsafeUtility.AppendUTF8Bytes(unsafePtr, ref destLength, 61, src, srcLength);
			if (formatError != 0)
			{
				return (int)formatError;
			}
			Length = destLength;
			return 0;
		}

		public unsafe static bool operator ==(in FixedString64Bytes a, in FixedString128Bytes b)
		{
			int aLength = a.utf8LengthInBytes;
			int bLength = b.utf8LengthInBytes;
			byte* aBytes = (byte*)UnsafeUtilityExtensions.AddressOf(in a.bytes);
			byte* bBytes = (byte*)UnsafeUtilityExtensions.AddressOf(in b.bytes);
			return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(aBytes, aLength, bBytes, bLength);
		}

		public static bool operator !=(in FixedString64Bytes a, in FixedString128Bytes b)
		{
			return !(a == b);
		}

		public bool Equals(FixedString128Bytes other)
		{
			return this == other;
		}

		public static implicit operator FixedString128Bytes(in FixedString64Bytes fs)
		{
			return new FixedString128Bytes(in fs);
		}

		public int CompareTo(FixedString512Bytes other)
		{
			return FixedStringMethods.CompareTo(ref this, in other);
		}

		public FixedString64Bytes(in FixedString512Bytes other)
		{
			this = default(FixedString64Bytes);
			Initialize(in other);
		}

		internal unsafe int Initialize(in FixedString512Bytes other)
		{
			bytes = default(FixedBytes62);
			utf8LengthInBytes = 0;
			int destLength = 0;
			byte* unsafePtr = GetUnsafePtr();
			byte* src = (byte*)UnsafeUtilityExtensions.AddressOf(in other.bytes);
			ushort srcLength = other.utf8LengthInBytes;
			FormatError formatError = UTF8ArrayUnsafeUtility.AppendUTF8Bytes(unsafePtr, ref destLength, 61, src, srcLength);
			if (formatError != 0)
			{
				return (int)formatError;
			}
			Length = destLength;
			return 0;
		}

		public unsafe static bool operator ==(in FixedString64Bytes a, in FixedString512Bytes b)
		{
			int aLength = a.utf8LengthInBytes;
			int bLength = b.utf8LengthInBytes;
			byte* aBytes = (byte*)UnsafeUtilityExtensions.AddressOf(in a.bytes);
			byte* bBytes = (byte*)UnsafeUtilityExtensions.AddressOf(in b.bytes);
			return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(aBytes, aLength, bBytes, bLength);
		}

		public static bool operator !=(in FixedString64Bytes a, in FixedString512Bytes b)
		{
			return !(a == b);
		}

		public bool Equals(FixedString512Bytes other)
		{
			return this == other;
		}

		public static implicit operator FixedString512Bytes(in FixedString64Bytes fs)
		{
			return new FixedString512Bytes(in fs);
		}

		public int CompareTo(FixedString4096Bytes other)
		{
			return FixedStringMethods.CompareTo(ref this, in other);
		}

		public FixedString64Bytes(in FixedString4096Bytes other)
		{
			this = default(FixedString64Bytes);
			Initialize(in other);
		}

		internal unsafe int Initialize(in FixedString4096Bytes other)
		{
			bytes = default(FixedBytes62);
			utf8LengthInBytes = 0;
			int destLength = 0;
			byte* unsafePtr = GetUnsafePtr();
			byte* src = (byte*)UnsafeUtilityExtensions.AddressOf(in other.bytes);
			ushort srcLength = other.utf8LengthInBytes;
			FormatError formatError = UTF8ArrayUnsafeUtility.AppendUTF8Bytes(unsafePtr, ref destLength, 61, src, srcLength);
			if (formatError != 0)
			{
				return (int)formatError;
			}
			Length = destLength;
			return 0;
		}

		public unsafe static bool operator ==(in FixedString64Bytes a, in FixedString4096Bytes b)
		{
			int aLength = a.utf8LengthInBytes;
			int bLength = b.utf8LengthInBytes;
			byte* aBytes = (byte*)UnsafeUtilityExtensions.AddressOf(in a.bytes);
			byte* bBytes = (byte*)UnsafeUtilityExtensions.AddressOf(in b.bytes);
			return UTF8ArrayUnsafeUtility.EqualsUTF8Bytes(aBytes, aLength, bBytes, bLength);
		}

		public static bool operator !=(in FixedString64Bytes a, in FixedString4096Bytes b)
		{
			return !(a == b);
		}

		public bool Equals(FixedString4096Bytes other)
		{
			return this == other;
		}

		public static implicit operator FixedString4096Bytes(in FixedString64Bytes fs)
		{
			return new FixedString4096Bytes(in fs);
		}

		[NotBurstCompatible]
		public static implicit operator FixedString64Bytes(string b)
		{
			return new FixedString64Bytes(b);
		}

		[NotBurstCompatible]
		public override string ToString()
		{
			return FixedStringMethods.ConvertToString(ref this);
		}

		public override int GetHashCode()
		{
			return FixedStringMethods.ComputeHashCode(ref this);
		}

		[NotBurstCompatible]
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (obj is string other)
			{
				return Equals(other);
			}
			if (obj is FixedString32Bytes other2)
			{
				return Equals(other2);
			}
			if (obj is FixedString64Bytes other3)
			{
				return Equals(other3);
			}
			if (obj is FixedString128Bytes other4)
			{
				return Equals(other4);
			}
			if (obj is FixedString512Bytes other5)
			{
				return Equals(other5);
			}
			if (obj is FixedString4096Bytes other6)
			{
				return Equals(other6);
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
			if (index >= utf8LengthInBytes)
			{
				throw new IndexOutOfRangeException($"Index {index} is out of range in FixedString64Bytes of '{utf8LengthInBytes}' Length.");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckLengthInRange(int length)
		{
			if (length < 0)
			{
				throw new ArgumentOutOfRangeException($"Length {length} must be positive.");
			}
			if (length > 61)
			{
				throw new ArgumentOutOfRangeException($"Length {length} is out of range in FixedString64Bytes of '{(ushort)61}' Capacity.");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private void CheckCapacityInRange(int capacity)
		{
			if (capacity > 61)
			{
				throw new ArgumentOutOfRangeException($"Capacity {capacity} must be lower than {(ushort)61}.");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckCopyError(CopyError error, string source)
		{
			if (error != 0)
			{
				throw new ArgumentException($"FixedString64Bytes: {error} while copying \"{source}\"");
			}
		}

		[Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
		private static void CheckFormatError(FormatError error)
		{
			if (error != 0)
			{
				throw new ArgumentException("Source is too long to fit into fixed string of this size");
			}
		}
	}
}
