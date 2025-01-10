using Unity.Services.Authentication.Shared;
using Unity.Services.Core;

namespace Unity.Services.Authentication
{
	internal interface IAuthenticationExceptionHandler
	{
		RequestFailedException BuildClientInvalidStateException(AuthenticationState state);

		RequestFailedException BuildClientInvalidProfileException();

		RequestFailedException BuildClientUnlinkExternalIdNotFoundException();

		RequestFailedException BuildClientSessionTokenNotExistsException();

		RequestFailedException BuildUnknownException(string error);

		RequestFailedException BuildInvalidIdProviderNameException();

		RequestFailedException BuildInvalidPlayerNameException();

		RequestFailedException BuildInvalidCredentialsException();

		RequestFailedException ConvertException(WebRequestException exception);

		RequestFailedException ConvertException(ApiException exception);
	}
}
