using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal class ISteamNetworkingSockets : SteamInterface
	{
		internal ISteamNetworkingSockets(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamNetworkingSockets_v008();

		public override IntPtr GetUserInterfacePointer()
		{
			return SteamAPI_SteamNetworkingSockets_v008();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamGameServerNetworkingSockets_v008();

		public override IntPtr GetServerInterfacePointer()
		{
			return SteamAPI_SteamGameServerNetworkingSockets_v008();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_CreateListenSocketIP")]
		private static extern Socket _CreateListenSocketIP(IntPtr self, ref NetAddress localAddress, int nOptions, [In][Out] NetKeyValue[] pOptions);

		internal Socket CreateListenSocketIP(ref NetAddress localAddress, int nOptions, [In][Out] NetKeyValue[] pOptions)
		{
			return _CreateListenSocketIP(Self, ref localAddress, nOptions, pOptions);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_ConnectByIPAddress")]
		private static extern Connection _ConnectByIPAddress(IntPtr self, ref NetAddress address, int nOptions, [In][Out] NetKeyValue[] pOptions);

		internal Connection ConnectByIPAddress(ref NetAddress address, int nOptions, [In][Out] NetKeyValue[] pOptions)
		{
			return _ConnectByIPAddress(Self, ref address, nOptions, pOptions);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_CreateListenSocketP2P")]
		private static extern Socket _CreateListenSocketP2P(IntPtr self, int nVirtualPort, int nOptions, [In][Out] NetKeyValue[] pOptions);

		internal Socket CreateListenSocketP2P(int nVirtualPort, int nOptions, [In][Out] NetKeyValue[] pOptions)
		{
			return _CreateListenSocketP2P(Self, nVirtualPort, nOptions, pOptions);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_ConnectP2P")]
		private static extern Connection _ConnectP2P(IntPtr self, ref NetIdentity identityRemote, int nVirtualPort, int nOptions, [In][Out] NetKeyValue[] pOptions);

		internal Connection ConnectP2P(ref NetIdentity identityRemote, int nVirtualPort, int nOptions, [In][Out] NetKeyValue[] pOptions)
		{
			return _ConnectP2P(Self, ref identityRemote, nVirtualPort, nOptions, pOptions);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_AcceptConnection")]
		private static extern Result _AcceptConnection(IntPtr self, Connection hConn);

		internal Result AcceptConnection(Connection hConn)
		{
			return _AcceptConnection(Self, hConn);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_CloseConnection")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _CloseConnection(IntPtr self, Connection hPeer, int nReason, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszDebug, [MarshalAs(UnmanagedType.U1)] bool bEnableLinger);

		internal bool CloseConnection(Connection hPeer, int nReason, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszDebug, [MarshalAs(UnmanagedType.U1)] bool bEnableLinger)
		{
			return _CloseConnection(Self, hPeer, nReason, pszDebug, bEnableLinger);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_CloseListenSocket")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _CloseListenSocket(IntPtr self, Socket hSocket);

		internal bool CloseListenSocket(Socket hSocket)
		{
			return _CloseListenSocket(Self, hSocket);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_SetConnectionUserData")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetConnectionUserData(IntPtr self, Connection hPeer, long nUserData);

		internal bool SetConnectionUserData(Connection hPeer, long nUserData)
		{
			return _SetConnectionUserData(Self, hPeer, nUserData);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_GetConnectionUserData")]
		private static extern long _GetConnectionUserData(IntPtr self, Connection hPeer);

		internal long GetConnectionUserData(Connection hPeer)
		{
			return _GetConnectionUserData(Self, hPeer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_SetConnectionName")]
		private static extern void _SetConnectionName(IntPtr self, Connection hPeer, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszName);

		internal void SetConnectionName(Connection hPeer, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszName)
		{
			_SetConnectionName(Self, hPeer, pszName);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_GetConnectionName")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetConnectionName(IntPtr self, Connection hPeer, IntPtr pszName, int nMaxLen);

		internal bool GetConnectionName(Connection hPeer, out string pszName)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			bool result = _GetConnectionName(Self, hPeer, intPtr, 32768);
			pszName = Helpers.MemoryToString(intPtr);
			return result;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_SendMessageToConnection")]
		private static extern Result _SendMessageToConnection(IntPtr self, Connection hConn, IntPtr pData, uint cbData, int nSendFlags, ref long pOutMessageNumber);

		internal Result SendMessageToConnection(Connection hConn, IntPtr pData, uint cbData, int nSendFlags, ref long pOutMessageNumber)
		{
			return _SendMessageToConnection(Self, hConn, pData, cbData, nSendFlags, ref pOutMessageNumber);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_SendMessages")]
		private static extern void _SendMessages(IntPtr self, int nMessages, ref NetMsg pMessages, [In][Out] long[] pOutMessageNumberOrResult);

		internal void SendMessages(int nMessages, ref NetMsg pMessages, [In][Out] long[] pOutMessageNumberOrResult)
		{
			_SendMessages(Self, nMessages, ref pMessages, pOutMessageNumberOrResult);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_FlushMessagesOnConnection")]
		private static extern Result _FlushMessagesOnConnection(IntPtr self, Connection hConn);

		internal Result FlushMessagesOnConnection(Connection hConn)
		{
			return _FlushMessagesOnConnection(Self, hConn);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnConnection")]
		private static extern int _ReceiveMessagesOnConnection(IntPtr self, Connection hConn, IntPtr ppOutMessages, int nMaxMessages);

		internal int ReceiveMessagesOnConnection(Connection hConn, IntPtr ppOutMessages, int nMaxMessages)
		{
			return _ReceiveMessagesOnConnection(Self, hConn, ppOutMessages, nMaxMessages);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_GetConnectionInfo")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetConnectionInfo(IntPtr self, Connection hConn, ref ConnectionInfo pInfo);

		internal bool GetConnectionInfo(Connection hConn, ref ConnectionInfo pInfo)
		{
			return _GetConnectionInfo(Self, hConn, ref pInfo);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_GetQuickConnectionStatus")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetQuickConnectionStatus(IntPtr self, Connection hConn, ref SteamNetworkingQuickConnectionStatus pStats);

		internal bool GetQuickConnectionStatus(Connection hConn, ref SteamNetworkingQuickConnectionStatus pStats)
		{
			return _GetQuickConnectionStatus(Self, hConn, ref pStats);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_GetDetailedConnectionStatus")]
		private static extern int _GetDetailedConnectionStatus(IntPtr self, Connection hConn, IntPtr pszBuf, int cbBuf);

		internal int GetDetailedConnectionStatus(Connection hConn, out string pszBuf)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			int result = _GetDetailedConnectionStatus(Self, hConn, intPtr, 32768);
			pszBuf = Helpers.MemoryToString(intPtr);
			return result;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_GetListenSocketAddress")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetListenSocketAddress(IntPtr self, Socket hSocket, ref NetAddress address);

		internal bool GetListenSocketAddress(Socket hSocket, ref NetAddress address)
		{
			return _GetListenSocketAddress(Self, hSocket, ref address);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_CreateSocketPair")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _CreateSocketPair(IntPtr self, [In][Out] Connection[] pOutConnection1, [In][Out] Connection[] pOutConnection2, [MarshalAs(UnmanagedType.U1)] bool bUseNetworkLoopback, ref NetIdentity pIdentity1, ref NetIdentity pIdentity2);

		internal bool CreateSocketPair([In][Out] Connection[] pOutConnection1, [In][Out] Connection[] pOutConnection2, [MarshalAs(UnmanagedType.U1)] bool bUseNetworkLoopback, ref NetIdentity pIdentity1, ref NetIdentity pIdentity2)
		{
			return _CreateSocketPair(Self, pOutConnection1, pOutConnection2, bUseNetworkLoopback, ref pIdentity1, ref pIdentity2);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_GetIdentity")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetIdentity(IntPtr self, ref NetIdentity pIdentity);

		internal bool GetIdentity(ref NetIdentity pIdentity)
		{
			return _GetIdentity(Self, ref pIdentity);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_InitAuthentication")]
		private static extern SteamNetworkingAvailability _InitAuthentication(IntPtr self);

		internal SteamNetworkingAvailability InitAuthentication()
		{
			return _InitAuthentication(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_GetAuthenticationStatus")]
		private static extern SteamNetworkingAvailability _GetAuthenticationStatus(IntPtr self, ref SteamNetAuthenticationStatus_t pDetails);

		internal SteamNetworkingAvailability GetAuthenticationStatus(ref SteamNetAuthenticationStatus_t pDetails)
		{
			return _GetAuthenticationStatus(Self, ref pDetails);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_CreatePollGroup")]
		private static extern HSteamNetPollGroup _CreatePollGroup(IntPtr self);

		internal HSteamNetPollGroup CreatePollGroup()
		{
			return _CreatePollGroup(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_DestroyPollGroup")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _DestroyPollGroup(IntPtr self, HSteamNetPollGroup hPollGroup);

		internal bool DestroyPollGroup(HSteamNetPollGroup hPollGroup)
		{
			return _DestroyPollGroup(Self, hPollGroup);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_SetConnectionPollGroup")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetConnectionPollGroup(IntPtr self, Connection hConn, HSteamNetPollGroup hPollGroup);

		internal bool SetConnectionPollGroup(Connection hConn, HSteamNetPollGroup hPollGroup)
		{
			return _SetConnectionPollGroup(Self, hConn, hPollGroup);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_ReceiveMessagesOnPollGroup")]
		private static extern int _ReceiveMessagesOnPollGroup(IntPtr self, HSteamNetPollGroup hPollGroup, IntPtr ppOutMessages, int nMaxMessages);

		internal int ReceiveMessagesOnPollGroup(HSteamNetPollGroup hPollGroup, IntPtr ppOutMessages, int nMaxMessages)
		{
			return _ReceiveMessagesOnPollGroup(Self, hPollGroup, ppOutMessages, nMaxMessages);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_ReceivedRelayAuthTicket")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _ReceivedRelayAuthTicket(IntPtr self, IntPtr pvTicket, int cbTicket, [In][Out] SteamDatagramRelayAuthTicket[] pOutParsedTicket);

		internal bool ReceivedRelayAuthTicket(IntPtr pvTicket, int cbTicket, [In][Out] SteamDatagramRelayAuthTicket[] pOutParsedTicket)
		{
			return _ReceivedRelayAuthTicket(Self, pvTicket, cbTicket, pOutParsedTicket);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_FindRelayAuthTicketForServer")]
		private static extern int _FindRelayAuthTicketForServer(IntPtr self, ref NetIdentity identityGameServer, int nVirtualPort, [In][Out] SteamDatagramRelayAuthTicket[] pOutParsedTicket);

		internal int FindRelayAuthTicketForServer(ref NetIdentity identityGameServer, int nVirtualPort, [In][Out] SteamDatagramRelayAuthTicket[] pOutParsedTicket)
		{
			return _FindRelayAuthTicketForServer(Self, ref identityGameServer, nVirtualPort, pOutParsedTicket);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_ConnectToHostedDedicatedServer")]
		private static extern Connection _ConnectToHostedDedicatedServer(IntPtr self, ref NetIdentity identityTarget, int nVirtualPort, int nOptions, [In][Out] NetKeyValue[] pOptions);

		internal Connection ConnectToHostedDedicatedServer(ref NetIdentity identityTarget, int nVirtualPort, int nOptions, [In][Out] NetKeyValue[] pOptions)
		{
			return _ConnectToHostedDedicatedServer(Self, ref identityTarget, nVirtualPort, nOptions, pOptions);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_GetHostedDedicatedServerPort")]
		private static extern ushort _GetHostedDedicatedServerPort(IntPtr self);

		internal ushort GetHostedDedicatedServerPort()
		{
			return _GetHostedDedicatedServerPort(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_GetHostedDedicatedServerPOPID")]
		private static extern SteamNetworkingPOPID _GetHostedDedicatedServerPOPID(IntPtr self);

		internal SteamNetworkingPOPID GetHostedDedicatedServerPOPID()
		{
			return _GetHostedDedicatedServerPOPID(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_GetHostedDedicatedServerAddress")]
		private static extern Result _GetHostedDedicatedServerAddress(IntPtr self, ref SteamDatagramHostedAddress pRouting);

		internal Result GetHostedDedicatedServerAddress(ref SteamDatagramHostedAddress pRouting)
		{
			return _GetHostedDedicatedServerAddress(Self, ref pRouting);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_CreateHostedDedicatedServerListenSocket")]
		private static extern Socket _CreateHostedDedicatedServerListenSocket(IntPtr self, int nVirtualPort, int nOptions, [In][Out] NetKeyValue[] pOptions);

		internal Socket CreateHostedDedicatedServerListenSocket(int nVirtualPort, int nOptions, [In][Out] NetKeyValue[] pOptions)
		{
			return _CreateHostedDedicatedServerListenSocket(Self, nVirtualPort, nOptions, pOptions);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_GetGameCoordinatorServerLogin")]
		private static extern Result _GetGameCoordinatorServerLogin(IntPtr self, ref SteamDatagramGameCoordinatorServerLogin pLoginInfo, ref int pcbSignedBlob, IntPtr pBlob);

		internal Result GetGameCoordinatorServerLogin(ref SteamDatagramGameCoordinatorServerLogin pLoginInfo, ref int pcbSignedBlob, IntPtr pBlob)
		{
			return _GetGameCoordinatorServerLogin(Self, ref pLoginInfo, ref pcbSignedBlob, pBlob);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_ConnectP2PCustomSignaling")]
		private static extern Connection _ConnectP2PCustomSignaling(IntPtr self, IntPtr pSignaling, ref NetIdentity pPeerIdentity, int nOptions, [In][Out] NetKeyValue[] pOptions);

		internal Connection ConnectP2PCustomSignaling(IntPtr pSignaling, ref NetIdentity pPeerIdentity, int nOptions, [In][Out] NetKeyValue[] pOptions)
		{
			return _ConnectP2PCustomSignaling(Self, pSignaling, ref pPeerIdentity, nOptions, pOptions);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_ReceivedP2PCustomSignal")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _ReceivedP2PCustomSignal(IntPtr self, IntPtr pMsg, int cbMsg, IntPtr pContext);

		internal bool ReceivedP2PCustomSignal(IntPtr pMsg, int cbMsg, IntPtr pContext)
		{
			return _ReceivedP2PCustomSignal(Self, pMsg, cbMsg, pContext);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_GetCertificateRequest")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetCertificateRequest(IntPtr self, ref int pcbBlob, IntPtr pBlob, ref NetErrorMessage errMsg);

		internal bool GetCertificateRequest(ref int pcbBlob, IntPtr pBlob, ref NetErrorMessage errMsg)
		{
			return _GetCertificateRequest(Self, ref pcbBlob, pBlob, ref errMsg);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingSockets_SetCertificate")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetCertificate(IntPtr self, IntPtr pCertificate, int cbCertificate, ref NetErrorMessage errMsg);

		internal bool SetCertificate(IntPtr pCertificate, int cbCertificate, ref NetErrorMessage errMsg)
		{
			return _SetCertificate(Self, pCertificate, cbCertificate, ref errMsg);
		}
	}
}
