using System;

namespace Unity.Services.Core.Environments
{
	public static class EnvironmentsOptionsExtensions
	{
		internal const string EnvironmentNameKey = "com.unity.services.core.environment-name";

		internal const string EnvironmentDefaultNameValue = "production";

		public static InitializationOptions SetEnvironmentName(this InitializationOptions self, string environmentName)
		{
			if (string.IsNullOrEmpty(environmentName))
			{
				throw new ArgumentException("Environment name cannot be null or empty.", "environmentName");
			}
			self.SetOption("com.unity.services.core.environment-name", environmentName);
			return self;
		}
	}
}
