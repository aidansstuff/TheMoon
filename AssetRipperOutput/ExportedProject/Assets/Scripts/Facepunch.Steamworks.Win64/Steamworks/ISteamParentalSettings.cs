using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	internal class ISteamParentalSettings : SteamInterface
	{
		internal ISteamParentalSettings(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamParentalSettings_v001();

		public override IntPtr GetUserInterfacePointer()
		{
			return SteamAPI_SteamParentalSettings_v001();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamParentalSettings_BIsParentalLockEnabled")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BIsParentalLockEnabled(IntPtr self);

		internal bool BIsParentalLockEnabled()
		{
			return _BIsParentalLockEnabled(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamParentalSettings_BIsParentalLockLocked")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BIsParentalLockLocked(IntPtr self);

		internal bool BIsParentalLockLocked()
		{
			return _BIsParentalLockLocked(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamParentalSettings_BIsAppBlocked")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BIsAppBlocked(IntPtr self, AppId nAppID);

		internal bool BIsAppBlocked(AppId nAppID)
		{
			return _BIsAppBlocked(Self, nAppID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamParentalSettings_BIsAppInBlockList")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BIsAppInBlockList(IntPtr self, AppId nAppID);

		internal bool BIsAppInBlockList(AppId nAppID)
		{
			return _BIsAppInBlockList(Self, nAppID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamParentalSettings_BIsFeatureBlocked")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BIsFeatureBlocked(IntPtr self, ParentalFeature eFeature);

		internal bool BIsFeatureBlocked(ParentalFeature eFeature)
		{
			return _BIsFeatureBlocked(Self, eFeature);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamParentalSettings_BIsFeatureInBlockList")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _BIsFeatureInBlockList(IntPtr self, ParentalFeature eFeature);

		internal bool BIsFeatureInBlockList(ParentalFeature eFeature)
		{
			return _BIsFeatureInBlockList(Self, eFeature);
		}
	}
}
