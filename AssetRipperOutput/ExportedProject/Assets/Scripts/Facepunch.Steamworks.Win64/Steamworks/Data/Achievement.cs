using System;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	public struct Achievement
	{
		internal string Value;

		public bool State
		{
			get
			{
				bool pbAchieved = false;
				SteamUserStats.Internal.GetAchievement(Value, ref pbAchieved);
				return pbAchieved;
			}
		}

		public string Identifier => Value;

		public string Name => SteamUserStats.Internal.GetAchievementDisplayAttribute(Value, "name");

		public string Description => SteamUserStats.Internal.GetAchievementDisplayAttribute(Value, "desc");

		public DateTime? UnlockTime
		{
			get
			{
				bool pbAchieved = false;
				uint punUnlockTime = 0u;
				if (!SteamUserStats.Internal.GetAchievementAndUnlockTime(Value, ref pbAchieved, ref punUnlockTime) || !pbAchieved)
				{
					return null;
				}
				return Epoch.ToDateTime(punUnlockTime);
			}
		}

		public float GlobalUnlocked
		{
			get
			{
				float pflPercent = 0f;
				if (!SteamUserStats.Internal.GetAchievementAchievedPercent(Value, ref pflPercent))
				{
					return -1f;
				}
				return pflPercent / 100f;
			}
		}

		public Achievement(string name)
		{
			Value = name;
		}

		public override string ToString()
		{
			return Value;
		}

		public Image? GetIcon()
		{
			return SteamUtils.GetImage(SteamUserStats.Internal.GetAchievementIcon(Value));
		}

		public async Task<Image?> GetIconAsync(int timeout = 5000)
		{
			int i = SteamUserStats.Internal.GetAchievementIcon(Value);
			if (i != 0)
			{
				return SteamUtils.GetImage(i);
			}
			string ident = Identifier;
			bool gotCallback = false;
			try
			{
				SteamUserStats.OnAchievementIconFetched += f;
				int waited = 0;
				while (!gotCallback)
				{
					await Task.Delay(10);
					waited += 10;
					if (waited > timeout)
					{
						return null;
					}
				}
				if (i == 0)
				{
					return null;
				}
				return SteamUtils.GetImage(i);
			}
			finally
			{
				SteamUserStats.OnAchievementIconFetched -= f;
			}
			void f(string x, int icon)
			{
				if (!(x != ident))
				{
					i = icon;
					gotCallback = true;
				}
			}
		}

		public bool Trigger(bool apply = true)
		{
			bool flag = SteamUserStats.Internal.SetAchievement(Value);
			if (apply && flag)
			{
				SteamUserStats.Internal.StoreStats();
			}
			return flag;
		}

		public bool Clear()
		{
			return SteamUserStats.Internal.ClearAchievement(Value);
		}
	}
}
