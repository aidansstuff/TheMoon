using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace Unity.Netcode
{
	internal static class XXHash
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static uint Hash32(byte* input, int length, uint seed = 0u)
		{
			uint num = seed + 374761393;
			if (length >= 16)
			{
				uint num2 = (uint)((int)seed + -1640531535 + -2048144777);
				uint num3 = seed + 2246822519u;
				uint num4 = seed;
				uint num5 = seed - 2654435761u;
				int num6 = length >> 4;
				for (int i = 0; i < num6; i++)
				{
					uint num7 = *(uint*)input;
					uint num8 = *(uint*)(input + 4);
					uint num9 = *(uint*)(input + 8);
					uint num10 = *(uint*)(input + 12);
					num2 += (uint)((int)num7 * -2048144777);
					num2 = (num2 << 13) | (num2 >> 19);
					num2 *= 2654435761u;
					num3 += (uint)((int)num8 * -2048144777);
					num3 = (num3 << 13) | (num3 >> 19);
					num3 *= 2654435761u;
					num4 += (uint)((int)num9 * -2048144777);
					num4 = (num4 << 13) | (num4 >> 19);
					num4 *= 2654435761u;
					num5 += (uint)((int)num10 * -2048144777);
					num5 = (num5 << 13) | (num5 >> 19);
					num5 *= 2654435761u;
					input += 16;
				}
				num = ((num2 << 1) | (num2 >> 31)) + ((num3 << 7) | (num3 >> 25)) + ((num4 << 12) | (num4 >> 20)) + ((num5 << 18) | (num5 >> 14));
			}
			num += (uint)length;
			for (length &= 0xF; length >= 4; length -= 4)
			{
				num += (uint)((int)(*(uint*)input) * -1028477379);
				num = ((num << 17) | (num >> 15)) * 668265263;
				input += 4;
			}
			while (length > 0)
			{
				num += (uint)(*input * 374761393);
				num = ((num << 11) | (num >> 21)) * 2654435761u;
				input++;
				length--;
			}
			num ^= num >> 15;
			num *= 2246822519u;
			num ^= num >> 13;
			num *= 3266489917u;
			return num ^ (num >> 16);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static ulong Hash64(byte* input, int length, uint seed = 0u)
		{
			ulong num = (ulong)seed + 2870177450012600261uL;
			if (length >= 32)
			{
				ulong num2 = (ulong)(seed + -7046029288634856825L + -4417276706812531889L);
				ulong num3 = (ulong)(seed + -4417276706812531889L);
				ulong num4 = seed;
				ulong num5 = (ulong)(seed - -7046029288634856825L);
				int num6 = length >> 5;
				for (int i = 0; i < num6; i++)
				{
					ulong num7 = *(ulong*)input;
					ulong num8 = *(ulong*)(input + 8);
					ulong num9 = *(ulong*)(input + 16);
					ulong num10 = *(ulong*)(input + 24);
					num2 += (ulong)((long)num7 * -4417276706812531889L);
					num2 = (num2 << 31) | (num2 >> 33);
					num2 *= 11400714785074694791uL;
					num3 += (ulong)((long)num8 * -4417276706812531889L);
					num3 = (num3 << 31) | (num3 >> 33);
					num3 *= 11400714785074694791uL;
					num4 += (ulong)((long)num9 * -4417276706812531889L);
					num4 = (num4 << 31) | (num4 >> 33);
					num4 *= 11400714785074694791uL;
					num5 += (ulong)((long)num10 * -4417276706812531889L);
					num5 = (num5 << 31) | (num5 >> 33);
					num5 *= 11400714785074694791uL;
					input += 32;
				}
				num = ((num2 << 1) | (num2 >> 63)) + ((num3 << 7) | (num3 >> 57)) + ((num4 << 12) | (num4 >> 52)) + ((num5 << 18) | (num5 >> 46));
				num2 *= 14029467366897019727uL;
				num2 = (num2 << 31) | (num2 >> 33);
				num2 *= 11400714785074694791uL;
				num ^= num2;
				num = (ulong)((long)num * -7046029288634856825L + -8796714831421723037L);
				num3 *= 14029467366897019727uL;
				num3 = (num3 << 31) | (num3 >> 33);
				num3 *= 11400714785074694791uL;
				num ^= num3;
				num = (ulong)((long)num * -7046029288634856825L + -8796714831421723037L);
				num4 *= 14029467366897019727uL;
				num4 = (num4 << 31) | (num4 >> 33);
				num4 *= 11400714785074694791uL;
				num ^= num4;
				num = (ulong)((long)num * -7046029288634856825L + -8796714831421723037L);
				num5 *= 14029467366897019727uL;
				num5 = (num5 << 31) | (num5 >> 33);
				num5 *= 11400714785074694791uL;
				num ^= num5;
				num = (ulong)((long)num * -7046029288634856825L + -8796714831421723037L);
			}
			num += (ulong)length;
			for (length &= 0x1F; length >= 8; length -= 8)
			{
				ulong num11 = (ulong)(*(long*)input * -4417276706812531889L);
				num11 = ((num11 << 31) | (num11 >> 33)) * 11400714785074694791uL;
				num ^= num11;
				num = (ulong)((long)((num << 27) | (num >> 37)) * -7046029288634856825L + -8796714831421723037L);
				input += 8;
			}
			if (length >= 4)
			{
				num ^= (ulong)((uint)(*(int*)input) * -7046029288634856825L);
				num = (ulong)((long)((num << 23) | (num >> 41)) * -4417276706812531889L + 1609587929392839161L);
				input += 4;
				length -= 4;
			}
			while (length > 0)
			{
				num ^= (ulong)(*input * 2870177450012600261L);
				num = ((num << 11) | (num >> 53)) * 11400714785074694791uL;
				input++;
				length--;
			}
			num ^= num >> 33;
			num *= 14029467366897019727uL;
			num ^= num >> 29;
			num *= 1609587929392839161L;
			return num ^ (num >> 32);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static uint Hash32(this byte[] buffer)
		{
			int length = buffer.Length;
			fixed (byte* input = buffer)
			{
				return Hash32(input, length);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint Hash32(this string text)
		{
			return Encoding.UTF8.GetBytes(text).Hash32();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint Hash32(this Type type)
		{
			return type.FullName.Hash32();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static uint Hash32<T>()
		{
			return typeof(T).Hash32();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public unsafe static ulong Hash64(this byte[] buffer)
		{
			int length = buffer.Length;
			fixed (byte* input = buffer)
			{
				return Hash64(input, length);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong Hash64(this string text)
		{
			return Encoding.UTF8.GetBytes(text).Hash64();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong Hash64(this Type type)
		{
			return type.FullName.Hash64();
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static ulong Hash64<T>()
		{
			return typeof(T).Hash64();
		}
	}
}
