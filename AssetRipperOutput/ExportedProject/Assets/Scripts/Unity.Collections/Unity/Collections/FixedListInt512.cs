using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 512)]
	[Obsolete("FixedListInt512 is deprecated, please use FixedList512Bytes<int> instead. (UnityUpgradable) -> FixedList512Bytes<int>", true)]
	public struct FixedListInt512
	{
	}
}
