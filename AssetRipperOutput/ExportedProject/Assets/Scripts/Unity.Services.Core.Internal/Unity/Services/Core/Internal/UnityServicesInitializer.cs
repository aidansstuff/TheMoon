using UnityEngine;

namespace Unity.Services.Core.Internal
{
	internal static class UnityServicesInitializer
	{
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
		private static void CreateStaticInstance()
		{
			CoreRegistry.Instance = new CoreRegistry();
			CoreMetrics.Instance = new CoreMetrics();
			CoreDiagnostics.Instance = new CoreDiagnostics();
			UnityServices.Instance = new UnityServicesInternal(CoreRegistry.Instance, CoreMetrics.Instance, CoreDiagnostics.Instance);
			UnityServices.InstantiationCompletion?.TrySetResult(null);
		}

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
		private static async void EnableServicesInitializationAsync()
		{
			await ((UnityServicesInternal)UnityServices.Instance).EnableInitializationAsync();
		}
	}
}
