using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Steamworks.Data;

namespace Steamworks
{
	public class SteamUserStats : SteamClientClass<SteamUserStats>
	{
		internal static ISteamUserStats Internal => SteamClientClass<SteamUserStats>.Interface as ISteamUserStats;

		public static bool StatsRecieved { get; internal set; }

		public static IEnumerable<Achievement> Achievements
		{
			get
			{
				for (int i = 0; i < Internal.GetNumAchievements(); i++)
				{
					yield return new Achievement(Internal.GetAchievementName((uint)i));
				}
			}
		}

		internal static event Action<string, int> OnAchievementIconFetched;

		public static event Action<SteamId, Result> OnUserStatsReceived;

		public static event Action<Result> OnUserStatsStored;

		public static event Action<Achievement, int, int> OnAchievementProgress;

		public static event Action<SteamId> OnUserStatsUnloaded;

		internal override void InitializeInterface(bool server)
		{
			SetInterface(server, new ISteamUserStats(server));
			InstallEvents();
			RequestCurrentStats();
		}

		internal static void InstallEvents()
		{
			Dispatch.Install(delegate(UserStatsReceived_t x)
			{
				if (x.SteamIDUser == (ulong)SteamClient.SteamId)
				{
					StatsRecieved = true;
				}
				SteamUserStats.OnUserStatsReceived?.Invoke(x.SteamIDUser, x.Result);
			});
			Dispatch.Install(delegate(UserStatsStored_t x)
			{
				SteamUserStats.OnUserStatsStored?.Invoke(x.Result);
			});
			Dispatch.Install(delegate(UserAchievementStored_t x)
			{
				SteamUserStats.OnAchievementProgress?.Invoke(new Achievement(x.AchievementNameUTF8()), (int)x.CurProgress, (int)x.MaxProgress);
			});
			Dispatch.Install(delegate(UserStatsUnloaded_t x)
			{
				SteamUserStats.OnUserStatsUnloaded?.Invoke(x.SteamIDUser);
			});
			Dispatch.Install(delegate(UserAchievementIconFetched_t x)
			{
				SteamUserStats.OnAchievementIconFetched?.Invoke(x.AchievementNameUTF8(), x.IconHandle);
			});
		}

		public static bool IndicateAchievementProgress(string achName, int curProg, int maxProg)
		{
			if (string.IsNullOrEmpty(achName))
			{
				throw new ArgumentNullException("Achievement string is null or empty");
			}
			if (curProg >= maxProg)
			{
				throw new ArgumentException($" Current progress [{curProg}] arguement toward achievement greater than or equal to max [{maxProg}]");
			}
			return Internal.IndicateAchievementProgress(achName, (uint)curProg, (uint)maxProg);
		}

		public static async Task<int> PlayerCountAsync()
		{
			NumberOfCurrentPlayers_t? result = await Internal.GetNumberOfCurrentPlayers();
			if (!result.HasValue || result.Value.Success == 0)
			{
				return -1;
			}
			return result.Value.CPlayers;
		}

		public static bool StoreStats()
		{
			return Internal.StoreStats();
		}

		public static bool RequestCurrentStats()
		{
			return Internal.RequestCurrentStats();
		}

		public static async Task<Result> RequestGlobalStatsAsync(int days)
		{
			GlobalStatsReceived_t? result = await Internal.RequestGlobalStats(days);
			if (!result.HasValue)
			{
				return Result.Fail;
			}
			return result.Value.Result;
		}

		public static async Task<Leaderboard?> FindOrCreateLeaderboardAsync(string name, LeaderboardSort sort, LeaderboardDisplay display)
		{
			LeaderboardFindResult_t? result = await Internal.FindOrCreateLeaderboard(name, sort, display);
			if (!result.HasValue || result.Value.LeaderboardFound == 0)
			{
				return null;
			}
			Leaderboard value = default(Leaderboard);
			value.Id = result.Value.SteamLeaderboard;
			return value;
		}

		public static async Task<Leaderboard?> FindLeaderboardAsync(string name)
		{
			LeaderboardFindResult_t? result = await Internal.FindLeaderboard(name);
			if (!result.HasValue || result.Value.LeaderboardFound == 0)
			{
				return null;
			}
			Leaderboard value = default(Leaderboard);
			value.Id = result.Value.SteamLeaderboard;
			return value;
		}

		public static bool AddStat(string name, int amount = 1)
		{
			int statInt = GetStatInt(name);
			statInt += amount;
			return SetStat(name, statInt);
		}

		public static bool AddStat(string name, float amount = 1f)
		{
			float statFloat = GetStatFloat(name);
			statFloat += amount;
			return SetStat(name, statFloat);
		}

		public static bool SetStat(string name, int value)
		{
			return Internal.SetStat(name, value);
		}

		public static bool SetStat(string name, float value)
		{
			return Internal.SetStat(name, value);
		}

		public static int GetStatInt(string name)
		{
			int pData = 0;
			Internal.GetStat(name, ref pData);
			return pData;
		}

		public static float GetStatFloat(string name)
		{
			float pData = 0f;
			Internal.GetStat(name, ref pData);
			return pData;
		}

		public static bool ResetAll(bool includeAchievements)
		{
			return Internal.ResetAllStats(includeAchievements);
		}
	}
}
