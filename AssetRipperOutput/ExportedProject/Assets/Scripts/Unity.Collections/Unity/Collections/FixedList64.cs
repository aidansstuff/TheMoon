using System;
using System.Runtime.InteropServices;

namespace Unity.Collections
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	[Obsolete("Renamed to FixedList64Bytes<T> (UnityUpgradable) -> FixedList64Bytes<T>", true)]
	public struct FixedList64<T> where T : unmanaged
	{
	}
}
