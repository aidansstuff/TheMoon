using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	internal class ISteamGameSearch : SteamInterface
	{
		internal ISteamGameSearch(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamGameSearch_v001();

		public override IntPtr GetUserInterfacePointer()
		{
			return SteamAPI_SteamGameSearch_v001();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameSearch_AddGameSearchParams")]
		private static extern GameSearchErrorCode_t _AddGameSearchParams(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchKeyToFind, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchValuesToFind);

		internal GameSearchErrorCode_t AddGameSearchParams([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchKeyToFind, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchValuesToFind)
		{
			return _AddGameSearchParams(Self, pchKeyToFind, pchValuesToFind);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameSearch_SearchForGameWithLobby")]
		private static extern GameSearchErrorCode_t _SearchForGameWithLobby(IntPtr self, SteamId steamIDLobby, int nPlayerMin, int nPlayerMax);

		internal GameSearchErrorCode_t SearchForGameWithLobby(SteamId steamIDLobby, int nPlayerMin, int nPlayerMax)
		{
			return _SearchForGameWithLobby(Self, steamIDLobby, nPlayerMin, nPlayerMax);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameSearch_SearchForGameSolo")]
		private static extern GameSearchErrorCode_t _SearchForGameSolo(IntPtr self, int nPlayerMin, int nPlayerMax);

		internal GameSearchErrorCode_t SearchForGameSolo(int nPlayerMin, int nPlayerMax)
		{
			return _SearchForGameSolo(Self, nPlayerMin, nPlayerMax);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameSearch_AcceptGame")]
		private static extern GameSearchErrorCode_t _AcceptGame(IntPtr self);

		internal GameSearchErrorCode_t AcceptGame()
		{
			return _AcceptGame(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameSearch_DeclineGame")]
		private static extern GameSearchErrorCode_t _DeclineGame(IntPtr self);

		internal GameSearchErrorCode_t DeclineGame()
		{
			return _DeclineGame(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameSearch_RetrieveConnectionDetails")]
		private static extern GameSearchErrorCode_t _RetrieveConnectionDetails(IntPtr self, SteamId steamIDHost, IntPtr pchConnectionDetails, int cubConnectionDetails);

		internal GameSearchErrorCode_t RetrieveConnectionDetails(SteamId steamIDHost, out string pchConnectionDetails)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			GameSearchErrorCode_t result = _RetrieveConnectionDetails(Self, steamIDHost, intPtr, 32768);
			pchConnectionDetails = Helpers.MemoryToString(intPtr);
			return result;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameSearch_EndGameSearch")]
		private static extern GameSearchErrorCode_t _EndGameSearch(IntPtr self);

		internal GameSearchErrorCode_t EndGameSearch()
		{
			return _EndGameSearch(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameSearch_SetGameHostParams")]
		private static extern GameSearchErrorCode_t _SetGameHostParams(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchKey, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchValue);

		internal GameSearchErrorCode_t SetGameHostParams([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchKey, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchValue)
		{
			return _SetGameHostParams(Self, pchKey, pchValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameSearch_SetConnectionDetails")]
		private static extern GameSearchErrorCode_t _SetConnectionDetails(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchConnectionDetails, int cubConnectionDetails);

		internal GameSearchErrorCode_t SetConnectionDetails([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchConnectionDetails, int cubConnectionDetails)
		{
			return _SetConnectionDetails(Self, pchConnectionDetails, cubConnectionDetails);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameSearch_RequestPlayersForGame")]
		private static extern GameSearchErrorCode_t _RequestPlayersForGame(IntPtr self, int nPlayerMin, int nPlayerMax, int nMaxTeamSize);

		internal GameSearchErrorCode_t RequestPlayersForGame(int nPlayerMin, int nPlayerMax, int nMaxTeamSize)
		{
			return _RequestPlayersForGame(Self, nPlayerMin, nPlayerMax, nMaxTeamSize);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameSearch_HostConfirmGameStart")]
		private static extern GameSearchErrorCode_t _HostConfirmGameStart(IntPtr self, ulong ullUniqueGameID);

		internal GameSearchErrorCode_t HostConfirmGameStart(ulong ullUniqueGameID)
		{
			return _HostConfirmGameStart(Self, ullUniqueGameID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameSearch_CancelRequestPlayersForGame")]
		private static extern GameSearchErrorCode_t _CancelRequestPlayersForGame(IntPtr self);

		internal GameSearchErrorCode_t CancelRequestPlayersForGame()
		{
			return _CancelRequestPlayersForGame(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameSearch_SubmitPlayerResult")]
		private static extern GameSearchErrorCode_t _SubmitPlayerResult(IntPtr self, ulong ullUniqueGameID, SteamId steamIDPlayer, PlayerResult_t EPlayerResult);

		internal GameSearchErrorCode_t SubmitPlayerResult(ulong ullUniqueGameID, SteamId steamIDPlayer, PlayerResult_t EPlayerResult)
		{
			return _SubmitPlayerResult(Self, ullUniqueGameID, steamIDPlayer, EPlayerResult);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamGameSearch_EndGame")]
		private static extern GameSearchErrorCode_t _EndGame(IntPtr self, ulong ullUniqueGameID);

		internal GameSearchErrorCode_t EndGame(ulong ullUniqueGameID)
		{
			return _EndGame(Self, ullUniqueGameID);
		}
	}
}
