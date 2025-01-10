using System;

namespace Unity.Services.Core.Analytics
{
	public static class AnalyticsOptionsExtensions
	{
		internal const string AnalyticsUserIdKey = "com.unity.services.core.analytics-user-id";

		[Obsolete("SetAnalyticsUserId is deprecated. Please use UnityServices.ExternalUserId instead.", false)]
		public static InitializationOptions SetAnalyticsUserId(this InitializationOptions self, string id)
		{
			if (string.IsNullOrEmpty(id))
			{
				throw new ArgumentException("Analytics user id cannot be null or empty.", "id");
			}
			return self.SetOption("com.unity.services.core.analytics-user-id", id);
		}
	}
}
