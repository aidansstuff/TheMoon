using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Unity.Networking.Transport
{
	internal static class Base64
	{
		private unsafe static int FromBase64_Decode_UTF16(byte* startInputPtr, int inputLength, byte* startDestPtr, int destLength)
		{
			if (inputLength == 0)
			{
				return 0;
			}
			if (inputLength % 4 != 0)
			{
				Debug.LogError("Base64 string's length must be multiple of 4");
				return -1;
			}
			if (destLength < inputLength / 4 * 3 - 2)
			{
				Debug.LogError("Dest array is too small");
				return -1;
			}
			byte* ptr = startDestPtr;
			int num = inputLength / 4;
			byte* ptr2 = stackalloc byte[256];
			UnsafeUtility.MemSet(ptr2, byte.MaxValue, 256L);
			for (byte b = 0; b < "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/".Length; b++)
			{
				ptr2[(int)"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/"[b]] = b;
			}
			ptr2[61] = 0;
			for (int i = 0; i < num - 1; i++)
			{
				byte b2 = ptr2[(int)(*startInputPtr)];
				byte b3 = ptr2[(int)startInputPtr[2]];
				byte b4 = ptr2[(int)startInputPtr[4]];
				byte b5 = ptr2[(int)startInputPtr[6]];
				if (b2 == byte.MaxValue || b3 == byte.MaxValue || b4 == byte.MaxValue || b5 == byte.MaxValue)
				{
					Debug.LogError("Invalid Base64 symbol");
					return -1;
				}
				*(startDestPtr++) = (byte)((b2 << 2) | (b3 >> 4));
				*(startDestPtr++) = (byte)((b3 << 4) | (b4 >> 2));
				*(startDestPtr++) = (byte)((b4 << 6) | b5);
				startInputPtr += 8;
			}
			byte b6 = startInputPtr[4];
			byte b7 = startInputPtr[6];
			byte b8 = ptr2[(int)(*startInputPtr)];
			byte b9 = ptr2[(int)startInputPtr[2]];
			byte b10 = ptr2[(int)b6];
			byte b11 = ptr2[(int)b7];
			if (b8 == byte.MaxValue || b9 == byte.MaxValue || b10 == byte.MaxValue || b11 == byte.MaxValue)
			{
				Debug.LogError("Invalid Base64 symbol");
				return -1;
			}
			*(startDestPtr++) = (byte)((b8 << 2) | (b9 >> 4));
			if (b6 != 61)
			{
				if (b7 == 61)
				{
					if (destLength < inputLength / 4 * 3 - 1)
					{
						Debug.LogError("Dest array is too small");
						return -1;
					}
					*(startDestPtr++) = (byte)((b9 << 4) | (b10 >> 2));
				}
				else
				{
					if (destLength < inputLength / 4 * 3)
					{
						Debug.LogError("Dest array is too small");
						return -1;
					}
					*(startDestPtr++) = (byte)((b9 << 4) | (b10 >> 2));
					*(startDestPtr++) = (byte)((b10 << 6) | b11);
				}
			}
			return (int)(startDestPtr - ptr);
		}

		public unsafe static int FromBase64String(string base64, byte* dest, int destMaxLength)
		{
			int result;
			fixed (char* startInputPtr = base64)
			{
				result = FromBase64_Decode_UTF16((byte*)startInputPtr, base64.Length, dest, destMaxLength);
			}
			return result;
		}
	}
}
