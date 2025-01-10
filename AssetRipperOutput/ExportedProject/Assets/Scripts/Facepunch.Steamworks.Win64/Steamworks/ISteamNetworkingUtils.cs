using System;
using System.Runtime.InteropServices;
using Steamworks.Data;

namespace Steamworks
{
	internal class ISteamNetworkingUtils : SteamInterface
	{
		internal ISteamNetworkingUtils(bool IsGameServer)
		{
			SetupInterface(IsGameServer);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl)]
		internal static extern IntPtr SteamAPI_SteamNetworkingUtils_v003();

		public override IntPtr GetGlobalInterfacePointer()
		{
			return SteamAPI_SteamNetworkingUtils_v003();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_AllocateMessage")]
		private static extern IntPtr _AllocateMessage(IntPtr self, int cbAllocateBuffer);

		internal NetMsg AllocateMessage(int cbAllocateBuffer)
		{
			IntPtr ptr = _AllocateMessage(Self, cbAllocateBuffer);
			return ptr.ToType<NetMsg>();
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_InitRelayNetworkAccess")]
		private static extern void _InitRelayNetworkAccess(IntPtr self);

		internal void InitRelayNetworkAccess()
		{
			_InitRelayNetworkAccess(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_GetRelayNetworkStatus")]
		private static extern SteamNetworkingAvailability _GetRelayNetworkStatus(IntPtr self, ref SteamRelayNetworkStatus_t pDetails);

		internal SteamNetworkingAvailability GetRelayNetworkStatus(ref SteamRelayNetworkStatus_t pDetails)
		{
			return _GetRelayNetworkStatus(Self, ref pDetails);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_GetLocalPingLocation")]
		private static extern float _GetLocalPingLocation(IntPtr self, ref NetPingLocation result);

		internal float GetLocalPingLocation(ref NetPingLocation result)
		{
			return _GetLocalPingLocation(Self, ref result);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_EstimatePingTimeBetweenTwoLocations")]
		private static extern int _EstimatePingTimeBetweenTwoLocations(IntPtr self, ref NetPingLocation location1, ref NetPingLocation location2);

		internal int EstimatePingTimeBetweenTwoLocations(ref NetPingLocation location1, ref NetPingLocation location2)
		{
			return _EstimatePingTimeBetweenTwoLocations(Self, ref location1, ref location2);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_EstimatePingTimeFromLocalHost")]
		private static extern int _EstimatePingTimeFromLocalHost(IntPtr self, ref NetPingLocation remoteLocation);

		internal int EstimatePingTimeFromLocalHost(ref NetPingLocation remoteLocation)
		{
			return _EstimatePingTimeFromLocalHost(Self, ref remoteLocation);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_ConvertPingLocationToString")]
		private static extern void _ConvertPingLocationToString(IntPtr self, ref NetPingLocation location, IntPtr pszBuf, int cchBufSize);

		internal void ConvertPingLocationToString(ref NetPingLocation location, out string pszBuf)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			_ConvertPingLocationToString(Self, ref location, intPtr, 32768);
			pszBuf = Helpers.MemoryToString(intPtr);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_ParsePingLocationString")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _ParsePingLocationString(IntPtr self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszString, ref NetPingLocation result);

		internal bool ParsePingLocationString([MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszString, ref NetPingLocation result)
		{
			return _ParsePingLocationString(Self, pszString, ref result);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_CheckPingDataUpToDate")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _CheckPingDataUpToDate(IntPtr self, float flMaxAgeSeconds);

		internal bool CheckPingDataUpToDate(float flMaxAgeSeconds)
		{
			return _CheckPingDataUpToDate(Self, flMaxAgeSeconds);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_GetPingToDataCenter")]
		private static extern int _GetPingToDataCenter(IntPtr self, SteamNetworkingPOPID popID, ref SteamNetworkingPOPID pViaRelayPoP);

		internal int GetPingToDataCenter(SteamNetworkingPOPID popID, ref SteamNetworkingPOPID pViaRelayPoP)
		{
			return _GetPingToDataCenter(Self, popID, ref pViaRelayPoP);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_GetDirectPingToPOP")]
		private static extern int _GetDirectPingToPOP(IntPtr self, SteamNetworkingPOPID popID);

		internal int GetDirectPingToPOP(SteamNetworkingPOPID popID)
		{
			return _GetDirectPingToPOP(Self, popID);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_GetPOPCount")]
		private static extern int _GetPOPCount(IntPtr self);

		internal int GetPOPCount()
		{
			return _GetPOPCount(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_GetPOPList")]
		private static extern int _GetPOPList(IntPtr self, ref SteamNetworkingPOPID list, int nListSz);

		internal int GetPOPList(ref SteamNetworkingPOPID list, int nListSz)
		{
			return _GetPOPList(Self, ref list, nListSz);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_GetLocalTimestamp")]
		private static extern long _GetLocalTimestamp(IntPtr self);

		internal long GetLocalTimestamp()
		{
			return _GetLocalTimestamp(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_SetDebugOutputFunction")]
		private static extern void _SetDebugOutputFunction(IntPtr self, NetDebugOutput eDetailLevel, NetDebugFunc pfnFunc);

		internal void SetDebugOutputFunction(NetDebugOutput eDetailLevel, NetDebugFunc pfnFunc)
		{
			_SetDebugOutputFunction(Self, eDetailLevel, pfnFunc);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_SetGlobalConfigValueInt32")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetGlobalConfigValueInt32(IntPtr self, NetConfig eValue, int val);

		internal bool SetGlobalConfigValueInt32(NetConfig eValue, int val)
		{
			return _SetGlobalConfigValueInt32(Self, eValue, val);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_SetGlobalConfigValueFloat")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetGlobalConfigValueFloat(IntPtr self, NetConfig eValue, float val);

		internal bool SetGlobalConfigValueFloat(NetConfig eValue, float val)
		{
			return _SetGlobalConfigValueFloat(Self, eValue, val);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_SetGlobalConfigValueString")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetGlobalConfigValueString(IntPtr self, NetConfig eValue, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string val);

		internal bool SetGlobalConfigValueString(NetConfig eValue, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string val)
		{
			return _SetGlobalConfigValueString(Self, eValue, val);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_SetConnectionConfigValueInt32")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetConnectionConfigValueInt32(IntPtr self, Connection hConn, NetConfig eValue, int val);

		internal bool SetConnectionConfigValueInt32(Connection hConn, NetConfig eValue, int val)
		{
			return _SetConnectionConfigValueInt32(Self, hConn, eValue, val);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_SetConnectionConfigValueFloat")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetConnectionConfigValueFloat(IntPtr self, Connection hConn, NetConfig eValue, float val);

		internal bool SetConnectionConfigValueFloat(Connection hConn, NetConfig eValue, float val)
		{
			return _SetConnectionConfigValueFloat(Self, hConn, eValue, val);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_SetConnectionConfigValueString")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetConnectionConfigValueString(IntPtr self, Connection hConn, NetConfig eValue, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string val);

		internal bool SetConnectionConfigValueString(Connection hConn, NetConfig eValue, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string val)
		{
			return _SetConnectionConfigValueString(Self, hConn, eValue, val);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_SetConfigValue")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetConfigValue(IntPtr self, NetConfig eValue, NetConfigScope eScopeType, IntPtr scopeObj, NetConfigType eDataType, IntPtr pArg);

		internal bool SetConfigValue(NetConfig eValue, NetConfigScope eScopeType, IntPtr scopeObj, NetConfigType eDataType, IntPtr pArg)
		{
			return _SetConfigValue(Self, eValue, eScopeType, scopeObj, eDataType, pArg);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_SetConfigValueStruct")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SetConfigValueStruct(IntPtr self, ref NetKeyValue opt, NetConfigScope eScopeType, IntPtr scopeObj);

		internal bool SetConfigValueStruct(ref NetKeyValue opt, NetConfigScope eScopeType, IntPtr scopeObj)
		{
			return _SetConfigValueStruct(Self, ref opt, eScopeType, scopeObj);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_GetConfigValue")]
		private static extern NetConfigResult _GetConfigValue(IntPtr self, NetConfig eValue, NetConfigScope eScopeType, IntPtr scopeObj, ref NetConfigType pOutDataType, IntPtr pResult, ref UIntPtr cbResult);

		internal NetConfigResult GetConfigValue(NetConfig eValue, NetConfigScope eScopeType, IntPtr scopeObj, ref NetConfigType pOutDataType, IntPtr pResult, ref UIntPtr cbResult)
		{
			return _GetConfigValue(Self, eValue, eScopeType, scopeObj, ref pOutDataType, pResult, ref cbResult);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_GetConfigValueInfo")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _GetConfigValueInfo(IntPtr self, NetConfig eValue, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pOutName, ref NetConfigType pOutDataType, [In][Out] NetConfigScope[] pOutScope, [In][Out] NetConfig[] pOutNextValue);

		internal bool GetConfigValueInfo(NetConfig eValue, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pOutName, ref NetConfigType pOutDataType, [In][Out] NetConfigScope[] pOutScope, [In][Out] NetConfig[] pOutNextValue)
		{
			return _GetConfigValueInfo(Self, eValue, pOutName, ref pOutDataType, pOutScope, pOutNextValue);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_GetFirstConfigValue")]
		private static extern NetConfig _GetFirstConfigValue(IntPtr self);

		internal NetConfig GetFirstConfigValue()
		{
			return _GetFirstConfigValue(Self);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_SteamNetworkingIPAddr_ToString")]
		private static extern void _SteamNetworkingIPAddr_ToString(IntPtr self, ref NetAddress addr, IntPtr buf, uint cbBuf, [MarshalAs(UnmanagedType.U1)] bool bWithPort);

		internal void SteamNetworkingIPAddr_ToString(ref NetAddress addr, out string buf, [MarshalAs(UnmanagedType.U1)] bool bWithPort)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			_SteamNetworkingIPAddr_ToString(Self, ref addr, intPtr, 32768u, bWithPort);
			buf = Helpers.MemoryToString(intPtr);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_SteamNetworkingIPAddr_ParseString")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SteamNetworkingIPAddr_ParseString(IntPtr self, ref NetAddress pAddr, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszStr);

		internal bool SteamNetworkingIPAddr_ParseString(ref NetAddress pAddr, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszStr)
		{
			return _SteamNetworkingIPAddr_ParseString(Self, ref pAddr, pszStr);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_SteamNetworkingIdentity_ToString")]
		private static extern void _SteamNetworkingIdentity_ToString(IntPtr self, ref NetIdentity identity, IntPtr buf, uint cbBuf);

		internal void SteamNetworkingIdentity_ToString(ref NetIdentity identity, out string buf)
		{
			IntPtr intPtr = Helpers.TakeMemory();
			_SteamNetworkingIdentity_ToString(Self, ref identity, intPtr, 32768u);
			buf = Helpers.MemoryToString(intPtr);
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_ISteamNetworkingUtils_SteamNetworkingIdentity_ParseString")]
		[return: MarshalAs(UnmanagedType.I1)]
		private static extern bool _SteamNetworkingIdentity_ParseString(IntPtr self, ref NetIdentity pIdentity, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszStr);

		internal bool SteamNetworkingIdentity_ParseString(ref NetIdentity pIdentity, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszStr)
		{
			return _SteamNetworkingIdentity_ParseString(Self, ref pIdentity, pszStr);
		}
	}
}
