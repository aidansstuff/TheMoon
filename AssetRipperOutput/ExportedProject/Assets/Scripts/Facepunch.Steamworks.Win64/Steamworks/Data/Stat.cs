using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	public struct Stat
	{
		public string Name { get; internal set; }

		public SteamId UserId { get; internal set; }

		public Stat(string name)
		{
			Name = name;
			UserId = 0uL;
		}

		public Stat(string name, SteamId user)
		{
			Name = name;
			UserId = user;
		}

		internal void LocalUserOnly([CallerMemberName] string caller = null)
		{
			if ((ulong)UserId == 0)
			{
				return;
			}
			throw new Exception("Stat." + caller + " can only be called for the local user");
		}

		public double GetGlobalFloat()
		{
			double pData = 0.0;
			if (SteamUserStats.Internal.GetGlobalStat(Name, ref pData))
			{
				return pData;
			}
			return 0.0;
		}

		public long GetGlobalInt()
		{
			long pData = 0L;
			SteamUserStats.Internal.GetGlobalStat(Name, ref pData);
			return pData;
		}

		public async Task<long[]> GetGlobalIntDaysAsync(int days)
		{
			GlobalStatsReceived_t? result = await SteamUserStats.Internal.RequestGlobalStats(days);
			if (!result.HasValue || result.GetValueOrDefault().Result != Result.OK)
			{
				return null;
			}
			long[] r = new long[days];
			int rows = SteamUserStats.Internal.GetGlobalStatHistory(Name, r, (uint)(r.Length * 8));
			if (days != rows)
			{
				r = r.Take(rows).ToArray();
			}
			return r;
		}

		public async Task<double[]> GetGlobalFloatDays(int days)
		{
			GlobalStatsReceived_t? result = await SteamUserStats.Internal.RequestGlobalStats(days);
			if (!result.HasValue || result.GetValueOrDefault().Result != Result.OK)
			{
				return null;
			}
			double[] r = new double[days];
			int rows = SteamUserStats.Internal.GetGlobalStatHistory(Name, r, (uint)(r.Length * 8));
			if (days != rows)
			{
				r = r.Take(rows).ToArray();
			}
			return r;
		}

		public float GetFloat()
		{
			float pData = 0f;
			if ((ulong)UserId != 0)
			{
				SteamUserStats.Internal.GetUserStat(UserId, Name, ref pData);
			}
			else
			{
				SteamUserStats.Internal.GetStat(Name, ref pData);
			}
			return 0f;
		}

		public int GetInt()
		{
			int pData = 0;
			if ((ulong)UserId != 0)
			{
				SteamUserStats.Internal.GetUserStat(UserId, Name, ref pData);
			}
			else
			{
				SteamUserStats.Internal.GetStat(Name, ref pData);
			}
			return pData;
		}

		public bool Set(int val)
		{
			LocalUserOnly("Set");
			return SteamUserStats.Internal.SetStat(Name, val);
		}

		public bool Set(float val)
		{
			LocalUserOnly("Set");
			return SteamUserStats.Internal.SetStat(Name, val);
		}

		public bool Add(int val)
		{
			LocalUserOnly("Add");
			return Set(GetInt() + val);
		}

		public bool Add(float val)
		{
			LocalUserOnly("Add");
			return Set(GetFloat() + val);
		}

		public bool UpdateAverageRate(float count, float sessionlength)
		{
			LocalUserOnly("UpdateAverageRate");
			return SteamUserStats.Internal.UpdateAvgRateStat(Name, count, sessionlength);
		}

		public bool Store()
		{
			LocalUserOnly("Store");
			return SteamUserStats.Internal.StoreStats();
		}
	}
}
