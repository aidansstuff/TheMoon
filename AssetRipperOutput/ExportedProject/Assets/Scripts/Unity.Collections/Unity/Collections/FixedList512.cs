using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[Obsolete("Renamed to FixedList512Bytes<T> (UnityUpgradable) -> FixedList512Bytes<T>", true)]
	public struct FixedList512<T> where T : unmanaged
	{
	}
}
