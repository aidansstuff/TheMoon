using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	internal class ISteamAppList : SteamInterface
	{
		internal ISteamAppList(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamAppList_v001();

		public override IntPtr GetUserInterfacePointer()
		{
			return SteamAPI_SteamAppList_v001();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamAppList_GetNumInstalledApps")]
		private static extern uint _GetNumInstalledApps(IntPtr self);

		internal uint GetNumInstalledApps()
		{
			return _GetNumInstalledApps(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamAppList_GetInstalledApps")]
		private static extern uint _GetInstalledApps(IntPtr self, [In][Out] AppId[] pvecAppID, uint unMaxAppIDs);

		internal uint GetInstalledApps([In][Out] AppId[] pvecAppID, uint unMaxAppIDs)
		{
			return _GetInstalledApps(Self, pvecAppID, unMaxAppIDs);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamAppList_GetAppName")]
		private static extern int _GetAppName(IntPtr self, AppId nAppID, IntPtr pchName, int cchNameMax);

		internal int GetAppName(AppId nAppID, out string pchName)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			int result = _GetAppName(Self, nAppID, intPtr, 32768);
			pchName = Helpers.MemoryToString(intPtr);
			return result;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamAppList_GetAppInstallDir")]
		private static extern int _GetAppInstallDir(IntPtr self, AppId nAppID, IntPtr pchDirectory, int cchNameMax);

		internal int GetAppInstallDir(AppId nAppID, out string pchDirectory)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			int result = _GetAppInstallDir(Self, nAppID, intPtr, 32768);
			pchDirectory = Helpers.MemoryToString(intPtr);
			return result;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamAppList_GetAppBuildId")]
		private static extern int _GetAppBuildId(IntPtr self, AppId nAppID);

		internal int GetAppBuildId(AppId nAppID)
		{
			return _GetAppBuildId(Self, nAppID);
		}
	}
}
