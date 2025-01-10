namespace Steamworks.Data
{
	public struct LeaderboardUpdate
	{
		public int Score;

		public bool Changed;

		public int NewGlobalRank;

		public int OldGlobalRank;

		public int RankChange => NewGlobalRank - OldGlobalRank;

		internal static LeaderboardUpdate From(LeaderboardScoreUploaded_t e)
		{
			LeaderboardUpdate result = default(LeaderboardUpdate);
			result.Score = e.Score;
			result.Changed = e.ScoreChanged == 1;
			result.NewGlobalRank = e.GlobalRankNew;
			result.OldGlobalRank = e.GlobalRankPrevious;
			return result;
		}
	}
}
