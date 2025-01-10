using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal class ISteamMatchmakingServerListResponse : SteamInterface
	{
		internal ISteamMatchmakingServerListResponse(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingServerListResponse_ServerResponded")]
		private static extern void _ServerResponded(IntPtr self, HServerListRequest hRequest, int iServer);

		internal void ServerResponded(HServerListRequest hRequest, int iServer)
		{
			_ServerResponded(Self, hRequest, iServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingServerListResponse_ServerFailedToRespond")]
		private static extern void _ServerFailedToRespond(IntPtr self, HServerListRequest hRequest, int iServer);

		internal void ServerFailedToRespond(HServerListRequest hRequest, int iServer)
		{
			_ServerFailedToRespond(Self, hRequest, iServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingServerListResponse_RefreshComplete")]
		private static extern void _RefreshComplete(IntPtr self, HServerListRequest hRequest, MatchMakingServerResponse response);

		internal void RefreshComplete(HServerListRequest hRequest, MatchMakingServerResponse response)
		{
			_RefreshComplete(Self, hRequest, response);
		}
	}
}
