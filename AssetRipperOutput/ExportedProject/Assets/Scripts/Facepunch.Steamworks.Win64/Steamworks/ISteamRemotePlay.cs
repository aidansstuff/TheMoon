using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal class ISteamRemotePlay : SteamInterface
	{
		internal ISteamRemotePlay(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamRemotePlay_v001();

		public override IntPtr GetUserInterfacePointer()
		{
			return SteamAPI_SteamRemotePlay_v001();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemotePlay_GetSessionCount")]
		private static extern uint _GetSessionCount(IntPtr self);

		internal uint GetSessionCount()
		{
			return _GetSessionCount(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemotePlay_GetSessionID")]
		private static extern RemotePlaySessionID_t _GetSessionID(IntPtr self, int iSessionIndex);

		internal RemotePlaySessionID_t GetSessionID(int iSessionIndex)
		{
			return _GetSessionID(Self, iSessionIndex);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemotePlay_GetSessionSteamID")]
		private static extern SteamId _GetSessionSteamID(IntPtr self, RemotePlaySessionID_t unSessionID);

		internal SteamId GetSessionSteamID(RemotePlaySessionID_t unSessionID)
		{
			return _GetSessionSteamID(Self, unSessionID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemotePlay_GetSessionClientName")]
		private static extern Utf8StringPointer _GetSessionClientName(IntPtr self, RemotePlaySessionID_t unSessionID);

		internal string GetSessionClientName(RemotePlaySessionID_t unSessionID)
		{
			Utf8StringPointer utf8StringPointer = _GetSessionClientName(Self, unSessionID);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemotePlay_GetSessionClientFormFactor")]
		private static extern SteamDeviceFormFactor _GetSessionClientFormFactor(IntPtr self, RemotePlaySessionID_t unSessionID);

		internal SteamDeviceFormFactor GetSessionClientFormFactor(RemotePlaySessionID_t unSessionID)
		{
			return _GetSessionClientFormFactor(Self, unSessionID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemotePlay_BGetSessionClientResolution")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BGetSessionClientResolution(IntPtr self, RemotePlaySessionID_t unSessionID, ref int pnResolutionX, ref int pnResolutionY);

		internal bool BGetSessionClientResolution(RemotePlaySessionID_t unSessionID, ref int pnResolutionX, ref int pnResolutionY)
		{
			return _BGetSessionClientResolution(Self, unSessionID, ref pnResolutionX, ref pnResolutionY);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamRemotePlay_BSendRemotePlayTogetherInvite")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BSendRemotePlayTogetherInvite(IntPtr self, SteamId steamIDFriend);

		internal bool BSendRemotePlayTogetherInvite(SteamId steamIDFriend)
		{
			return _BSendRemotePlayTogetherInvite(Self, steamIDFriend);
		}
	}
}
