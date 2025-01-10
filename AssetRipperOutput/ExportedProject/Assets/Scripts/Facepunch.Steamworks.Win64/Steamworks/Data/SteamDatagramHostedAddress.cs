using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SteamDatagramHostedAddress
	{
		internal int CbSize;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
		internal byte[] Data;

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamDatagramHostedAddress_Clear")]
		internal static extern void InternalClear(ref SteamDatagramHostedAddress self);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamDatagramHostedAddress_GetPopID")]
		internal static extern SteamNetworkingPOPID InternalGetPopID(ref SteamDatagramHostedAddress self);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamDatagramHostedAddress_SetDevAddress")]
		internal static extern void InternalSetDevAddress(ref SteamDatagramHostedAddress self, uint nIP, ushort nPort, SteamNetworkingPOPID popid);

		internal string DataUTF8()
		{
			return Encoding.UTF8.GetString(Data, 0, Array.IndexOf(Data, (byte)0));
		}
	}
}
