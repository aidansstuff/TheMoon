using System.Runtime.InteropServices;

namespace Unity.Multiplayer.Tools.NetStats
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct ByteBool
	{
		[FieldOffset(0)]
		public bool BoolValue;

		[FieldOffset(0)]
		public byte ByteValue;

		public byte Collapse()
		{
			return ByteValue = (byte)((uint)((ByteValue >> 7) | (ByteValue >> 6) | (ByteValue >> 5) | (ByteValue >> 4) | (ByteValue >> 3) | (ByteValue >> 2) | (ByteValue >> 1) | ByteValue) & 1u);
		}

		public byte Collapse(bool b)
		{
			BoolValue = b;
			return Collapse();
		}
	}
}
