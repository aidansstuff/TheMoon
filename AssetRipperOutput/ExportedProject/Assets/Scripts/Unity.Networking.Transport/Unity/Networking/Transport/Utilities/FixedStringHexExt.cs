using Unity.Collections;

namespace Unity.Networking.Transport.Utilities
{
	public static class FixedStringHexExt
	{
		public static FormatError AppendHex<T>(this ref T str, ushort val) where T : unmanaged, INativeList<byte>, IUTF8Bytes
		{
			int num = 12;
			while (num > 0 && ((val >> num) & 0xF) == 0)
			{
				num -= 4;
			}
			FormatError formatError = FormatError.None;
			while (num >= 0)
			{
				int num2 = (val >> num) & 0xF;
				formatError = ((num2 < 10) ? (formatError | FixedStringMethods.AppendRawByte(ref str, (byte)(48 + num2))) : (formatError | FixedStringMethods.AppendRawByte(ref str, (byte)(97 + num2 - 10))));
				num -= 4;
			}
			if (formatError == FormatError.None)
			{
				return FormatError.None;
			}
			return FormatError.Overflow;
		}
	}
}
