using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal class ISteamApps : SteamInterface
	{
		internal ISteamApps(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamApps_v008();

		public override IntPtr GetUserInterfacePointer()
		{
			return SteamAPI_SteamApps_v008();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamGameServerApps_v008();

		public override IntPtr GetServerInterfacePointer()
		{
			return SteamAPI_SteamGameServerApps_v008();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_BIsSubscribed")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BIsSubscribed(IntPtr self);

		internal bool BIsSubscribed()
		{
			return _BIsSubscribed(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_BIsLowViolence")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BIsLowViolence(IntPtr self);

		internal bool BIsLowViolence()
		{
			return _BIsLowViolence(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_BIsCybercafe")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BIsCybercafe(IntPtr self);

		internal bool BIsCybercafe()
		{
			return _BIsCybercafe(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_BIsVACBanned")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BIsVACBanned(IntPtr self);

		internal bool BIsVACBanned()
		{
			return _BIsVACBanned(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_GetCurrentGameLanguage")]
		private static extern Utf8StringPointer _GetCurrentGameLanguage(IntPtr self);

		internal string GetCurrentGameLanguage()
		{
			Utf8StringPointer utf8StringPointer = _GetCurrentGameLanguage(Self);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_GetAvailableGameLanguages")]
		private static extern Utf8StringPointer _GetAvailableGameLanguages(IntPtr self);

		internal string GetAvailableGameLanguages()
		{
			Utf8StringPointer utf8StringPointer = _GetAvailableGameLanguages(Self);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_BIsSubscribedApp")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BIsSubscribedApp(IntPtr self, AppId appID);

		internal bool BIsSubscribedApp(AppId appID)
		{
			return _BIsSubscribedApp(Self, appID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_BIsDlcInstalled")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BIsDlcInstalled(IntPtr self, AppId appID);

		internal bool BIsDlcInstalled(AppId appID)
		{
			return _BIsDlcInstalled(Self, appID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_GetEarliestPurchaseUnixTime")]
		private static extern uint _GetEarliestPurchaseUnixTime(IntPtr self, AppId nAppID);

		internal uint GetEarliestPurchaseUnixTime(AppId nAppID)
		{
			return _GetEarliestPurchaseUnixTime(Self, nAppID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_BIsSubscribedFromFreeWeekend")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BIsSubscribedFromFreeWeekend(IntPtr self);

		internal bool BIsSubscribedFromFreeWeekend()
		{
			return _BIsSubscribedFromFreeWeekend(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_GetDLCCount")]
		private static extern int _GetDLCCount(IntPtr self);

		internal int GetDLCCount()
		{
			return _GetDLCCount(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_BGetDLCDataByIndex")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BGetDLCDataByIndex(IntPtr self, int iDLC, ref AppId pAppID, [MarshalAs(UnmanagedType.U1)] ref bool pbAvailable, IntPtr pchName, int cchNameBufferSize);

		internal bool BGetDLCDataByIndex(int iDLC, ref AppId pAppID, [MarshalAs(UnmanagedType.U1)] ref bool pbAvailable, out string pchName)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			bool result = _BGetDLCDataByIndex(Self, iDLC, ref pAppID, ref pbAvailable, intPtr, 32768);
			pchName = Helpers.MemoryToString(intPtr);
			return result;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_InstallDLC")]
		private static extern void _InstallDLC(IntPtr self, AppId nAppID);

		internal void InstallDLC(AppId nAppID)
		{
			_InstallDLC(Self, nAppID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_UninstallDLC")]
		private static extern void _UninstallDLC(IntPtr self, AppId nAppID);

		internal void UninstallDLC(AppId nAppID)
		{
			_UninstallDLC(Self, nAppID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_RequestAppProofOfPurchaseKey")]
		private static extern void _RequestAppProofOfPurchaseKey(IntPtr self, AppId nAppID);

		internal void RequestAppProofOfPurchaseKey(AppId nAppID)
		{
			_RequestAppProofOfPurchaseKey(Self, nAppID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_GetCurrentBetaName")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetCurrentBetaName(IntPtr self, IntPtr pchName, int cchNameBufferSize);

		internal bool GetCurrentBetaName(out string pchName)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			bool result = _GetCurrentBetaName(Self, intPtr, 32768);
			pchName = Helpers.MemoryToString(intPtr);
			return result;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_MarkContentCorrupt")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _MarkContentCorrupt(IntPtr self, [MarshalAs(UnmanagedType.U1)] bool bMissingFilesOnly);

		internal bool MarkContentCorrupt([MarshalAs(UnmanagedType.U1)] bool bMissingFilesOnly)
		{
			return _MarkContentCorrupt(Self, bMissingFilesOnly);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_GetInstalledDepots")]
		private static extern uint _GetInstalledDepots(IntPtr self, AppId appID, [In][Out] DepotId_t[] pvecDepots, uint cMaxDepots);

		internal uint GetInstalledDepots(AppId appID, [In][Out] DepotId_t[] pvecDepots, uint cMaxDepots)
		{
			return _GetInstalledDepots(Self, appID, pvecDepots, cMaxDepots);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_GetAppInstallDir")]
		private static extern uint _GetAppInstallDir(IntPtr self, AppId appID, IntPtr pchFolder, uint cchFolderBufferSize);

		internal uint GetAppInstallDir(AppId appID, out string pchFolder)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			uint result = _GetAppInstallDir(Self, appID, intPtr, 32768u);
			pchFolder = Helpers.MemoryToString(intPtr);
			return result;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_BIsAppInstalled")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BIsAppInstalled(IntPtr self, AppId appID);

		internal bool BIsAppInstalled(AppId appID)
		{
			return _BIsAppInstalled(Self, appID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_GetAppOwner")]
		private static extern SteamId _GetAppOwner(IntPtr self);

		internal SteamId GetAppOwner()
		{
			return _GetAppOwner(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_GetLaunchQueryParam")]
		private static extern Utf8StringPointer _GetLaunchQueryParam(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchKey);

		internal string GetLaunchQueryParam([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchKey)
		{
			Utf8StringPointer utf8StringPointer = _GetLaunchQueryParam(Self, pchKey);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_GetDlcDownloadProgress")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetDlcDownloadProgress(IntPtr self, AppId nAppID, ref ulong punBytesDownloaded, ref ulong punBytesTotal);

		internal bool GetDlcDownloadProgress(AppId nAppID, ref ulong punBytesDownloaded, ref ulong punBytesTotal)
		{
			return _GetDlcDownloadProgress(Self, nAppID, ref punBytesDownloaded, ref punBytesTotal);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_GetAppBuildId")]
		private static extern int _GetAppBuildId(IntPtr self);

		internal int GetAppBuildId()
		{
			return _GetAppBuildId(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_RequestAllProofOfPurchaseKeys")]
		private static extern void _RequestAllProofOfPurchaseKeys(IntPtr self);

		internal void RequestAllProofOfPurchaseKeys()
		{
			_RequestAllProofOfPurchaseKeys(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_GetFileDetails")]
		private static extern SteamAPICall_t _GetFileDetails(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszFileName);

		internal CallResult<FileDetailsResult_t> GetFileDetails([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszFileName)
		{
			SteamAPICall_t call = _GetFileDetails(Self, pszFileName);
			return new CallResult<FileDetailsResult_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_GetLaunchCommandLine")]
		private static extern int _GetLaunchCommandLine(IntPtr self, IntPtr pszCommandLine, int cubCommandLine);

		internal int GetLaunchCommandLine(out string pszCommandLine)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			int result = _GetLaunchCommandLine(Self, intPtr, 32768);
			pszCommandLine = Helpers.MemoryToString(intPtr);
			return result;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamApps_BIsSubscribedFromFamilySharing")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BIsSubscribedFromFamilySharing(IntPtr self);

		internal bool BIsSubscribedFromFamilySharing()
		{
			return _BIsSubscribedFromFamilySharing(Self);
		}
	}
}
