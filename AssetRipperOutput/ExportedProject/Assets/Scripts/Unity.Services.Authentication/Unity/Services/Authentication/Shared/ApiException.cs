using System;

namespace Unity.Services.Authentication.Shared
{
	internal class ApiException : Exception
	{
		public ApiExceptionType Type { get; private set; }

		public IApiResponse Response { get; private set; }

		public ApiException(ApiExceptionType type, string message, IApiResponse response = null)
			: base(message)
		{
			Type = type;
			Response = response;
		}
	}
}
