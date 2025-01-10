using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal class ISteamScreenshots : SteamInterface
	{
		internal ISteamScreenshots(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamScreenshots_v003();

		public override IntPtr GetUserInterfacePointer()
		{
			return SteamAPI_SteamScreenshots_v003();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamScreenshots_WriteScreenshot")]
		private static extern ScreenshotHandle _WriteScreenshot(IntPtr self, IntPtr pubRGB, uint cubRGB, int nWidth, int nHeight);

		internal ScreenshotHandle WriteScreenshot(IntPtr pubRGB, uint cubRGB, int nWidth, int nHeight)
		{
			return _WriteScreenshot(Self, pubRGB, cubRGB, nWidth, nHeight);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamScreenshots_AddScreenshotToLibrary")]
		private static extern ScreenshotHandle _AddScreenshotToLibrary(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFilename, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchThumbnailFilename, int nWidth, int nHeight);

		internal ScreenshotHandle AddScreenshotToLibrary([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFilename, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchThumbnailFilename, int nWidth, int nHeight)
		{
			return _AddScreenshotToLibrary(Self, pchFilename, pchThumbnailFilename, nWidth, nHeight);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamScreenshots_TriggerScreenshot")]
		private static extern void _TriggerScreenshot(IntPtr self);

		internal void TriggerScreenshot()
		{
			_TriggerScreenshot(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamScreenshots_HookScreenshots")]
		private static extern void _HookScreenshots(IntPtr self, [MarshalAs(UnmanagedType.U1)] bool bHook);

		internal void HookScreenshots([MarshalAs(UnmanagedType.U1)] bool bHook)
		{
			_HookScreenshots(Self, bHook);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamScreenshots_SetLocation")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetLocation(IntPtr self, ScreenshotHandle hScreenshot, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchLocation);

		internal bool SetLocation(ScreenshotHandle hScreenshot, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchLocation)
		{
			return _SetLocation(Self, hScreenshot, pchLocation);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamScreenshots_TagUser")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _TagUser(IntPtr self, ScreenshotHandle hScreenshot, SteamId steamID);

		internal bool TagUser(ScreenshotHandle hScreenshot, SteamId steamID)
		{
			return _TagUser(Self, hScreenshot, steamID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamScreenshots_TagPublishedFile")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _TagPublishedFile(IntPtr self, ScreenshotHandle hScreenshot, PublishedFileId unPublishedFileID);

		internal bool TagPublishedFile(ScreenshotHandle hScreenshot, PublishedFileId unPublishedFileID)
		{
			return _TagPublishedFile(Self, hScreenshot, unPublishedFileID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamScreenshots_IsScreenshotsHooked")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _IsScreenshotsHooked(IntPtr self);

		internal bool IsScreenshotsHooked()
		{
			return _IsScreenshotsHooked(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamScreenshots_AddVRScreenshotToLibrary")]
		private static extern ScreenshotHandle _AddVRScreenshotToLibrary(IntPtr self, VRScreenshotType eType, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFilename, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVRFilename);

		internal ScreenshotHandle AddVRScreenshotToLibrary(VRScreenshotType eType, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchFilename, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchVRFilename)
		{
			return _AddVRScreenshotToLibrary(Self, eType, pchFilename, pchVRFilename);
		}
	}
}
