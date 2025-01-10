using System;
using Unity.Services.Authentication.Internal;
using Unity.Services.Core.Internal;

namespace Unity.Services.Authentication
{
	internal class PlayerIdComponent : IPlayerId, IServiceComponent
	{
		private const string k_CacheKey = "player_id";

		private readonly IAuthenticationCache m_Cache;

		public string PlayerId
		{
			get
			{
				return GetPlayerId();
			}
			internal set
			{
				SetPlayerId(value);
			}
		}

		public event Action<string> PlayerIdChanged;

		internal PlayerIdComponent(IAuthenticationCache cache)
		{
			m_Cache = cache;
		}

		internal void Clear()
		{
			m_Cache.DeleteKey("player_id");
		}

		private string GetPlayerId()
		{
			return m_Cache.GetString("player_id");
		}

		private void SetPlayerId(string playerId)
		{
			if (PlayerId != playerId)
			{
				m_Cache.SetString("player_id", playerId);
				this.PlayerIdChanged?.Invoke(playerId);
			}
		}
	}
}
