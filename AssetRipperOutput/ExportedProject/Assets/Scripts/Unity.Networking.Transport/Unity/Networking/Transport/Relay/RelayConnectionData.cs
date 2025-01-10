using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Unity.Networking.Transport.Relay
{
	public struct RelayConnectionData
	{
		public const int k_Length = 255;

		public unsafe fixed byte Value[255];

		public unsafe static RelayConnectionData FromBytePointer(byte* dataPtr, int length)
		{
			if (length > 255)
			{
				Debug.LogError($"Provided byte array length is invalid, must be less or equal to {255} but got {length}.");
				return default(RelayConnectionData);
			}
			RelayConnectionData result = default(RelayConnectionData);
			UnsafeUtility.MemCpy(result.Value, dataPtr, length);
			return result;
		}

		public unsafe static RelayConnectionData FromByteArray(byte[] data)
		{
			fixed (byte* dataPtr = data)
			{
				return FromBytePointer(dataPtr, data.Length);
			}
		}
	}
}
