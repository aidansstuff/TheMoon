namespace UnityEngine.Rendering.HighDefinition
{
	public abstract class VolumeComponentWithQuality : VolumeComponent
	{
		[Tooltip("Specifies the quality level to be used for performance relevant parameters.")]
		public ScalableSettingLevelParameter quality = new ScalableSettingLevelParameter(1, useOverride: false);

		internal static GlobalPostProcessingQualitySettings GetPostProcessingQualitySettings()
		{
			return ((HDRenderPipeline)RenderPipelineManager.currentPipeline)?.currentPlatformRenderPipelineSettings.postProcessQualitySettings;
		}

		internal static GlobalLightingQualitySettings GetLightingQualitySettings()
		{
			return ((HDRenderPipeline)RenderPipelineManager.currentPipeline)?.currentPlatformRenderPipelineSettings.lightingQualitySettings;
		}

		protected bool UsesQualitySettings()
		{
			if (!quality.levelAndOverride.useOverride)
			{
				return (HDRenderPipeline)RenderPipelineManager.currentPipeline != null;
			}
			return false;
		}
	}
}
