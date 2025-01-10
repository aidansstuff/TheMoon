using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Unity.Networking.Transport.Relay
{
	public struct RelayHMACKey
	{
		public const int k_Length = 64;

		public unsafe fixed byte Value[64];

		public unsafe static RelayHMACKey FromBytePointer(byte* data, int length)
		{
			if (length != 64)
			{
				Debug.LogError($"Provided byte array length is invalid, must be {64} but got {length}.");
				return default(RelayHMACKey);
			}
			RelayHMACKey result = default(RelayHMACKey);
			UnsafeUtility.MemCpy(result.Value, data, length);
			return result;
		}

		public unsafe static RelayHMACKey FromByteArray(byte[] data)
		{
			fixed (byte* data2 = data)
			{
				return FromBytePointer(data2, data.Length);
			}
		}
	}
}
