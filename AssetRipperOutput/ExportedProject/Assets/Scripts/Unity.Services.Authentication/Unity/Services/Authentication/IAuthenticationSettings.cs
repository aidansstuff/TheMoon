namespace Unity.Services.Authentication
{
	internal interface IAuthenticationSettings
	{
		int AccessTokenRefreshBuffer { get; }

		int AccessTokenExpiryBuffer { get; }

		int RefreshAttemptFrequency { get; }
	}
}
