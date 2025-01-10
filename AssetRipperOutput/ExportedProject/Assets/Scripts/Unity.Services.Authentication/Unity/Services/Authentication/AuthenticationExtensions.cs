using System.Text.RegularExpressions;
using Unity.Services.Core;

namespace Unity.Services.Authentication
{
	public static class AuthenticationExtensions
	{
		internal const string ProfileKey = "com.unity.services.authentication.profile";

		private const string k_ProfileRegex = "^[a-zA-Z0-9_-]{1,30}$";

		public static InitializationOptions SetProfile(this InitializationOptions options, string profile)
		{
			if (string.IsNullOrEmpty(profile) || !Regex.Match(profile, "^[a-zA-Z0-9_-]{1,30}$").Success)
			{
				throw AuthenticationException.Create(AuthenticationErrorCodes.ClientInvalidProfile, "Invalid profile name. The profile may only contain alphanumeric values, '-', '_', and must be no longer than 30 characters.");
			}
			return options.SetOption("com.unity.services.authentication.profile", profile);
		}
	}
}
