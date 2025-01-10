using System;

namespace Unity.Services.Core
{
	public class RequestFailedException : Exception
	{
		public int ErrorCode { get; }

		public RequestFailedException(int errorCode, string message)
			: this(errorCode, message, null)
		{
		}

		public RequestFailedException(int errorCode, string message, Exception innerException)
			: base(message, innerException)
		{
			ErrorCode = errorCode;
		}
	}
}
