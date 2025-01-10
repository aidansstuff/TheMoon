using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[Obsolete("Renamed to FixedList32Bytes<T> (UnityUpgradable) -> FixedList32Bytes<T>", true)]
	public struct FixedList32<T> where T : unmanaged
	{
	}
}
