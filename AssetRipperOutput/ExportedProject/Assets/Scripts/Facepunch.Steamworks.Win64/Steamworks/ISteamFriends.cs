using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal class ISteamFriends : SteamInterface
	{
		internal ISteamFriends(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamFriends_v017();

		public override IntPtr GetUserInterfacePointer()
		{
			return SteamAPI_SteamFriends_v017();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetPersonaName")]
		private static extern Utf8StringPointer _GetPersonaName(IntPtr self);

		internal string GetPersonaName()
		{
			Utf8StringPointer utf8StringPointer = _GetPersonaName(Self);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_SetPersonaName")]
		private static extern SteamAPICall_t _SetPersonaName(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchPersonaName);

		internal CallResult<SetPersonaNameResponse_t> SetPersonaName([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchPersonaName)
		{
			SteamAPICall_t call = _SetPersonaName(Self, pchPersonaName);
			return new CallResult<SetPersonaNameResponse_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetPersonaState")]
		private static extern FriendState _GetPersonaState(IntPtr self);

		internal FriendState GetPersonaState()
		{
			return _GetPersonaState(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetFriendCount")]
		private static extern int _GetFriendCount(IntPtr self, int iFriendFlags);

		internal int GetFriendCount(int iFriendFlags)
		{
			return _GetFriendCount(Self, iFriendFlags);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetFriendByIndex")]
		private static extern SteamId _GetFriendByIndex(IntPtr self, int iFriend, int iFriendFlags);

		internal SteamId GetFriendByIndex(int iFriend, int iFriendFlags)
		{
			return _GetFriendByIndex(Self, iFriend, iFriendFlags);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetFriendRelationship")]
		private static extern Relationship _GetFriendRelationship(IntPtr self, SteamId steamIDFriend);

		internal Relationship GetFriendRelationship(SteamId steamIDFriend)
		{
			return _GetFriendRelationship(Self, steamIDFriend);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetFriendPersonaState")]
		private static extern FriendState _GetFriendPersonaState(IntPtr self, SteamId steamIDFriend);

		internal FriendState GetFriendPersonaState(SteamId steamIDFriend)
		{
			return _GetFriendPersonaState(Self, steamIDFriend);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetFriendPersonaName")]
		private static extern Utf8StringPointer _GetFriendPersonaName(IntPtr self, SteamId steamIDFriend);

		internal string GetFriendPersonaName(SteamId steamIDFriend)
		{
			Utf8StringPointer utf8StringPointer = _GetFriendPersonaName(Self, steamIDFriend);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetFriendGamePlayed")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetFriendGamePlayed(IntPtr self, SteamId steamIDFriend, ref FriendGameInfo_t pFriendGameInfo);

		internal bool GetFriendGamePlayed(SteamId steamIDFriend, ref FriendGameInfo_t pFriendGameInfo)
		{
			return _GetFriendGamePlayed(Self, steamIDFriend, ref pFriendGameInfo);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetFriendPersonaNameHistory")]
		private static extern Utf8StringPointer _GetFriendPersonaNameHistory(IntPtr self, SteamId steamIDFriend, int iPersonaName);

		internal string GetFriendPersonaNameHistory(SteamId steamIDFriend, int iPersonaName)
		{
			Utf8StringPointer utf8StringPointer = _GetFriendPersonaNameHistory(Self, steamIDFriend, iPersonaName);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetFriendSteamLevel")]
		private static extern int _GetFriendSteamLevel(IntPtr self, SteamId steamIDFriend);

		internal int GetFriendSteamLevel(SteamId steamIDFriend)
		{
			return _GetFriendSteamLevel(Self, steamIDFriend);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetPlayerNickname")]
		private static extern Utf8StringPointer _GetPlayerNickname(IntPtr self, SteamId steamIDPlayer);

		internal string GetPlayerNickname(SteamId steamIDPlayer)
		{
			Utf8StringPointer utf8StringPointer = _GetPlayerNickname(Self, steamIDPlayer);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetFriendsGroupCount")]
		private static extern int _GetFriendsGroupCount(IntPtr self);

		internal int GetFriendsGroupCount()
		{
			return _GetFriendsGroupCount(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetFriendsGroupIDByIndex")]
		private static extern FriendsGroupID_t _GetFriendsGroupIDByIndex(IntPtr self, int iFG);

		internal FriendsGroupID_t GetFriendsGroupIDByIndex(int iFG)
		{
			return _GetFriendsGroupIDByIndex(Self, iFG);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetFriendsGroupName")]
		private static extern Utf8StringPointer _GetFriendsGroupName(IntPtr self, FriendsGroupID_t friendsGroupID);

		internal string GetFriendsGroupName(FriendsGroupID_t friendsGroupID)
		{
			Utf8StringPointer utf8StringPointer = _GetFriendsGroupName(Self, friendsGroupID);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetFriendsGroupMembersCount")]
		private static extern int _GetFriendsGroupMembersCount(IntPtr self, FriendsGroupID_t friendsGroupID);

		internal int GetFriendsGroupMembersCount(FriendsGroupID_t friendsGroupID)
		{
			return _GetFriendsGroupMembersCount(Self, friendsGroupID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetFriendsGroupMembersList")]
		private static extern void _GetFriendsGroupMembersList(IntPtr self, FriendsGroupID_t friendsGroupID, [In][Out] SteamId[] pOutSteamIDMembers, int nMembersCount);

		internal void GetFriendsGroupMembersList(FriendsGroupID_t friendsGroupID, [In][Out] SteamId[] pOutSteamIDMembers, int nMembersCount)
		{
			_GetFriendsGroupMembersList(Self, friendsGroupID, pOutSteamIDMembers, nMembersCount);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_HasFriend")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _HasFriend(IntPtr self, SteamId steamIDFriend, int iFriendFlags);

		internal bool HasFriend(SteamId steamIDFriend, int iFriendFlags)
		{
			return _HasFriend(Self, steamIDFriend, iFriendFlags);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetClanCount")]
		private static extern int _GetClanCount(IntPtr self);

		internal int GetClanCount()
		{
			return _GetClanCount(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetClanByIndex")]
		private static extern SteamId _GetClanByIndex(IntPtr self, int iClan);

		internal SteamId GetClanByIndex(int iClan)
		{
			return _GetClanByIndex(Self, iClan);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetClanName")]
		private static extern Utf8StringPointer _GetClanName(IntPtr self, SteamId steamIDClan);

		internal string GetClanName(SteamId steamIDClan)
		{
			Utf8StringPointer utf8StringPointer = _GetClanName(Self, steamIDClan);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetClanTag")]
		private static extern Utf8StringPointer _GetClanTag(IntPtr self, SteamId steamIDClan);

		internal string GetClanTag(SteamId steamIDClan)
		{
			Utf8StringPointer utf8StringPointer = _GetClanTag(Self, steamIDClan);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetClanActivityCounts")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetClanActivityCounts(IntPtr self, SteamId steamIDClan, ref int pnOnline, ref int pnInGame, ref int pnChatting);

		internal bool GetClanActivityCounts(SteamId steamIDClan, ref int pnOnline, ref int pnInGame, ref int pnChatting)
		{
			return _GetClanActivityCounts(Self, steamIDClan, ref pnOnline, ref pnInGame, ref pnChatting);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_DownloadClanActivityCounts")]
		private static extern SteamAPICall_t _DownloadClanActivityCounts(IntPtr self, [In][Out] SteamId[] psteamIDClans, int cClansToRequest);

		internal CallResult<DownloadClanActivityCountsResult_t> DownloadClanActivityCounts([In][Out] SteamId[] psteamIDClans, int cClansToRequest)
		{
			SteamAPICall_t call = _DownloadClanActivityCounts(Self, psteamIDClans, cClansToRequest);
			return new CallResult<DownloadClanActivityCountsResult_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetFriendCountFromSource")]
		private static extern int _GetFriendCountFromSource(IntPtr self, SteamId steamIDSource);

		internal int GetFriendCountFromSource(SteamId steamIDSource)
		{
			return _GetFriendCountFromSource(Self, steamIDSource);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetFriendFromSourceByIndex")]
		private static extern SteamId _GetFriendFromSourceByIndex(IntPtr self, SteamId steamIDSource, int iFriend);

		internal SteamId GetFriendFromSourceByIndex(SteamId steamIDSource, int iFriend)
		{
			return _GetFriendFromSourceByIndex(Self, steamIDSource, iFriend);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_IsUserInSource")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _IsUserInSource(IntPtr self, SteamId steamIDUser, SteamId steamIDSource);

		internal bool IsUserInSource(SteamId steamIDUser, SteamId steamIDSource)
		{
			return _IsUserInSource(Self, steamIDUser, steamIDSource);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_SetInGameVoiceSpeaking")]
		private static extern void _SetInGameVoiceSpeaking(IntPtr self, SteamId steamIDUser, [MarshalAs(UnmanagedType.U1)] bool bSpeaking);

		internal void SetInGameVoiceSpeaking(SteamId steamIDUser, [MarshalAs(UnmanagedType.U1)] bool bSpeaking)
		{
			_SetInGameVoiceSpeaking(Self, steamIDUser, bSpeaking);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_ActivateGameOverlay")]
		private static extern void _ActivateGameOverlay(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchDialog);

		internal void ActivateGameOverlay([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchDialog)
		{
			_ActivateGameOverlay(Self, pchDialog);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_ActivateGameOverlayToUser")]
		private static extern void _ActivateGameOverlayToUser(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchDialog, SteamId steamID);

		internal void ActivateGameOverlayToUser([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchDialog, SteamId steamID)
		{
			_ActivateGameOverlayToUser(Self, pchDialog, steamID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_ActivateGameOverlayToWebPage")]
		private static extern void _ActivateGameOverlayToWebPage(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchURL, ActivateGameOverlayToWebPageMode eMode);

		internal void ActivateGameOverlayToWebPage([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchURL, ActivateGameOverlayToWebPageMode eMode)
		{
			_ActivateGameOverlayToWebPage(Self, pchURL, eMode);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_ActivateGameOverlayToStore")]
		private static extern void _ActivateGameOverlayToStore(IntPtr self, AppId nAppID, OverlayToStoreFlag eFlag);

		internal void ActivateGameOverlayToStore(AppId nAppID, OverlayToStoreFlag eFlag)
		{
			_ActivateGameOverlayToStore(Self, nAppID, eFlag);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_SetPlayedWith")]
		private static extern void _SetPlayedWith(IntPtr self, SteamId steamIDUserPlayedWith);

		internal void SetPlayedWith(SteamId steamIDUserPlayedWith)
		{
			_SetPlayedWith(Self, steamIDUserPlayedWith);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_ActivateGameOverlayInviteDialog")]
		private static extern void _ActivateGameOverlayInviteDialog(IntPtr self, SteamId steamIDLobby);

		internal void ActivateGameOverlayInviteDialog(SteamId steamIDLobby)
		{
			_ActivateGameOverlayInviteDialog(Self, steamIDLobby);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetSmallFriendAvatar")]
		private static extern int _GetSmallFriendAvatar(IntPtr self, SteamId steamIDFriend);

		internal int GetSmallFriendAvatar(SteamId steamIDFriend)
		{
			return _GetSmallFriendAvatar(Self, steamIDFriend);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetMediumFriendAvatar")]
		private static extern int _GetMediumFriendAvatar(IntPtr self, SteamId steamIDFriend);

		internal int GetMediumFriendAvatar(SteamId steamIDFriend)
		{
			return _GetMediumFriendAvatar(Self, steamIDFriend);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetLargeFriendAvatar")]
		private static extern int _GetLargeFriendAvatar(IntPtr self, SteamId steamIDFriend);

		internal int GetLargeFriendAvatar(SteamId steamIDFriend)
		{
			return _GetLargeFriendAvatar(Self, steamIDFriend);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_RequestUserInformation")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _RequestUserInformation(IntPtr self, SteamId steamIDUser, [MarshalAs(UnmanagedType.U1)] bool bRequireNameOnly);

		internal bool RequestUserInformation(SteamId steamIDUser, [MarshalAs(UnmanagedType.U1)] bool bRequireNameOnly)
		{
			return _RequestUserInformation(Self, steamIDUser, bRequireNameOnly);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_RequestClanOfficerList")]
		private static extern SteamAPICall_t _RequestClanOfficerList(IntPtr self, SteamId steamIDClan);

		internal CallResult<ClanOfficerListResponse_t> RequestClanOfficerList(SteamId steamIDClan)
		{
			SteamAPICall_t call = _RequestClanOfficerList(Self, steamIDClan);
			return new CallResult<ClanOfficerListResponse_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetClanOwner")]
		private static extern SteamId _GetClanOwner(IntPtr self, SteamId steamIDClan);

		internal SteamId GetClanOwner(SteamId steamIDClan)
		{
			return _GetClanOwner(Self, steamIDClan);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetClanOfficerCount")]
		private static extern int _GetClanOfficerCount(IntPtr self, SteamId steamIDClan);

		internal int GetClanOfficerCount(SteamId steamIDClan)
		{
			return _GetClanOfficerCount(Self, steamIDClan);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetClanOfficerByIndex")]
		private static extern SteamId _GetClanOfficerByIndex(IntPtr self, SteamId steamIDClan, int iOfficer);

		internal SteamId GetClanOfficerByIndex(SteamId steamIDClan, int iOfficer)
		{
			return _GetClanOfficerByIndex(Self, steamIDClan, iOfficer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetUserRestrictions")]
		private static extern uint _GetUserRestrictions(IntPtr self);

		internal uint GetUserRestrictions()
		{
			return _GetUserRestrictions(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_SetRichPresence")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetRichPresence(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchKey, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchValue);

		internal bool SetRichPresence([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchKey, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchValue)
		{
			return _SetRichPresence(Self, pchKey, pchValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_ClearRichPresence")]
		private static extern void _ClearRichPresence(IntPtr self);

		internal void ClearRichPresence()
		{
			_ClearRichPresence(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetFriendRichPresence")]
		private static extern Utf8StringPointer _GetFriendRichPresence(IntPtr self, SteamId steamIDFriend, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchKey);

		internal string GetFriendRichPresence(SteamId steamIDFriend, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchKey)
		{
			Utf8StringPointer utf8StringPointer = _GetFriendRichPresence(Self, steamIDFriend, pchKey);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetFriendRichPresenceKeyCount")]
		private static extern int _GetFriendRichPresenceKeyCount(IntPtr self, SteamId steamIDFriend);

		internal int GetFriendRichPresenceKeyCount(SteamId steamIDFriend)
		{
			return _GetFriendRichPresenceKeyCount(Self, steamIDFriend);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetFriendRichPresenceKeyByIndex")]
		private static extern Utf8StringPointer _GetFriendRichPresenceKeyByIndex(IntPtr self, SteamId steamIDFriend, int iKey);

		internal string GetFriendRichPresenceKeyByIndex(SteamId steamIDFriend, int iKey)
		{
			Utf8StringPointer utf8StringPointer = _GetFriendRichPresenceKeyByIndex(Self, steamIDFriend, iKey);
			return utf8StringPointer;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_RequestFriendRichPresence")]
		private static extern void _RequestFriendRichPresence(IntPtr self, SteamId steamIDFriend);

		internal void RequestFriendRichPresence(SteamId steamIDFriend)
		{
			_RequestFriendRichPresence(Self, steamIDFriend);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_InviteUserToGame")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _InviteUserToGame(IntPtr self, SteamId steamIDFriend, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchConnectString);

		internal bool InviteUserToGame(SteamId steamIDFriend, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchConnectString)
		{
			return _InviteUserToGame(Self, steamIDFriend, pchConnectString);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetCoplayFriendCount")]
		private static extern int _GetCoplayFriendCount(IntPtr self);

		internal int GetCoplayFriendCount()
		{
			return _GetCoplayFriendCount(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetCoplayFriend")]
		private static extern SteamId _GetCoplayFriend(IntPtr self, int iCoplayFriend);

		internal SteamId GetCoplayFriend(int iCoplayFriend)
		{
			return _GetCoplayFriend(Self, iCoplayFriend);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetFriendCoplayTime")]
		private static extern int _GetFriendCoplayTime(IntPtr self, SteamId steamIDFriend);

		internal int GetFriendCoplayTime(SteamId steamIDFriend)
		{
			return _GetFriendCoplayTime(Self, steamIDFriend);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetFriendCoplayGame")]
		private static extern AppId _GetFriendCoplayGame(IntPtr self, SteamId steamIDFriend);

		internal AppId GetFriendCoplayGame(SteamId steamIDFriend)
		{
			return _GetFriendCoplayGame(Self, steamIDFriend);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_JoinClanChatRoom")]
		private static extern SteamAPICall_t _JoinClanChatRoom(IntPtr self, SteamId steamIDClan);

		internal CallResult<JoinClanChatRoomCompletionResult_t> JoinClanChatRoom(SteamId steamIDClan)
		{
			SteamAPICall_t call = _JoinClanChatRoom(Self, steamIDClan);
			return new CallResult<JoinClanChatRoomCompletionResult_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_LeaveClanChatRoom")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _LeaveClanChatRoom(IntPtr self, SteamId steamIDClan);

		internal bool LeaveClanChatRoom(SteamId steamIDClan)
		{
			return _LeaveClanChatRoom(Self, steamIDClan);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetClanChatMemberCount")]
		private static extern int _GetClanChatMemberCount(IntPtr self, SteamId steamIDClan);

		internal int GetClanChatMemberCount(SteamId steamIDClan)
		{
			return _GetClanChatMemberCount(Self, steamIDClan);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetChatMemberByIndex")]
		private static extern SteamId _GetChatMemberByIndex(IntPtr self, SteamId steamIDClan, int iUser);

		internal SteamId GetChatMemberByIndex(SteamId steamIDClan, int iUser)
		{
			return _GetChatMemberByIndex(Self, steamIDClan, iUser);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_SendClanChatMessage")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SendClanChatMessage(IntPtr self, SteamId steamIDClanChat, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchText);

		internal bool SendClanChatMessage(SteamId steamIDClanChat, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchText)
		{
			return _SendClanChatMessage(Self, steamIDClanChat, pchText);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetClanChatMessage")]
		private static extern int _GetClanChatMessage(IntPtr self, SteamId steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, ref ChatEntryType peChatEntryType, ref SteamId psteamidChatter);

		internal int GetClanChatMessage(SteamId steamIDClanChat, int iMessage, IntPtr prgchText, int cchTextMax, ref ChatEntryType peChatEntryType, ref SteamId psteamidChatter)
		{
			return _GetClanChatMessage(Self, steamIDClanChat, iMessage, prgchText, cchTextMax, ref peChatEntryType, ref psteamidChatter);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_IsClanChatAdmin")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _IsClanChatAdmin(IntPtr self, SteamId steamIDClanChat, SteamId steamIDUser);

		internal bool IsClanChatAdmin(SteamId steamIDClanChat, SteamId steamIDUser)
		{
			return _IsClanChatAdmin(Self, steamIDClanChat, steamIDUser);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_IsClanChatWindowOpenInSteam")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _IsClanChatWindowOpenInSteam(IntPtr self, SteamId steamIDClanChat);

		internal bool IsClanChatWindowOpenInSteam(SteamId steamIDClanChat)
		{
			return _IsClanChatWindowOpenInSteam(Self, steamIDClanChat);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_OpenClanChatWindowInSteam")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _OpenClanChatWindowInSteam(IntPtr self, SteamId steamIDClanChat);

		internal bool OpenClanChatWindowInSteam(SteamId steamIDClanChat)
		{
			return _OpenClanChatWindowInSteam(Self, steamIDClanChat);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_CloseClanChatWindowInSteam")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _CloseClanChatWindowInSteam(IntPtr self, SteamId steamIDClanChat);

		internal bool CloseClanChatWindowInSteam(SteamId steamIDClanChat)
		{
			return _CloseClanChatWindowInSteam(Self, steamIDClanChat);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_SetListenForFriendsMessages")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetListenForFriendsMessages(IntPtr self, [MarshalAs(UnmanagedType.U1)] bool bInterceptEnabled);

		internal bool SetListenForFriendsMessages([MarshalAs(UnmanagedType.U1)] bool bInterceptEnabled)
		{
			return _SetListenForFriendsMessages(Self, bInterceptEnabled);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_ReplyToFriendMessage")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _ReplyToFriendMessage(IntPtr self, SteamId steamIDFriend, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchMsgToSend);

		internal bool ReplyToFriendMessage(SteamId steamIDFriend, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchMsgToSend)
		{
			return _ReplyToFriendMessage(Self, steamIDFriend, pchMsgToSend);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetFriendMessage")]
		private static extern int _GetFriendMessage(IntPtr self, SteamId steamIDFriend, int iMessageID, IntPtr pvData, int cubData, ref ChatEntryType peChatEntryType);

		internal int GetFriendMessage(SteamId steamIDFriend, int iMessageID, IntPtr pvData, int cubData, ref ChatEntryType peChatEntryType)
		{
			return _GetFriendMessage(Self, steamIDFriend, iMessageID, pvData, cubData, ref peChatEntryType);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetFollowerCount")]
		private static extern SteamAPICall_t _GetFollowerCount(IntPtr self, SteamId steamID);

		internal CallResult<FriendsGetFollowerCount_t> GetFollowerCount(SteamId steamID)
		{
			SteamAPICall_t call = _GetFollowerCount(Self, steamID);
			return new CallResult<FriendsGetFollowerCount_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_IsFollowing")]
		private static extern SteamAPICall_t _IsFollowing(IntPtr self, SteamId steamID);

		internal CallResult<FriendsIsFollowing_t> IsFollowing(SteamId steamID)
		{
			SteamAPICall_t call = _IsFollowing(Self, steamID);
			return new CallResult<FriendsIsFollowing_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_EnumerateFollowingList")]
		private static extern SteamAPICall_t _EnumerateFollowingList(IntPtr self, uint unStartIndex);

		internal CallResult<FriendsEnumerateFollowingList_t> EnumerateFollowingList(uint unStartIndex)
		{
			SteamAPICall_t call = _EnumerateFollowingList(Self, unStartIndex);
			return new CallResult<FriendsEnumerateFollowingList_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_IsClanPublic")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _IsClanPublic(IntPtr self, SteamId steamIDClan);

		internal bool IsClanPublic(SteamId steamIDClan)
		{
			return _IsClanPublic(Self, steamIDClan);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_IsClanOfficialGameGroup")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _IsClanOfficialGameGroup(IntPtr self, SteamId steamIDClan);

		internal bool IsClanOfficialGameGroup(SteamId steamIDClan)
		{
			return _IsClanOfficialGameGroup(Self, steamIDClan);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_GetNumChatsWithUnreadPriorityMessages")]
		private static extern int _GetNumChatsWithUnreadPriorityMessages(IntPtr self);

		internal int GetNumChatsWithUnreadPriorityMessages()
		{
			return _GetNumChatsWithUnreadPriorityMessages(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamFriends_ActivateGameOverlayRemotePlayTogetherInviteDialog")]
		private static extern void _ActivateGameOverlayRemotePlayTogetherInviteDialog(IntPtr self, SteamId steamIDLobby);

		internal void ActivateGameOverlayRemotePlayTogetherInviteDialog(SteamId steamIDLobby)
		{
			_ActivateGameOverlayRemotePlayTogetherInviteDialog(Self, steamIDLobby);
		}
	}
}
