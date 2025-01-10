using System;
using System.Runtime.CompilerServices;
using Unity.Collections.LowLevel.Unsafe;

namespace Unity.Collections
{
	[BurstCompatible]
	[BurstCompatible]
	[BurstCompatible]
	[BurstCompatible]
	public static class FixedStringMethods
	{
		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public static FormatError Append<T>(this ref T fs, Unicode.Rune rune) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			int index = fs.Length;
			int num = rune.LengthInUtf8Bytes();
			if (!fs.TryResize(index + num, NativeArrayOptions.UninitializedMemory))
			{
				return FormatError.Overflow;
			}
			return Write(ref fs, ref index, rune);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public static FormatError Append<T>(this ref T fs, char ch) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			return Append(ref fs, (Unicode.Rune)ch);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static FormatError AppendRawByte<T>(this ref T fs, byte a) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			int length = fs.Length;
			if (!fs.TryResize(length + 1, NativeArrayOptions.UninitializedMemory))
			{
				return FormatError.Overflow;
			}
			fs.GetUnsafePtr()[length] = a;
			return FormatError.None;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static FormatError Append<T>(this ref T fs, Unicode.Rune rune, int count) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			int length = fs.Length;
			if (!fs.TryResize(length + rune.LengthInUtf8Bytes() * count, NativeArrayOptions.UninitializedMemory))
			{
				return FormatError.Overflow;
			}
			int capacity = fs.Capacity;
			byte* unsafePtr = fs.GetUnsafePtr();
			int index = length;
			for (int i = 0; i < count; i++)
			{
				if (Unicode.UcsToUtf8(unsafePtr, ref index, capacity, rune) != 0)
				{
					return FormatError.Overflow;
				}
			}
			return FormatError.None;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static FormatError Append<T>(this ref T fs, long input) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			byte* ptr = stackalloc byte[20];
			int num = 20;
			if (input >= 0)
			{
				do
				{
					byte b = (byte)(input % 10);
					ptr[--num] = (byte)(48 + b);
					input /= 10;
				}
				while (input != 0L);
			}
			else
			{
				do
				{
					byte b2 = (byte)(input % 10);
					ptr[--num] = (byte)(48 - b2);
					input /= 10;
				}
				while (input != 0L);
				ptr[--num] = 45;
			}
			return Append(ref fs, ptr + num, 20 - num);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public static FormatError Append<T>(this ref T fs, int input) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			return Append(ref fs, (long)input);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static FormatError Append<T>(this ref T fs, ulong input) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			byte* ptr = stackalloc byte[20];
			int num = 20;
			do
			{
				byte b = (byte)(input % 10);
				ptr[--num] = (byte)(48 + b);
				input /= 10;
			}
			while (input != 0L);
			return Append(ref fs, ptr + num, 20 - num);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public static FormatError Append<T>(this ref T fs, uint input) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			return Append(ref fs, (ulong)input);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static FormatError Append<T>(this ref T fs, float input, char decimalSeparator = '.') where T : struct, INativeList<byte>, IUTF8Bytes
		{
			FixedStringUtils.UintFloatUnion uintFloatUnion = default(FixedStringUtils.UintFloatUnion);
			uintFloatUnion.floatValue = input;
			uint num = uintFloatUnion.uintValue >> 31;
			uintFloatUnion.uintValue &= 2147483647u;
			FormatError result;
			if ((uintFloatUnion.uintValue & 0x7F800000) == 2139095040)
			{
				if (uintFloatUnion.uintValue == 2139095040)
				{
					if (num != 0 && (result = Append(ref fs, '-')) != 0)
					{
						return result;
					}
					return Append(ref fs, 'I', 'n', 'f', 'i', 'n', 'i', 't', 'y');
				}
				return Append(ref fs, 'N', 'a', 'N');
			}
			if (num != 0 && uintFloatUnion.uintValue != 0 && (result = Append(ref fs, '-')) != 0)
			{
				return result;
			}
			ulong mantissa = 0uL;
			int exponent = 0;
			FixedStringUtils.Base2ToBase10(ref mantissa, ref exponent, uintFloatUnion.floatValue);
			char* ptr = stackalloc char[9];
			int num2 = 0;
			do
			{
				if (num2 >= 9)
				{
					return FormatError.Overflow;
				}
				ulong num3 = mantissa % 10;
				ptr[8 - num2++] = (char)(48 + num3);
				mantissa /= 10;
			}
			while (mantissa != 0);
			char* ptr2 = ptr + 9 - num2;
			int num4 = -exponent - num2 + 1;
			if (num4 > 0)
			{
				if (num4 > 4)
				{
					return AppendScientific(ref fs, ptr2, num2, exponent, decimalSeparator);
				}
				if ((result = Append(ref fs, '0', decimalSeparator)) != 0)
				{
					return result;
				}
				for (num4--; num4 > 0; num4--)
				{
					if ((result = Append(ref fs, '0')) != 0)
					{
						return result;
					}
				}
				for (int i = 0; i < num2; i++)
				{
					if ((result = Append(ref fs, ptr2[i])) != 0)
					{
						return result;
					}
				}
				return FormatError.None;
			}
			int num5 = exponent;
			if (num5 > 0)
			{
				if (num5 > 4)
				{
					return AppendScientific(ref fs, ptr2, num2, exponent, decimalSeparator);
				}
				for (int j = 0; j < num2; j++)
				{
					if ((result = Append(ref fs, ptr2[j])) != 0)
					{
						return result;
					}
				}
				while (num5 > 0)
				{
					if ((result = Append(ref fs, '0')) != 0)
					{
						return result;
					}
					num5--;
				}
				return FormatError.None;
			}
			int num6 = num2 + exponent;
			for (int k = 0; k < num2; k++)
			{
				if (k == num6 && (result = Append(ref fs, decimalSeparator)) != 0)
				{
					return result;
				}
				if ((result = Append(ref fs, ptr2[k])) != 0)
				{
					return result;
				}
			}
			return FormatError.None;
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static FormatError Append<T, T2>(this ref T fs, in T2 input) where T : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref T2 reference = ref UnsafeUtilityExtensions.AsRef(in input);
			return Append(ref fs, reference.GetUnsafePtr(), reference.Length);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public static CopyError CopyFrom<T, T2>(this ref T fs, in T2 input) where T : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes
		{
			fs.Length = 0;
			if (Append(ref fs, in input) != 0)
			{
				return CopyError.Truncation;
			}
			return CopyError.None;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static FormatError Append<T>(this ref T fs, byte* utf8Bytes, int utf8BytesLength) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			int length = fs.Length;
			if (!fs.TryResize(length + utf8BytesLength, NativeArrayOptions.UninitializedMemory))
			{
				return FormatError.Overflow;
			}
			UnsafeUtility.MemCpy(fs.GetUnsafePtr() + length, utf8Bytes, utf8BytesLength);
			return FormatError.None;
		}

		[NotBurstCompatible]
		public unsafe static FormatError Append<T>(this ref T fs, string s) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			int num = s.Length * 4;
			byte* ptr = stackalloc byte[(int)(uint)num];
			int destLength;
			fixed (char* src = s)
			{
				if (UTF8ArrayUnsafeUtility.Copy(ptr, out destLength, num, src, s.Length) != 0)
				{
					return FormatError.Overflow;
				}
			}
			return Append(ref fs, ptr, destLength);
		}

		[NotBurstCompatible]
		public static CopyError CopyFrom<T>(this ref T fs, string s) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			fs.Length = 0;
			if (Append(ref fs, s) != 0)
			{
				return CopyError.Truncation;
			}
			return CopyError.None;
		}

		[NotBurstCompatible]
		public unsafe static void CopyFromTruncated<T>(this ref T fs, string s) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			fixed (char* src = s)
			{
				UTF8ArrayUnsafeUtility.Copy(fs.GetUnsafePtr(), out var destLength, fs.Capacity, src, s.Length);
				fs.Length = destLength;
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static void AppendFormat<T, U, T0>(this ref T dest, in U format, in T0 arg0) where T : struct, INativeList<byte>, IUTF8Bytes where U : struct, INativeList<byte>, IUTF8Bytes where T0 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref U reference = ref UnsafeUtilityExtensions.AsRef(in format);
			int length = reference.Length;
			byte* unsafePtr = reference.GetUnsafePtr();
			for (int i = 0; i < length; i++)
			{
				if (unsafePtr[i] == 123)
				{
					if (length - i >= 3 && unsafePtr[i + 1] != 123)
					{
						if (unsafePtr[i + 1] - 48 == 0)
						{
							Append(ref dest, in arg0);
							i += 2;
						}
						else
						{
							AppendRawByte(ref dest, unsafePtr[i]);
						}
					}
				}
				else
				{
					AppendRawByte(ref dest, unsafePtr[i]);
				}
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static void AppendFormat<T, U, T0, T1>(this ref T dest, in U format, in T0 arg0, in T1 arg1) where T : struct, INativeList<byte>, IUTF8Bytes where U : struct, INativeList<byte>, IUTF8Bytes where T0 : struct, INativeList<byte>, IUTF8Bytes where T1 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref U reference = ref UnsafeUtilityExtensions.AsRef(in format);
			int length = reference.Length;
			byte* unsafePtr = reference.GetUnsafePtr();
			for (int i = 0; i < length; i++)
			{
				if (unsafePtr[i] == 123)
				{
					if (length - i >= 3 && unsafePtr[i + 1] != 123)
					{
						switch (unsafePtr[i + 1] - 48)
						{
						case 0:
							Append(ref dest, in arg0);
							i += 2;
							break;
						case 1:
							Append(ref dest, in arg1);
							i += 2;
							break;
						default:
							AppendRawByte(ref dest, unsafePtr[i]);
							break;
						}
					}
				}
				else
				{
					AppendRawByte(ref dest, unsafePtr[i]);
				}
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static void AppendFormat<T, U, T0, T1, T2>(this ref T dest, in U format, in T0 arg0, in T1 arg1, in T2 arg2) where T : struct, INativeList<byte>, IUTF8Bytes where U : struct, INativeList<byte>, IUTF8Bytes where T0 : struct, INativeList<byte>, IUTF8Bytes where T1 : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref U reference = ref UnsafeUtilityExtensions.AsRef(in format);
			int length = reference.Length;
			byte* unsafePtr = reference.GetUnsafePtr();
			for (int i = 0; i < length; i++)
			{
				if (unsafePtr[i] == 123)
				{
					if (length - i >= 3 && unsafePtr[i + 1] != 123)
					{
						switch (unsafePtr[i + 1])
						{
						case 48:
							Append(ref dest, in arg0);
							i += 2;
							break;
						case 49:
							Append(ref dest, in arg1);
							i += 2;
							break;
						case 50:
							Append(ref dest, in arg2);
							i += 2;
							break;
						default:
							AppendRawByte(ref dest, unsafePtr[i]);
							break;
						}
					}
				}
				else
				{
					AppendRawByte(ref dest, unsafePtr[i]);
				}
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static void AppendFormat<T, U, T0, T1, T2, T3>(this ref T dest, in U format, in T0 arg0, in T1 arg1, in T2 arg2, in T3 arg3) where T : struct, INativeList<byte>, IUTF8Bytes where U : struct, INativeList<byte>, IUTF8Bytes where T0 : struct, INativeList<byte>, IUTF8Bytes where T1 : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes where T3 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref U reference = ref UnsafeUtilityExtensions.AsRef(in format);
			int length = reference.Length;
			byte* unsafePtr = reference.GetUnsafePtr();
			for (int i = 0; i < length; i++)
			{
				if (unsafePtr[i] == 123)
				{
					if (length - i >= 3 && unsafePtr[i + 1] != 123)
					{
						switch (unsafePtr[i + 1])
						{
						case 48:
							Append(ref dest, in arg0);
							i += 2;
							break;
						case 49:
							Append(ref dest, in arg1);
							i += 2;
							break;
						case 50:
							Append(ref dest, in arg2);
							i += 2;
							break;
						case 51:
							Append(ref dest, in arg3);
							i += 2;
							break;
						default:
							AppendRawByte(ref dest, unsafePtr[i]);
							break;
						}
					}
				}
				else
				{
					AppendRawByte(ref dest, unsafePtr[i]);
				}
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static void AppendFormat<T, U, T0, T1, T2, T3, T4>(this ref T dest, in U format, in T0 arg0, in T1 arg1, in T2 arg2, in T3 arg3, in T4 arg4) where T : struct, INativeList<byte>, IUTF8Bytes where U : struct, INativeList<byte>, IUTF8Bytes where T0 : struct, INativeList<byte>, IUTF8Bytes where T1 : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes where T3 : struct, INativeList<byte>, IUTF8Bytes where T4 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref U reference = ref UnsafeUtilityExtensions.AsRef(in format);
			int length = reference.Length;
			byte* unsafePtr = reference.GetUnsafePtr();
			for (int i = 0; i < length; i++)
			{
				if (unsafePtr[i] == 123)
				{
					if (length - i >= 3 && unsafePtr[i + 1] != 123)
					{
						switch (unsafePtr[i + 1])
						{
						case 48:
							Append(ref dest, in arg0);
							i += 2;
							break;
						case 49:
							Append(ref dest, in arg1);
							i += 2;
							break;
						case 50:
							Append(ref dest, in arg2);
							i += 2;
							break;
						case 51:
							Append(ref dest, in arg3);
							i += 2;
							break;
						case 52:
							Append(ref dest, in arg4);
							i += 2;
							break;
						default:
							AppendRawByte(ref dest, unsafePtr[i]);
							break;
						}
					}
				}
				else
				{
					AppendRawByte(ref dest, unsafePtr[i]);
				}
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static void AppendFormat<T, U, T0, T1, T2, T3, T4, T5>(this ref T dest, in U format, in T0 arg0, in T1 arg1, in T2 arg2, in T3 arg3, in T4 arg4, in T5 arg5) where T : struct, INativeList<byte>, IUTF8Bytes where U : struct, INativeList<byte>, IUTF8Bytes where T0 : struct, INativeList<byte>, IUTF8Bytes where T1 : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes where T3 : struct, INativeList<byte>, IUTF8Bytes where T4 : struct, INativeList<byte>, IUTF8Bytes where T5 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref U reference = ref UnsafeUtilityExtensions.AsRef(in format);
			int length = reference.Length;
			byte* unsafePtr = reference.GetUnsafePtr();
			for (int i = 0; i < length; i++)
			{
				if (unsafePtr[i] == 123)
				{
					if (length - i >= 3 && unsafePtr[i + 1] != 123)
					{
						switch (unsafePtr[i + 1])
						{
						case 48:
							Append(ref dest, in arg0);
							i += 2;
							break;
						case 49:
							Append(ref dest, in arg1);
							i += 2;
							break;
						case 50:
							Append(ref dest, in arg2);
							i += 2;
							break;
						case 51:
							Append(ref dest, in arg3);
							i += 2;
							break;
						case 52:
							Append(ref dest, in arg4);
							i += 2;
							break;
						case 53:
							Append(ref dest, in arg5);
							i += 2;
							break;
						default:
							AppendRawByte(ref dest, unsafePtr[i]);
							break;
						}
					}
				}
				else
				{
					AppendRawByte(ref dest, unsafePtr[i]);
				}
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static void AppendFormat<T, U, T0, T1, T2, T3, T4, T5, T6>(this ref T dest, in U format, in T0 arg0, in T1 arg1, in T2 arg2, in T3 arg3, in T4 arg4, in T5 arg5, in T6 arg6) where T : struct, INativeList<byte>, IUTF8Bytes where U : struct, INativeList<byte>, IUTF8Bytes where T0 : struct, INativeList<byte>, IUTF8Bytes where T1 : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes where T3 : struct, INativeList<byte>, IUTF8Bytes where T4 : struct, INativeList<byte>, IUTF8Bytes where T5 : struct, INativeList<byte>, IUTF8Bytes where T6 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref U reference = ref UnsafeUtilityExtensions.AsRef(in format);
			int length = reference.Length;
			byte* unsafePtr = reference.GetUnsafePtr();
			for (int i = 0; i < length; i++)
			{
				if (unsafePtr[i] == 123)
				{
					if (length - i >= 3 && unsafePtr[i + 1] != 123)
					{
						switch (unsafePtr[i + 1])
						{
						case 48:
							Append(ref dest, in arg0);
							i += 2;
							break;
						case 49:
							Append(ref dest, in arg1);
							i += 2;
							break;
						case 50:
							Append(ref dest, in arg2);
							i += 2;
							break;
						case 51:
							Append(ref dest, in arg3);
							i += 2;
							break;
						case 52:
							Append(ref dest, in arg4);
							i += 2;
							break;
						case 53:
							Append(ref dest, in arg5);
							i += 2;
							break;
						case 54:
							Append(ref dest, in arg6);
							i += 2;
							break;
						default:
							AppendRawByte(ref dest, unsafePtr[i]);
							break;
						}
					}
				}
				else
				{
					AppendRawByte(ref dest, unsafePtr[i]);
				}
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static void AppendFormat<T, U, T0, T1, T2, T3, T4, T5, T6, T7>(this ref T dest, in U format, in T0 arg0, in T1 arg1, in T2 arg2, in T3 arg3, in T4 arg4, in T5 arg5, in T6 arg6, in T7 arg7) where T : struct, INativeList<byte>, IUTF8Bytes where U : struct, INativeList<byte>, IUTF8Bytes where T0 : struct, INativeList<byte>, IUTF8Bytes where T1 : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes where T3 : struct, INativeList<byte>, IUTF8Bytes where T4 : struct, INativeList<byte>, IUTF8Bytes where T5 : struct, INativeList<byte>, IUTF8Bytes where T6 : struct, INativeList<byte>, IUTF8Bytes where T7 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref U reference = ref UnsafeUtilityExtensions.AsRef(in format);
			int length = reference.Length;
			byte* unsafePtr = reference.GetUnsafePtr();
			for (int i = 0; i < length; i++)
			{
				if (unsafePtr[i] == 123)
				{
					if (length - i >= 3 && unsafePtr[i + 1] != 123)
					{
						switch (unsafePtr[i + 1])
						{
						case 48:
							Append(ref dest, in arg0);
							i += 2;
							break;
						case 49:
							Append(ref dest, in arg1);
							i += 2;
							break;
						case 50:
							Append(ref dest, in arg2);
							i += 2;
							break;
						case 51:
							Append(ref dest, in arg3);
							i += 2;
							break;
						case 52:
							Append(ref dest, in arg4);
							i += 2;
							break;
						case 53:
							Append(ref dest, in arg5);
							i += 2;
							break;
						case 54:
							Append(ref dest, in arg6);
							i += 2;
							break;
						case 55:
							Append(ref dest, in arg7);
							i += 2;
							break;
						default:
							AppendRawByte(ref dest, unsafePtr[i]);
							break;
						}
					}
				}
				else
				{
					AppendRawByte(ref dest, unsafePtr[i]);
				}
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static void AppendFormat<T, U, T0, T1, T2, T3, T4, T5, T6, T7, T8>(this ref T dest, in U format, in T0 arg0, in T1 arg1, in T2 arg2, in T3 arg3, in T4 arg4, in T5 arg5, in T6 arg6, in T7 arg7, in T8 arg8) where T : struct, INativeList<byte>, IUTF8Bytes where U : struct, INativeList<byte>, IUTF8Bytes where T0 : struct, INativeList<byte>, IUTF8Bytes where T1 : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes where T3 : struct, INativeList<byte>, IUTF8Bytes where T4 : struct, INativeList<byte>, IUTF8Bytes where T5 : struct, INativeList<byte>, IUTF8Bytes where T6 : struct, INativeList<byte>, IUTF8Bytes where T7 : struct, INativeList<byte>, IUTF8Bytes where T8 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref U reference = ref UnsafeUtilityExtensions.AsRef(in format);
			int length = reference.Length;
			byte* unsafePtr = reference.GetUnsafePtr();
			for (int i = 0; i < length; i++)
			{
				if (unsafePtr[i] == 123)
				{
					if (length - i >= 3 && unsafePtr[i + 1] != 123)
					{
						switch (unsafePtr[i + 1])
						{
						case 48:
							Append(ref dest, in arg0);
							i += 2;
							break;
						case 49:
							Append(ref dest, in arg1);
							i += 2;
							break;
						case 50:
							Append(ref dest, in arg2);
							i += 2;
							break;
						case 51:
							Append(ref dest, in arg3);
							i += 2;
							break;
						case 52:
							Append(ref dest, in arg4);
							i += 2;
							break;
						case 53:
							Append(ref dest, in arg5);
							i += 2;
							break;
						case 54:
							Append(ref dest, in arg6);
							i += 2;
							break;
						case 55:
							Append(ref dest, in arg7);
							i += 2;
							break;
						case 56:
							Append(ref dest, in arg8);
							i += 2;
							break;
						default:
							AppendRawByte(ref dest, unsafePtr[i]);
							break;
						}
					}
				}
				else
				{
					AppendRawByte(ref dest, unsafePtr[i]);
				}
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static void AppendFormat<T, U, T0, T1, T2, T3, T4, T5, T6, T7, T8, T9>(this ref T dest, in U format, in T0 arg0, in T1 arg1, in T2 arg2, in T3 arg3, in T4 arg4, in T5 arg5, in T6 arg6, in T7 arg7, in T8 arg8, in T9 arg9) where T : struct, INativeList<byte>, IUTF8Bytes where U : struct, INativeList<byte>, IUTF8Bytes where T0 : struct, INativeList<byte>, IUTF8Bytes where T1 : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes where T3 : struct, INativeList<byte>, IUTF8Bytes where T4 : struct, INativeList<byte>, IUTF8Bytes where T5 : struct, INativeList<byte>, IUTF8Bytes where T6 : struct, INativeList<byte>, IUTF8Bytes where T7 : struct, INativeList<byte>, IUTF8Bytes where T8 : struct, INativeList<byte>, IUTF8Bytes where T9 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref U reference = ref UnsafeUtilityExtensions.AsRef(in format);
			int length = reference.Length;
			byte* unsafePtr = reference.GetUnsafePtr();
			for (int i = 0; i < length; i++)
			{
				if (unsafePtr[i] == 123)
				{
					if (length - i >= 3 && unsafePtr[i + 1] != 123)
					{
						switch (unsafePtr[i + 1])
						{
						case 48:
							Append(ref dest, in arg0);
							i += 2;
							break;
						case 49:
							Append(ref dest, in arg1);
							i += 2;
							break;
						case 50:
							Append(ref dest, in arg2);
							i += 2;
							break;
						case 51:
							Append(ref dest, in arg3);
							i += 2;
							break;
						case 52:
							Append(ref dest, in arg4);
							i += 2;
							break;
						case 53:
							Append(ref dest, in arg5);
							i += 2;
							break;
						case 54:
							Append(ref dest, in arg6);
							i += 2;
							break;
						case 55:
							Append(ref dest, in arg7);
							i += 2;
							break;
						case 56:
							Append(ref dest, in arg8);
							i += 2;
							break;
						case 57:
							Append(ref dest, in arg9);
							i += 2;
							break;
						default:
							AppendRawByte(ref dest, unsafePtr[i]);
							break;
						}
					}
				}
				else
				{
					AppendRawByte(ref dest, unsafePtr[i]);
				}
			}
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		internal static FormatError Append<T>(this ref T fs, char a, char b) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			if ((FormatError.None | Append(ref fs, (Unicode.Rune)a) | Append(ref fs, (Unicode.Rune)b)) != 0)
			{
				return FormatError.Overflow;
			}
			return FormatError.None;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		internal static FormatError Append<T>(this ref T fs, char a, char b, char c) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			if ((FormatError.None | Append(ref fs, (Unicode.Rune)a) | Append(ref fs, (Unicode.Rune)b) | Append(ref fs, (Unicode.Rune)c)) != 0)
			{
				return FormatError.Overflow;
			}
			return FormatError.None;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		internal static FormatError Append<T>(this ref T fs, char a, char b, char c, char d, char e, char f, char g, char h) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			if ((FormatError.None | Append(ref fs, (Unicode.Rune)a) | Append(ref fs, (Unicode.Rune)b) | Append(ref fs, (Unicode.Rune)c) | Append(ref fs, (Unicode.Rune)d) | Append(ref fs, (Unicode.Rune)e) | Append(ref fs, (Unicode.Rune)f) | Append(ref fs, (Unicode.Rune)g) | Append(ref fs, (Unicode.Rune)h)) != 0)
			{
				return FormatError.Overflow;
			}
			return FormatError.None;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		internal unsafe static FormatError AppendScientific<T>(this ref T fs, char* source, int sourceLength, int decimalExponent, char decimalSeparator = '.') where T : struct, INativeList<byte>, IUTF8Bytes
		{
			FormatError result;
			if ((result = Append(ref fs, *source)) != 0)
			{
				return result;
			}
			if (sourceLength > 1)
			{
				if ((result = Append(ref fs, decimalSeparator)) != 0)
				{
					return result;
				}
				for (int i = 1; i < sourceLength; i++)
				{
					if ((result = Append(ref fs, source[i])) != 0)
					{
						return result;
					}
				}
			}
			if ((result = Append(ref fs, 'E')) != 0)
			{
				return result;
			}
			if (decimalExponent < 0)
			{
				if ((result = Append(ref fs, '-')) != 0)
				{
					return result;
				}
				decimalExponent *= -1;
				decimalExponent -= sourceLength - 1;
			}
			else
			{
				if ((result = Append(ref fs, '+')) != 0)
				{
					return result;
				}
				decimalExponent += sourceLength - 1;
			}
			char* ptr = stackalloc char[2];
			for (int j = 0; j < 2; j++)
			{
				int num = decimalExponent % 10;
				ptr[1 - j] = (char)(48 + num);
				decimalExponent /= 10;
			}
			for (int k = 0; k < 2; k++)
			{
				if ((result = Append(ref fs, ptr[k])) != 0)
				{
					return result;
				}
			}
			return FormatError.None;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		internal static bool Found<T>(this ref T fs, ref int offset, char a, char b, char c) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			int num = offset;
			if ((Read(ref fs, ref offset).value | 0x20) == a && (Read(ref fs, ref offset).value | 0x20) == b && (Read(ref fs, ref offset).value | 0x20) == c)
			{
				return true;
			}
			offset = num;
			return false;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		internal static bool Found<T>(this ref T fs, ref int offset, char a, char b, char c, char d, char e, char f, char g, char h) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			int num = offset;
			if ((Read(ref fs, ref offset).value | 0x20) == a && (Read(ref fs, ref offset).value | 0x20) == b && (Read(ref fs, ref offset).value | 0x20) == c && (Read(ref fs, ref offset).value | 0x20) == d && (Read(ref fs, ref offset).value | 0x20) == e && (Read(ref fs, ref offset).value | 0x20) == f && (Read(ref fs, ref offset).value | 0x20) == g && (Read(ref fs, ref offset).value | 0x20) == h)
			{
				return true;
			}
			offset = num;
			return false;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static int IndexOf<T>(this ref T fs, byte* bytes, int bytesLen) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			byte* unsafePtr = fs.GetUnsafePtr();
			int length = fs.Length;
			for (int i = 0; i <= length - bytesLen; i++)
			{
				int num = 0;
				while (true)
				{
					if (num < bytesLen)
					{
						if (unsafePtr[i + num] != bytes[num])
						{
							break;
						}
						num++;
						continue;
					}
					return i;
				}
			}
			return -1;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static int IndexOf<T>(this ref T fs, byte* bytes, int bytesLen, int startIndex, int distance = int.MaxValue) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			byte* unsafePtr = fs.GetUnsafePtr();
			int length = fs.Length;
			int num = Math.Min(distance - 1, length - bytesLen);
			for (int i = startIndex; i <= num; i++)
			{
				int num2 = 0;
				while (true)
				{
					if (num2 < bytesLen)
					{
						if (unsafePtr[i + num2] != bytes[num2])
						{
							break;
						}
						num2++;
						continue;
					}
					return i;
				}
			}
			return -1;
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static int IndexOf<T, T2>(this ref T fs, in T2 other) where T : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref T2 reference = ref UnsafeUtilityExtensions.AsRef(in other);
			return IndexOf(ref fs, reference.GetUnsafePtr(), reference.Length);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static int IndexOf<T, T2>(this ref T fs, in T2 other, int startIndex, int distance = int.MaxValue) where T : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref T2 reference = ref UnsafeUtilityExtensions.AsRef(in other);
			return IndexOf(ref fs, reference.GetUnsafePtr(), reference.Length, startIndex, distance);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public static bool Contains<T, T2>(this ref T fs, in T2 other) where T : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes
		{
			return IndexOf(ref fs, in other) != -1;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static int LastIndexOf<T>(this ref T fs, byte* bytes, int bytesLen) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			byte* unsafePtr = fs.GetUnsafePtr();
			for (int num = fs.Length - bytesLen; num >= 0; num--)
			{
				int num2 = 0;
				while (true)
				{
					if (num2 < bytesLen)
					{
						if (unsafePtr[num + num2] != bytes[num2])
						{
							break;
						}
						num2++;
						continue;
					}
					return num;
				}
			}
			return -1;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static int LastIndexOf<T>(this ref T fs, byte* bytes, int bytesLen, int startIndex, int distance = int.MaxValue) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			byte* unsafePtr = fs.GetUnsafePtr();
			startIndex = Math.Min(fs.Length - bytesLen, startIndex);
			int num = Math.Max(0, startIndex - distance);
			for (int num2 = startIndex; num2 >= num; num2--)
			{
				int num3 = 0;
				while (true)
				{
					if (num3 < bytesLen)
					{
						if (unsafePtr[num2 + num3] != bytes[num3])
						{
							break;
						}
						num3++;
						continue;
					}
					return num2;
				}
			}
			return -1;
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static int LastIndexOf<T, T2>(this ref T fs, in T2 other) where T : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref T2 reference = ref UnsafeUtilityExtensions.AsRef(in other);
			return LastIndexOf(ref fs, reference.GetUnsafePtr(), reference.Length);
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static int LastIndexOf<T, T2>(this ref T fs, in T2 other, int startIndex, int distance = int.MaxValue) where T : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref T2 reference = ref UnsafeUtilityExtensions.AsRef(in other);
			return LastIndexOf(ref fs, reference.GetUnsafePtr(), reference.Length, startIndex, distance);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static int CompareTo<T>(this ref T fs, byte* bytes, int bytesLen) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			byte* unsafePtr = fs.GetUnsafePtr();
			int length = fs.Length;
			int num = ((length < bytesLen) ? length : bytesLen);
			for (int i = 0; i < num; i++)
			{
				if (unsafePtr[i] < bytes[i])
				{
					return -1;
				}
				if (unsafePtr[i] > bytes[i])
				{
					return 1;
				}
			}
			if (length < bytesLen)
			{
				return -1;
			}
			if (length > bytesLen)
			{
				return 1;
			}
			return 0;
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static int CompareTo<T, T2>(this ref T fs, in T2 other) where T : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref T2 reference = ref UnsafeUtilityExtensions.AsRef(in other);
			return CompareTo(ref fs, reference.GetUnsafePtr(), reference.Length);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static bool Equals<T>(this ref T fs, byte* bytes, int bytesLen) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			byte* unsafePtr = fs.GetUnsafePtr();
			if (fs.Length != bytesLen)
			{
				return false;
			}
			if (unsafePtr == bytes)
			{
				return true;
			}
			return CompareTo(ref fs, bytes, bytesLen) == 0;
		}

		[BurstCompatible(GenericTypeArguments = new Type[]
		{
			typeof(FixedString128Bytes),
			typeof(FixedString128Bytes)
		})]
		public unsafe static bool Equals<T, T2>(this ref T fs, in T2 other) where T : struct, INativeList<byte>, IUTF8Bytes where T2 : struct, INativeList<byte>, IUTF8Bytes
		{
			ref T2 reference = ref UnsafeUtilityExtensions.AsRef(in other);
			return Equals(ref fs, reference.GetUnsafePtr(), reference.Length);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static Unicode.Rune Peek<T>(this ref T fs, int index) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			if (index >= fs.Length)
			{
				return Unicode.BadRune;
			}
			Unicode.Utf8ToUcs(out var rune, fs.GetUnsafePtr(), ref index, fs.Capacity);
			return rune;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static Unicode.Rune Read<T>(this ref T fs, ref int index) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			if (index >= fs.Length)
			{
				return Unicode.BadRune;
			}
			Unicode.Utf8ToUcs(out var rune, fs.GetUnsafePtr(), ref index, fs.Capacity);
			return rune;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static FormatError Write<T>(this ref T fs, ref int index, Unicode.Rune rune) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			if (Unicode.UcsToUtf8(fs.GetUnsafePtr(), ref index, fs.Capacity, rune) != 0)
			{
				return FormatError.Overflow;
			}
			return FormatError.None;
		}

		[NotBurstCompatible]
		public unsafe static string ConvertToString<T>(this ref T fs) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			char* ptr = stackalloc char[fs.Length * 2];
			int utf16Length = 0;
			Unicode.Utf8ToUtf16(fs.GetUnsafePtr(), fs.Length, ptr, out utf16Length, fs.Length * 2);
			return new string(ptr, 0, utf16Length);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public unsafe static int ComputeHashCode<T>(this ref T fs) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			return (int)CollectionHelper.Hash(fs.GetUnsafePtr(), fs.Length);
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public static int EffectiveSizeOf<T>(this ref T fs) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			return 2 + fs.Length + 1;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		internal static bool ParseLongInternal<T>(ref T fs, ref int offset, out long value) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			int num = offset;
			int num2 = 1;
			if (offset < fs.Length)
			{
				if (Peek(ref fs, offset).value == 43)
				{
					Read(ref fs, ref offset);
				}
				else if (Peek(ref fs, offset).value == 45)
				{
					num2 = -1;
					Read(ref fs, ref offset);
				}
			}
			int num3 = offset;
			value = 0L;
			while (offset < fs.Length && Unicode.Rune.IsDigit(Peek(ref fs, offset)))
			{
				value *= 10L;
				value += Read(ref fs, ref offset).value - 48;
			}
			value = num2 * value;
			if (offset == num3)
			{
				offset = num;
				return false;
			}
			return true;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public static ParseError Parse<T>(this ref T fs, ref int offset, ref int output) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			if (!ParseLongInternal(ref fs, ref offset, out var value))
			{
				return ParseError.Syntax;
			}
			if (value > int.MaxValue)
			{
				return ParseError.Overflow;
			}
			if (value < int.MinValue)
			{
				return ParseError.Overflow;
			}
			output = (int)value;
			return ParseError.None;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public static ParseError Parse<T>(this ref T fs, ref int offset, ref uint output) where T : struct, INativeList<byte>, IUTF8Bytes
		{
			if (!ParseLongInternal(ref fs, ref offset, out var value))
			{
				return ParseError.Syntax;
			}
			if (value > uint.MaxValue)
			{
				return ParseError.Overflow;
			}
			if (value < 0)
			{
				return ParseError.Overflow;
			}
			output = (uint)value;
			return ParseError.None;
		}

		[BurstCompatible(GenericTypeArguments = new Type[] { typeof(FixedString128Bytes) })]
		public static ParseError Parse<T>(this ref T fs, ref int offset, ref float output, char decimalSeparator = '.') where T : struct, INativeList<byte>, IUTF8Bytes
		{
			int num = offset;
			int num2 = 1;
			if (offset < fs.Length)
			{
				if (Peek(ref fs, offset).value == 43)
				{
					Read(ref fs, ref offset);
				}
				else if (Peek(ref fs, offset).value == 45)
				{
					num2 = -1;
					Read(ref fs, ref offset);
				}
			}
			if (Found(ref fs, ref offset, 'n', 'a', 'n'))
			{
				FixedStringUtils.UintFloatUnion uintFloatUnion = default(FixedStringUtils.UintFloatUnion);
				uintFloatUnion.uintValue = 4290772992u;
				output = uintFloatUnion.floatValue;
				return ParseError.None;
			}
			if (Found(ref fs, ref offset, 'i', 'n', 'f', 'i', 'n', 'i', 't', 'y'))
			{
				output = ((num2 == 1) ? float.PositiveInfinity : float.NegativeInfinity);
				return ParseError.None;
			}
			ulong num3 = 0uL;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			while (offset < fs.Length && Unicode.Rune.IsDigit(Peek(ref fs, offset)))
			{
				num6++;
				if (num4 < 9)
				{
					long num7 = (long)(num3 * 10) + (long)(Peek(ref fs, offset).value - 48);
					if ((ulong)num7 > num3)
					{
						num4++;
					}
					num3 = (ulong)num7;
				}
				else
				{
					num5--;
				}
				Read(ref fs, ref offset);
			}
			if (offset < fs.Length && Peek(ref fs, offset).value == decimalSeparator)
			{
				Read(ref fs, ref offset);
				while (offset < fs.Length && Unicode.Rune.IsDigit(Peek(ref fs, offset)))
				{
					num6++;
					if (num4 < 9)
					{
						long num8 = (long)(num3 * 10) + (long)(Peek(ref fs, offset).value - 48);
						if ((ulong)num8 > num3)
						{
							num4++;
						}
						num3 = (ulong)num8;
						num5++;
					}
					Read(ref fs, ref offset);
				}
			}
			if (num6 == 0)
			{
				offset = num;
				return ParseError.Syntax;
			}
			int num9 = 0;
			int num10 = 1;
			if (offset < fs.Length && (Peek(ref fs, offset).value | 0x20) == 101)
			{
				Read(ref fs, ref offset);
				if (offset < fs.Length)
				{
					if (Peek(ref fs, offset).value == 43)
					{
						Read(ref fs, ref offset);
					}
					else if (Peek(ref fs, offset).value == 45)
					{
						num10 = -1;
						Read(ref fs, ref offset);
					}
				}
				int num11 = offset;
				while (offset < fs.Length && Unicode.Rune.IsDigit(Peek(ref fs, offset)))
				{
					num9 = num9 * 10 + (Peek(ref fs, offset).value - 48);
					Read(ref fs, ref offset);
				}
				if (offset == num11)
				{
					offset = num;
					return ParseError.Syntax;
				}
				if (num9 > 38)
				{
					if (num10 == 1)
					{
						return ParseError.Overflow;
					}
					return ParseError.Underflow;
				}
			}
			num9 = num9 * num10 - num5;
			ParseError parseError = FixedStringUtils.Base10ToBase2(ref output, num3, num9);
			if (parseError != 0)
			{
				return parseError;
			}
			output *= num2;
			return ParseError.None;
		}
	}
}
