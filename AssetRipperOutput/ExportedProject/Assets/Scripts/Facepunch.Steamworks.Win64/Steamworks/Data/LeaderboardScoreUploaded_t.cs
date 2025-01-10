using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LeaderboardScoreUploaded_t : ICallbackData
	{
		internal byte Success;

		internal ulong SteamLeaderboard;

		internal int Score;

		internal byte ScoreChanged;

		internal int GlobalRankNew;

		internal int GlobalRankPrevious;

		public static int _datasize = Marshal.SizeOf(typeof(LeaderboardScoreUploaded_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.LeaderboardScoreUploaded;
	}
}
