using System;
using System.Runtime.InteropServices;

namespace Steamworks
{
	internal class ISteamVideo : SteamInterface
	{
		internal ISteamVideo(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamVideo_v002();

		public override IntPtr GetUserInterfacePointer()
		{
			return SteamAPI_SteamVideo_v002();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamVideo_GetVideoURL")]
		private static extern void _GetVideoURL(IntPtr self, AppId unVideoAppID);

		internal void GetVideoURL(AppId unVideoAppID)
		{
			_GetVideoURL(Self, unVideoAppID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamVideo_IsBroadcasting")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _IsBroadcasting(IntPtr self, ref int pnNumViewers);

		internal bool IsBroadcasting(ref int pnNumViewers)
		{
			return _IsBroadcasting(Self, ref pnNumViewers);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamVideo_GetOPFSettings")]
		private static extern void _GetOPFSettings(IntPtr self, AppId unVideoAppID);

		internal void GetOPFSettings(AppId unVideoAppID)
		{
			_GetOPFSettings(Self, unVideoAppID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamVideo_GetOPFStringForApp")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetOPFStringForApp(IntPtr self, AppId unVideoAppID, IntPtr pchBuffer, ref int pnBufferSize);

		internal bool GetOPFStringForApp(AppId unVideoAppID, out string pchBuffer, ref int pnBufferSize)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			bool result = _GetOPFStringForApp(Self, unVideoAppID, intPtr, ref pnBufferSize);
			pchBuffer = Helpers.MemoryToString(intPtr);
			return result;
		}
	}
}
