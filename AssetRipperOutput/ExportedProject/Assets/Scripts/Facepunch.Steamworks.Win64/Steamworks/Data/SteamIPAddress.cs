using System;
using System.Net;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Explicit, Pack = 8)]
	internal struct SteamIPAddress
	{
		[FieldOffset(0)]
		public uint Ip4Address;

		[FieldOffset(16)]
		internal SteamIPType Type;

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamIPAddress_t_IsSet")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool InternalIsSet(ref SteamIPAddress self);

		public static implicit operator IPAddress(SteamIPAddress value)
		{
			if (value.Type == SteamIPType.Type4)
			{
				return Utility.Int32ToIp(value.Ip4Address);
			}
			throw new Exception($"Oops - can't convert SteamIPAddress to System.Net.IPAddress because no-one coded support for {value.Type} yet");
		}
	}
}
