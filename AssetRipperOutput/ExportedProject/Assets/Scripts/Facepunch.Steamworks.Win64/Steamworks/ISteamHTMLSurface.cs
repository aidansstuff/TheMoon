using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal class ISteamHTMLSurface : SteamInterface
	{
		internal ISteamHTMLSurface(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamHTMLSurface_v005();

		public override IntPtr GetUserInterfacePointer()
		{
			return SteamAPI_SteamHTMLSurface_v005();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_Init")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _Init(IntPtr self);

		internal bool Init()
		{
			return _Init(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_Shutdown")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _Shutdown(IntPtr self);

		internal bool Shutdown()
		{
			return _Shutdown(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_CreateBrowser")]
		private static extern SteamAPICall_t _CreateBrowser(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchUserAgent, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchUserCSS);

		internal CallResult<HTML_BrowserReady_t> CreateBrowser([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchUserAgent, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchUserCSS)
		{
			SteamAPICall_t call = _CreateBrowser(Self, pchUserAgent, pchUserCSS);
			return new CallResult<HTML_BrowserReady_t>(call, base.IsServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_RemoveBrowser")]
		private static extern void _RemoveBrowser(IntPtr self, HHTMLBrowser unBrowserHandle);

		internal void RemoveBrowser(HHTMLBrowser unBrowserHandle)
		{
			_RemoveBrowser(Self, unBrowserHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_LoadURL")]
		private static extern void _LoadURL(IntPtr self, HHTMLBrowser unBrowserHandle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchURL, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchPostData);

		internal void LoadURL(HHTMLBrowser unBrowserHandle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchURL, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchPostData)
		{
			_LoadURL(Self, unBrowserHandle, pchURL, pchPostData);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_SetSize")]
		private static extern void _SetSize(IntPtr self, HHTMLBrowser unBrowserHandle, uint unWidth, uint unHeight);

		internal void SetSize(HHTMLBrowser unBrowserHandle, uint unWidth, uint unHeight)
		{
			_SetSize(Self, unBrowserHandle, unWidth, unHeight);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_StopLoad")]
		private static extern void _StopLoad(IntPtr self, HHTMLBrowser unBrowserHandle);

		internal void StopLoad(HHTMLBrowser unBrowserHandle)
		{
			_StopLoad(Self, unBrowserHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_Reload")]
		private static extern void _Reload(IntPtr self, HHTMLBrowser unBrowserHandle);

		internal void Reload(HHTMLBrowser unBrowserHandle)
		{
			_Reload(Self, unBrowserHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_GoBack")]
		private static extern void _GoBack(IntPtr self, HHTMLBrowser unBrowserHandle);

		internal void GoBack(HHTMLBrowser unBrowserHandle)
		{
			_GoBack(Self, unBrowserHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_GoForward")]
		private static extern void _GoForward(IntPtr self, HHTMLBrowser unBrowserHandle);

		internal void GoForward(HHTMLBrowser unBrowserHandle)
		{
			_GoForward(Self, unBrowserHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_AddHeader")]
		private static extern void _AddHeader(IntPtr self, HHTMLBrowser unBrowserHandle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchKey, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchValue);

		internal void AddHeader(HHTMLBrowser unBrowserHandle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchKey, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchValue)
		{
			_AddHeader(Self, unBrowserHandle, pchKey, pchValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_ExecuteJavascript")]
		private static extern void _ExecuteJavascript(IntPtr self, HHTMLBrowser unBrowserHandle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchScript);

		internal void ExecuteJavascript(HHTMLBrowser unBrowserHandle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchScript)
		{
			_ExecuteJavascript(Self, unBrowserHandle, pchScript);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_MouseUp")]
		private static extern void _MouseUp(IntPtr self, HHTMLBrowser unBrowserHandle, IntPtr eMouseButton);

		internal void MouseUp(HHTMLBrowser unBrowserHandle, IntPtr eMouseButton)
		{
			_MouseUp(Self, unBrowserHandle, eMouseButton);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_MouseDown")]
		private static extern void _MouseDown(IntPtr self, HHTMLBrowser unBrowserHandle, IntPtr eMouseButton);

		internal void MouseDown(HHTMLBrowser unBrowserHandle, IntPtr eMouseButton)
		{
			_MouseDown(Self, unBrowserHandle, eMouseButton);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_MouseDoubleClick")]
		private static extern void _MouseDoubleClick(IntPtr self, HHTMLBrowser unBrowserHandle, IntPtr eMouseButton);

		internal void MouseDoubleClick(HHTMLBrowser unBrowserHandle, IntPtr eMouseButton)
		{
			_MouseDoubleClick(Self, unBrowserHandle, eMouseButton);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_MouseMove")]
		private static extern void _MouseMove(IntPtr self, HHTMLBrowser unBrowserHandle, int x, int y);

		internal void MouseMove(HHTMLBrowser unBrowserHandle, int x, int y)
		{
			_MouseMove(Self, unBrowserHandle, x, y);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_MouseWheel")]
		private static extern void _MouseWheel(IntPtr self, HHTMLBrowser unBrowserHandle, int nDelta);

		internal void MouseWheel(HHTMLBrowser unBrowserHandle, int nDelta)
		{
			_MouseWheel(Self, unBrowserHandle, nDelta);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_KeyDown")]
		private static extern void _KeyDown(IntPtr self, HHTMLBrowser unBrowserHandle, uint nNativeKeyCode, IntPtr eHTMLKeyModifiers, [MarshalAs(UnmanagedType.U1)] bool bIsSystemKey);

		internal void KeyDown(HHTMLBrowser unBrowserHandle, uint nNativeKeyCode, IntPtr eHTMLKeyModifiers, [MarshalAs(UnmanagedType.U1)] bool bIsSystemKey)
		{
			_KeyDown(Self, unBrowserHandle, nNativeKeyCode, eHTMLKeyModifiers, bIsSystemKey);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_KeyUp")]
		private static extern void _KeyUp(IntPtr self, HHTMLBrowser unBrowserHandle, uint nNativeKeyCode, IntPtr eHTMLKeyModifiers);

		internal void KeyUp(HHTMLBrowser unBrowserHandle, uint nNativeKeyCode, IntPtr eHTMLKeyModifiers)
		{
			_KeyUp(Self, unBrowserHandle, nNativeKeyCode, eHTMLKeyModifiers);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_KeyChar")]
		private static extern void _KeyChar(IntPtr self, HHTMLBrowser unBrowserHandle, uint cUnicodeChar, IntPtr eHTMLKeyModifiers);

		internal void KeyChar(HHTMLBrowser unBrowserHandle, uint cUnicodeChar, IntPtr eHTMLKeyModifiers)
		{
			_KeyChar(Self, unBrowserHandle, cUnicodeChar, eHTMLKeyModifiers);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_SetHorizontalScroll")]
		private static extern void _SetHorizontalScroll(IntPtr self, HHTMLBrowser unBrowserHandle, uint nAbsolutePixelScroll);

		internal void SetHorizontalScroll(HHTMLBrowser unBrowserHandle, uint nAbsolutePixelScroll)
		{
			_SetHorizontalScroll(Self, unBrowserHandle, nAbsolutePixelScroll);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_SetVerticalScroll")]
		private static extern void _SetVerticalScroll(IntPtr self, HHTMLBrowser unBrowserHandle, uint nAbsolutePixelScroll);

		internal void SetVerticalScroll(HHTMLBrowser unBrowserHandle, uint nAbsolutePixelScroll)
		{
			_SetVerticalScroll(Self, unBrowserHandle, nAbsolutePixelScroll);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_SetKeyFocus")]
		private static extern void _SetKeyFocus(IntPtr self, HHTMLBrowser unBrowserHandle, [MarshalAs(UnmanagedType.U1)] bool bHasKeyFocus);

		internal void SetKeyFocus(HHTMLBrowser unBrowserHandle, [MarshalAs(UnmanagedType.U1)] bool bHasKeyFocus)
		{
			_SetKeyFocus(Self, unBrowserHandle, bHasKeyFocus);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_ViewSource")]
		private static extern void _ViewSource(IntPtr self, HHTMLBrowser unBrowserHandle);

		internal void ViewSource(HHTMLBrowser unBrowserHandle)
		{
			_ViewSource(Self, unBrowserHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_CopyToClipboard")]
		private static extern void _CopyToClipboard(IntPtr self, HHTMLBrowser unBrowserHandle);

		internal void CopyToClipboard(HHTMLBrowser unBrowserHandle)
		{
			_CopyToClipboard(Self, unBrowserHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_PasteFromClipboard")]
		private static extern void _PasteFromClipboard(IntPtr self, HHTMLBrowser unBrowserHandle);

		internal void PasteFromClipboard(HHTMLBrowser unBrowserHandle)
		{
			_PasteFromClipboard(Self, unBrowserHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_Find")]
		private static extern void _Find(IntPtr self, HHTMLBrowser unBrowserHandle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchSearchStr, [MarshalAs(UnmanagedType.U1)] bool bCurrentlyInFind, [MarshalAs(UnmanagedType.U1)] bool bReverse);

		internal void Find(HHTMLBrowser unBrowserHandle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchSearchStr, [MarshalAs(UnmanagedType.U1)] bool bCurrentlyInFind, [MarshalAs(UnmanagedType.U1)] bool bReverse)
		{
			_Find(Self, unBrowserHandle, pchSearchStr, bCurrentlyInFind, bReverse);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_StopFind")]
		private static extern void _StopFind(IntPtr self, HHTMLBrowser unBrowserHandle);

		internal void StopFind(HHTMLBrowser unBrowserHandle)
		{
			_StopFind(Self, unBrowserHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_GetLinkAtPosition")]
		private static extern void _GetLinkAtPosition(IntPtr self, HHTMLBrowser unBrowserHandle, int x, int y);

		internal void GetLinkAtPosition(HHTMLBrowser unBrowserHandle, int x, int y)
		{
			_GetLinkAtPosition(Self, unBrowserHandle, x, y);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_SetCookie")]
		private static extern void _SetCookie(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchHostname, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchKey, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchValue, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchPath, RTime32 nExpires, [MarshalAs(UnmanagedType.U1)] bool bSecure, [MarshalAs(UnmanagedType.U1)] bool bHTTPOnly);

		internal void SetCookie([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchHostname, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchKey, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchValue, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchPath, RTime32 nExpires, [MarshalAs(UnmanagedType.U1)] bool bSecure, [MarshalAs(UnmanagedType.U1)] bool bHTTPOnly)
		{
			_SetCookie(Self, pchHostname, pchKey, pchValue, pchPath, nExpires, bSecure, bHTTPOnly);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_SetPageScaleFactor")]
		private static extern void _SetPageScaleFactor(IntPtr self, HHTMLBrowser unBrowserHandle, float flZoom, int nPointX, int nPointY);

		internal void SetPageScaleFactor(HHTMLBrowser unBrowserHandle, float flZoom, int nPointX, int nPointY)
		{
			_SetPageScaleFactor(Self, unBrowserHandle, flZoom, nPointX, nPointY);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_SetBackgroundMode")]
		private static extern void _SetBackgroundMode(IntPtr self, HHTMLBrowser unBrowserHandle, [MarshalAs(UnmanagedType.U1)] bool bBackgroundMode);

		internal void SetBackgroundMode(HHTMLBrowser unBrowserHandle, [MarshalAs(UnmanagedType.U1)] bool bBackgroundMode)
		{
			_SetBackgroundMode(Self, unBrowserHandle, bBackgroundMode);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_SetDPIScalingFactor")]
		private static extern void _SetDPIScalingFactor(IntPtr self, HHTMLBrowser unBrowserHandle, float flDPIScaling);

		internal void SetDPIScalingFactor(HHTMLBrowser unBrowserHandle, float flDPIScaling)
		{
			_SetDPIScalingFactor(Self, unBrowserHandle, flDPIScaling);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_OpenDeveloperTools")]
		private static extern void _OpenDeveloperTools(IntPtr self, HHTMLBrowser unBrowserHandle);

		internal void OpenDeveloperTools(HHTMLBrowser unBrowserHandle)
		{
			_OpenDeveloperTools(Self, unBrowserHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_AllowStartRequest")]
		private static extern void _AllowStartRequest(IntPtr self, HHTMLBrowser unBrowserHandle, [MarshalAs(UnmanagedType.U1)] bool bAllowed);

		internal void AllowStartRequest(HHTMLBrowser unBrowserHandle, [MarshalAs(UnmanagedType.U1)] bool bAllowed)
		{
			_AllowStartRequest(Self, unBrowserHandle, bAllowed);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_JSDialogResponse")]
		private static extern void _JSDialogResponse(IntPtr self, HHTMLBrowser unBrowserHandle, [MarshalAs(UnmanagedType.U1)] bool bResult);

		internal void JSDialogResponse(HHTMLBrowser unBrowserHandle, [MarshalAs(UnmanagedType.U1)] bool bResult)
		{
			_JSDialogResponse(Self, unBrowserHandle, bResult);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTMLSurface_FileLoadDialogResponse")]
		private static extern void _FileLoadDialogResponse(IntPtr self, HHTMLBrowser unBrowserHandle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchSelectedFiles);

		internal void FileLoadDialogResponse(HHTMLBrowser unBrowserHandle, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchSelectedFiles)
		{
			_FileLoadDialogResponse(Self, unBrowserHandle, pchSelectedFiles);
		}
	}
}
