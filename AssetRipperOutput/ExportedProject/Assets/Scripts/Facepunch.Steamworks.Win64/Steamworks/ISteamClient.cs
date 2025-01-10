using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal class ISteamClient : SteamInterface
	{
		internal ISteamClient(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_CreateSteamPipe")]
		private static extern HSteamPipe _CreateSteamPipe(IntPtr self);

		internal HSteamPipe CreateSteamPipe()
		{
			return _CreateSteamPipe(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_BReleaseSteamPipe")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BReleaseSteamPipe(IntPtr self, HSteamPipe hSteamPipe);

		internal bool BReleaseSteamPipe(HSteamPipe hSteamPipe)
		{
			return _BReleaseSteamPipe(Self, hSteamPipe);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_ConnectToGlobalUser")]
		private static extern HSteamUser _ConnectToGlobalUser(IntPtr self, HSteamPipe hSteamPipe);

		internal HSteamUser ConnectToGlobalUser(HSteamPipe hSteamPipe)
		{
			return _ConnectToGlobalUser(Self, hSteamPipe);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_CreateLocalUser")]
		private static extern HSteamUser _CreateLocalUser(IntPtr self, ref HSteamPipe phSteamPipe, AccountType eAccountType);

		internal HSteamUser CreateLocalUser(ref HSteamPipe phSteamPipe, AccountType eAccountType)
		{
			return _CreateLocalUser(Self, ref phSteamPipe, eAccountType);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_ReleaseUser")]
		private static extern void _ReleaseUser(IntPtr self, HSteamPipe hSteamPipe, HSteamUser hUser);

		internal void ReleaseUser(HSteamPipe hSteamPipe, HSteamUser hUser)
		{
			_ReleaseUser(Self, hSteamPipe, hUser);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamUser")]
		private static extern IntPtr _GetISteamUser(IntPtr self, HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamUser(HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamUser(Self, hSteamUser, hSteamPipe, pchVersion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamGameServer")]
		private static extern IntPtr _GetISteamGameServer(IntPtr self, HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamGameServer(HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamGameServer(Self, hSteamUser, hSteamPipe, pchVersion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_SetLocalIPBinding")]
		private static extern void _SetLocalIPBinding(IntPtr self, ref SteamIPAddress unIP, ushort usPort);

		internal void SetLocalIPBinding(ref SteamIPAddress unIP, ushort usPort)
		{
			_SetLocalIPBinding(Self, ref unIP, usPort);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamFriends")]
		private static extern IntPtr _GetISteamFriends(IntPtr self, HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamFriends(HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamFriends(Self, hSteamUser, hSteamPipe, pchVersion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamUtils")]
		private static extern IntPtr _GetISteamUtils(IntPtr self, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamUtils(HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamUtils(Self, hSteamPipe, pchVersion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamMatchmaking")]
		private static extern IntPtr _GetISteamMatchmaking(IntPtr self, HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamMatchmaking(HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamMatchmaking(Self, hSteamUser, hSteamPipe, pchVersion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamMatchmakingServers")]
		private static extern IntPtr _GetISteamMatchmakingServers(IntPtr self, HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamMatchmakingServers(HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamMatchmakingServers(Self, hSteamUser, hSteamPipe, pchVersion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamGenericInterface")]
		private static extern IntPtr _GetISteamGenericInterface(IntPtr self, HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamGenericInterface(HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamGenericInterface(Self, hSteamUser, hSteamPipe, pchVersion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamUserStats")]
		private static extern IntPtr _GetISteamUserStats(IntPtr self, HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamUserStats(HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamUserStats(Self, hSteamUser, hSteamPipe, pchVersion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamGameServerStats")]
		private static extern IntPtr _GetISteamGameServerStats(IntPtr self, HSteamUser hSteamuser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamGameServerStats(HSteamUser hSteamuser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamGameServerStats(Self, hSteamuser, hSteamPipe, pchVersion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamApps")]
		private static extern IntPtr _GetISteamApps(IntPtr self, HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamApps(HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamApps(Self, hSteamUser, hSteamPipe, pchVersion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamNetworking")]
		private static extern IntPtr _GetISteamNetworking(IntPtr self, HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamNetworking(HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamNetworking(Self, hSteamUser, hSteamPipe, pchVersion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamRemoteStorage")]
		private static extern IntPtr _GetISteamRemoteStorage(IntPtr self, HSteamUser hSteamuser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamRemoteStorage(HSteamUser hSteamuser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamRemoteStorage(Self, hSteamuser, hSteamPipe, pchVersion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamScreenshots")]
		private static extern IntPtr _GetISteamScreenshots(IntPtr self, HSteamUser hSteamuser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamScreenshots(HSteamUser hSteamuser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamScreenshots(Self, hSteamuser, hSteamPipe, pchVersion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamGameSearch")]
		private static extern IntPtr _GetISteamGameSearch(IntPtr self, HSteamUser hSteamuser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamGameSearch(HSteamUser hSteamuser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamGameSearch(Self, hSteamuser, hSteamPipe, pchVersion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetIPCCallCount")]
		private static extern uint _GetIPCCallCount(IntPtr self);

		internal uint GetIPCCallCount()
		{
			return _GetIPCCallCount(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_SetWarningMessageHook")]
		private static extern void _SetWarningMessageHook(IntPtr self, IntPtr pFunction);

		internal void SetWarningMessageHook(IntPtr pFunction)
		{
			_SetWarningMessageHook(Self, pFunction);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_BShutdownIfAllPipesClosed")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BShutdownIfAllPipesClosed(IntPtr self);

		internal bool BShutdownIfAllPipesClosed()
		{
			return _BShutdownIfAllPipesClosed(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamHTTP")]
		private static extern IntPtr _GetISteamHTTP(IntPtr self, HSteamUser hSteamuser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamHTTP(HSteamUser hSteamuser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamHTTP(Self, hSteamuser, hSteamPipe, pchVersion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamController")]
		private static extern IntPtr _GetISteamController(IntPtr self, HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamController(HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamController(Self, hSteamUser, hSteamPipe, pchVersion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamUGC")]
		private static extern IntPtr _GetISteamUGC(IntPtr self, HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamUGC(HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamUGC(Self, hSteamUser, hSteamPipe, pchVersion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamAppList")]
		private static extern IntPtr _GetISteamAppList(IntPtr self, HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamAppList(HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamAppList(Self, hSteamUser, hSteamPipe, pchVersion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamMusic")]
		private static extern IntPtr _GetISteamMusic(IntPtr self, HSteamUser hSteamuser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamMusic(HSteamUser hSteamuser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamMusic(Self, hSteamuser, hSteamPipe, pchVersion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamMusicRemote")]
		private static extern IntPtr _GetISteamMusicRemote(IntPtr self, HSteamUser hSteamuser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamMusicRemote(HSteamUser hSteamuser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamMusicRemote(Self, hSteamuser, hSteamPipe, pchVersion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamHTMLSurface")]
		private static extern IntPtr _GetISteamHTMLSurface(IntPtr self, HSteamUser hSteamuser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamHTMLSurface(HSteamUser hSteamuser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamHTMLSurface(Self, hSteamuser, hSteamPipe, pchVersion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamInventory")]
		private static extern IntPtr _GetISteamInventory(IntPtr self, HSteamUser hSteamuser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamInventory(HSteamUser hSteamuser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamInventory(Self, hSteamuser, hSteamPipe, pchVersion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamVideo")]
		private static extern IntPtr _GetISteamVideo(IntPtr self, HSteamUser hSteamuser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamVideo(HSteamUser hSteamuser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamVideo(Self, hSteamuser, hSteamPipe, pchVersion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamParentalSettings")]
		private static extern IntPtr _GetISteamParentalSettings(IntPtr self, HSteamUser hSteamuser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamParentalSettings(HSteamUser hSteamuser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamParentalSettings(Self, hSteamuser, hSteamPipe, pchVersion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamInput")]
		private static extern IntPtr _GetISteamInput(IntPtr self, HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamInput(HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamInput(Self, hSteamUser, hSteamPipe, pchVersion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamParties")]
		private static extern IntPtr _GetISteamParties(IntPtr self, HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamParties(HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamParties(Self, hSteamUser, hSteamPipe, pchVersion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamClient_GetISteamRemotePlay")]
		private static extern IntPtr _GetISteamRemotePlay(IntPtr self, HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion);

		internal IntPtr GetISteamRemotePlay(HSteamUser hSteamUser, HSteamPipe hSteamPipe, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVersion)
		{
			return _GetISteamRemotePlay(Self, hSteamUser, hSteamPipe, pchVersion);
		}
	}
}
