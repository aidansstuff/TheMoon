using System.Linq;

namespace Steamworks.Data
{
	public struct LeaderboardEntry
	{
		public Friend User;

		public int GlobalRank;

		public int Score;

		public int[] Details;

		internal static LeaderboardEntry From(LeaderboardEntry_t e, int[] detailsBuffer)
		{
			LeaderboardEntry leaderboardEntry = default(LeaderboardEntry);
			leaderboardEntry.User = new Friend(e.SteamIDUser);
			leaderboardEntry.GlobalRank = e.GlobalRank;
			leaderboardEntry.Score = e.Score;
			leaderboardEntry.Details = null;
			LeaderboardEntry result = leaderboardEntry;
			if (e.CDetails > 0)
			{
				result.Details = detailsBuffer.Take(e.CDetails).ToArray();
			}
			return result;
		}
	}
}
