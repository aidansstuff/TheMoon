using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal class ISteamNetworking : SteamInterface
	{
		internal ISteamNetworking(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamNetworking_v006();

		public override IntPtr GetUserInterfacePointer()
		{
			return SteamAPI_SteamNetworking_v006();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamGameServerNetworking_v006();

		public override IntPtr GetServerInterfacePointer()
		{
			return SteamAPI_SteamGameServerNetworking_v006();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworking_SendP2PPacket")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SendP2PPacket(IntPtr self, SteamId steamIDRemote, IntPtr pubData, uint cubData, P2PSend eP2PSendType, int nChannel);

		internal bool SendP2PPacket(SteamId steamIDRemote, IntPtr pubData, uint cubData, P2PSend eP2PSendType, int nChannel)
		{
			return _SendP2PPacket(Self, steamIDRemote, pubData, cubData, eP2PSendType, nChannel);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworking_IsP2PPacketAvailable")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _IsP2PPacketAvailable(IntPtr self, ref uint pcubMsgSize, int nChannel);

		internal bool IsP2PPacketAvailable(ref uint pcubMsgSize, int nChannel)
		{
			return _IsP2PPacketAvailable(Self, ref pcubMsgSize, nChannel);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworking_ReadP2PPacket")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _ReadP2PPacket(IntPtr self, IntPtr pubDest, uint cubDest, ref uint pcubMsgSize, ref SteamId psteamIDRemote, int nChannel);

		internal bool ReadP2PPacket(IntPtr pubDest, uint cubDest, ref uint pcubMsgSize, ref SteamId psteamIDRemote, int nChannel)
		{
			return _ReadP2PPacket(Self, pubDest, cubDest, ref pcubMsgSize, ref psteamIDRemote, nChannel);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworking_AcceptP2PSessionWithUser")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _AcceptP2PSessionWithUser(IntPtr self, SteamId steamIDRemote);

		internal bool AcceptP2PSessionWithUser(SteamId steamIDRemote)
		{
			return _AcceptP2PSessionWithUser(Self, steamIDRemote);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworking_CloseP2PSessionWithUser")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _CloseP2PSessionWithUser(IntPtr self, SteamId steamIDRemote);

		internal bool CloseP2PSessionWithUser(SteamId steamIDRemote)
		{
			return _CloseP2PSessionWithUser(Self, steamIDRemote);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworking_CloseP2PChannelWithUser")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _CloseP2PChannelWithUser(IntPtr self, SteamId steamIDRemote, int nChannel);

		internal bool CloseP2PChannelWithUser(SteamId steamIDRemote, int nChannel)
		{
			return _CloseP2PChannelWithUser(Self, steamIDRemote, nChannel);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworking_GetP2PSessionState")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetP2PSessionState(IntPtr self, SteamId steamIDRemote, ref P2PSessionState_t pConnectionState);

		internal bool GetP2PSessionState(SteamId steamIDRemote, ref P2PSessionState_t pConnectionState)
		{
			return _GetP2PSessionState(Self, steamIDRemote, ref pConnectionState);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworking_AllowP2PPacketRelay")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _AllowP2PPacketRelay(IntPtr self, [MarshalAs(UnmanagedType.U1)] bool bAllow);

		internal bool AllowP2PPacketRelay([MarshalAs(UnmanagedType.U1)] bool bAllow)
		{
			return _AllowP2PPacketRelay(Self, bAllow);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworking_CreateP2PConnectionSocket")]
		private static extern SNetSocket_t _CreateP2PConnectionSocket(IntPtr self, SteamId steamIDTarget, int nVirtualPort, int nTimeoutSec, [MarshalAs(UnmanagedType.U1)] bool bAllowUseOfPacketRelay);

		internal SNetSocket_t CreateP2PConnectionSocket(SteamId steamIDTarget, int nVirtualPort, int nTimeoutSec, [MarshalAs(UnmanagedType.U1)] bool bAllowUseOfPacketRelay)
		{
			return _CreateP2PConnectionSocket(Self, steamIDTarget, nVirtualPort, nTimeoutSec, bAllowUseOfPacketRelay);
		}
	}
}
