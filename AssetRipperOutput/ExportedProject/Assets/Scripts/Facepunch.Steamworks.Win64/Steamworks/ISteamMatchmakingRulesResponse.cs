using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	internal class ISteamMatchmakingRulesResponse : SteamInterface
	{
		internal ISteamMatchmakingRulesResponse(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingRulesResponse_RulesResponded")]
		private static extern void _RulesResponded(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchRule, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchValue);

		internal void RulesResponded([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchRule, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchValue)
		{
			_RulesResponded(Self, pchRule, pchValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingRulesResponse_RulesFailedToRespond")]
		private static extern void _RulesFailedToRespond(IntPtr self);

		internal void RulesFailedToRespond()
		{
			_RulesFailedToRespond(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamMatchmakingRulesResponse_RulesRefreshComplete")]
		private static extern void _RulesRefreshComplete(IntPtr self);

		internal void RulesRefreshComplete()
		{
			_RulesRefreshComplete(Self);
		}
	}
}
