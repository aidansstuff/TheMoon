using System.Runtime.InteropServices;

namespace Unity.Netcode
{
	[StructLayout(LayoutKind.Explicit)]
	internal struct UIntFloat
	{
		[FieldOffset(0)]
		public float FloatValue;

		[FieldOffset(0)]
		public uint UIntValue;

		[FieldOffset(0)]
		public double DoubleValue;

		[FieldOffset(0)]
		public ulong ULongValue;
	}
}
