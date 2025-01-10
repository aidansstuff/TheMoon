using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class HDRuntimeReflectionSystem : ScriptableRuntimeReflectionSystem
	{
		private static HDRuntimeReflectionSystem k_instance = new HDRuntimeReflectionSystem();

		[RuntimeInitializeOnLoadMethod]
		private static void Initialize()
		{
			if (GraphicsSettings.currentRenderPipeline is HDRenderPipelineAsset)
			{
				ScriptableRuntimeReflectionSystemSettings.system = k_instance;
			}
		}

		public override bool TickRealtimeProbes()
		{
			ReflectionProbe.UpdateCachedState();
			return base.TickRealtimeProbes();
		}
	}
}
