using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal class ISteamHTTP : SteamInterface
	{
		internal ISteamHTTP(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamHTTP_v003();

		public override IntPtr GetUserInterfacePointer()
		{
			return SteamAPI_SteamHTTP_v003();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamGameServerHTTP_v003();

		public override IntPtr GetServerInterfacePointer()
		{
			return SteamAPI_SteamGameServerHTTP_v003();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_CreateHTTPRequest")]
		private static extern HTTPRequestHandle _CreateHTTPRequest(IntPtr self, HTTPMethod eHTTPRequestMethod, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchAbsoluteURL);

		internal HTTPRequestHandle CreateHTTPRequest(HTTPMethod eHTTPRequestMethod, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchAbsoluteURL)
		{
			return _CreateHTTPRequest(Self, eHTTPRequestMethod, pchAbsoluteURL);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_SetHTTPRequestContextValue")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetHTTPRequestContextValue(IntPtr self, HTTPRequestHandle hRequest, ulong ulContextValue);

		internal bool SetHTTPRequestContextValue(HTTPRequestHandle hRequest, ulong ulContextValue)
		{
			return _SetHTTPRequestContextValue(Self, hRequest, ulContextValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_SetHTTPRequestNetworkActivityTimeout")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetHTTPRequestNetworkActivityTimeout(IntPtr self, HTTPRequestHandle hRequest, uint unTimeoutSeconds);

		internal bool SetHTTPRequestNetworkActivityTimeout(HTTPRequestHandle hRequest, uint unTimeoutSeconds)
		{
			return _SetHTTPRequestNetworkActivityTimeout(Self, hRequest, unTimeoutSeconds);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_SetHTTPRequestHeaderValue")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetHTTPRequestHeaderValue(IntPtr self, HTTPRequestHandle hRequest, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchHeaderName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchHeaderValue);

		internal bool SetHTTPRequestHeaderValue(HTTPRequestHandle hRequest, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchHeaderName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchHeaderValue)
		{
			return _SetHTTPRequestHeaderValue(Self, hRequest, pchHeaderName, pchHeaderValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_SetHTTPRequestGetOrPostParameter")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetHTTPRequestGetOrPostParameter(IntPtr self, HTTPRequestHandle hRequest, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchParamName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchParamValue);

		internal bool SetHTTPRequestGetOrPostParameter(HTTPRequestHandle hRequest, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchParamName, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchParamValue)
		{
			return _SetHTTPRequestGetOrPostParameter(Self, hRequest, pchParamName, pchParamValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_SendHTTPRequest")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SendHTTPRequest(IntPtr self, HTTPRequestHandle hRequest, ref SteamAPICall_t pCallHandle);

		internal bool SendHTTPRequest(HTTPRequestHandle hRequest, ref SteamAPICall_t pCallHandle)
		{
			return _SendHTTPRequest(Self, hRequest, ref pCallHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_SendHTTPRequestAndStreamResponse")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SendHTTPRequestAndStreamResponse(IntPtr self, HTTPRequestHandle hRequest, ref SteamAPICall_t pCallHandle);

		internal bool SendHTTPRequestAndStreamResponse(HTTPRequestHandle hRequest, ref SteamAPICall_t pCallHandle)
		{
			return _SendHTTPRequestAndStreamResponse(Self, hRequest, ref pCallHandle);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_DeferHTTPRequest")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _DeferHTTPRequest(IntPtr self, HTTPRequestHandle hRequest);

		internal bool DeferHTTPRequest(HTTPRequestHandle hRequest)
		{
			return _DeferHTTPRequest(Self, hRequest);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_PrioritizeHTTPRequest")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _PrioritizeHTTPRequest(IntPtr self, HTTPRequestHandle hRequest);

		internal bool PrioritizeHTTPRequest(HTTPRequestHandle hRequest)
		{
			return _PrioritizeHTTPRequest(Self, hRequest);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_GetHTTPResponseHeaderSize")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetHTTPResponseHeaderSize(IntPtr self, HTTPRequestHandle hRequest, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchHeaderName, ref uint unResponseHeaderSize);

		internal bool GetHTTPResponseHeaderSize(HTTPRequestHandle hRequest, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchHeaderName, ref uint unResponseHeaderSize)
		{
			return _GetHTTPResponseHeaderSize(Self, hRequest, pchHeaderName, ref unResponseHeaderSize);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_GetHTTPResponseHeaderValue")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetHTTPResponseHeaderValue(IntPtr self, HTTPRequestHandle hRequest, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchHeaderName, ref byte pHeaderValueBuffer, uint unBufferSize);

		internal bool GetHTTPResponseHeaderValue(HTTPRequestHandle hRequest, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchHeaderName, ref byte pHeaderValueBuffer, uint unBufferSize)
		{
			return _GetHTTPResponseHeaderValue(Self, hRequest, pchHeaderName, ref pHeaderValueBuffer, unBufferSize);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_GetHTTPResponseBodySize")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetHTTPResponseBodySize(IntPtr self, HTTPRequestHandle hRequest, ref uint unBodySize);

		internal bool GetHTTPResponseBodySize(HTTPRequestHandle hRequest, ref uint unBodySize)
		{
			return _GetHTTPResponseBodySize(Self, hRequest, ref unBodySize);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_GetHTTPResponseBodyData")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetHTTPResponseBodyData(IntPtr self, HTTPRequestHandle hRequest, ref byte pBodyDataBuffer, uint unBufferSize);

		internal bool GetHTTPResponseBodyData(HTTPRequestHandle hRequest, ref byte pBodyDataBuffer, uint unBufferSize)
		{
			return _GetHTTPResponseBodyData(Self, hRequest, ref pBodyDataBuffer, unBufferSize);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_GetHTTPStreamingResponseBodyData")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetHTTPStreamingResponseBodyData(IntPtr self, HTTPRequestHandle hRequest, uint cOffset, ref byte pBodyDataBuffer, uint unBufferSize);

		internal bool GetHTTPStreamingResponseBodyData(HTTPRequestHandle hRequest, uint cOffset, ref byte pBodyDataBuffer, uint unBufferSize)
		{
			return _GetHTTPStreamingResponseBodyData(Self, hRequest, cOffset, ref pBodyDataBuffer, unBufferSize);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_ReleaseHTTPRequest")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _ReleaseHTTPRequest(IntPtr self, HTTPRequestHandle hRequest);

		internal bool ReleaseHTTPRequest(HTTPRequestHandle hRequest)
		{
			return _ReleaseHTTPRequest(Self, hRequest);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_GetHTTPDownloadProgressPct")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetHTTPDownloadProgressPct(IntPtr self, HTTPRequestHandle hRequest, ref float pflPercentOut);

		internal bool GetHTTPDownloadProgressPct(HTTPRequestHandle hRequest, ref float pflPercentOut)
		{
			return _GetHTTPDownloadProgressPct(Self, hRequest, ref pflPercentOut);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_SetHTTPRequestRawPostBody")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetHTTPRequestRawPostBody(IntPtr self, HTTPRequestHandle hRequest, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchContentType, [In][Out] byte[] pubBody, uint unBodyLen);

		internal bool SetHTTPRequestRawPostBody(HTTPRequestHandle hRequest, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchContentType, [In][Out] byte[] pubBody, uint unBodyLen)
		{
			return _SetHTTPRequestRawPostBody(Self, hRequest, pchContentType, pubBody, unBodyLen);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_CreateCookieContainer")]
		private static extern HTTPCookieContainerHandle _CreateCookieContainer(IntPtr self, [MarshalAs(UnmanagedType.U1)] bool bAllowResponsesToModify);

		internal HTTPCookieContainerHandle CreateCookieContainer([MarshalAs(UnmanagedType.U1)] bool bAllowResponsesToModify)
		{
			return _CreateCookieContainer(Self, bAllowResponsesToModify);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_ReleaseCookieContainer")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _ReleaseCookieContainer(IntPtr self, HTTPCookieContainerHandle hCookieContainer);

		internal bool ReleaseCookieContainer(HTTPCookieContainerHandle hCookieContainer)
		{
			return _ReleaseCookieContainer(Self, hCookieContainer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_SetCookie")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetCookie(IntPtr self, HTTPCookieContainerHandle hCookieContainer, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchHost, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchUrl, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchCookie);

		internal bool SetCookie(HTTPCookieContainerHandle hCookieContainer, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchHost, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchUrl, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchCookie)
		{
			return _SetCookie(Self, hCookieContainer, pchHost, pchUrl, pchCookie);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_SetHTTPRequestCookieContainer")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetHTTPRequestCookieContainer(IntPtr self, HTTPRequestHandle hRequest, HTTPCookieContainerHandle hCookieContainer);

		internal bool SetHTTPRequestCookieContainer(HTTPRequestHandle hRequest, HTTPCookieContainerHandle hCookieContainer)
		{
			return _SetHTTPRequestCookieContainer(Self, hRequest, hCookieContainer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_SetHTTPRequestUserAgentInfo")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetHTTPRequestUserAgentInfo(IntPtr self, HTTPRequestHandle hRequest, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchUserAgentInfo);

		internal bool SetHTTPRequestUserAgentInfo(HTTPRequestHandle hRequest, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pchUserAgentInfo)
		{
			return _SetHTTPRequestUserAgentInfo(Self, hRequest, pchUserAgentInfo);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_SetHTTPRequestRequiresVerifiedCertificate")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetHTTPRequestRequiresVerifiedCertificate(IntPtr self, HTTPRequestHandle hRequest, [MarshalAs(UnmanagedType.U1)] bool bRequireVerifiedCertificate);

		internal bool SetHTTPRequestRequiresVerifiedCertificate(HTTPRequestHandle hRequest, [MarshalAs(UnmanagedType.U1)] bool bRequireVerifiedCertificate)
		{
			return _SetHTTPRequestRequiresVerifiedCertificate(Self, hRequest, bRequireVerifiedCertificate);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_SetHTTPRequestAbsoluteTimeoutMS")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetHTTPRequestAbsoluteTimeoutMS(IntPtr self, HTTPRequestHandle hRequest, uint unMilliseconds);

		internal bool SetHTTPRequestAbsoluteTimeoutMS(HTTPRequestHandle hRequest, uint unMilliseconds)
		{
			return _SetHTTPRequestAbsoluteTimeoutMS(Self, hRequest, unMilliseconds);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamHTTP_GetHTTPRequestWasTimedOut")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetHTTPRequestWasTimedOut(IntPtr self, HTTPRequestHandle hRequest, [MarshalAs(UnmanagedType.U1)] ref bool pbWasTimedOut);

		internal bool GetHTTPRequestWasTimedOut(HTTPRequestHandle hRequest, [MarshalAs(UnmanagedType.U1)] ref bool pbWasTimedOut)
		{
			return _GetHTTPRequestWasTimedOut(Self, hRequest, ref pbWasTimedOut);
		}
	}
}
