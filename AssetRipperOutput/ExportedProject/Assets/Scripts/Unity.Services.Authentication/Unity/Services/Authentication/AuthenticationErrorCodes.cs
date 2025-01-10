namespace Unity.Services.Authentication
{
	public static class AuthenticationErrorCodes
	{
		public static readonly int MinValue = 10000;

		public static readonly int ClientInvalidUserState = 10000;

		public static readonly int ClientNoActiveSession = 10001;

		public static readonly int InvalidParameters = 10002;

		public static readonly int AccountAlreadyLinked = 10003;

		public static readonly int AccountLinkLimitExceeded = 10004;

		public static readonly int ClientUnlinkExternalIdNotFound = 10005;

		public static readonly int ClientInvalidProfile = 10006;

		public static readonly int InvalidSessionToken = 10007;
	}
}
