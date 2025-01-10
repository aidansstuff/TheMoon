using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 4096)]
	[Obsolete("FixedListInt4096 is deprecated, please use FixedList4096Bytes<int> instead. (UnityUpgradable) -> FixedList4096Bytes<int>", true)]
	public struct FixedListInt4096
	{
	}
}
