using System.Runtime.CompilerServices;

namespace Unity.Netcode
{
	internal class ByteUtility
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static byte ToByte(bool b)
		{
			return b ? ((byte)1) : ((byte)0);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool GetBit(byte bitField, ushort bitPosition)
		{
			return (bitField & (1 << (int)bitPosition)) != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void SetBit(ref byte bitField, ushort bitPosition, bool value)
		{
			bitField = (byte)((bitField & ~(1 << (int)bitPosition)) | (ToByte(value) << (int)bitPosition));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool GetBit(ushort bitField, ushort bitPosition)
		{
			return (bitField & (1 << (int)bitPosition)) != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void SetBit(ref ushort bitField, ushort bitPosition, bool value)
		{
			bitField = (ushort)((bitField & ~(1 << (int)bitPosition)) | (ToByte(value) << (int)bitPosition));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool GetBit(uint bitField, ushort bitPosition)
		{
			return (bitField & (1 << (int)bitPosition)) != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void SetBit(ref uint bitField, ushort bitPosition, bool value)
		{
			bitField = (uint)((bitField & ~(1 << (int)bitPosition)) | (uint)(ToByte(value) << (int)bitPosition));
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static bool GetBit(ulong bitField, ushort bitPosition)
		{
			return (bitField & (ulong)(1 << (int)bitPosition)) != 0;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal static void SetBit(ref ulong bitField, ushort bitPosition, bool value)
		{
			bitField = (bitField & (ulong)(~(1 << (int)bitPosition))) | ((ulong)ToByte(value) << (int)bitPosition);
		}
	}
}
