using System;
using Unity.Services.Core;

namespace Unity.Services.Authentication
{
	public sealed class AuthenticationException : RequestFailedException
	{
		private AuthenticationException(int errorCode, string message, Exception innerException = null)
			: base(errorCode, message, innerException)
		{
		}

		public static RequestFailedException Create(int errorCode, string message, Exception innerException = null)
		{
			if (errorCode < AuthenticationErrorCodes.MinValue)
			{
				return new RequestFailedException(errorCode, message, innerException);
			}
			return new AuthenticationException(errorCode, message, innerException);
		}
	}
}
