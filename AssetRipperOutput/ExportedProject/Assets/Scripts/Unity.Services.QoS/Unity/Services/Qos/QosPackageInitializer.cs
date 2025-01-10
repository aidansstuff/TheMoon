using System.Threading.Tasks;
using Unity.Services.Authentication.Internal;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Internal;
using Unity.Services.Core.Telemetry.Internal;
using Unity.Services.Qos.Http;
using Unity.Services.Qos.Internal;
using Unity.Services.Qos.Runner;
using UnityEngine;

namespace Unity.Services.Qos
{
	internal class QosPackageInitializer : IInitializablePackage
	{
		private const string PackageName = "com.unity.services.qos";

		private const string k_CloudEnvironmentKey = "com.unity.services.core.cloud-environment";

		private const string k_StagingEnvironment = "staging";

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Register()
		{
			new QosPackageInitializer().Register(CoreRegistry.Instance);
		}

		internal void Register(CoreRegistry registry)
		{
			registry.RegisterPackage(this).DependsOn<IAccessToken>().DependsOn<IMetricsFactory>()
				.DependsOn<IProjectConfiguration>()
				.ProvidesComponent<IQosResults>();
		}

		public Task Initialize(CoreRegistry registry)
		{
			IProjectConfiguration serviceComponent = registry.GetServiceComponent<IProjectConfiguration>();
			HttpClient httpClient = new HttpClient();
			IAccessToken serviceComponent2 = registry.GetServiceComponent<IAccessToken>();
			IMetrics metrics = registry.GetServiceComponent<IMetricsFactory>().Create("com.unity.services.qos");
			QosDiscoveryService.Instance = new InternalQosDiscoveryService(GetHost(serviceComponent), httpClient, serviceComponent2);
			WrappedQosService qosService2 = (WrappedQosService)(QosService.Instance = new WrappedQosService(QosDiscoveryService.Instance.QosDiscoveryApi, new BaselibQosRunner(), serviceComponent2, metrics));
			registry.RegisterServiceComponent((IQosResults)new QosResults(qosService2));
			return Task.CompletedTask;
		}

		private string GetHost(IProjectConfiguration projectConfiguration)
		{
			if (projectConfiguration?.GetString("com.unity.services.core.cloud-environment") == "staging")
			{
				return "https://qos-discovery-stg.services.api.unity.com";
			}
			return "https://qos-discovery.services.api.unity.com";
		}
	}
}
