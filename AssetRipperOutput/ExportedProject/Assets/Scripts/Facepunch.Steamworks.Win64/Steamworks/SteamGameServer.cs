using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal static class SteamGameServer
	{
		internal static class Native
		{
			[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
			public static extern void SteamGameServer_RunCallbacks();

			[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
			public static extern void SteamGameServer_Shutdown();

			[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
			public static extern HSteamPipe SteamGameServer_GetHSteamPipe();
		}

		internal static void RunCallbacks()
		{
			Native.SteamGameServer_RunCallbacks();
		}

		internal static void Shutdown()
		{
			Native.SteamGameServer_Shutdown();
		}

		internal static HSteamPipe GetHSteamPipe()
		{
			return Native.SteamGameServer_GetHSteamPipe();
		}
	}
}
