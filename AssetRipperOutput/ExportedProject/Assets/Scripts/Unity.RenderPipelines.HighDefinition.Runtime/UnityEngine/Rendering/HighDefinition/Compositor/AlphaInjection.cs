using System;

namespace UnityEngine.Rendering.HighDefinition.Compositor
{
	[Serializable]
	[HideInInspector]
	internal sealed class AlphaInjection : CustomPostProcessVolumeComponent, IPostProcessComponent
	{
		internal class ShaderIDs
		{
			public static readonly int k_AlphaTexture = Shader.PropertyToID("_AlphaTexture");

			public static readonly int k_InputTexture = Shader.PropertyToID("_InputTexture");
		}

		private Material m_Material;

		public override CustomPostProcessInjectionPoint injectionPoint => CustomPostProcessInjectionPoint.BeforePostProcess;

		public bool IsActive()
		{
			return m_Material != null;
		}

		public override void Setup()
		{
			if (HDRenderPipeline.isReady)
			{
				m_Material = CoreUtils.CreateEngineMaterial(HDRenderPipelineGlobalSettings.instance.renderPipelineResources.shaders.alphaInjectionPS);
			}
		}

		public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
		{
			AdditionalCompositorData component = null;
			camera.camera.gameObject.TryGetComponent<AdditionalCompositorData>(out component);
			if (component == null || component.layerFilters == null)
			{
				HDUtils.BlitCameraTexture(cmd, source, destination);
				return;
			}
			int num = component.layerFilters.FindIndex((CompositionFilter x) => x.filterType == CompositionFilter.FilterType.ALPHA_MASK);
			if (num < 0)
			{
				HDUtils.BlitCameraTexture(cmd, source, destination);
				return;
			}
			CompositionFilter compositionFilter = component.layerFilters[num];
			m_Material.SetTexture(ShaderIDs.k_InputTexture, source);
			m_Material.SetTexture(ShaderIDs.k_AlphaTexture, compositionFilter.alphaMask);
			HDUtils.DrawFullScreen(cmd, m_Material, destination);
		}

		public override void Cleanup()
		{
			CoreUtils.Destroy(m_Material);
		}
	}
}
