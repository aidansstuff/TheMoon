using System;
using Unity.Services.Core;

namespace Unity.Services.Relay
{
	public class RelayServiceException : RequestFailedException
	{
		public RelayExceptionReason Reason { get; private set; }

		public RelayServiceException(RelayExceptionReason reason, string message, Exception innerException)
			: base((int)reason, message, innerException)
		{
			Reason = reason;
		}

		public RelayServiceException(RelayExceptionReason reason, string message)
			: base((int)reason, message)
		{
			Reason = reason;
		}

		public RelayServiceException(long errorCode, string message)
			: base((int)errorCode, message)
		{
			if (Enum.IsDefined(typeof(RelayExceptionReason), errorCode))
			{
				Reason = (RelayExceptionReason)errorCode;
			}
			else
			{
				Reason = RelayExceptionReason.Unknown;
			}
		}

		public RelayServiceException(Exception innerException)
			: base(15999, "Unknown Relay Service Exception", innerException)
		{
		}
	}
}
