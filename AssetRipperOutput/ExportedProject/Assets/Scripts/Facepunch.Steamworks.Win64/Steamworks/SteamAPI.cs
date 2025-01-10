using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal static class SteamAPI
	{
		internal static class Native
		{
			[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
			[return: MarshalAs(UnmanagedType.I1)]
			public static extern bool SteamAPI_Init();

			[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
			public static extern void SteamAPI_Shutdown();

			[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
			public static extern HSteamPipe SteamAPI_GetHSteamPipe();

			[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
			[return: MarshalAs(UnmanagedType.I1)]
			public static extern bool SteamAPI_RestartAppIfNecessary(uint unOwnAppID);
		}

		internal static bool Init()
		{
			return Native.SteamAPI_Init();
		}

		internal static void Shutdown()
		{
			Native.SteamAPI_Shutdown();
		}

		internal static HSteamPipe GetHSteamPipe()
		{
			return Native.SteamAPI_GetHSteamPipe();
		}

		internal static bool RestartAppIfNecessary(uint unOwnAppID)
		{
			return Native.SteamAPI_RestartAppIfNecessary(unOwnAppID);
		}
	}
}
