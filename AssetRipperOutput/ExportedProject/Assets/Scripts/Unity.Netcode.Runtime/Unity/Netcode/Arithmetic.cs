namespace Unity.Netcode
{
	public static class Arithmetic
	{
		internal const long SIGN_BIT_64 = long.MinValue;

		internal const int SIGN_BIT_32 = int.MinValue;

		internal const short SIGN_BIT_16 = short.MinValue;

		internal const sbyte SIGN_BIT_8 = sbyte.MinValue;

		internal static ulong CeilingExact(ulong u1, ulong u2)
		{
			return (u1 + u2 - 1) / u2;
		}

		internal static long CeilingExact(long u1, long u2)
		{
			return (u1 + u2 - 1) / u2;
		}

		internal static uint CeilingExact(uint u1, uint u2)
		{
			return (u1 + u2 - 1) / u2;
		}

		internal static int CeilingExact(int u1, int u2)
		{
			return (u1 + u2 - 1) / u2;
		}

		internal static ushort CeilingExact(ushort u1, ushort u2)
		{
			return (ushort)((u1 + u2 - 1) / u2);
		}

		internal static short CeilingExact(short u1, short u2)
		{
			return (short)((u1 + u2 - 1) / u2);
		}

		internal static byte CeilingExact(byte u1, byte u2)
		{
			return (byte)((u1 + u2 - 1) / u2);
		}

		internal static sbyte CeilingExact(sbyte u1, sbyte u2)
		{
			return (sbyte)((u1 + u2 - 1) / u2);
		}

		public static ulong ZigZagEncode(long value)
		{
			return (ulong)((value >> 63) ^ (value << 1));
		}

		public static long ZigZagDecode(ulong value)
		{
			return (long)((value >> 1) & 0x7FFFFFFFFFFFFFFFL) ^ ((long)(value << 63) >> 63);
		}

		public static int VarIntSize(ulong value)
		{
			if (value > 240)
			{
				if (value > 2287)
				{
					if (value > 67823)
					{
						if (value > 16777215)
						{
							if (value > uint.MaxValue)
							{
								if (value > 1099511627775L)
								{
									if (value > 281474976710655L)
									{
										if (value > 72057594037927935L)
										{
											return 9;
										}
										return 8;
									}
									return 7;
								}
								return 6;
							}
							return 5;
						}
						return 4;
					}
					return 3;
				}
				return 2;
			}
			return 1;
		}

		internal static long Div8Ceil(ulong value)
		{
			return (long)((value >> 3) + ((value & 1) | ((value >> 1) & 1) | ((value >> 2) & 1)));
		}
	}
}
