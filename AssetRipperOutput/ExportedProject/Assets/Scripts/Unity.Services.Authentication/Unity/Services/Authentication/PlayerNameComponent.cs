using System;
using Unity.Services.Authentication.Internal;

namespace Unity.Services.Authentication
{
	internal class PlayerNameComponent : IPlayerName
	{
		private const string k_CacheKey = "player_name";

		private readonly IAuthenticationCache m_Cache;

		public string PlayerName
		{
			get
			{
				return GetPlayerName();
			}
			internal set
			{
				SetPlayerName(value);
			}
		}

		public event Action<string> PlayerNameChanged;

		internal PlayerNameComponent(IAuthenticationCache cache)
		{
			m_Cache = cache;
		}

		internal void Clear()
		{
			m_Cache.DeleteKey("player_name");
		}

		private string GetPlayerName()
		{
			return m_Cache.GetString("player_name");
		}

		private void SetPlayerName(string playerName)
		{
			if (PlayerName != playerName)
			{
				m_Cache.SetString("player_name", playerName);
				this.PlayerNameChanged?.Invoke(playerName);
			}
		}
	}
}
