using Unity.Services.Authentication.Internal;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Qos.Internal;
using Unity.Services.Relay.Apis.RelayAllocations;
using Unity.Services.Relay.Http;

namespace Unity.Services.Relay
{
	internal class InternalRelayService : IRelayServiceSdk
	{
		private const string k_CloudEnvironmentKey = "com.unity.services.core.cloud-environment";

		private const string k_StagingEnvironment = "staging";

		public IRelayAllocationsApiClient AllocationsApi { get; set; }

		public Configuration Configuration { get; set; }

		public IAccessToken AccessToken { get; set; }

		public IQosResults QosResults { get; set; }

		public InternalRelayService(HttpClient httpClient, IProjectConfiguration projectConfiguration = null, IAccessToken accessToken = null, IQosResults qosResults = null)
		{
			AllocationsApi = new RelayAllocationsApiClient(httpClient, accessToken);
			Configuration = new Configuration(GetHost(projectConfiguration), 10, 4, null);
			AccessToken = accessToken;
			QosResults = qosResults;
		}

		private string GetHost(IProjectConfiguration projectConfiguration)
		{
			if (projectConfiguration?.GetString("com.unity.services.core.cloud-environment") == "staging")
			{
				return "https://relay-allocations-stg.services.api.unity.com";
			}
			return "https://relay-allocations.services.api.unity.com";
		}
	}
}
