using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 64)]
	[Obsolete("FixedListInt64 is deprecated, please use FixedList64Bytes<int> instead. (UnityUpgradable) -> FixedList64Bytes<int>", true)]
	public struct FixedListInt64
	{
	}
}
