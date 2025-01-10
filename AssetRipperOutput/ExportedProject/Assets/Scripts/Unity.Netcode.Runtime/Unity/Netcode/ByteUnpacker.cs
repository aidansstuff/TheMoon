using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Unity.Netcode
{
	public static class ByteUnpacker
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void ReadValuePacked<TEnum>(FastBufferReader reader, out TEnum value) where TEnum : unmanaged, Enum
		{
			switch (sizeof(TEnum))
			{
			case 4:
			{
				ReadValuePacked(reader, out int value5);
				value = *(TEnum*)(&value5);
				break;
			}
			case 1:
			{
				ReadValuePacked(reader, out byte value4);
				value = *(TEnum*)(&value4);
				break;
			}
			case 2:
			{
				ReadValuePacked(reader, out short value3);
				value = *(TEnum*)(&value3);
				break;
			}
			case 8:
			{
				ReadValuePacked(reader, out long value2);
				value = *(TEnum*)(&value2);
				break;
			}
			default:
				throw new InvalidOperationException("Enum is a size that cannot exist?!");
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReadValuePacked(FastBufferReader reader, out float value)
		{
			ReadValueBitPacked(reader, out uint value2);
			value = ToSingle(value2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReadValuePacked(FastBufferReader reader, out double value)
		{
			ReadValueBitPacked(reader, out ulong value2);
			value = ToDouble(value2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReadValuePacked(FastBufferReader reader, out byte value)
		{
			reader.ReadByteSafe(out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReadValuePacked(FastBufferReader reader, out sbyte value)
		{
			reader.ReadByteSafe(out var value2);
			value = (sbyte)value2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReadValuePacked(FastBufferReader reader, out bool value)
		{
			reader.ReadValueSafe(out value, default(FastBufferWriter.ForPrimitives));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReadValuePacked(FastBufferReader reader, out short value)
		{
			ReadValueBitPacked(reader, out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReadValuePacked(FastBufferReader reader, out ushort value)
		{
			ReadValueBitPacked(reader, out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReadValuePacked(FastBufferReader reader, out char c)
		{
			ReadValueBitPacked(reader, out ushort value);
			c = (char)value;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReadValuePacked(FastBufferReader reader, out int value)
		{
			ReadValueBitPacked(reader, out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReadValuePacked(FastBufferReader reader, out uint value)
		{
			ReadValueBitPacked(reader, out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReadValuePacked(FastBufferReader reader, out ulong value)
		{
			ReadValueBitPacked(reader, out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReadValuePacked(FastBufferReader reader, out long value)
		{
			ReadValueBitPacked(reader, out value);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReadValuePacked(FastBufferReader reader, out Ray ray)
		{
			ReadValuePacked(reader, out Vector3 vector);
			ReadValuePacked(reader, out Vector3 vector2);
			ray = new Ray(vector, vector2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReadValuePacked(FastBufferReader reader, out Ray2D ray2d)
		{
			ReadValuePacked(reader, out Vector2 vector);
			ReadValuePacked(reader, out Vector2 vector2);
			ray2d = new Ray2D(vector, vector2);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReadValuePacked(FastBufferReader reader, out Color color)
		{
			color = default(Color);
			ReadValuePacked(reader, out color.r);
			ReadValuePacked(reader, out color.g);
			ReadValuePacked(reader, out color.b);
			ReadValuePacked(reader, out color.a);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReadValuePacked(FastBufferReader reader, out Color32 color)
		{
			color = default(Color32);
			ReadValuePacked(reader, out color.r);
			ReadValuePacked(reader, out color.g);
			ReadValuePacked(reader, out color.b);
			ReadValuePacked(reader, out color.a);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReadValuePacked(FastBufferReader reader, out Vector2 vector2)
		{
			vector2 = default(Vector2);
			ReadValuePacked(reader, out vector2.x);
			ReadValuePacked(reader, out vector2.y);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReadValuePacked(FastBufferReader reader, out Vector3 vector3)
		{
			vector3 = default(Vector3);
			ReadValuePacked(reader, out vector3.x);
			ReadValuePacked(reader, out vector3.y);
			ReadValuePacked(reader, out vector3.z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReadValuePacked(FastBufferReader reader, out Vector4 vector4)
		{
			vector4 = default(Vector4);
			ReadValuePacked(reader, out vector4.x);
			ReadValuePacked(reader, out vector4.y);
			ReadValuePacked(reader, out vector4.z);
			ReadValuePacked(reader, out vector4.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void ReadValuePacked(FastBufferReader reader, out Quaternion rotation)
		{
			rotation = default(Quaternion);
			ReadValuePacked(reader, out rotation.x);
			ReadValuePacked(reader, out rotation.y);
			ReadValuePacked(reader, out rotation.z);
			ReadValuePacked(reader, out rotation.w);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static void ReadValuePacked(FastBufferReader reader, out string s)
		{
			ReadValuePacked(reader, out uint value);
			s = "".PadRight((int)value);
			int length = s.Length;
			fixed (char* ptr = s)
			{
				for (int i = 0; i < length; i++)
				{
					ReadValuePacked(reader, out ptr[i]);
				}
			}
		}

		public static void ReadValueBitPacked(FastBufferReader reader, out short value)
		{
			ReadValueBitPacked(reader, out ushort value2);
			value = (short)Arithmetic.ZigZagDecode(value2);
		}

		public unsafe static void ReadValueBitPacked(FastBufferReader reader, out ushort value)
		{
			ushort num = 0;
			byte* ptr = (byte*)(&num);
			byte* unsafePtrAtCurrentPosition = reader.GetUnsafePtrAtCurrentPosition();
			int num2 = *unsafePtrAtCurrentPosition & 3;
			if (!reader.TryBeginReadInternal(num2))
			{
				throw new OverflowException("Reading past the end of the buffer");
			}
			reader.MarkBytesRead(num2);
			switch (num2)
			{
			case 1:
				*ptr = *unsafePtrAtCurrentPosition;
				break;
			case 2:
				*ptr = *unsafePtrAtCurrentPosition;
				ptr[1] = unsafePtrAtCurrentPosition[1];
				break;
			case 3:
				*ptr = unsafePtrAtCurrentPosition[1];
				ptr[1] = unsafePtrAtCurrentPosition[2];
				value = num;
				return;
			default:
				throw new InvalidOperationException("Could not read bit-packed value: impossible byte count");
			}
			value = (ushort)(num >> 2);
		}

		public static void ReadValueBitPacked(FastBufferReader reader, out int value)
		{
			ReadValueBitPacked(reader, out uint value2);
			value = (int)Arithmetic.ZigZagDecode(value2);
		}

		public unsafe static void ReadValueBitPacked(FastBufferReader reader, out uint value)
		{
			uint num = 0u;
			byte* ptr = (byte*)(&num);
			byte* unsafePtrAtCurrentPosition = reader.GetUnsafePtrAtCurrentPosition();
			int num2 = *unsafePtrAtCurrentPosition & 7;
			if (!reader.TryBeginReadInternal(num2))
			{
				throw new OverflowException("Reading past the end of the buffer");
			}
			reader.MarkBytesRead(num2);
			switch (num2)
			{
			case 1:
				*ptr = *unsafePtrAtCurrentPosition;
				break;
			case 2:
				*ptr = *unsafePtrAtCurrentPosition;
				ptr[1] = unsafePtrAtCurrentPosition[1];
				break;
			case 3:
				*ptr = *unsafePtrAtCurrentPosition;
				ptr[1] = unsafePtrAtCurrentPosition[1];
				ptr[2] = unsafePtrAtCurrentPosition[2];
				break;
			case 4:
				*ptr = *unsafePtrAtCurrentPosition;
				ptr[1] = unsafePtrAtCurrentPosition[1];
				ptr[2] = unsafePtrAtCurrentPosition[2];
				ptr[3] = unsafePtrAtCurrentPosition[3];
				break;
			case 5:
				*ptr = unsafePtrAtCurrentPosition[1];
				ptr[1] = unsafePtrAtCurrentPosition[2];
				ptr[2] = unsafePtrAtCurrentPosition[3];
				ptr[3] = unsafePtrAtCurrentPosition[4];
				value = num;
				return;
			}
			value = num >> 3;
		}

		public static void ReadValueBitPacked(FastBufferReader reader, out long value)
		{
			ReadValueBitPacked(reader, out ulong value2);
			value = Arithmetic.ZigZagDecode(value2);
		}

		public unsafe static void ReadValueBitPacked(FastBufferReader reader, out ulong value)
		{
			ulong num = 0uL;
			byte* ptr = (byte*)(&num);
			byte* unsafePtrAtCurrentPosition = reader.GetUnsafePtrAtCurrentPosition();
			int num2 = *unsafePtrAtCurrentPosition & 0xF;
			if (!reader.TryBeginReadInternal(num2))
			{
				throw new OverflowException("Reading past the end of the buffer");
			}
			reader.MarkBytesRead(num2);
			switch (num2)
			{
			case 1:
				*ptr = *unsafePtrAtCurrentPosition;
				break;
			case 2:
				*ptr = *unsafePtrAtCurrentPosition;
				ptr[1] = unsafePtrAtCurrentPosition[1];
				break;
			case 3:
				*ptr = *unsafePtrAtCurrentPosition;
				ptr[1] = unsafePtrAtCurrentPosition[1];
				ptr[2] = unsafePtrAtCurrentPosition[2];
				break;
			case 4:
				*ptr = *unsafePtrAtCurrentPosition;
				ptr[1] = unsafePtrAtCurrentPosition[1];
				ptr[2] = unsafePtrAtCurrentPosition[2];
				ptr[3] = unsafePtrAtCurrentPosition[3];
				break;
			case 5:
				*ptr = *unsafePtrAtCurrentPosition;
				ptr[1] = unsafePtrAtCurrentPosition[1];
				ptr[2] = unsafePtrAtCurrentPosition[2];
				ptr[3] = unsafePtrAtCurrentPosition[3];
				ptr[4] = unsafePtrAtCurrentPosition[4];
				break;
			case 6:
				*ptr = *unsafePtrAtCurrentPosition;
				ptr[1] = unsafePtrAtCurrentPosition[1];
				ptr[2] = unsafePtrAtCurrentPosition[2];
				ptr[3] = unsafePtrAtCurrentPosition[3];
				ptr[4] = unsafePtrAtCurrentPosition[4];
				ptr[5] = unsafePtrAtCurrentPosition[5];
				break;
			case 7:
				*ptr = *unsafePtrAtCurrentPosition;
				ptr[1] = unsafePtrAtCurrentPosition[1];
				ptr[2] = unsafePtrAtCurrentPosition[2];
				ptr[3] = unsafePtrAtCurrentPosition[3];
				ptr[4] = unsafePtrAtCurrentPosition[4];
				ptr[5] = unsafePtrAtCurrentPosition[5];
				ptr[6] = unsafePtrAtCurrentPosition[6];
				break;
			case 8:
				*ptr = *unsafePtrAtCurrentPosition;
				ptr[1] = unsafePtrAtCurrentPosition[1];
				ptr[2] = unsafePtrAtCurrentPosition[2];
				ptr[3] = unsafePtrAtCurrentPosition[3];
				ptr[4] = unsafePtrAtCurrentPosition[4];
				ptr[5] = unsafePtrAtCurrentPosition[5];
				ptr[6] = unsafePtrAtCurrentPosition[6];
				ptr[7] = unsafePtrAtCurrentPosition[7];
				break;
			case 9:
				*ptr = unsafePtrAtCurrentPosition[1];
				ptr[1] = unsafePtrAtCurrentPosition[2];
				ptr[2] = unsafePtrAtCurrentPosition[3];
				ptr[3] = unsafePtrAtCurrentPosition[4];
				ptr[4] = unsafePtrAtCurrentPosition[5];
				ptr[5] = unsafePtrAtCurrentPosition[6];
				ptr[6] = unsafePtrAtCurrentPosition[7];
				ptr[7] = unsafePtrAtCurrentPosition[8];
				value = num;
				return;
			}
			value = num >> 4;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static float ToSingle<T>(T value) where T : unmanaged
		{
			float* ptr = (float*)(&value);
			return *ptr;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private unsafe static double ToDouble<T>(T value) where T : unmanaged
		{
			double* ptr = (double*)(&value);
			return *ptr;
		}
	}
}
