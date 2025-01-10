using System;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class IBLFilterCharlie : IBLFilterBSDF
	{
		public IBLFilterCharlie(HDRenderPipelineRuntimeResources renderPipelineResources, MipGenerator mipGenerator)
		{
			m_RenderPipelineResources = renderPipelineResources;
			m_MipGenerator = mipGenerator;
		}

		public override bool IsInitialized()
		{
			return m_convolveMaterial != null;
		}

		public override void Initialize(CommandBuffer cmd)
		{
			if (!m_convolveMaterial)
			{
				m_convolveMaterial = CoreUtils.CreateEngineMaterial(m_RenderPipelineResources.shaders.charlieConvolvePS);
			}
			for (int i = 0; i < 6; i++)
			{
				Matrix4x4 matrix4x = Matrix4x4.LookAt(Vector3.zero, CoreUtils.lookAtList[i], CoreUtils.upVectorList[i]);
				m_faceWorldToViewMatrixMatrices[i] = matrix4x * Matrix4x4.Scale(new Vector3(1f, 1f, -1f));
			}
		}

		public override void Cleanup()
		{
			CoreUtils.Destroy(m_convolveMaterial);
			m_convolveMaterial = null;
		}

		private void FilterCubemapCommon(CommandBuffer cmd, Texture source, RenderTexture target, Matrix4x4[] worldToViewMatrices)
		{
			using (new ProfilingScope(cmd, ProfilingSampler.Get(HDProfileId.FilterCubemapCharlie)))
			{
				if (1 + (int)Mathf.Log(source.width, 2f) < 7)
				{
					Debug.LogWarning("RenderCubemapCharlieConvolution: Cubemap size is too small for Charlie convolution, needs at least " + 7 + " mip levels");
					return;
				}
				float value = 6f * (float)source.width * (float)source.width / (MathF.PI * 4f);
				MaterialPropertyBlock materialPropertyBlock = new MaterialPropertyBlock();
				materialPropertyBlock.SetTexture("_MainTex", source);
				materialPropertyBlock.SetFloat("_InvOmegaP", value);
				float value2 = 1f / (8.377581f / (3.1734369f * (float)source.width * (float)source.width));
				materialPropertyBlock.SetFloat("_InvFaceCenterTexelSolidAngle", value2);
				for (int i = 0; i < 7; i++)
				{
					materialPropertyBlock.SetFloat("_Level", i);
					for (int j = 0; j < 6; j++)
					{
						Matrix4x4 value3 = HDUtils.ComputePixelCoordToWorldSpaceViewDirectionMatrix(screenSize: new Vector4(source.width >> i, source.height >> i, 1f / (float)(source.width >> i), 1f / (float)(source.height >> i)), verticalFoV: MathF.PI / 2f, lensShift: Vector2.zero, worldToViewMatrix: worldToViewMatrices[j], renderToCubemap: true);
						materialPropertyBlock.SetMatrix(HDShaderIDs._PixelCoordToViewDirWS, value3);
						CoreUtils.SetRenderTarget(cmd, target, ClearFlag.None, i, (CubemapFace)j);
						CoreUtils.DrawFullScreen(cmd, m_convolveMaterial, materialPropertyBlock);
					}
				}
			}
		}

		public override void FilterCubemap(CommandBuffer cmd, Texture source, RenderTexture target)
		{
			FilterCubemapCommon(cmd, source, target, m_faceWorldToViewMatrixMatrices);
		}

		public override void FilterCubemapMIS(CommandBuffer cmd, Texture source, RenderTexture target, RenderTexture conditionalCdf, RenderTexture marginalRowCdf)
		{
		}

		public override void FilterPlanarTexture(CommandBuffer cmd, RenderTexture source, ref PlanarTextureFilteringParameters planarTextureFilteringParameters, RenderTexture target)
		{
			m_MipGenerator.RenderColorGaussianPyramid(cmd, new Vector2Int(source.width, source.height), source, target);
		}
	}
}
