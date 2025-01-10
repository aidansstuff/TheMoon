using System;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Unity.Networking.Transport.Relay
{
	public struct RelayAllocationId : IEquatable<RelayAllocationId>, IComparable<RelayAllocationId>
	{
		public const int k_Length = 16;

		public unsafe fixed byte Value[16];

		public unsafe static RelayAllocationId FromBytePointer(byte* dataPtr, int length)
		{
			if (length != 16)
			{
				Debug.LogError($"Provided byte array length is invalid, must be {16} but got {length}.");
				return default(RelayAllocationId);
			}
			RelayAllocationId result = default(RelayAllocationId);
			UnsafeUtility.MemCpy(result.Value, dataPtr, 16L);
			return result;
		}

		public unsafe static RelayAllocationId FromByteArray(byte[] data)
		{
			fixed (byte* dataPtr = data)
			{
				return FromBytePointer(dataPtr, data.Length);
			}
		}

		public static bool operator ==(RelayAllocationId lhs, RelayAllocationId rhs)
		{
			return lhs.Compare(rhs) == 0;
		}

		public static bool operator !=(RelayAllocationId lhs, RelayAllocationId rhs)
		{
			return lhs.Compare(rhs) != 0;
		}

		public bool Equals(RelayAllocationId other)
		{
			return Compare(other) == 0;
		}

		public int CompareTo(RelayAllocationId other)
		{
			return Compare(other);
		}

		public override bool Equals(object other)
		{
			if (other != null)
			{
				return this == (RelayAllocationId)other;
			}
			return false;
		}

		public unsafe override int GetHashCode()
		{
			fixed (byte* ptr = Value)
			{
				int num = 0;
				for (int i = 0; i < 16; i++)
				{
					num = (num * 31) ^ ptr[i];
				}
				return num;
			}
		}

		private unsafe int Compare(RelayAllocationId other)
		{
			fixed (byte* ptr = Value)
			{
				void* ptr2 = ptr;
				return UnsafeUtility.MemCmp(ptr2, other.Value, 16L);
			}
		}
	}
}
