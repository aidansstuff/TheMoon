using System;
using Unity.Services.Relay.Http;
using Unity.Services.Relay.Models;

namespace Unity.Services.Relay
{
	internal static class ApiErrorExtender
	{
		public static RelayExceptionReason GetExceptionReason(this ErrorResponseBody error)
		{
			RelayExceptionReason result = RelayExceptionReason.Unknown;
			if (error.Code != 15000)
			{
				if (Enum.IsDefined(typeof(RelayExceptionReason), error.Code))
				{
					result = (RelayExceptionReason)error.Code;
				}
			}
			else if (Enum.IsDefined(typeof(RelayExceptionReason), error.Status))
			{
				result = (RelayExceptionReason)error.Status;
			}
			return result;
		}

		public static RelayExceptionReason GetExceptionReason(this HttpClientResponse error)
		{
			RelayExceptionReason result = RelayExceptionReason.Unknown;
			if (error.IsHttpError)
			{
				int num = (int)error.StatusCode + 15000;
				if (Enum.IsDefined(typeof(RelayExceptionReason), num))
				{
					result = (RelayExceptionReason)num;
				}
			}
			else if (error.IsNetworkError)
			{
				result = RelayExceptionReason.NetworkError;
			}
			return result;
		}

		public static string GetExceptionMessage(this ErrorResponseBody error)
		{
			return error.Title + ": " + error?.Detail;
		}
	}
}
