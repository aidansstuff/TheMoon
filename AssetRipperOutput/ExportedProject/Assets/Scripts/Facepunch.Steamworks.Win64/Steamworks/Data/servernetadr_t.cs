using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct servernetadr_t
	{
		internal ushort ConnectionPort;

		internal ushort QueryPort;

		internal uint IP;

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_servernetadr_t_Construct")]
		internal static extern void InternalConstruct(ref servernetadr_t self);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_servernetadr_t_Init")]
		internal static extern void InternalInit(ref servernetadr_t self, uint ip, ushort usQueryPort, ushort usConnectionPort);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_servernetadr_t_GetQueryPort")]
		internal static extern ushort InternalGetQueryPort(ref servernetadr_t self);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_servernetadr_t_SetQueryPort")]
		internal static extern void InternalSetQueryPort(ref servernetadr_t self, ushort usPort);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_servernetadr_t_GetConnectionPort")]
		internal static extern ushort InternalGetConnectionPort(ref servernetadr_t self);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_servernetadr_t_SetConnectionPort")]
		internal static extern void InternalSetConnectionPort(ref servernetadr_t self, ushort usPort);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_servernetadr_t_GetIP")]
		internal static extern uint InternalGetIP(ref servernetadr_t self);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_servernetadr_t_SetIP")]
		internal static extern void InternalSetIP(ref servernetadr_t self, uint unIP);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_servernetadr_t_GetConnectionAddressString")]
		internal static extern Utf8StringPointer InternalGetConnectionAddressString(ref servernetadr_t self);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_servernetadr_t_GetQueryAddressString")]
		internal static extern Utf8StringPointer InternalGetQueryAddressString(ref servernetadr_t self);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_servernetadr_t_IsLessThan")]
		[return: MarshalAs(UnmanagedType.I1)]
		internal static extern bool InternalIsLessThan(ref servernetadr_t self, ref servernetadr_t netadr);

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_servernetadr_t_Assign")]
		internal static extern void InternalAssign(ref servernetadr_t self, ref servernetadr_t that);
	}
}
