using System.Threading.Tasks;
using Steamworks.Data;

namespace Steamworks
{
	public class SteamServerStats : SteamServerClass<SteamServerStats>
	{
		internal static ISteamGameServerStats Internal => SteamServerClass<SteamServerStats>.Interface as ISteamGameServerStats;

		internal override void InitializeInterface(bool server)
		{
			SetInterface(server, new ISteamGameServerStats(server));
		}

		public static async Task<Result> RequestUserStatsAsync(SteamId steamid)
		{
			GSStatsReceived_t? r = await Internal.RequestUserStats(steamid);
			if (!r.HasValue)
			{
				return Result.Fail;
			}
			return r.Value.Result;
		}

		public static bool SetInt(SteamId steamid, string name, int stat)
		{
			return Internal.SetUserStat(steamid, name, stat);
		}

		public static bool SetFloat(SteamId steamid, string name, float stat)
		{
			return Internal.SetUserStat(steamid, name, stat);
		}

		public static int GetInt(SteamId steamid, string name, int defaultValue = 0)
		{
			int pData = defaultValue;
			if (!Internal.GetUserStat(steamid, name, ref pData))
			{
				return defaultValue;
			}
			return pData;
		}

		public static float GetFloat(SteamId steamid, string name, float defaultValue = 0f)
		{
			float pData = defaultValue;
			if (!Internal.GetUserStat(steamid, name, ref pData))
			{
				return defaultValue;
			}
			return pData;
		}

		public static bool SetAchievement(SteamId steamid, string name)
		{
			return Internal.SetUserAchievement(steamid, name);
		}

		public static bool ClearAchievement(SteamId steamid, string name)
		{
			return Internal.ClearUserAchievement(steamid, name);
		}

		public static bool GetAchievement(SteamId steamid, string name)
		{
			bool pbAchieved = false;
			if (!Internal.GetUserAchievement(steamid, name, ref pbAchieved))
			{
				return false;
			}
			return pbAchieved;
		}

		public static async Task<Result> StoreUserStats(SteamId steamid)
		{
			GSStatsStored_t? r = await Internal.StoreUserStats(steamid);
			if (!r.HasValue)
			{
				return Result.Fail;
			}
			return r.Value.Result;
		}
	}
}
