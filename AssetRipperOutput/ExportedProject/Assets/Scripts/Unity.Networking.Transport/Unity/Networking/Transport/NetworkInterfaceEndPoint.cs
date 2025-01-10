using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using Unity.Networking.Transport.Utilities;

namespace Unity.Networking.Transport
{
	public struct NetworkInterfaceEndPoint : IEquatable<NetworkInterfaceEndPoint>
	{
		public const int k_MaxLength = 56;

		public int dataLength;

		public unsafe fixed byte data[56];

		public bool IsValid => dataLength != 0;

		public static bool operator ==(NetworkInterfaceEndPoint lhs, NetworkInterfaceEndPoint rhs)
		{
			return lhs.Equals(rhs);
		}

		public static bool operator !=(NetworkInterfaceEndPoint lhs, NetworkInterfaceEndPoint rhs)
		{
			return !lhs.Equals(rhs);
		}

		public override bool Equals(object other)
		{
			return Equals((NetworkInterfaceEndPoint)other);
		}

		public unsafe override int GetHashCode()
		{
			fixed (byte* ptr = data)
			{
				int num = 0;
				for (int i = 0; i < dataLength; i++)
				{
					num = (num * 31) ^ ptr[i];
				}
				return num;
			}
		}

		public unsafe bool Equals(NetworkInterfaceEndPoint other)
		{
			if (dataLength != other.dataLength && (dataLength <= 0 || other.dataLength <= 0))
			{
				return false;
			}
			fixed (byte* ptr = data)
			{
				void* ptr2 = ptr;
				return UnsafeUtility.MemCmp(ptr2, other.data, math.min(dataLength, other.dataLength)) == 0;
			}
		}

		public unsafe FixedString64Bytes ToFixedString()
		{
			if (!IsValid)
			{
				return "Not Valid";
			}
			int num = dataLength;
			FixedString64Bytes fs = default(FixedString64Bytes);
			if (num == 4)
			{
				FixedStringMethods.Append(ref fs, data[0]);
				FixedStringMethods.Append(ref fs, '.');
				FixedStringMethods.Append(ref fs, data[1]);
				FixedStringMethods.Append(ref fs, '.');
				FixedStringMethods.Append(ref fs, data[2]);
				FixedStringMethods.Append(ref fs, '.');
				FixedStringMethods.Append(ref fs, data[3]);
				return fs;
			}
			FixedString32Bytes input = "0x";
			FixedStringMethods.Append(ref fs, in input);
			fixed (byte* ptr = data)
			{
				for (int i = 0; i < num; i += 2)
				{
					ushort* ptr2 = (ushort*)(ptr + i);
					FixedStringHexExt.AppendHex(ref fs, *ptr2);
				}
			}
			return fs;
		}

		public override string ToString()
		{
			return ToFixedString().ToString();
		}
	}
}
