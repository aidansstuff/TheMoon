using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 512)]
	[Obsolete("FixedListFloat512 is deprecated, please use FixedList512Bytes<float> instead. (UnityUpgradable) -> FixedList512Bytes<float>", true)]
	public struct FixedListFloat512
	{
	}
}
