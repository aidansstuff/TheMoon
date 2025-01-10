using System.Runtime.CompilerServices;
using UnityEngine;

namespace Unity.Netcode
{
	public static class QuaternionCompressor
	{
		private const ushort k_PrecisionMask = 511;

		private const float k_SqrtTwoOverTwoEncoding = 0.70710677f;

		private const float k_CompressionEcodingMask = 722.66315f;

		private const ushort k_ShiftNegativeBit = 9;

		private const float k_DcompressionDecodingMask = 0.0013837706f;

		private const ushort k_NegShortBit = 512;

		private const ushort k_True = 1;

		private const ushort k_False = 0;

		private static Quaternion s_QuatAbsValues = Quaternion.identity;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint CompressQuaternion(ref Quaternion quaternion)
		{
			s_QuatAbsValues[0] = Mathf.Abs(quaternion[0]);
			s_QuatAbsValues[1] = Mathf.Abs(quaternion[1]);
			s_QuatAbsValues[2] = Mathf.Abs(quaternion[2]);
			s_QuatAbsValues[3] = Mathf.Abs(quaternion[3]);
			float num = Mathf.Max(s_QuatAbsValues[0], s_QuatAbsValues[1], s_QuatAbsValues[2], s_QuatAbsValues[3]);
			ushort num2 = (ushort)((s_QuatAbsValues[0] != num) ? ((s_QuatAbsValues[1] == num) ? 1u : ((s_QuatAbsValues[2] == num) ? 2u : 3u)) : 0u);
			ushort num3 = ((quaternion[num2] < 0f) ? ((ushort)1) : ((ushort)0));
			uint num4 = num2;
			int num5 = 0;
			num4 = ((num5 != num2) ? ((num4 << 10) | (uint)(((((quaternion[num5] < 0f) ? 1u : 0u) != num3) ? 1 : 0) << 9) | (ushort)Mathf.Round(722.66315f * s_QuatAbsValues[num5])) : num4);
			num5++;
			num4 = ((num5 != num2) ? ((num4 << 10) | (uint)(((((quaternion[num5] < 0f) ? 1u : 0u) != num3) ? 1 : 0) << 9) | (ushort)Mathf.Round(722.66315f * s_QuatAbsValues[num5])) : num4);
			num5++;
			num4 = ((num5 != num2) ? ((num4 << 10) | (uint)(((((quaternion[num5] < 0f) ? 1u : 0u) != num3) ? 1 : 0) << 9) | (ushort)Mathf.Round(722.66315f * s_QuatAbsValues[num5])) : num4);
			num5++;
			return (num5 != num2) ? ((num4 << 10) | (uint)(((((quaternion[num5] < 0f) ? 1u : 0u) != num3) ? 1 : 0) << 9) | (ushort)Mathf.Round(722.66315f * s_QuatAbsValues[num5])) : num4;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void DecompressQuaternion(ref Quaternion quaternion, uint compressed)
		{
			int num = (int)(compressed >> 30);
			float num2 = 0f;
			for (int num3 = 3; num3 >= 0; num3--)
			{
				if (num3 != num)
				{
					quaternion[num3] = (((compressed & 0x200u) != 0) ? (-1f) : 1f) * ((float)(compressed & 0x1FFu) * 0.0013837706f);
					num2 += quaternion[num3] * quaternion[num3];
					compressed >>= 10;
				}
			}
			quaternion[num] = Mathf.Sqrt(1f - num2);
		}
	}
}
