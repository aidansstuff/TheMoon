using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 32)]
	[Obsolete("FixedListInt32 is deprecated, please use FixedList32Bytes<int> instead. (UnityUpgradable) -> FixedList32Bytes<int>", true)]
	public struct FixedListInt32
	{
	}
}
