using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 64)]
	[Obsolete("FixedListByte64 is deprecated, please use FixedList64Bytes<byte> instead. (UnityUpgradable) -> FixedList64Bytes<byte>", true)]
	public struct FixedListByte64
	{
	}
}
