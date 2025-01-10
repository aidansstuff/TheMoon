using System;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	public struct Leaderboard
	{
		internal SteamLeaderboard_t Id;

		private static int[] detailsBuffer = new int[64];

		private static int[] noDetails = Array.Empty<int>();

		public string Name => SteamUserStats.Internal.GetLeaderboardName(Id);

		public LeaderboardSort Sort => SteamUserStats.Internal.GetLeaderboardSortMethod(Id);

		public LeaderboardDisplay Display => SteamUserStats.Internal.GetLeaderboardDisplayType(Id);

		public int EntryCount => SteamUserStats.Internal.GetLeaderboardEntryCount(Id);

		public async Task<LeaderboardUpdate?> ReplaceScore(int score, int[] details = null)
		{
			if (details == null)
			{
				details = noDetails;
			}
			LeaderboardScoreUploaded_t? r = await SteamUserStats.Internal.UploadLeaderboardScore(Id, LeaderboardUploadScoreMethod.ForceUpdate, score, details, details.Length);
			if (!r.HasValue)
			{
				return null;
			}
			return LeaderboardUpdate.From(r.Value);
		}

		public async Task<LeaderboardUpdate?> SubmitScoreAsync(int score, int[] details = null)
		{
			if (details == null)
			{
				details = noDetails;
			}
			LeaderboardScoreUploaded_t? r = await SteamUserStats.Internal.UploadLeaderboardScore(Id, LeaderboardUploadScoreMethod.KeepBest, score, details, details.Length);
			if (!r.HasValue)
			{
				return null;
			}
			return LeaderboardUpdate.From(r.Value);
		}

		public async Task<Result> AttachUgc(Ugc file)
		{
			LeaderboardUGCSet_t? r = await SteamUserStats.Internal.AttachLeaderboardUGC(Id, file.Handle);
			if (!r.HasValue)
			{
				return Result.Fail;
			}
			return r.Value.Result;
		}

		public async Task<LeaderboardEntry[]> GetScoresAsync(int count, int offset = 1)
		{
			if (offset <= 0)
			{
				throw new ArgumentException("Should be 1+", "offset");
			}
			LeaderboardScoresDownloaded_t? r = await SteamUserStats.Internal.DownloadLeaderboardEntries(Id, LeaderboardDataRequest.Global, offset, offset + count);
			if (!r.HasValue)
			{
				return null;
			}
			return await LeaderboardResultToEntries(r.Value);
		}

		public async Task<LeaderboardEntry[]> GetScoresAroundUserAsync(int start = -10, int end = 10)
		{
			LeaderboardScoresDownloaded_t? r = await SteamUserStats.Internal.DownloadLeaderboardEntries(Id, LeaderboardDataRequest.GlobalAroundUser, start, end);
			if (!r.HasValue)
			{
				return null;
			}
			return await LeaderboardResultToEntries(r.Value);
		}

		public async Task<LeaderboardEntry[]> GetScoresFromFriendsAsync()
		{
			LeaderboardScoresDownloaded_t? r = await SteamUserStats.Internal.DownloadLeaderboardEntries(Id, LeaderboardDataRequest.Friends, 0, 0);
			if (!r.HasValue)
			{
				return null;
			}
			return await LeaderboardResultToEntries(r.Value);
		}

		internal async Task<LeaderboardEntry[]> LeaderboardResultToEntries(LeaderboardScoresDownloaded_t r)
		{
			if (r.CEntryCount <= 0)
			{
				return null;
			}
			LeaderboardEntry[] output = new LeaderboardEntry[r.CEntryCount];
			LeaderboardEntry_t e = default(LeaderboardEntry_t);
			for (int i = 0; i < output.Length; i++)
			{
				if (SteamUserStats.Internal.GetDownloadedLeaderboardEntry(r.SteamLeaderboardEntries, i, ref e, detailsBuffer, detailsBuffer.Length))
				{
					output[i] = LeaderboardEntry.From(e, detailsBuffer);
				}
			}
			await WaitForUserNames(output);
			return output;
		}

		internal static async Task WaitForUserNames(LeaderboardEntry[] entries)
		{
			bool gotAll = false;
			while (!gotAll)
			{
				gotAll = true;
				for (int i = 0; i < entries.Length; i++)
				{
					LeaderboardEntry entry = entries[i];
					if ((ulong)entry.User.Id != 0 && SteamFriends.Internal.RequestUserInformation(entry.User.Id, bRequireNameOnly: true))
					{
						gotAll = false;
					}
				}
				await Task.Delay(1);
			}
		}
	}
}
