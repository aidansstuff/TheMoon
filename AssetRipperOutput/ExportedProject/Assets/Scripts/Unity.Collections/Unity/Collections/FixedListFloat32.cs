using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[Serializable]
	[StructLayout(LayoutKind.Explicit, Size = 32)]
	[Obsolete("FixedListFloat32 is deprecated, please use FixedList32Bytes<float> instead. (UnityUpgradable) -> FixedList32Bytes<float>", true)]
	public struct FixedListFloat32
	{
	}
}
