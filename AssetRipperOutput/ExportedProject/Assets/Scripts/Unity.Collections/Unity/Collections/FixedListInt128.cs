using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 128)]
	[Obsolete("FixedListInt128 is deprecated, please use FixedList128Bytes<int> instead. (UnityUpgradable) -> FixedList128Bytes<int>", true)]
	public struct FixedListInt128
	{
	}
}
