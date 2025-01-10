using System;

namespace UnityEngine.Rendering.HighDefinition.Compositor
{
	[Serializable]
	[HideInInspector]
	internal sealed class ChromaKeying : CustomPostProcessVolumeComponent, IPostProcessComponent
	{
		internal class ShaderIDs
		{
			public static readonly int k_KeyColor = Shader.PropertyToID("_KeyColor");

			public static readonly int k_KeyParams = Shader.PropertyToID("_KeyParams");

			public static readonly int k_InputTexture = Shader.PropertyToID("_InputTexture");
		}

		public BoolParameter activate = new BoolParameter(value: false);

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
				m_Material = CoreUtils.CreateEngineMaterial(HDRenderPipelineGlobalSettings.instance.renderPipelineResources.shaders.chromaKeyingPS);
			}
		}

		public override void Render(CommandBuffer cmd, HDCamera camera, RTHandle source, RTHandle destination)
		{
			AdditionalCompositorData component = null;
			camera.camera.gameObject.TryGetComponent<AdditionalCompositorData>(out component);
			if (!activate.value || component == null || component.layerFilters == null)
			{
				HDUtils.BlitCameraTexture(cmd, source, destination);
				return;
			}
			int num = component.layerFilters.FindIndex((CompositionFilter x) => x.filterType == CompositionFilter.FilterType.CHROMA_KEYING);
			if (num < 0)
			{
				HDUtils.BlitCameraTexture(cmd, source, destination);
				return;
			}
			CompositionFilter compositionFilter = component.layerFilters[num];
			Vector4 value = default(Vector4);
			value.x = compositionFilter.keyThreshold;
			value.y = compositionFilter.keyTolerance;
			value.z = compositionFilter.spillRemoval;
			value.w = 1f;
			m_Material.SetVector(ShaderIDs.k_KeyColor, compositionFilter.maskColor);
			m_Material.SetVector(ShaderIDs.k_KeyParams, value);
			m_Material.SetTexture(ShaderIDs.k_InputTexture, source);
			HDUtils.DrawFullScreen(cmd, m_Material, destination);
		}

		public override void Cleanup()
		{
			CoreUtils.Destroy(m_Material);
		}
	}
}
