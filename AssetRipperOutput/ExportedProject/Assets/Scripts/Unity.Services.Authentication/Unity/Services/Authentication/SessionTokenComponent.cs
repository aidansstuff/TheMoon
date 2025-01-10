namespace Unity.Services.Authentication
{
	internal class SessionTokenComponent
	{
		private const string k_CacheKey = "session_token";

		private readonly IAuthenticationCache m_Cache;

		internal string SessionToken
		{
			get
			{
				return GetSessionToken();
			}
			set
			{
				SetSessionToken(value);
			}
		}

		internal SessionTokenComponent(IAuthenticationCache cache)
		{
			m_Cache = cache;
		}

		internal void Clear()
		{
			m_Cache.DeleteKey("session_token");
		}

		internal void Migrate()
		{
			m_Cache.Migrate("session_token");
		}

		private string GetSessionToken()
		{
			return m_Cache.GetString("session_token");
		}

		private void SetSessionToken(string sessionToken)
		{
			m_Cache.SetString("session_token", sessionToken);
		}
	}
}
