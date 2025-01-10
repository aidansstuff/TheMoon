using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct ClanOfficerListResponse_t : ICallbackData
	{
		internal ulong SteamIDClan;

		internal int COfficers;

		internal byte Success;

		public static int _datasize = Marshal.SizeOf(typeof(ClanOfficerListResponse_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.ClanOfficerListResponse;
	}
}
