using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal class ISteamGameServer : SteamInterface
	{
		internal ISteamGameServer(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamGameServer_v013();

		public override IntPtr GetServerInterfacePointer()
		{
			return SteamAPI_SteamGameServer_v013();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_SetProduct")]
		private static extern void _SetProduct(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszProduct);

		internal void SetProduct([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszProduct)
		{
			_SetProduct(Self, pszProduct);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_SetGameDescription")]
		private static extern void _SetGameDescription(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszGameDescription);

		internal void SetGameDescription([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszGameDescription)
		{
			_SetGameDescription(Self, pszGameDescription);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_SetModDir")]
		private static extern void _SetModDir(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszModDir);

		internal void SetModDir([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszModDir)
		{
			_SetModDir(Self, pszModDir);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_SetDedicatedServer")]
		private static extern void _SetDedicatedServer(IntPtr self, [MarshalAs(UnmanagedType.U1)] bool bDedicated);

		internal void SetDedicatedServer([MarshalAs(UnmanagedType.U1)] bool bDedicated)
		{
			_SetDedicatedServer(Self, bDedicated);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_LogOn")]
		private static extern void _LogOn(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszToken);

		internal void LogOn([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszToken)
		{
			_LogOn(Self, pszToken);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_LogOnAnonymous")]
		private static extern void _LogOnAnonymous(IntPtr self);

		internal void LogOnAnonymous()
		{
			_LogOnAnonymous(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_LogOff")]
		private static extern void _LogOff(IntPtr self);

		internal void LogOff()
		{
			_LogOff(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_BLoggedOn")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BLoggedOn(IntPtr self);

		internal bool BLoggedOn()
		{
			return _BLoggedOn(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_BSecure")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BSecure(IntPtr self);

		internal bool BSecure()
		{
			return _BSecure(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_GetSteamID")]
		private static extern SteamId _GetSteamID(IntPtr self);

		internal SteamId GetSteamID()
		{
			return _GetSteamID(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_WasRestartRequested")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _WasRestartRequested(IntPtr self);

		internal bool WasRestartRequested()
		{
			return _WasRestartRequested(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_SetMaxPlayerCount")]
		private static extern void _SetMaxPlayerCount(IntPtr self, int cPlayersMax);

		internal void SetMaxPlayerCount(int cPlayersMax)
		{
			_SetMaxPlayerCount(Self, cPlayersMax);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_SetBotPlayerCount")]
		private static extern void _SetBotPlayerCount(IntPtr self, int cBotplayers);

		internal void SetBotPlayerCount(int cBotplayers)
		{
			_SetBotPlayerCount(Self, cBotplayers);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_SetServerName")]
		private static extern void _SetServerName(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszServerName);

		internal void SetServerName([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszServerName)
		{
			_SetServerName(Self, pszServerName);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_SetMapName")]
		private static extern void _SetMapName(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszMapName);

		internal void SetMapName([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszMapName)
		{
			_SetMapName(Self, pszMapName);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_SetPasswordProtected")]
		private static extern void _SetPasswordProtected(IntPtr self, [MarshalAs(UnmanagedType.U1)] bool bPasswordProtected);

		internal void SetPasswordProtected([MarshalAs(UnmanagedType.U1)] bool bPasswordProtected)
		{
			_SetPasswordProtected(Self, bPasswordProtected);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_SetSpectatorPort")]
		private static extern void _SetSpectatorPort(IntPtr self, ushort unSpectatorPort);

		internal void SetSpectatorPort(ushort unSpectatorPort)
		{
			_SetSpectatorPort(Self, unSpectatorPort);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_SetSpectatorServerName")]
		private static extern void _SetSpectatorServerName(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszSpectatorServerName);

		internal void SetSpectatorServerName([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszSpectatorServerName)
		{
			_SetSpectatorServerName(Self, pszSpectatorServerName);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_ClearAllKeyValues")]
		private static extern void _ClearAllKeyValues(IntPtr self);

		internal void ClearAllKeyValues()
		{
			_ClearAllKeyValues(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_SetKeyValue")]
		private static extern void _SetKeyValue(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pKey, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pValue);

		internal void SetKeyValue([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pKey, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pValue)
		{
			_SetKeyValue(Self, pKey, pValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_SetGameTags")]
		private static extern void _SetGameTags(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchGameTags);

		internal void SetGameTags([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchGameTags)
		{
			_SetGameTags(Self, pchGameTags);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_SetGameData")]
		private static extern void _SetGameData(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchGameData);

		internal void SetGameData([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchGameData)
		{
			_SetGameData(Self, pchGameData);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_SetRegion")]
		private static extern void _SetRegion(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszRegion);

		internal void SetRegion([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszRegion)
		{
			_SetRegion(Self, pszRegion);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_SendUserConnectAndAuthenticate")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SendUserConnectAndAuthenticate(IntPtr self, uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, ref SteamId pSteamIDUser);

		internal bool SendUserConnectAndAuthenticate(uint unIPClient, IntPtr pvAuthBlob, uint cubAuthBlobSize, ref SteamId pSteamIDUser)
		{
			return _SendUserConnectAndAuthenticate(Self, unIPClient, pvAuthBlob, cubAuthBlobSize, ref pSteamIDUser);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_CreateUnauthenticatedUserConnection")]
		private static extern SteamId _CreateUnauthenticatedUserConnection(IntPtr self);

		internal SteamId CreateUnauthenticatedUserConnection()
		{
			return _CreateUnauthenticatedUserConnection(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_SendUserDisconnect")]
		private static extern void _SendUserDisconnect(IntPtr self, SteamId steamIDUser);

		internal void SendUserDisconnect(SteamId steamIDUser)
		{
			_SendUserDisconnect(Self, steamIDUser);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_BUpdateUserData")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BUpdateUserData(IntPtr self, SteamId steamIDUser, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchPlayerName, uint uScore);

		internal bool BUpdateUserData(SteamId steamIDUser, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchPlayerName, uint uScore)
		{
			return _BUpdateUserData(Self, steamIDUser, pchPlayerName, uScore);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_GetAuthSessionTicket")]
		private static extern HAuthTicket _GetAuthSessionTicket(IntPtr self, IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket);

		internal HAuthTicket GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
		{
			return _GetAuthSessionTicket(Self, pTicket, cbMaxTicket, ref pcbTicket);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_BeginAuthSession")]
		private static extern BeginAuthResult _BeginAuthSession(IntPtr self, IntPtr pAuthTicket, int cbAuthTicket, SteamId steamID);

		internal BeginAuthResult BeginAuthSession(IntPtr pAuthTicket, int cbAuthTicket, SteamId steamID)
		{
			return _BeginAuthSession(Self, pAuthTicket, cbAuthTicket, steamID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_EndAuthSession")]
		private static extern void _EndAuthSession(IntPtr self, SteamId steamID);

		internal void EndAuthSession(SteamId steamID)
		{
			_EndAuthSession(Self, steamID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_CancelAuthTicket")]
		private static extern void _CancelAuthTicket(IntPtr self, HAuthTicket hAuthTicket);

		internal void CancelAuthTicket(HAuthTicket hAuthTicket)
		{
			_CancelAuthTicket(Self, hAuthTicket);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_UserHasLicenseForApp")]
		private static extern UserHasLicenseForAppResult _UserHasLicenseForApp(IntPtr self, SteamId steamID, AppId appID);

		internal UserHasLicenseForAppResult UserHasLicenseForApp(SteamId steamID, AppId appID)
		{
			return _UserHasLicenseForApp(Self, steamID, appID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_RequestUserGroupStatus")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _RequestUserGroupStatus(IntPtr self, SteamId steamIDUser, SteamId steamIDGroup);

		internal bool RequestUserGroupStatus(SteamId steamIDUser, SteamId steamIDGroup)
		{
			return _RequestUserGroupStatus(Self, steamIDUser, steamIDGroup);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_GetGameplayStats")]
		private static extern void _GetGameplayStats(IntPtr self);

		internal void GetGameplayStats()
		{
			_GetGameplayStats(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_GetServerReputation")]
		private static extern SteamAPICall_t _GetServerReputation(IntPtr self);

		internal CallResult<GSReputation_t> GetServerReputation()
		{
			SteamAPICall_t call = _GetServerReputation(Self);
			return new CallResult<GSReputation_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_GetPublicIP")]
		private static extern SteamIPAddress _GetPublicIP(IntPtr self);

		internal SteamIPAddress GetPublicIP()
		{
			return _GetPublicIP(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_HandleIncomingPacket")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _HandleIncomingPacket(IntPtr self, IntPtr pData, int cbData, uint srcIP, ushort srcPort);

		internal bool HandleIncomingPacket(IntPtr pData, int cbData, uint srcIP, ushort srcPort)
		{
			return _HandleIncomingPacket(Self, pData, cbData, srcIP, srcPort);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_GetNextOutgoingPacket")]
		private static extern int _GetNextOutgoingPacket(IntPtr self, IntPtr pOut, int cbMaxOut, ref uint pNetAdr, ref ushort pPort);

		internal int GetNextOutgoingPacket(IntPtr pOut, int cbMaxOut, ref uint pNetAdr, ref ushort pPort)
		{
			return _GetNextOutgoingPacket(Self, pOut, cbMaxOut, ref pNetAdr, ref pPort);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_EnableHeartbeats")]
		private static extern void _EnableHeartbeats(IntPtr self, [MarshalAs(UnmanagedType.U1)] bool bActive);

		internal void EnableHeartbeats([MarshalAs(UnmanagedType.U1)] bool bActive)
		{
			_EnableHeartbeats(Self, bActive);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_SetHeartbeatInterval")]
		private static extern void _SetHeartbeatInterval(IntPtr self, int iHeartbeatInterval);

		internal void SetHeartbeatInterval(int iHeartbeatInterval)
		{
			_SetHeartbeatInterval(Self, iHeartbeatInterval);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_ForceHeartbeat")]
		private static extern void _ForceHeartbeat(IntPtr self);

		internal void ForceHeartbeat()
		{
			_ForceHeartbeat(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_AssociateWithClan")]
		private static extern SteamAPICall_t _AssociateWithClan(IntPtr self, SteamId steamIDClan);

		internal CallResult<AssociateWithClanResult_t> AssociateWithClan(SteamId steamIDClan)
		{
			SteamAPICall_t call = _AssociateWithClan(Self, steamIDClan);
			return new CallResult<AssociateWithClanResult_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameServer_ComputeNewPlayerCompatibility")]
		private static extern SteamAPICall_t _ComputeNewPlayerCompatibility(IntPtr self, SteamId steamIDNewPlayer);

		internal CallResult<ComputeNewPlayerCompatibilityResult_t> ComputeNewPlayerCompatibility(SteamId steamIDNewPlayer)
		{
			SteamAPICall_t call = _ComputeNewPlayerCompatibility(Self, steamIDNewPlayer);
			return new CallResult<ComputeNewPlayerCompatibilityResult_t>(call, base.IsServer);
		}
	}
}
