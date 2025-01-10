namespace Unity.Services.Core
{
	public static class CommonErrorCodes
	{
		public const int Unknown = 0;

		public const int TransportError = 1;

		public const int Timeout = 2;

		public const int ServiceUnavailable = 3;

		public const int ApiMissing = 4;

		public const int RequestRejected = 5;

		public const int TooManyRequests = 50;

		public const int InvalidToken = 51;

		public const int TokenExpired = 52;

		public const int Forbidden = 53;

		public const int NotFound = 54;

		public const int InvalidRequest = 55;

		public const int ProjectPolicyAccessDenied = 56;

		public const int PlayerPolicyAccessDenied = 57;
	}
}
