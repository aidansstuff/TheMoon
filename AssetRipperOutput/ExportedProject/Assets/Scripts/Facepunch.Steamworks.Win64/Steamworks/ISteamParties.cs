using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal class ISteamParties : SteamInterface
	{
		internal ISteamParties(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamParties_v002();

		public override IntPtr GetUserInterfacePointer()
		{
			return SteamAPI_SteamParties_v002();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamParties_GetNumActiveBeacons")]
		private static extern uint _GetNumActiveBeacons(IntPtr self);

		internal uint GetNumActiveBeacons()
		{
			return _GetNumActiveBeacons(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamParties_GetBeaconByIndex")]
		private static extern PartyBeaconID_t _GetBeaconByIndex(IntPtr self, uint unIndex);

		internal PartyBeaconID_t GetBeaconByIndex(uint unIndex)
		{
			return _GetBeaconByIndex(Self, unIndex);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamParties_GetBeaconDetails")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetBeaconDetails(IntPtr self, PartyBeaconID_t ulBeaconID, ref SteamId pSteamIDBeaconOwner, ref SteamPartyBeaconLocation_t pLocation, IntPtr pchMetadata, int cchMetadata);

		internal bool GetBeaconDetails(PartyBeaconID_t ulBeaconID, ref SteamId pSteamIDBeaconOwner, ref SteamPartyBeaconLocation_t pLocation, out string pchMetadata)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			bool result = _GetBeaconDetails(Self, ulBeaconID, ref pSteamIDBeaconOwner, ref pLocation, intPtr, 32768);
			pchMetadata = Helpers.MemoryToString(intPtr);
			return result;
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamParties_JoinParty")]
		private static extern SteamAPICall_t _JoinParty(IntPtr self, PartyBeaconID_t ulBeaconID);

		internal CallResult<JoinPartyCallback_t> JoinParty(PartyBeaconID_t ulBeaconID)
		{
			SteamAPICall_t call = _JoinParty(Self, ulBeaconID);
			return new CallResult<JoinPartyCallback_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamParties_GetNumAvailableBeaconLocations")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetNumAvailableBeaconLocations(IntPtr self, ref uint puNumLocations);

		internal bool GetNumAvailableBeaconLocations(ref uint puNumLocations)
		{
			return _GetNumAvailableBeaconLocations(Self, ref puNumLocations);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamParties_GetAvailableBeaconLocations")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetAvailableBeaconLocations(IntPtr self, ref SteamPartyBeaconLocation_t pLocationList, uint uMaxNumLocations);

		internal bool GetAvailableBeaconLocations(ref SteamPartyBeaconLocation_t pLocationList, uint uMaxNumLocations)
		{
			return _GetAvailableBeaconLocations(Self, ref pLocationList, uMaxNumLocations);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamParties_CreateBeacon")]
		private static extern SteamAPICall_t _CreateBeacon(IntPtr self, uint unOpenSlots, ref SteamPartyBeaconLocation_t pBeaconLocation, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchConnectString, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchMetadata);

		internal CallResult<CreateBeaconCallback_t> CreateBeacon(uint unOpenSlots, SteamPartyBeaconLocation_t pBeaconLocation, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchConnectString, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchMetadata)
		{
			SteamAPICall_t call = _CreateBeacon(Self, unOpenSlots, ref pBeaconLocation, pchConnectString, pchMetadata);
			return new CallResult<CreateBeaconCallback_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamParties_OnReservationCompleted")]
		private static extern void _OnReservationCompleted(IntPtr self, PartyBeaconID_t ulBeacon, SteamId steamIDUser);

		internal void OnReservationCompleted(PartyBeaconID_t ulBeacon, SteamId steamIDUser)
		{
			_OnReservationCompleted(Self, ulBeacon, steamIDUser);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamParties_CancelReservation")]
		private static extern void _CancelReservation(IntPtr self, PartyBeaconID_t ulBeacon, SteamId steamIDUser);

		internal void CancelReservation(PartyBeaconID_t ulBeacon, SteamId steamIDUser)
		{
			_CancelReservation(Self, ulBeacon, steamIDUser);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamParties_ChangeNumOpenSlots")]
		private static extern SteamAPICall_t _ChangeNumOpenSlots(IntPtr self, PartyBeaconID_t ulBeacon, uint unOpenSlots);

		internal CallResult<ChangeNumOpenSlotsCallback_t> ChangeNumOpenSlots(PartyBeaconID_t ulBeacon, uint unOpenSlots)
		{
			SteamAPICall_t call = _ChangeNumOpenSlots(Self, ulBeacon, unOpenSlots);
			return new CallResult<ChangeNumOpenSlotsCallback_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamParties_DestroyBeacon")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _DestroyBeacon(IntPtr self, PartyBeaconID_t ulBeacon);

		internal bool DestroyBeacon(PartyBeaconID_t ulBeacon)
		{
			return _DestroyBeacon(Self, ulBeacon);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamParties_GetBeaconLocationData")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetBeaconLocationData(IntPtr self, SteamPartyBeaconLocation_t BeaconLocation, SteamPartyBeaconLocationData eData, IntPtr pchDataStringOut, int cchDataStringOut);

		internal bool GetBeaconLocationData(SteamPartyBeaconLocation_t BeaconLocation, SteamPartyBeaconLocationData eData, out string pchDataStringOut)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			bool result = _GetBeaconLocationData(Self, BeaconLocation, eData, intPtr, 32768);
			pchDataStringOut = Helpers.MemoryToString(intPtr);
			return result;
		}
	}
}
