using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[Obsolete("Renamed to FixedList128Bytes<T> (UnityUpgradable) -> FixedList128Bytes<T>", true)]
	public struct FixedList128<T> where T : unmanaged
	{
	}
}
