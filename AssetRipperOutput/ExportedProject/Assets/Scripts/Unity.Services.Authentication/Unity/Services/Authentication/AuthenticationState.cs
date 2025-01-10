namespace Unity.Services.Authentication
{
	internal enum AuthenticationState
	{
		SignedOut = 0,
		SigningIn = 1,
		Authorized = 2,
		Refreshing = 3,
		Expired = 4
	}
}
