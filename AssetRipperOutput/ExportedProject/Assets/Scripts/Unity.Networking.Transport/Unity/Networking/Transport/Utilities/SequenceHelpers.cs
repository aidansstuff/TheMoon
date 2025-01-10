namespace Unity.Networking.Transport.Utilities
{
	public static class SequenceHelpers
	{
		public static int AbsDistance(ushort lhs, ushort rhs)
		{
			if (lhs < rhs)
			{
				return lhs + 65535 + 1 - rhs;
			}
			return lhs - rhs;
		}

		public static bool IsNewer(uint current, uint old)
		{
			return old - current >= 2147483648u;
		}

		public static bool GreaterThan16(ushort lhs, ushort rhs)
		{
			if (lhs <= rhs || lhs - rhs > 32767)
			{
				if (lhs < rhs)
				{
					return rhs - lhs > 32767;
				}
				return false;
			}
			return true;
		}

		public static bool LessThan16(ushort lhs, ushort rhs)
		{
			return GreaterThan16(rhs, lhs);
		}

		public static bool StalePacket(ushort sequence, ushort oldSequence, ushort windowSize)
		{
			return LessThan16(sequence, (ushort)(oldSequence - windowSize));
		}

		public static string BitMaskToString(uint mask)
		{
			char[] array = new char[32];
			for (int num = 31; num >= 0; num--)
			{
				array[num] = (((mask & (true ? 1u : 0u)) != 0) ? '1' : '0');
				mask >>= 1;
			}
			return new string(array);
		}
	}
}
