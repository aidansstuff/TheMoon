using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LeaderboardScoresDownloaded_t : ICallbackData
	{
		internal ulong SteamLeaderboard;

		internal ulong SteamLeaderboardEntries;

		internal int CEntryCount;

		public static int _datasize = Marshal.SizeOf(typeof(LeaderboardScoresDownloaded_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.LeaderboardScoresDownloaded;
	}
}
