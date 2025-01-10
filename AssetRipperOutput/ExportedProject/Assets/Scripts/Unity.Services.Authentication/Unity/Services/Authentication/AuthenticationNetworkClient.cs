using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Environments.Internal;

namespace Unity.Services.Authentication
{
	internal class AuthenticationNetworkClient : IAuthenticationNetworkClient
	{
		private const string k_AnonymousUrlStem = "/v1/authentication/anonymous";

		private const string k_SessionTokenUrlStem = "/v1/authentication/session-token";

		private const string k_ExternalTokenUrlStem = "/v1/authentication/external-token";

		private const string k_LinkExternalTokenUrlStem = "/v1/authentication/link";

		private const string k_UnlinkExternalTokenUrlStem = "/v1/authentication/unlink";

		private const string k_UsersUrlStem = "/v1/users";

		private const string k_UsernamePasswordSignInUrlStem = "/v1/authentication/usernamepassword/sign-in";

		private const string k_UsernamePasswordSignUpUrlStem = "/v1/authentication/usernamepassword/sign-up";

		private const string k_UpdatePasswordUrlStem = "/v1/authentication/usernamepassword/update-password";

		private readonly string m_AnonymousUrl;

		private readonly string m_SessionTokenUrl;

		private readonly string m_ExternalTokenUrl;

		private readonly string m_LinkExternalTokenUrl;

		private readonly string m_UnlinkExternalTokenUrl;

		private readonly string m_UsersUrl;

		private readonly string m_UsernamePasswordSignInUrl;

		private readonly string m_UsernamePasswordSignUpUrl;

		private readonly string m_UpdatePasswordUrl;

		private readonly Dictionary<string, string> m_CommonHeaders;

		internal AccessTokenComponent AccessTokenComponent { get; }

		internal ICloudProjectId CloudProjectIdComponent { get; }

		internal IEnvironments EnvironmentComponent { get; }

		internal INetworkHandler NetworkHandler { get; }

		private string AccessToken => AccessTokenComponent.AccessToken;

		private string EnvironmentName => EnvironmentComponent.Current;

		internal AuthenticationNetworkClient(string host, ICloudProjectId cloudProjectId, IEnvironments environment, INetworkHandler networkHandler, AccessTokenComponent accessToken)
		{
			AccessTokenComponent = accessToken;
			CloudProjectIdComponent = cloudProjectId;
			EnvironmentComponent = environment;
			NetworkHandler = networkHandler;
			m_AnonymousUrl = host + "/v1/authentication/anonymous";
			m_SessionTokenUrl = host + "/v1/authentication/session-token";
			m_ExternalTokenUrl = host + "/v1/authentication/external-token";
			m_LinkExternalTokenUrl = host + "/v1/authentication/link";
			m_UnlinkExternalTokenUrl = host + "/v1/authentication/unlink";
			m_UsersUrl = host + "/v1/users";
			m_UsernamePasswordSignInUrl = host + "/v1/authentication/usernamepassword/sign-in";
			m_UsernamePasswordSignUpUrl = host + "/v1/authentication/usernamepassword/sign-up";
			m_UpdatePasswordUrl = host + "/v1/authentication/usernamepassword/update-password";
			m_CommonHeaders = new Dictionary<string, string>
			{
				["ProjectId"] = CloudProjectIdComponent.GetCloudProjectId(),
				["Error-Version"] = "v1"
			};
		}

		public Task<SignInResponse> SignInAnonymouslyAsync()
		{
			return NetworkHandler.PostAsync<SignInResponse>(m_AnonymousUrl, WithEnvironment(GetCommonHeaders()));
		}

		public Task<SignInResponse> SignInWithSessionTokenAsync(string token)
		{
			return NetworkHandler.PostAsync<SignInResponse>(m_SessionTokenUrl, new SessionTokenRequest
			{
				SessionToken = token
			}, WithEnvironment(GetCommonHeaders()));
		}

		public Task<SignInResponse> SignInWithExternalTokenAsync(string idProvider, SignInWithExternalTokenRequest externalToken)
		{
			string url = m_ExternalTokenUrl + "/" + idProvider;
			return NetworkHandler.PostAsync<SignInResponse>(url, externalToken, WithEnvironment(GetCommonHeaders()));
		}

		public Task<LinkResponse> LinkWithExternalTokenAsync(string idProvider, LinkWithExternalTokenRequest externalToken)
		{
			string url = m_LinkExternalTokenUrl + "/" + idProvider;
			return NetworkHandler.PostAsync<LinkResponse>(url, externalToken, WithEnvironment(WithAccessToken(GetCommonHeaders())));
		}

		public Task<UnlinkResponse> UnlinkExternalTokenAsync(string idProvider, UnlinkRequest request)
		{
			string url = m_UnlinkExternalTokenUrl + "/" + idProvider;
			return NetworkHandler.PostAsync<UnlinkResponse>(url, request, WithEnvironment(WithAccessToken(GetCommonHeaders())));
		}

		public Task<PlayerInfoResponse> GetPlayerInfoAsync(string playerId)
		{
			return NetworkHandler.GetAsync<PlayerInfoResponse>(CreateUserRequestUrl(playerId), WithAccessToken(GetCommonHeaders()));
		}

		public Task DeleteAccountAsync(string playerId)
		{
			return NetworkHandler.DeleteAsync(CreateUserRequestUrl(playerId), WithEnvironment(WithAccessToken(GetCommonHeaders())));
		}

		public Task<SignInResponse> SignInWithUsernamePasswordAsync(UsernamePasswordRequest credentials)
		{
			return NetworkHandler.PostAsync<SignInResponse>(m_UsernamePasswordSignInUrl, credentials, WithEnvironment(GetCommonHeaders()));
		}

		public Task<SignInResponse> SignUpWithUsernamePasswordAsync(UsernamePasswordRequest credentials)
		{
			return NetworkHandler.PostAsync<SignInResponse>(m_UsernamePasswordSignUpUrl, credentials, WithEnvironment(GetCommonHeaders()));
		}

		public Task<SignInResponse> AddUsernamePasswordAsync(UsernamePasswordRequest credentials)
		{
			return NetworkHandler.PostAsync<SignInResponse>(m_UsernamePasswordSignUpUrl, credentials, WithEnvironment(WithAccessToken(GetCommonHeaders())));
		}

		public Task<SignInResponse> UpdatePasswordAsync(UpdatePasswordRequest credentials)
		{
			return NetworkHandler.PostAsync<SignInResponse>(m_UpdatePasswordUrl, credentials, WithEnvironment(WithAccessToken(GetCommonHeaders())));
		}

		private string CreateUserRequestUrl(string user)
		{
			return m_UsersUrl + "/" + user;
		}

		private Dictionary<string, string> WithAccessToken(Dictionary<string, string> headers)
		{
			headers["Authorization"] = "Bearer " + AccessToken;
			return headers;
		}

		private Dictionary<string, string> WithEnvironment(Dictionary<string, string> headers)
		{
			string environmentName = EnvironmentName;
			if (!string.IsNullOrEmpty(environmentName))
			{
				headers["UnityEnvironment"] = environmentName;
			}
			return headers;
		}

		private Dictionary<string, string> GetCommonHeaders()
		{
			return new Dictionary<string, string>(m_CommonHeaders);
		}
	}
}
