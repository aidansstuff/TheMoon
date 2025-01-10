using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	internal class ISteamMatchmakingPlayersResponse : SteamInterface
	{
		internal ISteamMatchmakingPlayersResponse(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingPlayersResponse_AddPlayerToList")]
		private static extern void _AddPlayerToList(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, int nScore, float flTimePlayed);

		internal void AddPlayerToList([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchName, int nScore, float flTimePlayed)
		{
			_AddPlayerToList(Self, pchName, nScore, flTimePlayed);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingPlayersResponse_PlayersFailedToRespond")]
		private static extern void _PlayersFailedToRespond(IntPtr self);

		internal void PlayersFailedToRespond()
		{
			_PlayersFailedToRespond(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingPlayersResponse_PlayersRefreshComplete")]
		private static extern void _PlayersRefreshComplete(IntPtr self);

		internal void PlayersRefreshComplete()
		{
			_PlayersRefreshComplete(Self);
		}
	}
}
