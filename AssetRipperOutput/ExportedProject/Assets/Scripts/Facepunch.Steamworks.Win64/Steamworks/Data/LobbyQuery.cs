using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Steamworks.Data
{
	public struct LobbyQuery
	{
		internal LobbyDistanceFilter? distance;

		internal Dictionary<string, string> stringFilters;

		internal List<NumericalFilter> numericalFilters;

		internal Dictionary<string, int> nearValFilters;

		internal int? slotsAvailable;

		internal int? maxResults;

		public LobbyQuery FilterDistanceClose()
		{
			distance = LobbyDistanceFilter.Close;
			return this;
		}

		public LobbyQuery FilterDistanceFar()
		{
			distance = LobbyDistanceFilter.Far;
			return this;
		}

		public LobbyQuery FilterDistanceWorldwide()
		{
			distance = LobbyDistanceFilter.Worldwide;
			return this;
		}

		public LobbyQuery WithKeyValue(string key, string value)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentException("Key string provided for LobbyQuery filter is null or empty", "key");
			}
			if (key.Length > SteamMatchmaking.MaxLobbyKeyLength)
			{
				throw new ArgumentException($"Key length is longer than {SteamMatchmaking.MaxLobbyKeyLength}", "key");
			}
			if (stringFilters == null)
			{
				stringFilters = new Dictionary<string, string>();
			}
			stringFilters.Add(key, value);
			return this;
		}

		public LobbyQuery WithLower(string key, int value)
		{
			AddNumericalFilter(key, value, LobbyComparison.LessThan);
			return this;
		}

		public LobbyQuery WithHigher(string key, int value)
		{
			AddNumericalFilter(key, value, LobbyComparison.GreaterThan);
			return this;
		}

		public LobbyQuery WithEqual(string key, int value)
		{
			AddNumericalFilter(key, value, LobbyComparison.Equal);
			return this;
		}

		public LobbyQuery WithNotEqual(string key, int value)
		{
			AddNumericalFilter(key, value, LobbyComparison.NotEqual);
			return this;
		}

		internal void AddNumericalFilter(string key, int value, LobbyComparison compare)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentException("Key string provided for LobbyQuery filter is null or empty", "key");
			}
			if (key.Length > SteamMatchmaking.MaxLobbyKeyLength)
			{
				throw new ArgumentException($"Key length is longer than {SteamMatchmaking.MaxLobbyKeyLength}", "key");
			}
			if (numericalFilters == null)
			{
				numericalFilters = new List<NumericalFilter>();
			}
			numericalFilters.Add(new NumericalFilter(key, value, compare));
		}

		public LobbyQuery OrderByNear(string key, int value)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentException("Key string provided for LobbyQuery filter is null or empty", "key");
			}
			if (key.Length > SteamMatchmaking.MaxLobbyKeyLength)
			{
				throw new ArgumentException($"Key length is longer than {SteamMatchmaking.MaxLobbyKeyLength}", "key");
			}
			if (nearValFilters == null)
			{
				nearValFilters = new Dictionary<string, int>();
			}
			nearValFilters.Add(key, value);
			return this;
		}

		public LobbyQuery WithSlotsAvailable(int minSlots)
		{
			slotsAvailable = minSlots;
			return this;
		}

		public LobbyQuery WithMaxResults(int max)
		{
			maxResults = max;
			return this;
		}

		private void ApplyFilters()
		{
			if (distance.HasValue)
			{
				SteamMatchmaking.Internal.AddRequestLobbyListDistanceFilter(distance.Value);
			}
			if (slotsAvailable.HasValue)
			{
				SteamMatchmaking.Internal.AddRequestLobbyListFilterSlotsAvailable(slotsAvailable.Value);
			}
			if (maxResults.HasValue)
			{
				SteamMatchmaking.Internal.AddRequestLobbyListResultCountFilter(maxResults.Value);
			}
			if (stringFilters != null)
			{
				foreach (KeyValuePair<string, string> stringFilter in stringFilters)
				{
					SteamMatchmaking.Internal.AddRequestLobbyListStringFilter(stringFilter.Key, stringFilter.Value, LobbyComparison.Equal);
				}
			}
			if (numericalFilters != null)
			{
				foreach (NumericalFilter numericalFilter in numericalFilters)
				{
					SteamMatchmaking.Internal.AddRequestLobbyListNumericalFilter(numericalFilter.Key, numericalFilter.Value, numericalFilter.Comparer);
				}
			}
			if (nearValFilters == null)
			{
				return;
			}
			foreach (KeyValuePair<string, int> nearValFilter in nearValFilters)
			{
				SteamMatchmaking.Internal.AddRequestLobbyListNearValueFilter(nearValFilter.Key, nearValFilter.Value);
			}
		}

		public async Task<Lobby[]> RequestAsync()
		{
			ApplyFilters();
			LobbyMatchList_t? list = await SteamMatchmaking.Internal.RequestLobbyList();
			if (!list.HasValue || list.Value.LobbiesMatching == 0)
			{
				return null;
			}
			Lobby[] lobbies = new Lobby[list.Value.LobbiesMatching];
			for (int i = 0; i < list.Value.LobbiesMatching; i++)
			{
				lobbies[i] = new Lobby
				{
					Id = SteamMatchmaking.Internal.GetLobbyByIndex(i)
				};
			}
			return lobbies;
		}
	}
}
