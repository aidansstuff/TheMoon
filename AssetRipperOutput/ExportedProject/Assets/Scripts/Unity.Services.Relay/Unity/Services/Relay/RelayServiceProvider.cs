using System.Threading.Tasks;
using Unity.Services.Authentication.Internal;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Internal;
using Unity.Services.Qos.Internal;
using Unity.Services.Relay.Http;
using UnityEngine;

namespace Unity.Services.Relay
{
	internal class RelayServiceProvider : IInitializablePackage
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Register()
		{
			CoreRegistration coreRegistration = CoreRegistry.Instance.RegisterPackage(new RelayServiceProvider());
			coreRegistration.DependsOn<IAccessToken>();
			coreRegistration.DependsOn<IProjectConfiguration>();
			coreRegistration.DependsOn<IQosResults>();
		}

		public Task Initialize(CoreRegistry registry)
		{
			HttpClient httpClient = new HttpClient();
			IAccessToken serviceComponent = registry.GetServiceComponent<IAccessToken>();
			IProjectConfiguration serviceComponent2 = registry.GetServiceComponent<IProjectConfiguration>();
			IQosResults serviceComponent3 = registry.GetServiceComponent<IQosResults>();
			if (serviceComponent != null)
			{
				RelayServiceSdk.Instance = new InternalRelayService(httpClient, serviceComponent2, serviceComponent, serviceComponent3);
			}
			return Task.CompletedTask;
		}
	}
}
