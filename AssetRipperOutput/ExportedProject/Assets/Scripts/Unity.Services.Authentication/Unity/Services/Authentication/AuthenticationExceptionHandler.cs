using System;
using System.Text;
using Newtonsoft.Json;
using Unity.Services.Authentication.Shared;
using Unity.Services.Core;

namespace Unity.Services.Authentication
{
	internal class AuthenticationExceptionHandler : IAuthenticationExceptionHandler
	{
		private IAuthenticationMetrics Metrics { get; }

		public AuthenticationExceptionHandler(IAuthenticationMetrics metrics)
		{
			Metrics = metrics;
		}

		public RequestFailedException BuildClientInvalidStateException(AuthenticationState state)
		{
			string message = string.Empty;
			switch (state)
			{
			case AuthenticationState.SignedOut:
				message = "Invalid state for this operation. The player is signed out.";
				break;
			case AuthenticationState.SigningIn:
				message = "Invalid state for this operation. The player is already signing in.";
				break;
			case AuthenticationState.Authorized:
			case AuthenticationState.Refreshing:
				message = "Invalid state for this operation. The player is already signed in.";
				break;
			case AuthenticationState.Expired:
				message = "Invalid state for this operation. The player session has expired.";
				break;
			}
			Metrics.SendClientInvalidStateExceptionMetric();
			return AuthenticationException.Create(AuthenticationErrorCodes.ClientInvalidUserState, message);
		}

		public RequestFailedException BuildClientInvalidProfileException()
		{
			return AuthenticationException.Create(AuthenticationErrorCodes.ClientInvalidProfile, "Invalid profile name. The profile may only contain alphanumeric values, '-', '_', and must be no longer than 30 characters.");
		}

		public RequestFailedException BuildClientUnlinkExternalIdNotFoundException()
		{
			Metrics.SendUnlinkExternalIdNotFoundExceptionMetric();
			return AuthenticationException.Create(AuthenticationErrorCodes.ClientUnlinkExternalIdNotFound, "No external id was found to unlink from the provider. Use GetPlayerInfoAsync to load the linked external ids.");
		}

		public RequestFailedException BuildClientSessionTokenNotExistsException()
		{
			Metrics.SendClientSessionTokenNotExistsExceptionMetric();
			return AuthenticationException.Create(AuthenticationErrorCodes.ClientNoActiveSession, "There is no cached session token.");
		}

		public RequestFailedException BuildUnknownException(string error)
		{
			return AuthenticationException.Create(0, error);
		}

		public RequestFailedException BuildInvalidIdProviderNameException()
		{
			return AuthenticationException.Create(AuthenticationErrorCodes.InvalidParameters, "Invalid IdProviderName. The Id Provider name should start with 'oidc-' and have between 6 and 20 characters (including 'oidc-')");
		}

		public RequestFailedException BuildInvalidPlayerNameException()
		{
			return AuthenticationException.Create(AuthenticationErrorCodes.InvalidParameters, "Invalid Player Name. Player names cannot be empty or contain spaces.");
		}

		public RequestFailedException BuildInvalidCredentialsException()
		{
			return AuthenticationException.Create(AuthenticationErrorCodes.InvalidParameters, "Username and/or Password are not in the correct format");
		}

		public RequestFailedException ConvertException(WebRequestException exception)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string value = $"Request failed: {exception.ResponseCode}, {exception.Message}";
			stringBuilder.Append(value);
			if (exception.ResponseHeaders != null && exception.ResponseHeaders.TryGetValue("x-request-id", out var value2))
			{
				stringBuilder.Append(", request-id: " + value2);
			}
			Logger.Log(stringBuilder.ToString());
			if (exception.NetworkError)
			{
				Metrics.SendNetworkErrorMetric();
				return AuthenticationException.Create(1, "Network Error: " + exception.Message, exception);
			}
			try
			{
				AuthenticationErrorResponse authenticationErrorResponse = IsolatedJsonConvert.DeserializeObject<AuthenticationErrorResponse>(exception.Message, SerializerSettings.DefaultSerializerSettings);
				return AuthenticationException.Create(MapErrorCodes(authenticationErrorResponse.Title), authenticationErrorResponse.Detail, exception);
			}
			catch (JsonException innerException)
			{
				return AuthenticationException.Create(0, "Failed to deserialize server response.", innerException);
			}
			catch (Exception)
			{
				return AuthenticationException.Create(0, "Unknown error deserializing server response. ", exception);
			}
		}

		public RequestFailedException ConvertException(ApiException exception)
		{
			return exception?.Type switch
			{
				ApiExceptionType.InvalidParameters => AuthenticationException.Create(AuthenticationErrorCodes.InvalidParameters, exception.Message), 
				ApiExceptionType.Deserialization => AuthenticationException.Create(0, exception.Message), 
				ApiExceptionType.Network => CreateNetworkException(exception), 
				ApiExceptionType.Http => CreateHttpException(exception), 
				_ => CreateUnknownException(exception), 
			};
		}

		private static RequestFailedException CreateNetworkException(ApiException exception)
		{
			return exception?.Response?.StatusCode switch
			{
				503 => AuthenticationException.Create(3, exception.Message), 
				504 => AuthenticationException.Create(2, exception.Message), 
				_ => AuthenticationException.Create(1, exception.Message), 
			};
		}

		private static RequestFailedException CreateHttpException(ApiException exception)
		{
			return exception?.Response?.StatusCode switch
			{
				400 => AuthenticationException.Create(55, exception.Message), 
				401 => AuthenticationException.Create(51, exception.Message), 
				403 => AuthenticationException.Create(53, exception.Message), 
				404 => AuthenticationException.Create(54, exception.Message), 
				408 => AuthenticationException.Create(2, exception.Message), 
				429 => AuthenticationException.Create(50, exception.Message), 
				_ => AuthenticationException.Create(55, exception.Message), 
			};
		}

		private static RequestFailedException CreateUnknownException(Exception exception)
		{
			return AuthenticationException.Create(0, "Unknown Error: " + exception.Message);
		}

		private int MapErrorCodes(string serverErrorTitle)
		{
			return serverErrorTitle switch
			{
				"ENTITY_EXISTS" => AuthenticationErrorCodes.AccountAlreadyLinked, 
				"LINKED_ACCOUNT_LIMIT_EXCEEDED" => AuthenticationErrorCodes.AccountLinkLimitExceeded, 
				"INVALID_PARAMETERS" => AuthenticationErrorCodes.InvalidParameters, 
				"INVALID_SESSION_TOKEN" => AuthenticationErrorCodes.InvalidSessionToken, 
				"PERMISSION_DENIED" => AuthenticationErrorCodes.InvalidParameters, 
				"UNAUTHORIZED_REQUEST" => 51, 
				_ => 0, 
			};
		}
	}
}
