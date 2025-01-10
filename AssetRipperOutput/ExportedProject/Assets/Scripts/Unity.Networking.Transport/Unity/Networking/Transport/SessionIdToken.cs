using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Networking.Transport
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct SessionIdToken : IEquatable<SessionIdToken>, IComparable<SessionIdToken>
	{
		public const int k_Length = 8;

		[FieldOffset(0)]
		public unsafe fixed byte Value[8];

		public static bool operator ==(SessionIdToken lhs, SessionIdToken rhs)
		{
			return lhs.Compare(rhs) == 0;
		}

		public static bool operator !=(SessionIdToken lhs, SessionIdToken rhs)
		{
			return lhs.Compare(rhs) != 0;
		}

		public bool Equals(SessionIdToken other)
		{
			return Compare(other) == 0;
		}

		public int CompareTo(SessionIdToken other)
		{
			return Compare(other);
		}

		public override bool Equals(object other)
		{
			if (other != null)
			{
				return this == (SessionIdToken)other;
			}
			return false;
		}

		public unsafe override int GetHashCode()
		{
			fixed (byte* ptr = Value)
			{
				int num = 0;
				for (int i = 0; i < 8; i++)
				{
					num = (num * 31) ^ ptr[i];
				}
				return num;
			}
		}

		private unsafe int Compare(SessionIdToken other)
		{
			fixed (byte* ptr = Value)
			{
				void* ptr2 = ptr;
				return UnsafeUtility.MemCmp(ptr2, other.Value, 8L);
			}
		}
	}
}
