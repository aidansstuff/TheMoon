using System;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 136)]
	public struct NetIdentity
	{
		internal enum IdentityType
		{
			Invalid = 0,
			IPAddress = 1,
			GenericString = 2,
			GenericBytes = 3,
			SteamID = 16
		}

		[FieldOffset(0)]
		internal IdentityType type;

		[FieldOffset(4)]
		internal int size;

		[FieldOffset(8)]
		internal ulong steamid;

		[FieldOffset(8)]
		internal NetAddress netaddress;

		public static NetIdentity LocalHost
		{
			get
			{
				NetIdentity self = default(NetIdentity);
				InternalSetLocalHost(ref self);
				return self;
			}
		}

		public bool IsSteamId => type == IdentityType.SteamID;

		public bool IsIpAddress => type == IdentityType.IPAddress;

		public bool IsLocalHost
		{
			get
			{
				NetIdentity self = default(NetIdentity);
				return InternalIsLocalHost(ref self);
			}
		}

		public SteamId SteamId
		{
			get
			{
				if (type != IdentityType.SteamID)
				{
					return default(SteamId);
				}
				NetIdentity self = this;
				return InternalGetSteamID(ref self);
			}
		}

		public NetAddress Address
		{
			get
			{
				if (type != IdentityType.IPAddress)
				{
					return default(NetAddress);
				}
				NetIdentity self = this;
				IntPtr ptr = InternalGetIPAddr(ref self);
				return ptr.ToType<NetAddress>();
			}
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIdentity_Clear")]
		internal static extern void InternalClear(ref NetIdentity self);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIdentity_IsInvalid")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool InternalIsInvalid(ref NetIdentity self);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIdentity_SetSteamID")]
		internal static extern void InternalSetSteamID(ref NetIdentity self, SteamId steamID);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIdentity_GetSteamID")]
		internal static extern SteamId InternalGetSteamID(ref NetIdentity self);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIdentity_SetSteamID64")]
		internal static extern void InternalSetSteamID64(ref NetIdentity self, ulong steamID);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIdentity_GetSteamID64")]
		internal static extern ulong InternalGetSteamID64(ref NetIdentity self);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIdentity_SetXboxPairwiseID")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool InternalSetXboxPairwiseID(ref NetIdentity self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszString);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIdentity_GetXboxPairwiseID")]
		internal static extern Utf8StringPointer InternalGetXboxPairwiseID(ref NetIdentity self);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIdentity_SetIPAddr")]
		internal static extern void InternalSetIPAddr(ref NetIdentity self, ref NetAddress addr);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIdentity_GetIPAddr")]
		internal static extern IntPtr InternalGetIPAddr(ref NetIdentity self);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIdentity_SetLocalHost")]
		internal static extern void InternalSetLocalHost(ref NetIdentity self);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIdentity_IsLocalHost")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool InternalIsLocalHost(ref NetIdentity self);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIdentity_SetGenericString")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool InternalSetGenericString(ref NetIdentity self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszString);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIdentity_GetGenericString")]
		internal static extern Utf8StringPointer InternalGetGenericString(ref NetIdentity self);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIdentity_SetGenericBytes")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool InternalSetGenericBytes(ref NetIdentity self, IntPtr data, uint cbLen);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIdentity_GetGenericBytes")]
		internal static extern byte InternalGetGenericBytes(ref NetIdentity self, ref int cbLen);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIdentity_IsEqualTo")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool InternalIsEqualTo(ref NetIdentity self, ref NetIdentity x);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIdentity_ToString")]
		internal static extern void InternalToString(ref NetIdentity self, IntPtr buf, uint cbBuf);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIdentity_ParseString")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool InternalParseString(ref NetIdentity self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszStr);

		public static implicit operator NetIdentity(SteamId value)
		{
			NetIdentity self = default(NetIdentity);
			InternalSetSteamID(ref self, value);
			return self;
		}

		public static implicit operator NetIdentity(NetAddress address)
		{
			NetIdentity self = default(NetIdentity);
			InternalSetIPAddr(ref self, ref address);
			return self;
		}

		public static implicit operator SteamId(NetIdentity value)
		{
			return value.SteamId;
		}

		public override string ToString()
		{
			NetIdentity identity = this;
			SteamNetworkingUtils.Internal.SteamNetworkingIdentity_ToString(ref identity, out var buf);
			return buf;
		}
	}
}
