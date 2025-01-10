using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Explicit, Pack = 1, Size = 18)]
	public struct NetAddress
	{
		internal struct IPV4
		{
			internal ulong m_8zeros;

			internal ushort m_0000;

			internal ushort m_ffff;

			internal byte ip0;

			internal byte ip1;

			internal byte ip2;

			internal byte ip3;
		}

		[FieldOffset(0)]
		internal IPV4 ip;

		[FieldOffset(16)]
		internal ushort port;

		public ushort Port => port;

		public static NetAddress Cleared
		{
			get
			{
				NetAddress self = default(NetAddress);
				InternalClear(ref self);
				return self;
			}
		}

		public bool IsIPv6AllZeros
		{
			get
			{
				NetAddress self = this;
				return InternalIsIPv6AllZeros(ref self);
			}
		}

		public bool IsIPv4
		{
			get
			{
				NetAddress self = this;
				return InternalIsIPv4(ref self);
			}
		}

		public bool IsLocalHost
		{
			get
			{
				NetAddress self = this;
				return InternalIsLocalHost(ref self);
			}
		}

		public IPAddress Address
		{
			get
			{
				if (IsIPv4)
				{
					NetAddress self = this;
					uint ipAddress = InternalGetIPv4(ref self);
					return Utility.Int32ToIp(ipAddress);
				}
				throw new NotImplementedException("Oops - no IPV6 support yet?");
			}
		}

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIPAddr_Clear")]
		internal static extern void InternalClear(ref NetAddress self);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIPAddr_IsIPv6AllZeros")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool InternalIsIPv6AllZeros(ref NetAddress self);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIPAddr_SetIPv6")]
		internal static extern void InternalSetIPv6(ref NetAddress self, ref byte ipv6, ushort nPort);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIPAddr_SetIPv4")]
		internal static extern void InternalSetIPv4(ref NetAddress self, uint nIP, ushort nPort);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIPAddr_IsIPv4")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool InternalIsIPv4(ref NetAddress self);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIPAddr_GetIPv4")]
		internal static extern uint InternalGetIPv4(ref NetAddress self);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIPAddr_SetIPv6LocalHost")]
		internal static extern void InternalSetIPv6LocalHost(ref NetAddress self, ushort nPort);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIPAddr_IsLocalHost")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool InternalIsLocalHost(ref NetAddress self);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIPAddr_ToString")]
		internal static extern void InternalToString(ref NetAddress self, IntPtr buf, uint cbBuf, [MarshalAs(UnmanagedType.U1)] bool bWithPort);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIPAddr_ParseString")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool InternalParseString(ref NetAddress self, [MarshalAs(UnmanagedType.CustomMarshaler, MarshalType = "Steamworks.Utf8StringToNative")] string pszStr);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_SteamNetworkingIPAddr_IsEqualTo")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool InternalIsEqualTo(ref NetAddress self, ref NetAddress x);

		public static NetAddress AnyIp(ushort port)
		{
			NetAddress cleared = Cleared;
			cleared.port = port;
			return cleared;
		}

		public static NetAddress LocalHost(ushort port)
		{
			NetAddress self = Cleared;
			InternalSetIPv6LocalHost(ref self, port);
			return self;
		}

		public static NetAddress From(string addrStr, ushort port)
		{
			return From(IPAddress.Parse(addrStr), port);
		}

		public static NetAddress From(IPAddress address, ushort port)
		{
			byte[] addressBytes = address.GetAddressBytes();
			if (address.AddressFamily == AddressFamily.InterNetwork)
			{
				NetAddress self = Cleared;
				InternalSetIPv4(ref self, address.IpToInt32(), port);
				return self;
			}
			throw new NotImplementedException("Oops - no IPV6 support yet?");
		}

		public override string ToString()
		{
			IntPtr intPtr = Helpers.TakeMemory();
			NetAddress self = this;
			InternalToString(ref self, intPtr, 32768u, bWithPort: true);
			return Helpers.MemoryToString(intPtr);
		}
	}
}
