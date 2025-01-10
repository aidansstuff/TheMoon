using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct MatchMakingKeyValuePair
	{
		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		internal string Key;

		[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
		internal string Value;

		[DllImport("steam_api64", CallingConvention = CallingConvention.Cdecl, EntryPoint = "SteamAPI_MatchMakingKeyValuePair_t_Construct")]
		internal static extern void InternalConstruct(ref MatchMakingKeyValuePair self);
	}
}
