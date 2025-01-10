using System;
using Unity.Collections;
using Unity.Mathematics;

namespace Unity.Networking.Transport
{
	public struct NetworkCompressionModel : IDisposable
	{
		internal static readonly byte[] k_BucketSizes = new byte[16]
		{
			0, 0, 1, 2, 3, 4, 6, 8, 10, 12,
			15, 18, 21, 24, 27, 32
		};

		internal static readonly uint[] k_BucketOffsets = new uint[16]
		{
			0u, 1u, 2u, 4u, 8u, 16u, 32u, 96u, 352u, 1376u,
			5472u, 38240u, 300384u, 2397536u, 19174752u, 153392480u
		};

		internal static readonly int[] k_FirstBucketCandidate = new int[33]
		{
			15, 15, 15, 15, 14, 14, 14, 13, 13, 13,
			12, 12, 12, 11, 11, 11, 10, 10, 10, 9,
			9, 8, 8, 7, 7, 6, 5, 4, 3, 2,
			1, 1, 0
		};

		internal static readonly byte[] k_DefaultModelData = new byte[19]
		{
			16, 2, 3, 3, 3, 4, 4, 4, 5, 5,
			5, 6, 6, 6, 6, 6, 6, 0, 0
		};

		internal const int k_AlphabetSize = 16;

		internal const int k_MaxHuffmanSymbolLength = 6;

		internal const int k_MaxContexts = 1;

		internal unsafe fixed ushort encodeTable[16];

		internal unsafe fixed ushort decodeTable[64];

		internal unsafe fixed byte bucketSizes[16];

		internal unsafe fixed uint bucketOffsets[16];

		public void Dispose()
		{
		}

		public unsafe NetworkCompressionModel(Allocator allocator)
		{
			for (int i = 0; i < 16; i++)
			{
				bucketSizes[i] = k_BucketSizes[i];
				bucketOffsets[i] = k_BucketOffsets[i];
			}
			byte[] array = k_DefaultModelData;
			int num = 1;
			byte[,] array2 = new byte[num, 16];
			int num2 = 0;
			_ = array[num2++];
			for (int j = 0; j < 16; j++)
			{
				byte b = array[num2++];
				for (int k = 0; k < num; k++)
				{
					array2[k, j] = b;
				}
			}
			int num3 = array[num2] | (array[num2 + 1] << 8);
			num2 += 2;
			for (int l = 0; l < num3; l++)
			{
				int num4 = array[num2] | (array[num2 + 1] << 8);
				num2 += 2;
				_ = array[num2++];
				for (int m = 0; m < 16; m++)
				{
					byte b2 = array[num2++];
					array2[num4, m] = b2;
				}
			}
			byte[] array3 = new byte[16];
			ushort[] array4 = new ushort[64];
			byte[] array5 = new byte[16];
			for (int n = 0; n < num; n++)
			{
				for (int num5 = 0; num5 < 16; num5++)
				{
					array3[num5] = array2[n, num5];
				}
				GenerateHuffmanCodes(array5, 0, array3, 0, 16, 6);
				GenerateHuffmanDecodeTable(array4, 0, array3, array5, 16, 6);
				for (int num6 = 0; num6 < 16; num6++)
				{
					encodeTable[n * 16 + num6] = (ushort)((array5[num6] << 8) | array2[n, num6]);
				}
				for (int num7 = 0; num7 < 64; num7++)
				{
					decodeTable[n * 64 + num7] = array4[num7];
				}
			}
		}

		private static void GenerateHuffmanCodes(byte[] symboLCodes, int symbolCodesOffset, byte[] symbolLengths, int symbolLengthsOffset, int alphabetSize, int maxCodeLength)
		{
			byte[] array = new byte[maxCodeLength + 1];
			byte[,] array2 = new byte[maxCodeLength + 1, alphabetSize];
			for (int i = 0; i < alphabetSize; i++)
			{
				int num = symbolLengths[i + symbolLengthsOffset];
				array2[num, array[num]++] = (byte)i;
			}
			uint num2 = 0u;
			for (int j = 1; j <= maxCodeLength; j++)
			{
				int num3 = array[j];
				for (int k = 0; k < num3; k++)
				{
					int num4 = array2[j, k];
					symboLCodes[num4 + symbolCodesOffset] = (byte)ReverseBits(num2++, j);
				}
				num2 <<= 1;
			}
		}

		private static uint ReverseBits(uint value, int num_bits)
		{
			value = ((value & 0x55555555) << 1) | ((value & 0xAAAAAAAAu) >> 1);
			value = ((value & 0x33333333) << 2) | ((value & 0xCCCCCCCCu) >> 2);
			value = ((value & 0xF0F0F0F) << 4) | ((value & 0xF0F0F0F0u) >> 4);
			value = ((value & 0xFF00FF) << 8) | ((value & 0xFF00FF00u) >> 8);
			value = (value << 16) | (value >> 16);
			return value >> 32 - num_bits;
		}

		private static void GenerateHuffmanDecodeTable(ushort[] decodeTable, int decodeTableOffset, byte[] symbolLengths, byte[] symbolCodes, int alphabetSize, int maxCodeLength)
		{
			uint num = (uint)(1 << maxCodeLength);
			for (int i = 0; i < alphabetSize; i++)
			{
				int num2 = symbolLengths[i];
				if (num2 > 0)
				{
					uint num3 = symbolCodes[i];
					uint num4 = (uint)(1 << num2);
					do
					{
						decodeTable[decodeTableOffset + num3] = (ushort)((i << 8) | num2);
						num3 += num4;
					}
					while (num3 < num);
				}
			}
		}

		public unsafe int CalculateBucket(uint value)
		{
			int num = k_FirstBucketCandidate[math.lzcnt(value)];
			if (num + 1 < 16 && value >= bucketOffsets[num + 1])
			{
				num++;
			}
			return num;
		}
	}
}
