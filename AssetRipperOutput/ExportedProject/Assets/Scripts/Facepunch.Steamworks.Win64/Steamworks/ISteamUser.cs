using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal class ISteamUser : SteamInterface
	{
		internal ISteamUser(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamUser_v020();

		public override IntPtr GetUserInterfacePointer()
		{
			return SteamAPI_SteamUser_v020();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_GetHSteamUser")]
		private static extern HSteamUser _GetHSteamUser(IntPtr self);

		internal HSteamUser GetHSteamUser()
		{
			return _GetHSteamUser(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_BLoggedOn")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BLoggedOn(IntPtr self);

		internal bool BLoggedOn()
		{
			return _BLoggedOn(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_GetSteamID")]
		private static extern SteamId _GetSteamID(IntPtr self);

		internal SteamId GetSteamID()
		{
			return _GetSteamID(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_InitiateGameConnection")]
		private static extern int _InitiateGameConnection(IntPtr self, IntPtr pAuthBlob, int cbMaxAuthBlob, SteamId steamIDGameServer, uint unIPServer, ushort usPortServer, [MarshalAs(UnmanagedType.U1)] bool bSecure);

		internal int InitiateGameConnection(IntPtr pAuthBlob, int cbMaxAuthBlob, SteamId steamIDGameServer, uint unIPServer, ushort usPortServer, [MarshalAs(UnmanagedType.U1)] bool bSecure)
		{
			return _InitiateGameConnection(Self, pAuthBlob, cbMaxAuthBlob, steamIDGameServer, unIPServer, usPortServer, bSecure);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_TerminateGameConnection")]
		private static extern void _TerminateGameConnection(IntPtr self, uint unIPServer, ushort usPortServer);

		internal void TerminateGameConnection(uint unIPServer, ushort usPortServer)
		{
			_TerminateGameConnection(Self, unIPServer, usPortServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_TrackAppUsageEvent")]
		private static extern void _TrackAppUsageEvent(IntPtr self, GameId gameID, int eAppUsageEvent, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchExtraInfo);

		internal void TrackAppUsageEvent(GameId gameID, int eAppUsageEvent, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchExtraInfo)
		{
			_TrackAppUsageEvent(Self, gameID, eAppUsageEvent, pchExtraInfo);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_GetUserDataFolder")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetUserDataFolder(IntPtr self, IntPtr pchBuffer, int cubBuffer);

		internal bool GetUserDataFolder(out string pchBuffer)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			bool result = _GetUserDataFolder(Self, intPtr, 32768);
			pchBuffer = Helpers.MemoryToString(intPtr);
			return result;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_StartVoiceRecording")]
		private static extern void _StartVoiceRecording(IntPtr self);

		internal void StartVoiceRecording()
		{
			_StartVoiceRecording(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_StopVoiceRecording")]
		private static extern void _StopVoiceRecording(IntPtr self);

		internal void StopVoiceRecording()
		{
			_StopVoiceRecording(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_GetAvailableVoice")]
		private static extern VoiceResult _GetAvailableVoice(IntPtr self, ref uint pcbCompressed, ref uint pcbUncompressed_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated);

		internal VoiceResult GetAvailableVoice(ref uint pcbCompressed, ref uint pcbUncompressed_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
		{
			return _GetAvailableVoice(Self, ref pcbCompressed, ref pcbUncompressed_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_GetVoice")]
		private static extern VoiceResult _GetVoice(IntPtr self, [MarshalAs(UnmanagedType.U1)] bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, ref uint nBytesWritten, [MarshalAs(UnmanagedType.U1)] bool bWantUncompressed_Deprecated, IntPtr pUncompressedDestBuffer_Deprecated, uint cbUncompressedDestBufferSize_Deprecated, ref uint nUncompressBytesWritten_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated);

		internal VoiceResult GetVoice([MarshalAs(UnmanagedType.U1)] bool bWantCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, ref uint nBytesWritten, [MarshalAs(UnmanagedType.U1)] bool bWantUncompressed_Deprecated, IntPtr pUncompressedDestBuffer_Deprecated, uint cbUncompressedDestBufferSize_Deprecated, ref uint nUncompressBytesWritten_Deprecated, uint nUncompressedVoiceDesiredSampleRate_Deprecated)
		{
			return _GetVoice(Self, bWantCompressed, pDestBuffer, cbDestBufferSize, ref nBytesWritten, bWantUncompressed_Deprecated, pUncompressedDestBuffer_Deprecated, cbUncompressedDestBufferSize_Deprecated, ref nUncompressBytesWritten_Deprecated, nUncompressedVoiceDesiredSampleRate_Deprecated);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_DecompressVoice")]
		private static extern VoiceResult _DecompressVoice(IntPtr self, IntPtr pCompressed, uint cbCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, ref uint nBytesWritten, uint nDesiredSampleRate);

		internal VoiceResult DecompressVoice(IntPtr pCompressed, uint cbCompressed, IntPtr pDestBuffer, uint cbDestBufferSize, ref uint nBytesWritten, uint nDesiredSampleRate)
		{
			return _DecompressVoice(Self, pCompressed, cbCompressed, pDestBuffer, cbDestBufferSize, ref nBytesWritten, nDesiredSampleRate);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_GetVoiceOptimalSampleRate")]
		private static extern uint _GetVoiceOptimalSampleRate(IntPtr self);

		internal uint GetVoiceOptimalSampleRate()
		{
			return _GetVoiceOptimalSampleRate(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_GetAuthSessionTicket")]
		private static extern HAuthTicket _GetAuthSessionTicket(IntPtr self, IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket);

		internal HAuthTicket GetAuthSessionTicket(IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
		{
			return _GetAuthSessionTicket(Self, pTicket, cbMaxTicket, ref pcbTicket);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_BeginAuthSession")]
		private static extern BeginAuthResult _BeginAuthSession(IntPtr self, IntPtr pAuthTicket, int cbAuthTicket, SteamId steamID);

		internal BeginAuthResult BeginAuthSession(IntPtr pAuthTicket, int cbAuthTicket, SteamId steamID)
		{
			return _BeginAuthSession(Self, pAuthTicket, cbAuthTicket, steamID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_EndAuthSession")]
		private static extern void _EndAuthSession(IntPtr self, SteamId steamID);

		internal void EndAuthSession(SteamId steamID)
		{
			_EndAuthSession(Self, steamID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_CancelAuthTicket")]
		private static extern void _CancelAuthTicket(IntPtr self, HAuthTicket hAuthTicket);

		internal void CancelAuthTicket(HAuthTicket hAuthTicket)
		{
			_CancelAuthTicket(Self, hAuthTicket);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_UserHasLicenseForApp")]
		private static extern UserHasLicenseForAppResult _UserHasLicenseForApp(IntPtr self, SteamId steamID, AppId appID);

		internal UserHasLicenseForAppResult UserHasLicenseForApp(SteamId steamID, AppId appID)
		{
			return _UserHasLicenseForApp(Self, steamID, appID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_BIsBehindNAT")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BIsBehindNAT(IntPtr self);

		internal bool BIsBehindNAT()
		{
			return _BIsBehindNAT(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_AdvertiseGame")]
		private static extern void _AdvertiseGame(IntPtr self, SteamId steamIDGameServer, uint unIPServer, ushort usPortServer);

		internal void AdvertiseGame(SteamId steamIDGameServer, uint unIPServer, ushort usPortServer)
		{
			_AdvertiseGame(Self, steamIDGameServer, unIPServer, usPortServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_RequestEncryptedAppTicket")]
		private static extern SteamAPICall_t _RequestEncryptedAppTicket(IntPtr self, IntPtr pDataToInclude, int cbDataToInclude);

		internal CallResult<EncryptedAppTicketResponse_t> RequestEncryptedAppTicket(IntPtr pDataToInclude, int cbDataToInclude)
		{
			SteamAPICall_t call = _RequestEncryptedAppTicket(Self, pDataToInclude, cbDataToInclude);
			return new CallResult<EncryptedAppTicketResponse_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_GetEncryptedAppTicket")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetEncryptedAppTicket(IntPtr self, IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket);

		internal bool GetEncryptedAppTicket(IntPtr pTicket, int cbMaxTicket, ref uint pcbTicket)
		{
			return _GetEncryptedAppTicket(Self, pTicket, cbMaxTicket, ref pcbTicket);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_GetGameBadgeLevel")]
		private static extern int _GetGameBadgeLevel(IntPtr self, int nSeries, [MarshalAs(UnmanagedType.U1)] bool bFoil);

		internal int GetGameBadgeLevel(int nSeries, [MarshalAs(UnmanagedType.U1)] bool bFoil)
		{
			return _GetGameBadgeLevel(Self, nSeries, bFoil);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_GetPlayerSteamLevel")]
		private static extern int _GetPlayerSteamLevel(IntPtr self);

		internal int GetPlayerSteamLevel()
		{
			return _GetPlayerSteamLevel(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_RequestStoreAuthURL")]
		private static extern SteamAPICall_t _RequestStoreAuthURL(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchRedirectURL);

		internal CallResult<StoreAuthURLResponse_t> RequestStoreAuthURL([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchRedirectURL)
		{
			SteamAPICall_t call = _RequestStoreAuthURL(Self, pchRedirectURL);
			return new CallResult<StoreAuthURLResponse_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_BIsPhoneVerified")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BIsPhoneVerified(IntPtr self);

		internal bool BIsPhoneVerified()
		{
			return _BIsPhoneVerified(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_BIsTwoFactorEnabled")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BIsTwoFactorEnabled(IntPtr self);

		internal bool BIsTwoFactorEnabled()
		{
			return _BIsTwoFactorEnabled(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_BIsPhoneIdentifying")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BIsPhoneIdentifying(IntPtr self);

		internal bool BIsPhoneIdentifying()
		{
			return _BIsPhoneIdentifying(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_BIsPhoneRequiringVerification")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BIsPhoneRequiringVerification(IntPtr self);

		internal bool BIsPhoneRequiringVerification()
		{
			return _BIsPhoneRequiringVerification(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_GetMarketEligibility")]
		private static extern SteamAPICall_t _GetMarketEligibility(IntPtr self);

		internal CallResult<MarketEligibilityResponse_t> GetMarketEligibility()
		{
			SteamAPICall_t call = _GetMarketEligibility(Self);
			return new CallResult<MarketEligibilityResponse_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamUser_GetDurationControl")]
		private static extern SteamAPICall_t _GetDurationControl(IntPtr self);

		internal CallResult<DurationControl_t> GetDurationControl()
		{
			SteamAPICall_t call = _GetDurationControl(Self);
			return new CallResult<DurationControl_t>(call, base.IsServer);
		}
	}
}
