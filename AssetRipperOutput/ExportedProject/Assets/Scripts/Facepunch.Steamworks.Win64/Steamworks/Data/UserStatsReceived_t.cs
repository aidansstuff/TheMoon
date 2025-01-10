using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct UserStatsReceived_t : ICallbackData
	{
		internal ulong GameID;

		internal Result Result;

		internal ulong SteamIDUser;

		public static int _datasize = Marshal.SizeOf(typeof(UserStatsReceived_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.UserStatsReceived;
	}
}
