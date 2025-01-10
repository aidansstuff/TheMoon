using System.Runtime.CompilerServices;

namespace Unity.Netcode
{
	public static class BitCounter
	{
		private const ulong k_DeBruijnMagic64 = 251784493209109903uL;

		private const uint k_DeBruijnMagic32 = 116069625u;

		private static readonly int[] k_DeBruijnTableBytes64 = new int[64]
		{
			1, 1, 3, 1, 3, 7, 1, 8, 6, 3,
			3, 7, 4, 1, 5, 8, 2, 7, 3, 4,
			4, 3, 7, 6, 7, 4, 5, 1, 6, 5,
			8, 2, 8, 3, 7, 8, 6, 3, 4, 5,
			2, 4, 4, 6, 7, 5, 6, 1, 8, 7,
			6, 4, 2, 5, 5, 1, 8, 6, 2, 5,
			8, 2, 2, 2
		};

		private static readonly int[] k_DeBruijnTableBytes32 = new int[32]
		{
			1, 1, 3, 1, 4, 3, 1, 3, 4, 3,
			3, 2, 2, 1, 1, 3, 4, 2, 4, 3,
			3, 2, 2, 1, 2, 4, 2, 1, 4, 2,
			4, 4
		};

		private static readonly int[] k_DeBruijnTableBits64 = new int[64]
		{
			1, 2, 18, 3, 19, 51, 4, 58, 48, 20,
			23, 52, 30, 5, 34, 59, 16, 49, 21, 28,
			26, 24, 53, 42, 55, 31, 39, 6, 44, 35,
			60, 9, 64, 17, 50, 57, 47, 22, 29, 33,
			15, 27, 25, 41, 54, 38, 43, 8, 63, 56,
			46, 32, 14, 40, 37, 7, 62, 45, 13, 36,
			61, 12, 11, 10
		};

		private static readonly int[] k_DeBruijnTableBits32 = new int[32]
		{
			1, 2, 17, 3, 30, 18, 4, 23, 31, 21,
			19, 12, 14, 5, 8, 24, 32, 16, 29, 22,
			20, 11, 13, 7, 15, 28, 10, 6, 27, 9,
			26, 25
		};

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetUsedByteCount(uint value)
		{
			value |= value >> 1;
			value |= value >> 2;
			value |= value >> 4;
			value |= value >> 8;
			value |= value >> 16;
			value &= ~(value >> 1);
			return k_DeBruijnTableBytes32[value * 116069625 >> 27];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetUsedByteCount(ulong value)
		{
			value |= value >> 1;
			value |= value >> 2;
			value |= value >> 4;
			value |= value >> 8;
			value |= value >> 16;
			value |= value >> 32;
			value &= ~(value >> 1);
			return k_DeBruijnTableBytes64[value * 251784493209109903L >> 58];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetUsedBitCount(uint value)
		{
			value |= value >> 1;
			value |= value >> 2;
			value |= value >> 4;
			value |= value >> 8;
			value |= value >> 16;
			value &= ~(value >> 1);
			return k_DeBruijnTableBits32[value * 116069625 >> 27];
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int GetUsedBitCount(ulong value)
		{
			value |= value >> 1;
			value |= value >> 2;
			value |= value >> 4;
			value |= value >> 8;
			value |= value >> 16;
			value |= value >> 32;
			value &= ~(value >> 1);
			return k_DeBruijnTableBits64[value * 251784493209109903L >> 58];
		}
	}
}
