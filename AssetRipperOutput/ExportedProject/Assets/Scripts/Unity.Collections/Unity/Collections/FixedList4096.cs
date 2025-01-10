using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[Obsolete("Renamed to FixedList4096Bytes<T> (UnityUpgradable) -> FixedList4096Bytes<T>", true)]
	public struct FixedList4096<T> where T : unmanaged
	{
	}
}
