using System;

namespace Unity.Collections.LowLevel.Unsafe
{
	[Obsolete("This storage will no longer be used. (RemovedAfter 2021-06-01)")]
	public struct NumberedWords
	{
		private int Index;

		private int Suffix;

		private const int kPositiveNumericSuffixShift = 0;

		private const int kPositiveNumericSuffixBits = 29;

		private const int kMaxPositiveNumericSuffix = 536870911;

		private const int kPositiveNumericSuffixMask = 536870911;

		private const int kLeadingZeroesShift = 29;

		private const int kLeadingZeroesBits = 3;

		private const int kMaxLeadingZeroes = 7;

		private const int kLeadingZeroesMask = 7;

		private int LeadingZeroes
		{
			get
			{
				return (Suffix >> 29) & 7;
			}
			set
			{
				Suffix &= 536870911;
				Suffix |= (value & 7) << 29;
			}
		}

		private int PositiveNumericSuffix
		{
			get
			{
				return Suffix & 0x1FFFFFFF;
			}
			set
			{
				Suffix &= -536870912;
				Suffix |= value & 0x1FFFFFFF;
			}
		}

		private bool HasPositiveNumericSuffix => PositiveNumericSuffix != 0;

		[NotBurstCompatible]
		private string NewString(char c, int count)
		{
			char[] array = new char[count];
			for (int i = 0; i < count; i++)
			{
				array[i] = c;
			}
			return new string(array, 0, count);
		}

		[NotBurstCompatible]
		public unsafe int ToFixedString<T>(ref T result) where T : IUTF8Bytes, INativeList<byte>
		{
			int num = PositiveNumericSuffix;
			int leadingZeroes = LeadingZeroes;
			WordStorage.Instance.GetFixedString(Index, ref result);
			if (num == 0 && leadingZeroes == 0)
			{
				return 0;
			}
			byte* ptr = stackalloc byte[17];
			int num2 = 17;
			while (num > 0)
			{
				ptr[--num2] = (byte)(48 + num % 10);
				num /= 10;
			}
			while (leadingZeroes-- > 0)
			{
				ptr[--num2] = 48;
			}
			byte* ptr2 = result.GetUnsafePtr() + result.Length;
			result.Length += 17 - num2;
			while (num2 < 17)
			{
				*(ptr2++) = ptr[num2++];
			}
			return 0;
		}

		[NotBurstCompatible]
		public override string ToString()
		{
			FixedString512Bytes result = default(FixedString512Bytes);
			ToFixedString(ref result);
			return result.ToString();
		}

		private bool IsDigit(byte b)
		{
			if (b >= 48)
			{
				return b <= 57;
			}
			return false;
		}

		[NotBurstCompatible]
		public void SetString<T>(ref T value) where T : IUTF8Bytes, INativeList<byte>
		{
			int num = value.Length;
			while (num > 0 && IsDigit(value[num - 1]))
			{
				num--;
			}
			int i;
			for (i = num; i < value.Length && value[i] == 48; i++)
			{
			}
			int num2 = i - num;
			if (num2 > 7)
			{
				int num3 = num2 - 7;
				num += num3;
				num2 -= num3;
			}
			PositiveNumericSuffix = 0;
			int num4 = 0;
			for (int j = i; j < value.Length; j++)
			{
				num4 *= 10;
				num4 += value[j] - 48;
			}
			if (num4 <= 536870911)
			{
				PositiveNumericSuffix = num4;
			}
			else
			{
				num = value.Length;
				num2 = 0;
			}
			LeadingZeroes = num2;
			T value2 = value;
			_ = value2.Length;
			if (num != value2.Length)
			{
				value2.Length = num;
			}
			Index = WordStorage.Instance.GetOrCreateIndex(ref value2);
		}

		[NotBurstCompatible]
		public void SetString(string value)
		{
			FixedString512Bytes value2 = value;
			SetString(ref value2);
		}
	}
}
