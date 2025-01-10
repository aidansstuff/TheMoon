using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Steamworks.Data;

namespace Steamworks
{
	public struct Friend
	{
		public struct FriendGameInfo
		{
			internal ulong GameID;

			internal uint GameIP;

			internal ulong SteamIDLobby;

			public int ConnectionPort;

			public int QueryPort;

			public uint IpAddressRaw => GameIP;

			public IPAddress IpAddress => Utility.Int32ToIp(GameIP);

			public Lobby? Lobby
			{
				get
				{
					if (SteamIDLobby == 0)
					{
						return null;
					}
					return new Lobby(SteamIDLobby);
				}
			}

			internal static FriendGameInfo From(FriendGameInfo_t i)
			{
				FriendGameInfo result = default(FriendGameInfo);
				result.GameID = i.GameID;
				result.GameIP = i.GameIP;
				result.ConnectionPort = i.GamePort;
				result.QueryPort = i.QueryPort;
				result.SteamIDLobby = i.SteamIDLobby;
				return result;
			}
		}

		public SteamId Id;

		public bool IsMe => (ulong)Id == (ulong)SteamClient.SteamId;

		public bool IsFriend => Relationship == Relationship.Friend;

		public bool IsBlocked => Relationship == Relationship.Blocked;

		public bool IsPlayingThisGame => GameInfo?.GameID == (uint)SteamClient.AppId;

		public bool IsOnline => State != FriendState.Offline;

		public bool IsAway => State == FriendState.Away;

		public bool IsBusy => State == FriendState.Busy;

		public bool IsSnoozing => State == FriendState.Snooze;

		public Relationship Relationship => SteamFriends.Internal.GetFriendRelationship(Id);

		public FriendState State => SteamFriends.Internal.GetFriendPersonaState(Id);

		public string Name => SteamFriends.Internal.GetFriendPersonaName(Id);

		public IEnumerable<string> NameHistory
		{
			get
			{
				for (int i = 0; i < 32; i++)
				{
					string j = SteamFriends.Internal.GetFriendPersonaNameHistory(Id, i);
					if (string.IsNullOrEmpty(j))
					{
						break;
					}
					yield return j;
				}
			}
		}

		public int SteamLevel => SteamFriends.Internal.GetFriendSteamLevel(Id);

		public FriendGameInfo? GameInfo
		{
			get
			{
				FriendGameInfo_t pFriendGameInfo = default(FriendGameInfo_t);
				if (!SteamFriends.Internal.GetFriendGamePlayed(Id, ref pFriendGameInfo))
				{
					return null;
				}
				return FriendGameInfo.From(pFriendGameInfo);
			}
		}

		public Friend(SteamId steamid)
		{
			Id = steamid;
		}

		public override string ToString()
		{
			return Name + " (" + Id.ToString() + ")";
		}

		public async Task RequestInfoAsync()
		{
			await SteamFriends.CacheUserInformationAsync(Id, nameonly: true);
		}

		public bool IsIn(SteamId group_or_room)
		{
			return SteamFriends.Internal.IsUserInSource(Id, group_or_room);
		}

		public async Task<Image?> GetSmallAvatarAsync()
		{
			return await SteamFriends.GetSmallAvatarAsync(Id);
		}

		public async Task<Image?> GetMediumAvatarAsync()
		{
			return await SteamFriends.GetMediumAvatarAsync(Id);
		}

		public async Task<Image?> GetLargeAvatarAsync()
		{
			return await SteamFriends.GetLargeAvatarAsync(Id);
		}

		public string GetRichPresence(string key)
		{
			string friendRichPresence = SteamFriends.Internal.GetFriendRichPresence(Id, key);
			if (string.IsNullOrEmpty(friendRichPresence))
			{
				return null;
			}
			return friendRichPresence;
		}

		public bool InviteToGame(string Text)
		{
			return SteamFriends.Internal.InviteUserToGame(Id, Text);
		}

		public bool SendMessage(string message)
		{
			return SteamFriends.Internal.ReplyToFriendMessage(Id, message);
		}

		public async Task<bool> RequestUserStatsAsync()
		{
			UserStatsReceived_t? result = await SteamUserStats.Internal.RequestUserStats(Id);
			return result.HasValue && result.Value.Result == Result.OK;
		}

		public float GetStatFloat(string statName, float defult = 0f)
		{
			float pData = defult;
			if (!SteamUserStats.Internal.GetUserStat(Id, statName, ref pData))
			{
				return defult;
			}
			return pData;
		}

		public int GetStatInt(string statName, int defult = 0)
		{
			int pData = defult;
			if (!SteamUserStats.Internal.GetUserStat(Id, statName, ref pData))
			{
				return defult;
			}
			return pData;
		}

		public bool GetAchievement(string statName, bool defult = false)
		{
			bool pbAchieved = defult;
			if (!SteamUserStats.Internal.GetUserAchievement(Id, statName, ref pbAchieved))
			{
				return defult;
			}
			return pbAchieved;
		}

		public DateTime GetAchievementUnlockTime(string statName)
		{
			bool pbAchieved = false;
			uint punUnlockTime = 0u;
			if (!SteamUserStats.Internal.GetUserAchievementAndUnlockTime(Id, statName, ref pbAchieved, ref punUnlockTime) || !pbAchieved)
			{
				return DateTime.MinValue;
			}
			return Epoch.ToDateTime(punUnlockTime);
		}
	}
}
