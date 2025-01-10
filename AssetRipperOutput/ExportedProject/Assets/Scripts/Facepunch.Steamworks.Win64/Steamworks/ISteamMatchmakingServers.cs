using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal class ISteamMatchmakingServers : SteamInterface
	{
		internal ISteamMatchmakingServers(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamMatchmakingServers_v002();

		public override IntPtr GetUserInterfacePointer()
		{
			return SteamAPI_SteamMatchmakingServers_v002();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingServers_RequestInternetServerList")]
		private static extern HServerListRequest _RequestInternetServerList(IntPtr self, AppId iApp, [In][Out] ref MatchMakingKeyValuePair[] ppchFilters, uint nFilters, IntPtr pRequestServersResponse);

		internal HServerListRequest RequestInternetServerList(AppId iApp, [In][Out] ref MatchMakingKeyValuePair[] ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
		{
			return _RequestInternetServerList(Self, iApp, ref ppchFilters, nFilters, pRequestServersResponse);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingServers_RequestLANServerList")]
		private static extern HServerListRequest _RequestLANServerList(IntPtr self, AppId iApp, IntPtr pRequestServersResponse);

		internal HServerListRequest RequestLANServerList(AppId iApp, IntPtr pRequestServersResponse)
		{
			return _RequestLANServerList(Self, iApp, pRequestServersResponse);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingServers_RequestFriendsServerList")]
		private static extern HServerListRequest _RequestFriendsServerList(IntPtr self, AppId iApp, [In][Out] ref MatchMakingKeyValuePair[] ppchFilters, uint nFilters, IntPtr pRequestServersResponse);

		internal HServerListRequest RequestFriendsServerList(AppId iApp, [In][Out] ref MatchMakingKeyValuePair[] ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
		{
			return _RequestFriendsServerList(Self, iApp, ref ppchFilters, nFilters, pRequestServersResponse);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingServers_RequestFavoritesServerList")]
		private static extern HServerListRequest _RequestFavoritesServerList(IntPtr self, AppId iApp, [In][Out] ref MatchMakingKeyValuePair[] ppchFilters, uint nFilters, IntPtr pRequestServersResponse);

		internal HServerListRequest RequestFavoritesServerList(AppId iApp, [In][Out] ref MatchMakingKeyValuePair[] ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
		{
			return _RequestFavoritesServerList(Self, iApp, ref ppchFilters, nFilters, pRequestServersResponse);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingServers_RequestHistoryServerList")]
		private static extern HServerListRequest _RequestHistoryServerList(IntPtr self, AppId iApp, [In][Out] ref MatchMakingKeyValuePair[] ppchFilters, uint nFilters, IntPtr pRequestServersResponse);

		internal HServerListRequest RequestHistoryServerList(AppId iApp, [In][Out] ref MatchMakingKeyValuePair[] ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
		{
			return _RequestHistoryServerList(Self, iApp, ref ppchFilters, nFilters, pRequestServersResponse);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingServers_RequestSpectatorServerList")]
		private static extern HServerListRequest _RequestSpectatorServerList(IntPtr self, AppId iApp, [In][Out] ref MatchMakingKeyValuePair[] ppchFilters, uint nFilters, IntPtr pRequestServersResponse);

		internal HServerListRequest RequestSpectatorServerList(AppId iApp, [In][Out] ref MatchMakingKeyValuePair[] ppchFilters, uint nFilters, IntPtr pRequestServersResponse)
		{
			return _RequestSpectatorServerList(Self, iApp, ref ppchFilters, nFilters, pRequestServersResponse);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingServers_ReleaseRequest")]
		private static extern void _ReleaseRequest(IntPtr self, HServerListRequest hServerListRequest);

		internal void ReleaseRequest(HServerListRequest hServerListRequest)
		{
			_ReleaseRequest(Self, hServerListRequest);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingServers_GetServerDetails")]
		private static extern IntPtr _GetServerDetails(IntPtr self, HServerListRequest hRequest, int iServer);

		internal gameserveritem_t GetServerDetails(HServerListRequest hRequest, int iServer)
		{
			IntPtr ptr = _GetServerDetails(Self, hRequest, iServer);
			return ptr.ToType<gameserveritem_t>();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingServers_CancelQuery")]
		private static extern void _CancelQuery(IntPtr self, HServerListRequest hRequest);

		internal void CancelQuery(HServerListRequest hRequest)
		{
			_CancelQuery(Self, hRequest);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingServers_RefreshQuery")]
		private static extern void _RefreshQuery(IntPtr self, HServerListRequest hRequest);

		internal void RefreshQuery(HServerListRequest hRequest)
		{
			_RefreshQuery(Self, hRequest);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingServers_IsRefreshing")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _IsRefreshing(IntPtr self, HServerListRequest hRequest);

		internal bool IsRefreshing(HServerListRequest hRequest)
		{
			return _IsRefreshing(Self, hRequest);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingServers_GetServerCount")]
		private static extern int _GetServerCount(IntPtr self, HServerListRequest hRequest);

		internal int GetServerCount(HServerListRequest hRequest)
		{
			return _GetServerCount(Self, hRequest);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingServers_RefreshServer")]
		private static extern void _RefreshServer(IntPtr self, HServerListRequest hRequest, int iServer);

		internal void RefreshServer(HServerListRequest hRequest, int iServer)
		{
			_RefreshServer(Self, hRequest, iServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingServers_PingServer")]
		private static extern HServerQuery _PingServer(IntPtr self, uint unIP, ushort usPort, IntPtr pRequestServersResponse);

		internal HServerQuery PingServer(uint unIP, ushort usPort, IntPtr pRequestServersResponse)
		{
			return _PingServer(Self, unIP, usPort, pRequestServersResponse);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingServers_PlayerDetails")]
		private static extern HServerQuery _PlayerDetails(IntPtr self, uint unIP, ushort usPort, IntPtr pRequestServersResponse);

		internal HServerQuery PlayerDetails(uint unIP, ushort usPort, IntPtr pRequestServersResponse)
		{
			return _PlayerDetails(Self, unIP, usPort, pRequestServersResponse);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingServers_ServerRules")]
		private static extern HServerQuery _ServerRules(IntPtr self, uint unIP, ushort usPort, IntPtr pRequestServersResponse);

		internal HServerQuery ServerRules(uint unIP, ushort usPort, IntPtr pRequestServersResponse)
		{
			return _ServerRules(Self, unIP, usPort, pRequestServersResponse);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingServers_CancelServerQuery")]
		private static extern void _CancelServerQuery(IntPtr self, HServerQuery hServerQuery);

		internal void CancelServerQuery(HServerQuery hServerQuery)
		{
			_CancelServerQuery(Self, hServerQuery);
		}
	}
}
