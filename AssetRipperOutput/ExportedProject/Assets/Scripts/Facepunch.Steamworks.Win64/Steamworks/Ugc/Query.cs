using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Steamworks.Data;

namespace Steamworks.Ugc
{
	public struct Query
	{
		private UgcType matchingType;

		private UGCQuery queryType;

		private AppId consumerApp;

		private AppId creatorApp;

		private string searchText;

		private SteamId? steamid;

		private UserUGCList userType;

		private UserUGCListSortOrder userSort;

		private PublishedFileId[] Files;

		private int? maxCacheAge;

		private string language;

		private int? trendDays;

		private List<string> requiredTags;

		private bool? matchAnyTag;

		private List<string> excludedTags;

		private Dictionary<string, string> requiredKv;

		private bool? WantsReturnOnlyIDs;

		private bool? WantsReturnKeyValueTags;

		private bool? WantsReturnLongDescription;

		private bool? WantsReturnMetadata;

		private bool? WantsReturnChildren;

		private bool? WantsReturnAdditionalPreviews;

		private bool? WantsReturnTotalOnly;

		private uint? WantsReturnPlaytimeStats;

		public static Query All => new Query(UgcType.All);

		public static Query Items => new Query(UgcType.Items);

		public static Query ItemsMtx => new Query(UgcType.Items_Mtx);

		public static Query ItemsReadyToUse => new Query(UgcType.Items_ReadyToUse);

		public static Query Collections => new Query(UgcType.Collections);

		public static Query Artwork => new Query(UgcType.Artwork);

		public static Query Videos => new Query(UgcType.Videos);

		public static Query Screenshots => new Query(UgcType.Screenshots);

		public static Query AllGuides => new Query(UgcType.AllGuides);

		public static Query WebGuides => new Query(UgcType.WebGuides);

		public static Query IntegratedGuides => new Query(UgcType.IntegratedGuides);

		public static Query UsableInGame => new Query(UgcType.UsableInGame);

		public static Query ControllerBindings => new Query(UgcType.ControllerBindings);

		public static Query GameManagedItems => new Query(UgcType.GameManagedItems);

		public Query(UgcType type)
		{
			this = default(Query);
			matchingType = type;
		}

		public Query RankedByVote()
		{
			queryType = UGCQuery.RankedByVote;
			return this;
		}

		public Query RankedByPublicationDate()
		{
			queryType = UGCQuery.RankedByPublicationDate;
			return this;
		}

		public Query RankedByAcceptanceDate()
		{
			queryType = UGCQuery.AcceptedForGameRankedByAcceptanceDate;
			return this;
		}

		public Query RankedByTrend()
		{
			queryType = UGCQuery.RankedByTrend;
			return this;
		}

		public Query FavoritedByFriends()
		{
			queryType = UGCQuery.FavoritedByFriendsRankedByPublicationDate;
			return this;
		}

		public Query CreatedByFriends()
		{
			queryType = UGCQuery.CreatedByFriendsRankedByPublicationDate;
			return this;
		}

		public Query RankedByNumTimesReported()
		{
			queryType = UGCQuery.RankedByNumTimesReported;
			return this;
		}

		public Query CreatedByFollowedUsers()
		{
			queryType = UGCQuery.CreatedByFollowedUsersRankedByPublicationDate;
			return this;
		}

		public Query NotYetRated()
		{
			queryType = UGCQuery.NotYetRated;
			return this;
		}

		public Query RankedByTotalVotesAsc()
		{
			queryType = UGCQuery.RankedByTotalVotesAsc;
			return this;
		}

		public Query RankedByVotesUp()
		{
			queryType = UGCQuery.RankedByVotesUp;
			return this;
		}

		public Query RankedByTextSearch()
		{
			queryType = UGCQuery.RankedByTextSearch;
			return this;
		}

		public Query RankedByTotalUniqueSubscriptions()
		{
			queryType = UGCQuery.RankedByTotalUniqueSubscriptions;
			return this;
		}

		public Query RankedByPlaytimeTrend()
		{
			queryType = UGCQuery.RankedByPlaytimeTrend;
			return this;
		}

		public Query RankedByTotalPlaytime()
		{
			queryType = UGCQuery.RankedByTotalPlaytime;
			return this;
		}

		public Query RankedByAveragePlaytimeTrend()
		{
			queryType = UGCQuery.RankedByAveragePlaytimeTrend;
			return this;
		}

		public Query RankedByLifetimeAveragePlaytime()
		{
			queryType = UGCQuery.RankedByLifetimeAveragePlaytime;
			return this;
		}

		public Query RankedByPlaytimeSessionsTrend()
		{
			queryType = UGCQuery.RankedByPlaytimeSessionsTrend;
			return this;
		}

		public Query RankedByLifetimePlaytimeSessions()
		{
			queryType = UGCQuery.RankedByLifetimePlaytimeSessions;
			return this;
		}

		internal Query LimitUser(SteamId steamid)
		{
			if (steamid.Value == 0)
			{
				steamid = SteamClient.SteamId;
			}
			this.steamid = steamid;
			return this;
		}

		public Query WhereUserPublished(SteamId user = default(SteamId))
		{
			userType = UserUGCList.Published;
			LimitUser(user);
			return this;
		}

		public Query WhereUserVotedOn(SteamId user = default(SteamId))
		{
			userType = UserUGCList.VotedOn;
			LimitUser(user);
			return this;
		}

		public Query WhereUserVotedUp(SteamId user = default(SteamId))
		{
			userType = UserUGCList.VotedUp;
			LimitUser(user);
			return this;
		}

		public Query WhereUserVotedDown(SteamId user = default(SteamId))
		{
			userType = UserUGCList.VotedDown;
			LimitUser(user);
			return this;
		}

		public Query WhereUserWillVoteLater(SteamId user = default(SteamId))
		{
			userType = UserUGCList.WillVoteLater;
			LimitUser(user);
			return this;
		}

		public Query WhereUserFavorited(SteamId user = default(SteamId))
		{
			userType = UserUGCList.Favorited;
			LimitUser(user);
			return this;
		}

		public Query WhereUserSubscribed(SteamId user = default(SteamId))
		{
			userType = UserUGCList.Subscribed;
			LimitUser(user);
			return this;
		}

		public Query WhereUserUsedOrPlayed(SteamId user = default(SteamId))
		{
			userType = UserUGCList.UsedOrPlayed;
			LimitUser(user);
			return this;
		}

		public Query WhereUserFollowed(SteamId user = default(SteamId))
		{
			userType = UserUGCList.Followed;
			LimitUser(user);
			return this;
		}

		public Query SortByCreationDate()
		{
			userSort = UserUGCListSortOrder.CreationOrderDesc;
			return this;
		}

		public Query SortByCreationDateAsc()
		{
			userSort = UserUGCListSortOrder.CreationOrderAsc;
			return this;
		}

		public Query SortByTitleAsc()
		{
			userSort = UserUGCListSortOrder.TitleAsc;
			return this;
		}

		public Query SortByUpdateDate()
		{
			userSort = UserUGCListSortOrder.LastUpdatedDesc;
			return this;
		}

		public Query SortBySubscriptionDate()
		{
			userSort = UserUGCListSortOrder.SubscriptionDateDesc;
			return this;
		}

		public Query SortByVoteScore()
		{
			userSort = UserUGCListSortOrder.VoteScoreDesc;
			return this;
		}

		public Query SortByModeration()
		{
			userSort = UserUGCListSortOrder.ForModeration;
			return this;
		}

		public Query WhereSearchText(string searchText)
		{
			this.searchText = searchText;
			return this;
		}

		public Query WithFileId(params PublishedFileId[] files)
		{
			Files = files;
			return this;
		}

		public async Task<ResultPage?> GetPageAsync(int page)
		{
			if (page <= 0)
			{
				throw new Exception("page should be > 0");
			}
			if ((uint)consumerApp == 0)
			{
				consumerApp = SteamClient.AppId;
			}
			if ((uint)creatorApp == 0)
			{
				creatorApp = consumerApp;
			}
			UGCQueryHandle_t handle = ((Files != null) ? SteamUGC.Internal.CreateQueryUGCDetailsRequest(Files, (uint)Files.Length) : ((!steamid.HasValue) ? SteamUGC.Internal.CreateQueryAllUGCRequest(queryType, matchingType, creatorApp.Value, consumerApp.Value, (uint)page) : SteamUGC.Internal.CreateQueryUserUGCRequest(steamid.Value.AccountId, userType, matchingType, userSort, creatorApp.Value, consumerApp.Value, (uint)page)));
			ApplyReturns(handle);
			if (maxCacheAge.HasValue)
			{
				SteamUGC.Internal.SetAllowCachedResponse(handle, (uint)maxCacheAge.Value);
			}
			ApplyConstraints(handle);
			SteamUGCQueryCompleted_t? result = await SteamUGC.Internal.SendQueryUGCRequest(handle);
			if (!result.HasValue)
			{
				return null;
			}
			if (result.Value.Result != Result.OK)
			{
				return null;
			}
			ResultPage value = default(ResultPage);
			value.Handle = result.Value.Handle;
			value.ResultCount = (int)result.Value.NumResultsReturned;
			value.TotalCount = (int)result.Value.TotalMatchingResults;
			value.CachedData = result.Value.CachedData;
			return value;
		}

		public Query WithType(UgcType type)
		{
			matchingType = type;
			return this;
		}

		public Query AllowCachedResponse(int maxSecondsAge)
		{
			maxCacheAge = maxSecondsAge;
			return this;
		}

		public Query InLanguage(string lang)
		{
			language = lang;
			return this;
		}

		public Query WithTrendDays(int days)
		{
			trendDays = days;
			return this;
		}

		public Query MatchAnyTag()
		{
			matchAnyTag = true;
			return this;
		}

		public Query MatchAllTags()
		{
			matchAnyTag = false;
			return this;
		}

		public Query WithTag(string tag)
		{
			if (requiredTags == null)
			{
				requiredTags = new List<string>();
			}
			requiredTags.Add(tag);
			return this;
		}

		public Query AddRequiredKeyValueTag(string key, string value)
		{
			if (requiredKv == null)
			{
				requiredKv = new Dictionary<string, string>();
			}
			requiredKv.Add(key, value);
			return this;
		}

		public Query WithoutTag(string tag)
		{
			if (excludedTags == null)
			{
				excludedTags = new List<string>();
			}
			excludedTags.Add(tag);
			return this;
		}

		private void ApplyConstraints(UGCQueryHandle_t handle)
		{
			if (requiredTags != null)
			{
				foreach (string requiredTag in requiredTags)
				{
					SteamUGC.Internal.AddRequiredTag(handle, requiredTag);
				}
			}
			if (excludedTags != null)
			{
				foreach (string excludedTag in excludedTags)
				{
					SteamUGC.Internal.AddExcludedTag(handle, excludedTag);
				}
			}
			if (requiredKv != null)
			{
				foreach (KeyValuePair<string, string> item in requiredKv)
				{
					SteamUGC.Internal.AddRequiredKeyValueTag(handle, item.Key, item.Value);
				}
			}
			if (matchAnyTag.HasValue)
			{
				SteamUGC.Internal.SetMatchAnyTag(handle, matchAnyTag.Value);
			}
			if (trendDays.HasValue)
			{
				SteamUGC.Internal.SetRankedByTrendDays(handle, (uint)trendDays.Value);
			}
			if (!string.IsNullOrEmpty(searchText))
			{
				SteamUGC.Internal.SetSearchText(handle, searchText);
			}
		}

		public Query WithOnlyIDs(bool b)
		{
			WantsReturnOnlyIDs = b;
			return this;
		}

		public Query WithKeyValueTag(bool b)
		{
			WantsReturnKeyValueTags = b;
			return this;
		}

		public Query WithLongDescription(bool b)
		{
			WantsReturnLongDescription = b;
			return this;
		}

		public Query WithMetadata(bool b)
		{
			WantsReturnMetadata = b;
			return this;
		}

		public Query WithChildren(bool b)
		{
			WantsReturnChildren = b;
			return this;
		}

		public Query WithAdditionalPreviews(bool b)
		{
			WantsReturnAdditionalPreviews = b;
			return this;
		}

		public Query WithTotalOnly(bool b)
		{
			WantsReturnTotalOnly = b;
			return this;
		}

		public Query WithPlaytimeStats(uint unDays)
		{
			WantsReturnPlaytimeStats = unDays;
			return this;
		}

		private void ApplyReturns(UGCQueryHandle_t handle)
		{
			if (WantsReturnOnlyIDs.HasValue)
			{
				SteamUGC.Internal.SetReturnOnlyIDs(handle, WantsReturnOnlyIDs.Value);
			}
			if (WantsReturnKeyValueTags.HasValue)
			{
				SteamUGC.Internal.SetReturnKeyValueTags(handle, WantsReturnKeyValueTags.Value);
			}
			if (WantsReturnLongDescription.HasValue)
			{
				SteamUGC.Internal.SetReturnLongDescription(handle, WantsReturnLongDescription.Value);
			}
			if (WantsReturnMetadata.HasValue)
			{
				SteamUGC.Internal.SetReturnMetadata(handle, WantsReturnMetadata.Value);
			}
			if (WantsReturnChildren.HasValue)
			{
				SteamUGC.Internal.SetReturnChildren(handle, WantsReturnChildren.Value);
			}
			if (WantsReturnAdditionalPreviews.HasValue)
			{
				SteamUGC.Internal.SetReturnAdditionalPreviews(handle, WantsReturnAdditionalPreviews.Value);
			}
			if (WantsReturnTotalOnly.HasValue)
			{
				SteamUGC.Internal.SetReturnTotalOnly(handle, WantsReturnTotalOnly.Value);
			}
			if (WantsReturnPlaytimeStats.HasValue)
			{
				SteamUGC.Internal.SetReturnPlaytimeStats(handle, WantsReturnPlaytimeStats.Value);
			}
		}
	}
}
