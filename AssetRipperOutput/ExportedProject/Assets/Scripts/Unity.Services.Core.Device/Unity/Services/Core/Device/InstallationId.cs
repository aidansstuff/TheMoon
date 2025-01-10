using System;
using Unity.Services.Core.Device.Internal;
using Unity.Services.Core.Internal;
using UnityEngine;

namespace Unity.Services.Core.Device
{
	internal class InstallationId : IInstallationId, IServiceComponent
	{
		private const string k_UnityInstallationIdKey = "UnityInstallationId";

		internal string Identifier;

		internal IUserIdentifierProvider UnityAdsIdentifierProvider;

		internal IUserIdentifierProvider UnityAnalyticsIdentifierProvider;

		public InstallationId()
		{
			UnityAdsIdentifierProvider = new UnityAdsIdentifier();
			UnityAnalyticsIdentifierProvider = new UnityAnalyticsIdentifier();
		}

		public string GetOrCreateIdentifier()
		{
			if (string.IsNullOrEmpty(Identifier))
			{
				CreateIdentifier();
			}
			return Identifier;
		}

		public void CreateIdentifier()
		{
			Identifier = ReadIdentifierFromFile();
			if (string.IsNullOrEmpty(Identifier))
			{
				string userId = UnityAnalyticsIdentifierProvider.UserId;
				string userId2 = UnityAdsIdentifierProvider.UserId;
				if (!string.IsNullOrEmpty(userId))
				{
					Identifier = userId;
				}
				else if (!string.IsNullOrEmpty(userId2))
				{
					Identifier = userId2;
				}
				else
				{
					Identifier = GenerateGuid();
				}
				WriteIdentifierToFile(Identifier);
				if (string.IsNullOrEmpty(userId))
				{
					UnityAnalyticsIdentifierProvider.UserId = Identifier;
				}
				if (string.IsNullOrEmpty(userId2))
				{
					UnityAdsIdentifierProvider.UserId = Identifier;
				}
			}
		}

		private static string ReadIdentifierFromFile()
		{
			return PlayerPrefs.GetString("UnityInstallationId");
		}

		private static void WriteIdentifierToFile(string identifier)
		{
			PlayerPrefs.SetString("UnityInstallationId", identifier);
			PlayerPrefs.Save();
		}

		private static string GenerateGuid()
		{
			return Guid.NewGuid().ToString();
		}
	}
}
