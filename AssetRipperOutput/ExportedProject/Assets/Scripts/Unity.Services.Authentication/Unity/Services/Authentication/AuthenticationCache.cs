using JetBrains.Annotations;
using Unity.Services.Core.Configuration.Internal;
using UnityEngine;

namespace Unity.Services.Authentication
{
	internal class AuthenticationCache : IAuthenticationCache, ICache
	{
		private ICloudProjectId m_CloudProjectId;

		private IProfile m_Profile;

		public string CloudProjectId => m_CloudProjectId.GetCloudProjectId();

		public string Profile => m_Profile.Current;

		private string Prefix => CloudProjectId + "." + Profile + ".unity.services.authentication.";

		private string OldPrefix => "unity.services.authentication.";

		public AuthenticationCache(ICloudProjectId cloudProjectId, IProfile profile)
		{
			m_CloudProjectId = cloudProjectId;
			m_Profile = profile;
		}

		public bool HasKey(string key)
		{
			return PlayerPrefs.HasKey(GetKey(key));
		}

		public void DeleteKey(string key)
		{
			PlayerPrefs.DeleteKey(GetKey(key));
		}

		[CanBeNull]
		public string GetString(string key)
		{
			if (!HasKey(key))
			{
				return null;
			}
			return PlayerPrefs.GetString(GetKey(key));
		}

		public void SetString(string key, string value)
		{
			PlayerPrefs.SetString(GetKey(key), value);
			PlayerPrefs.Save();
		}

		public void Migrate(string key)
		{
			string oldKey = GetOldKey(key);
			if (PlayerPrefs.HasKey(oldKey))
			{
				PlayerPrefs.SetString(GetKey(key), PlayerPrefs.GetString(oldKey));
				PlayerPrefs.DeleteKey(oldKey);
			}
		}

		internal string GetKey(string key)
		{
			return Prefix + key;
		}

		internal string GetOldKey(string key)
		{
			return OldPrefix + key;
		}
	}
}
