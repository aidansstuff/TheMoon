using System;
using UnityEngine;

namespace Unity.Services.Core.Device
{
	internal class UnityAnalyticsIdentifier : IUserIdentifierProvider
	{
		private const string k_PlayerUserIdKey = "unity.cloud_userid";

		public string UserId
		{
			get
			{
				return PlayerPrefs.GetString("unity.cloud_userid");
			}
			set
			{
				try
				{
					PlayerPrefs.SetString("unity.cloud_userid", value);
					PlayerPrefs.Save();
				}
				catch (Exception)
				{
				}
			}
		}
	}
}
