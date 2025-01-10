namespace UnityEngine.Rendering.HighDefinition.Compositor
{
	[HideInInspector]
	internal class CustomClear : CustomPass
	{
		internal class ShaderIDs
		{
			public static readonly int k_BlitScaleBiasRt = Shader.PropertyToID("_BlitScaleBiasRt");

			public static readonly int k_BlitScaleBias = Shader.PropertyToID("_BlitScaleBias");

			public static readonly int k_BlitTexture = Shader.PropertyToID("_BlitTexture");

			public static readonly int k_ClearAlpha = Shader.PropertyToID("_ClearAlpha");
		}

		private enum PassType
		{
			ClearColorAndStencil = 0,
			DrawTextureAndClearStencil = 1
		}

		private Material m_FullscreenPassMaterial;

		protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
		{
			if (HDRenderPipeline.isReady)
			{
				if (string.IsNullOrEmpty(base.name))
				{
					base.name = "CustomClear";
				}
				m_FullscreenPassMaterial = CoreUtils.CreateEngineMaterial(HDRenderPipelineGlobalSettings.instance.renderPipelineResources.shaders.customClearPS);
			}
		}

		protected override void Execute(CustomPassContext ctx)
		{
			AdditionalCompositorData component = null;
			ctx.hdCamera.camera.gameObject.TryGetComponent<AdditionalCompositorData>(out component);
			if (!(component == null) && !(component.clearColorTexture == null))
			{
				float num = (float)ctx.hdCamera.actualWidth / (float)ctx.hdCamera.actualHeight;
				float num2 = (float)component.clearColorTexture.width / (float)component.clearColorTexture.height;
				Vector4 value = new Vector4(1f, 1f, 0f, 0f);
				if (component.imageFitMode == BackgroundFitMode.FitHorizontally)
				{
					value.y = num / num2;
					value.w = (1f - value.y) / 2f;
				}
				else if (component.imageFitMode == BackgroundFitMode.FitVertically)
				{
					value.x = num2 / num;
					value.z = (1f - value.x) / 2f;
				}
				if (value.x < 1f || value.y < 1f)
				{
					m_FullscreenPassMaterial.SetVector(ShaderIDs.k_BlitScaleBiasRt, new Vector4(1f, 1f, 0f, 0f));
					m_FullscreenPassMaterial.SetVector(ShaderIDs.k_BlitScaleBias, new Vector4(1f, 1f, 0f, 0f));
					ctx.cmd.DrawProcedural(Matrix4x4.identity, m_FullscreenPassMaterial, 0, MeshTopology.Quads, 4, 1);
				}
				m_FullscreenPassMaterial.SetTexture(ShaderIDs.k_BlitTexture, component.clearColorTexture);
				m_FullscreenPassMaterial.SetVector(ShaderIDs.k_BlitScaleBiasRt, value);
				m_FullscreenPassMaterial.SetVector(ShaderIDs.k_BlitScaleBias, new Vector4(1f, 1f, 0f, 0f));
				m_FullscreenPassMaterial.SetInt(ShaderIDs.k_ClearAlpha, component.clearAlpha ? 1 : 0);
				ctx.cmd.DrawProcedural(Matrix4x4.identity, m_FullscreenPassMaterial, 1, MeshTopology.Quads, 4, 1);
			}
		}

		protected override void Cleanup()
		{
			CoreUtils.Destroy(m_FullscreenPassMaterial);
		}
	}
}
