using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Authentication.Shared;

namespace Unity.Services.Authentication.Generated
{
	internal class PlayerNamesApi : IPlayerNamesApi, IApiAccessor
	{
		public IApiClient Client { get; }

		public IApiConfiguration Configuration { get; }

		public PlayerNamesApi(IApiClient apiClient)
		{
			if (apiClient == null)
			{
				throw new ArgumentNullException("apiClient");
			}
			Client = apiClient;
			Configuration = new ApiConfiguration
			{
				BasePath = "https://social.services.api.unity.com/v1"
			};
		}

		public PlayerNamesApi(IApiClient apiClient, IApiConfiguration apiConfiguration)
		{
			if (apiClient == null)
			{
				throw new ArgumentNullException("apiClient");
			}
			if (apiConfiguration == null)
			{
				throw new ArgumentNullException("apiConfiguration");
			}
			Client = apiClient;
			Configuration = apiConfiguration;
		}

		public string GetBasePath()
		{
			return Configuration.BasePath;
		}

		public async Task<ApiResponse<Player>> GetNameAsync(string playerId, bool? autoGenerate = null, bool? showMetadata = null, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (playerId == null)
			{
				throw new ApiException(ApiExceptionType.InvalidParameters, "Missing required parameter 'playerId' when calling PlayerNamesApi->GetName");
			}
			ApiRequestOptions apiRequestOptions = new ApiRequestOptions();
			string[] contentTypes = new string[0];
			string[] accepts = new string[2] { "application/json", "application/problem+json" };
			string text = ApiUtils.SelectHeaderContentType(contentTypes);
			if (text != null)
			{
				apiRequestOptions.HeaderParameters.Add("Content-Type", text);
			}
			string text2 = ApiUtils.SelectHeaderAccept(accepts);
			if (text2 != null)
			{
				apiRequestOptions.HeaderParameters.Add("Accept", text2);
			}
			apiRequestOptions.PathParameters.Add("playerId", ApiUtils.ParameterToString(Configuration, playerId));
			if (autoGenerate.HasValue)
			{
				apiRequestOptions.QueryParameters.Add(ApiUtils.ParameterToMultiMap(Configuration, "", "autoGenerate", autoGenerate));
			}
			if (showMetadata.HasValue)
			{
				apiRequestOptions.QueryParameters.Add(ApiUtils.ParameterToMultiMap(Configuration, "", "showMetadata", showMetadata));
			}
			apiRequestOptions.Operation = "PlayerNamesApi.GetName";
			if (!string.IsNullOrEmpty(Configuration.AccessToken) && !apiRequestOptions.HeaderParameters.ContainsKey("Authorization"))
			{
				apiRequestOptions.HeaderParameters.Add("Authorization", "Bearer " + Configuration.AccessToken);
			}
			return await Client.GetAsync<Player>("/names/{playerId}", apiRequestOptions, Configuration, cancellationToken);
		}

		public async Task<ApiResponse<Player>> UpdateNameAsync(string playerId, UpdateNameRequest updateNameRequest, CancellationToken cancellationToken = default(CancellationToken))
		{
			if (playerId == null)
			{
				throw new ApiException(ApiExceptionType.InvalidParameters, "Missing required parameter 'playerId' when calling PlayerNamesApi->UpdateName");
			}
			if (updateNameRequest == null)
			{
				throw new ApiException(ApiExceptionType.InvalidParameters, "Missing required parameter 'updateNameRequest' when calling PlayerNamesApi->UpdateName");
			}
			ApiRequestOptions apiRequestOptions = new ApiRequestOptions();
			string[] contentTypes = new string[1] { "application/json" };
			string[] accepts = new string[2] { "application/json", "application/problem+json" };
			string text = ApiUtils.SelectHeaderContentType(contentTypes);
			if (text != null)
			{
				apiRequestOptions.HeaderParameters.Add("Content-Type", text);
			}
			string text2 = ApiUtils.SelectHeaderAccept(accepts);
			if (text2 != null)
			{
				apiRequestOptions.HeaderParameters.Add("Accept", text2);
			}
			apiRequestOptions.PathParameters.Add("playerId", ApiUtils.ParameterToString(Configuration, playerId));
			apiRequestOptions.Data = updateNameRequest;
			apiRequestOptions.Operation = "PlayerNamesApi.UpdateName";
			if (!string.IsNullOrEmpty(Configuration.AccessToken) && !apiRequestOptions.HeaderParameters.ContainsKey("Authorization"))
			{
				apiRequestOptions.HeaderParameters.Add("Authorization", "Bearer " + Configuration.AccessToken);
			}
			return await Client.PostAsync<Player>("/names/{playerId}", apiRequestOptions, Configuration, cancellationToken);
		}
	}
}
