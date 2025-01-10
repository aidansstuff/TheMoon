using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal class ISteamMatchmakingPingResponse : SteamInterface
	{
		internal ISteamMatchmakingPingResponse(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingPingResponse_ServerResponded")]
		private static extern void _ServerResponded(IntPtr self, ref gameserveritem_t server);

		internal void ServerResponded(ref gameserveritem_t server)
		{
			_ServerResponded(Self, ref server);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingPingResponse_ServerFailedToRespond")]
		private static extern void _ServerFailedToRespond(IntPtr self);

		internal void ServerFailedToRespond()
		{
			_ServerFailedToRespond(Self);
		}
	}
}
