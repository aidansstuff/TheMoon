namespace UnityEngine.Rendering.HighDefinition
{
	internal class GradientSkyRenderer : SkyRenderer
	{
		private Material m_GradientSkyMaterial;

		private MaterialPropertyBlock m_PropertyBlock = new MaterialPropertyBlock();

		private readonly int _GradientBottom = Shader.PropertyToID("_GradientBottom");

		private readonly int _GradientMiddle = Shader.PropertyToID("_GradientMiddle");

		private readonly int _GradientTop = Shader.PropertyToID("_GradientTop");

		private readonly int _GradientDiffusion = Shader.PropertyToID("_GradientDiffusion");

		public GradientSkyRenderer()
		{
			SupportDynamicSunLight = false;
		}

		public override void Build()
		{
			m_GradientSkyMaterial = CoreUtils.CreateEngineMaterial(HDRenderPipelineGlobalSettings.instance.renderPipelineResources.shaders.gradientSkyPS);
		}

		public override void Cleanup()
		{
			CoreUtils.Destroy(m_GradientSkyMaterial);
		}

		public override void RenderSky(BuiltinSkyParameters builtinParams, bool renderForCubemap, bool renderSunDisk)
		{
			GradientSky gradientSky = builtinParams.skySettings as GradientSky;
			m_GradientSkyMaterial.SetColor(_GradientBottom, gradientSky.bottom.value);
			m_GradientSkyMaterial.SetColor(_GradientMiddle, gradientSky.middle.value);
			m_GradientSkyMaterial.SetColor(_GradientTop, gradientSky.top.value);
			m_GradientSkyMaterial.SetFloat(_GradientDiffusion, gradientSky.gradientDiffusion.value);
			m_GradientSkyMaterial.SetFloat(HDShaderIDs._SkyIntensity, SkyRenderer.GetSkyIntensity(gradientSky, builtinParams.debugSettings));
			m_PropertyBlock.SetMatrix(HDShaderIDs._PixelCoordToViewDirWS, builtinParams.pixelCoordToViewDirMatrix);
			CoreUtils.DrawFullScreen(builtinParams.commandBuffer, m_GradientSkyMaterial, m_PropertyBlock, (!renderForCubemap) ? 1 : 0);
		}
	}
}
