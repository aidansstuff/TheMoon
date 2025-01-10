using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Unity.Netcode
{
	public static class BytePacker
	{
		public const ushort BitPackedUshortMax = 32767;

		public const short BitPackedShortMax = 16383;

		public const short BitPackedShortMin = -16384;

		public const uint BitPackedUintMax = 1073741823u;

		public const int BitPackedIntMax = 536870911;

		public const int BitPackedIntMin = -536870912;

		public const ulong BitPackedULongMax = 2305843009213693951uL;

		public const long BitPackedLongMax = 1152921504606846975L;

		public const long BitPackedLongMin = -1152921504606846976L;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void WriteValuePacked<TEnum>(FastBufferWriter writer, TEnum value) where TEnum : unmanaged, Enum
		{
			TEnum val = value;
			switch (sizeof(TEnum))
			{
			case 4:
				WriteValuePacked(writer, *(int*)(&val));
				break;
			case 1:
				WriteValuePacked(writer, *(byte*)(&val));
				break;
			case 2:
				WriteValuePacked(writer, *(short*)(&val));
				break;
			case 8:
				WriteValuePacked(writer, *(long*)(&val));
				break;
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteValuePacked(FastBufferWriter writer, float value)
		{
			WriteValueBitPacked(writer, ToUint(value));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteValuePacked(FastBufferWriter writer, double value)
		{
			WriteValueBitPacked(writer, ToUlong(value));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteValuePacked(FastBufferWriter writer, byte value)
		{
			writer.WriteByteSafe(value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteValuePacked(FastBufferWriter writer, sbyte value)
		{
			writer.WriteByteSafe((byte)value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteValuePacked(FastBufferWriter writer, bool value)
		{
			writer.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteValuePacked(FastBufferWriter writer, short value)
		{
			WriteValueBitPacked(writer, value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteValuePacked(FastBufferWriter writer, ushort value)
		{
			WriteValueBitPacked(writer, value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteValuePacked(FastBufferWriter writer, char c)
		{
			WriteValueBitPacked(writer, c);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteValuePacked(FastBufferWriter writer, int value)
		{
			WriteValueBitPacked(writer, value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteValuePacked(FastBufferWriter writer, uint value)
		{
			WriteValueBitPacked(writer, value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteValuePacked(FastBufferWriter writer, ulong value)
		{
			WriteValueBitPacked(writer, value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteValuePacked(FastBufferWriter writer, long value)
		{
			WriteValueBitPacked(writer, value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteValuePacked(FastBufferWriter writer, Ray ray)
		{
			WriteValuePacked(writer, ray.origin);
			WriteValuePacked(writer, ray.direction);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteValuePacked(FastBufferWriter writer, Ray2D ray2d)
		{
			WriteValuePacked(writer, ray2d.origin);
			WriteValuePacked(writer, ray2d.direction);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteValuePacked(FastBufferWriter writer, Color color)
		{
			WriteValuePacked(writer, color.r);
			WriteValuePacked(writer, color.g);
			WriteValuePacked(writer, color.b);
			WriteValuePacked(writer, color.a);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteValuePacked(FastBufferWriter writer, Color32 color)
		{
			WriteValuePacked(writer, color.r);
			WriteValuePacked(writer, color.g);
			WriteValuePacked(writer, color.b);
			WriteValuePacked(writer, color.a);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteValuePacked(FastBufferWriter writer, Vector2 vector2)
		{
			WriteValuePacked(writer, vector2.x);
			WriteValuePacked(writer, vector2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteValuePacked(FastBufferWriter writer, Vector3 vector3)
		{
			WriteValuePacked(writer, vector3.x);
			WriteValuePacked(writer, vector3.y);
			WriteValuePacked(writer, vector3.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteValuePacked(FastBufferWriter writer, Vector4 vector4)
		{
			WriteValuePacked(writer, vector4.x);
			WriteValuePacked(writer, vector4.y);
			WriteValuePacked(writer, vector4.z);
			WriteValuePacked(writer, vector4.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteValuePacked(FastBufferWriter writer, Quaternion rotation)
		{
			WriteValuePacked(writer, rotation.x);
			WriteValuePacked(writer, rotation.y);
			WriteValuePacked(writer, rotation.z);
			WriteValuePacked(writer, rotation.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void WriteValuePacked(FastBufferWriter writer, string s)
		{
			WriteValuePacked(writer, (uint)s.Length);
			int length = s.Length;
			for (int i = 0; i < length; i++)
			{
				WriteValuePacked(writer, s[i]);
			}
		}

		public static void WriteValueBitPacked(FastBufferWriter writer, short value)
		{
			WriteValueBitPacked(writer, (ushort)Arithmetic.ZigZagEncode(value));
		}

		public static void WriteValueBitPacked(FastBufferWriter writer, ushort value)
		{
			if (value > 16383)
			{
				if (!writer.TryBeginWriteInternal(3))
				{
					throw new OverflowException("Writing past the end of the buffer");
				}
				writer.WriteByte(3);
				writer.WriteValue(in value, default(FastBufferWriter.ForPrimitives));
				return;
			}
			value <<= 2;
			int usedByteCount = BitCounter.GetUsedByteCount(value);
			if (!writer.TryBeginWriteInternal(usedByteCount))
			{
				throw new OverflowException("Writing past the end of the buffer");
			}
			writer.WritePartialValue(value | (ushort)usedByteCount, usedByteCount);
		}

		public static void WriteValueBitPacked(FastBufferWriter writer, int value)
		{
			WriteValueBitPacked(writer, (uint)Arithmetic.ZigZagEncode(value));
		}

		public static void WriteValueBitPacked(FastBufferWriter writer, uint value)
		{
			if (value > 536870911)
			{
				if (!writer.TryBeginWriteInternal(5))
				{
					throw new OverflowException("Writing past the end of the buffer");
				}
				writer.WriteByte(5);
				writer.WriteValue(in value, default(FastBufferWriter.ForPrimitives));
				return;
			}
			value <<= 3;
			int usedByteCount = BitCounter.GetUsedByteCount(value);
			if (!writer.TryBeginWriteInternal(usedByteCount))
			{
				throw new OverflowException("Writing past the end of the buffer");
			}
			writer.WritePartialValue(value | (uint)usedByteCount, usedByteCount);
		}

		public static void WriteValueBitPacked(FastBufferWriter writer, long value)
		{
			WriteValueBitPacked(writer, Arithmetic.ZigZagEncode(value));
		}

		public static void WriteValueBitPacked(FastBufferWriter writer, ulong value)
		{
			if (value > 1152921504606846975L)
			{
				if (!writer.TryBeginWriteInternal(9))
				{
					throw new OverflowException("Writing past the end of the buffer");
				}
				writer.WriteByte(9);
				writer.WriteValue(in value, default(FastBufferWriter.ForPrimitives));
				return;
			}
			value <<= 4;
			int usedByteCount = BitCounter.GetUsedByteCount(value);
			if (!writer.TryBeginWriteInternal(usedByteCount))
			{
				throw new OverflowException("Writing past the end of the buffer");
			}
			writer.WritePartialValue(value | (uint)usedByteCount, usedByteCount);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static uint ToUint<T>(T value) where T : unmanaged
		{
			uint* ptr = (uint*)(&value);
			return *ptr;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static ulong ToUlong<T>(T value) where T : unmanaged
		{
			ulong* ptr = (ulong*)(&value);
			return *ptr;
		}
	}
}
