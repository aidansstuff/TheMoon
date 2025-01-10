using System.Threading.Tasks;
using Unity.Services.Authentication.Generated;
using Unity.Services.Authentication.Internal;
using Unity.Services.Authentication.Shared;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Environments.Internal;
using Unity.Services.Core.Internal;
using Unity.Services.Core.Scheduler.Internal;
using Unity.Services.Core.Telemetry.Internal;
using UnityEngine;

namespace Unity.Services.Authentication
{
	internal class AuthenticationPackageInitializer : IInitializablePackage
	{
		private const string k_CloudEnvironmentKey = "com.unity.services.core.cloud-environment";

		private const string k_StagingEnvironment = "staging";

		public Task Initialize(CoreRegistry registry)
		{
			AuthenticationSettings settings = new AuthenticationSettings();
			IActionScheduler serviceComponent = registry.GetServiceComponent<IActionScheduler>();
			IEnvironments serviceComponent2 = registry.GetServiceComponent<IEnvironments>();
			ICloudProjectId serviceComponent3 = registry.GetServiceComponent<ICloudProjectId>();
			IProjectConfiguration serviceComponent4 = registry.GetServiceComponent<IProjectConfiguration>();
			ProfileComponent profile = new ProfileComponent(serviceComponent4.GetString("com.unity.services.authentication.profile", "default"));
			AuthenticationMetrics metrics = new AuthenticationMetrics(registry.GetServiceComponent<IMetricsFactory>());
			JwtDecoder jwtDecoder = new JwtDecoder();
			AuthenticationCache cache = new AuthenticationCache(serviceComponent3, profile);
			AccessTokenComponent accessToken = new AccessTokenComponent();
			EnvironmentIdComponent environmentId = new EnvironmentIdComponent();
			PlayerIdComponent playerId = new PlayerIdComponent(cache);
			PlayerNameComponent playerName = new PlayerNameComponent(cache);
			SessionTokenComponent sessionToken = new SessionTokenComponent(cache);
			NetworkConfiguration configuration = new NetworkConfiguration();
			NetworkHandler networkHandler = new NetworkHandler(configuration);
			string playerAuthHost = GetPlayerAuthHost(serviceComponent4);
			PlayerNamesApi playerNamesApi = new PlayerNamesApi(new AuthenticationApiClient(configuration), new ApiConfiguration
			{
				BasePath = GetPlayerNamesHost(serviceComponent4)
			});
			AuthenticationNetworkClient networkClient = new AuthenticationNetworkClient(playerAuthHost, serviceComponent3, serviceComponent2, networkHandler, accessToken);
			AuthenticationServiceInternal authenticationServiceInternal = (AuthenticationServiceInternal)(AuthenticationService.Instance = new AuthenticationServiceInternal(settings, networkClient, playerNamesApi, profile, jwtDecoder, cache, serviceComponent, metrics, accessToken, environmentId, playerId, playerName, sessionToken));
			registry.RegisterServiceComponent((IAccessToken)authenticationServiceInternal.AccessTokenComponent);
			registry.RegisterServiceComponent((IEnvironmentId)authenticationServiceInternal.EnvironmentIdComponent);
			registry.RegisterServiceComponent((IPlayerId)authenticationServiceInternal.PlayerIdComponent);
			return Task.CompletedTask;
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Register()
		{
			CoreRegistry.Instance.RegisterPackage(new AuthenticationPackageInitializer()).DependsOn<IEnvironments>().DependsOn<IActionScheduler>()
				.DependsOn<ICloudProjectId>()
				.DependsOn<IProjectConfiguration>()
				.DependsOn<IMetricsFactory>()
				.ProvidesComponent<IPlayerId>()
				.ProvidesComponent<IAccessToken>()
				.ProvidesComponent<IEnvironmentId>();
		}

		private string GetPlayerAuthHost(IProjectConfiguration projectConfiguration)
		{
			if (projectConfiguration?.GetString("com.unity.services.core.cloud-environment") == "staging")
			{
				return "https://player-auth-stg.services.api.unity.com";
			}
			return "https://player-auth.services.api.unity.com";
		}

		private string GetPlayerNamesHost(IProjectConfiguration projectConfiguration)
		{
			if (projectConfiguration?.GetString("com.unity.services.core.cloud-environment") == "staging")
			{
				return "https://social-stg.services.api.unity.com/v1";
			}
			return "https://social.services.api.unity.com/v1";
		}
	}
}
