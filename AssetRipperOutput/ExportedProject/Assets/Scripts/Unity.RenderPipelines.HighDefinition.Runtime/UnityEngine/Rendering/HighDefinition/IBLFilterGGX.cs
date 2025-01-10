using System;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class IBLFilterGGX : IBLFilterBSDF
	{
		private static readonly int k_PlanarReflectionFilterTex0ID = Shader.PropertyToID("PlanarReflectionFilterTex0");

		private static readonly int k_PlanarReflectionFilterTex1ID = Shader.PropertyToID("PlanarReflectionFilterTex1");

		private static readonly int k_PlanarReflectionFilterDepthTex0ID = Shader.PropertyToID("PlanarReflectionFilterDepthTex0");

		private static readonly int k_PlanarReflectionFilterDepthTex1ID = Shader.PropertyToID("PlanarReflectionFilterDepthTex1");

		private RenderTexture m_GgxIblSampleData;

		private int m_GgxIblMaxSampleCount = (TextureCache.isMobileBuildTarget ? 34 : 89);

		private const int k_GgxIblMipCountMinusOne = 6;

		private ComputeShader m_ComputeGgxIblSampleDataCS;

		private int m_ComputeGgxIblSampleDataKernel = -1;

		private ComputeShader m_BuildProbabilityTablesCS;

		private int m_ConditionalDensitiesKernel = -1;

		private int m_MarginalRowDensitiesKernel = -1;

		private ComputeShader m_PlanarReflectionFilteringCS;

		private int m_PlanarReflectionDepthConversionKernel = -1;

		private int m_PlanarReflectionDownScaleKernel = -1;

		private int m_PlanarReflectionFilteringKernel = -1;

		private const int k_DefaultPlanarResolution = 512;

		private Vector4 currentScreenSize = new Vector4(1f, 1f, 1f, 1f);

		private MaterialPropertyBlock m_MaterialPropertyBlock = new MaterialPropertyBlock();

		public IBLFilterGGX(HDRenderPipelineRuntimeResources renderPipelineResources, MipGenerator mipGenerator)
		{
			m_RenderPipelineResources = renderPipelineResources;
			m_MipGenerator = mipGenerator;
		}

		public override bool IsInitialized()
		{
			return m_GgxIblSampleData != null;
		}

		public override void Initialize(CommandBuffer cmd)
		{
			if (!m_ComputeGgxIblSampleDataCS)
			{
				m_ComputeGgxIblSampleDataCS = m_RenderPipelineResources.shaders.computeGgxIblSampleDataCS;
				m_ComputeGgxIblSampleDataKernel = m_ComputeGgxIblSampleDataCS.FindKernel("ComputeGgxIblSampleData");
			}
			if (!m_BuildProbabilityTablesCS)
			{
				m_BuildProbabilityTablesCS = m_RenderPipelineResources.shaders.buildProbabilityTablesCS;
				m_ConditionalDensitiesKernel = m_BuildProbabilityTablesCS.FindKernel("ComputeConditionalDensities");
				m_MarginalRowDensitiesKernel = m_BuildProbabilityTablesCS.FindKernel("ComputeMarginalRowDensities");
			}
			if (!m_convolveMaterial)
			{
				m_convolveMaterial = CoreUtils.CreateEngineMaterial(m_RenderPipelineResources.shaders.GGXConvolvePS);
			}
			if (!m_GgxIblSampleData)
			{
				m_GgxIblSampleData = new RenderTexture(m_GgxIblMaxSampleCount, 6, 0, GraphicsFormat.R16G16B16A16_SFloat);
				m_GgxIblSampleData.useMipMap = false;
				m_GgxIblSampleData.autoGenerateMips = false;
				m_GgxIblSampleData.enableRandomWrite = true;
				m_GgxIblSampleData.filterMode = FilterMode.Point;
				m_GgxIblSampleData.name = CoreUtils.GetRenderTargetAutoName(m_GgxIblMaxSampleCount, 6, 1, GraphicsFormat.R16G16B16A16_SFloat, "GGXIblSampleData");
				m_GgxIblSampleData.hideFlags = HideFlags.HideAndDontSave;
				m_GgxIblSampleData.Create();
				InitializeGgxIblSampleData(cmd);
			}
			if (!m_PlanarReflectionFilteringCS)
			{
				m_PlanarReflectionFilteringCS = m_RenderPipelineResources.shaders.planarReflectionFilteringCS;
				m_PlanarReflectionDepthConversionKernel = m_PlanarReflectionFilteringCS.FindKernel("DepthConversion");
				m_PlanarReflectionDownScaleKernel = m_PlanarReflectionFilteringCS.FindKernel("DownScale");
				m_PlanarReflectionFilteringKernel = m_PlanarReflectionFilteringCS.FindKernel("FilterPlanarReflection");
			}
			for (int i = 0; i < 6; i++)
			{
				Matrix4x4 matrix4x = Matrix4x4.LookAt(Vector3.zero, CoreUtils.lookAtList[i], CoreUtils.upVectorList[i]);
				m_faceWorldToViewMatrixMatrices[i] = matrix4x * Matrix4x4.Scale(new Vector3(1f, 1f, -1f));
			}
		}

		private void InitializeGgxIblSampleData(CommandBuffer cmd)
		{
			m_ComputeGgxIblSampleDataCS.SetTexture(m_ComputeGgxIblSampleDataKernel, "outputResult", m_GgxIblSampleData);
			cmd.DispatchCompute(m_ComputeGgxIblSampleDataCS, m_ComputeGgxIblSampleDataKernel, 1, 1, 1);
		}

		private static RenderTextureDescriptor MakeRenderTextureDescriptor(int texWidth, int texHeight, GraphicsFormat format, bool useMipMap)
		{
			RenderTextureDescriptor result = default(RenderTextureDescriptor);
			result.dimension = TextureDimension.Tex2D;
			result.width = texWidth;
			result.height = texHeight;
			result.volumeDepth = TextureXR.slices;
			result.graphicsFormat = format;
			result.enableRandomWrite = true;
			result.useDynamicScale = false;
			result.useMipMap = useMipMap;
			result.msaaSamples = 1;
			return result;
		}

		private static void CreateIntermediateTextures(CommandBuffer cmd, int texWidth, int texHeight)
		{
			GraphicsFormat reflectionProbeFormat = (GraphicsFormat)((HDRenderPipeline)RenderPipelineManager.currentPipeline).currentPlatformRenderPipelineSettings.lightLoopSettings.reflectionProbeFormat;
			cmd.GetTemporaryRT(k_PlanarReflectionFilterTex0ID, MakeRenderTextureDescriptor(texWidth, texHeight, reflectionProbeFormat, useMipMap: true));
			cmd.GetTemporaryRT(k_PlanarReflectionFilterTex1ID, MakeRenderTextureDescriptor(texWidth, texHeight, reflectionProbeFormat, useMipMap: false));
			cmd.GetTemporaryRT(k_PlanarReflectionFilterDepthTex0ID, MakeRenderTextureDescriptor(texWidth, texHeight, GraphicsFormat.R32_SFloat, useMipMap: true));
			cmd.GetTemporaryRT(k_PlanarReflectionFilterDepthTex1ID, MakeRenderTextureDescriptor(texWidth, texHeight, GraphicsFormat.R32_SFloat, useMipMap: false));
		}

		private static void ReleaseItrermediateTextures(CommandBuffer cmd)
		{
			cmd.ReleaseTemporaryRT(k_PlanarReflectionFilterTex0ID);
			cmd.ReleaseTemporaryRT(k_PlanarReflectionFilterTex1ID);
			cmd.ReleaseTemporaryRT(k_PlanarReflectionFilterDepthTex0ID);
			cmd.ReleaseTemporaryRT(k_PlanarReflectionFilterDepthTex1ID);
		}

		public override void Cleanup()
		{
			CoreUtils.Destroy(m_convolveMaterial);
			CoreUtils.Destroy(m_GgxIblSampleData);
		}

		private void FilterCubemapCommon(CommandBuffer cmd, Texture source, RenderTexture target, Matrix4x4[] worldToViewMatrices)
		{
			using (new ProfilingScope(cmd, ProfilingSampler.Get(HDProfileId.FilterCubemapGGX)))
			{
				if (1 + (int)Mathf.Log(source.width, 2f) < 7)
				{
					Debug.LogWarning("RenderCubemapGGXConvolution: Cubemap size is too small for GGX convolution, needs at least " + 7 + " mip levels");
					return;
				}
				for (int i = 0; i < 6; i++)
				{
					cmd.CopyTexture(source, i, 0, target, i, 0);
				}
				float value = 6f * (float)source.width * (float)source.width / (MathF.PI * 4f);
				if (!m_GgxIblSampleData.IsCreated())
				{
					m_GgxIblSampleData.Create();
					InitializeGgxIblSampleData(cmd);
				}
				m_convolveMaterial.SetTexture("_GgxIblSamples", m_GgxIblSampleData);
				m_MaterialPropertyBlock.SetTexture("_MainTex", source);
				m_MaterialPropertyBlock.SetFloat("_InvOmegaP", value);
				for (int j = 1; j < 7; j++)
				{
					m_MaterialPropertyBlock.SetFloat("_Level", j);
					for (int k = 0; k < 6; k++)
					{
						Matrix4x4 value2 = HDUtils.ComputePixelCoordToWorldSpaceViewDirectionMatrix(screenSize: new Vector4(source.width >> j, source.height >> j, 1f / (float)(source.width >> j), 1f / (float)(source.height >> j)), verticalFoV: MathF.PI / 2f, lensShift: Vector2.zero, worldToViewMatrix: worldToViewMatrices[k], renderToCubemap: true);
						m_MaterialPropertyBlock.SetMatrix(HDShaderIDs._PixelCoordToViewDirWS, value2);
						CoreUtils.SetRenderTarget(cmd, target, ClearFlag.None, j, (CubemapFace)k);
						CoreUtils.DrawFullScreen(cmd, m_convolveMaterial, m_MaterialPropertyBlock);
					}
				}
			}
		}

		public override void FilterCubemapMIS(CommandBuffer cmd, Texture source, RenderTexture target, RenderTexture conditionalCdf, RenderTexture marginalRowCdf)
		{
			m_BuildProbabilityTablesCS.SetTexture(m_ConditionalDensitiesKernel, "envMap", source);
			m_BuildProbabilityTablesCS.SetTexture(m_ConditionalDensitiesKernel, "conditionalDensities", conditionalCdf);
			m_BuildProbabilityTablesCS.SetTexture(m_ConditionalDensitiesKernel, "marginalRowDensities", marginalRowCdf);
			m_BuildProbabilityTablesCS.SetTexture(m_MarginalRowDensitiesKernel, "marginalRowDensities", marginalRowCdf);
			int height = conditionalCdf.height;
			cmd.DispatchCompute(m_BuildProbabilityTablesCS, m_ConditionalDensitiesKernel, height, 1, 1);
			cmd.DispatchCompute(m_BuildProbabilityTablesCS, m_MarginalRowDensitiesKernel, 1, 1, 1);
			m_convolveMaterial.EnableKeyword("USE_MIS");
			m_convolveMaterial.SetTexture("_ConditionalDensities", conditionalCdf);
			m_convolveMaterial.SetTexture("_MarginalRowDensities", marginalRowCdf);
			FilterCubemapCommon(cmd, source, target, m_faceWorldToViewMatrixMatrices);
		}

		public override void FilterCubemap(CommandBuffer cmd, Texture source, RenderTexture target)
		{
			FilterCubemapCommon(cmd, source, target, m_faceWorldToViewMatrixMatrices);
		}

		private void BuildColorAndDepthMipChain(CommandBuffer cmd, RenderTexture sourceColor, RenderTexture sourceDepth, ref PlanarTextureFilteringParameters planarTextureFilteringParameters)
		{
			int num = sourceColor.width;
			int num2 = sourceColor.height;
			cmd.CopyTexture(sourceColor, 0, 0, 0, 0, sourceColor.width, sourceColor.height, k_PlanarReflectionFilterTex0ID, 0, 0, 0, 0);
			cmd.SetComputeVectorParam(m_PlanarReflectionFilteringCS, HDShaderIDs._CaptureCameraPositon, planarTextureFilteringParameters.captureCameraPosition);
			cmd.SetComputeMatrixParam(m_PlanarReflectionFilteringCS, HDShaderIDs._CaptureCameraVP_NO, planarTextureFilteringParameters.captureCameraVP_NonOblique);
			cmd.SetComputeMatrixParam(m_PlanarReflectionFilteringCS, HDShaderIDs._CaptureCameraIVP, planarTextureFilteringParameters.captureCameraIVP);
			currentScreenSize.Set(num, num2, 1f / (float)num, 1f / (float)num2);
			cmd.SetComputeVectorParam(m_PlanarReflectionFilteringCS, HDShaderIDs._CaptureCurrentScreenSize, currentScreenSize);
			cmd.SetComputeFloatParam(m_PlanarReflectionFilteringCS, HDShaderIDs._CaptureCameraFarPlane, planarTextureFilteringParameters.captureFarPlane);
			cmd.SetComputeTextureParam(m_PlanarReflectionFilteringCS, m_PlanarReflectionDepthConversionKernel, HDShaderIDs._DepthTextureOblique, sourceDepth);
			cmd.SetComputeTextureParam(m_PlanarReflectionFilteringCS, m_PlanarReflectionDepthConversionKernel, HDShaderIDs._DepthTextureNonOblique, k_PlanarReflectionFilterDepthTex0ID);
			int num3 = 8;
			int threadGroupsX = (num + (num3 - 1)) / num3;
			int threadGroupsY = (num2 + (num3 - 1)) / num3;
			cmd.DispatchCompute(m_PlanarReflectionFilteringCS, m_PlanarReflectionDepthConversionKernel, threadGroupsX, threadGroupsY, 1);
			int num4 = 0;
			int num5 = sourceColor.width >> 1;
			int num6 = sourceColor.height >> 1;
			while (num5 >= 2 && num6 >= 2)
			{
				cmd.SetComputeIntParam(m_PlanarReflectionFilteringCS, HDShaderIDs._SourceMipIndex, num4);
				cmd.SetComputeTextureParam(m_PlanarReflectionFilteringCS, m_PlanarReflectionDownScaleKernel, HDShaderIDs._ReflectionColorMipChain, k_PlanarReflectionFilterTex0ID);
				cmd.SetComputeTextureParam(m_PlanarReflectionFilteringCS, m_PlanarReflectionDownScaleKernel, HDShaderIDs._HalfResReflectionBuffer, k_PlanarReflectionFilterTex1ID);
				cmd.SetComputeTextureParam(m_PlanarReflectionFilteringCS, m_PlanarReflectionDownScaleKernel, HDShaderIDs._DepthTextureMipChain, k_PlanarReflectionFilterDepthTex0ID);
				cmd.SetComputeTextureParam(m_PlanarReflectionFilteringCS, m_PlanarReflectionDownScaleKernel, HDShaderIDs._HalfResDepthBuffer, k_PlanarReflectionFilterDepthTex1ID);
				currentScreenSize.Set(num, num2, 1f / (float)num, 1f / (float)num2);
				cmd.SetComputeVectorParam(m_PlanarReflectionFilteringCS, HDShaderIDs._CaptureCurrentScreenSize, currentScreenSize);
				int threadGroupsX2 = (num5 + (num3 - 1)) / num3;
				int threadGroupsY2 = (num6 + (num3 - 1)) / num3;
				cmd.DispatchCompute(m_PlanarReflectionFilteringCS, m_PlanarReflectionDownScaleKernel, threadGroupsX2, threadGroupsY2, 1);
				cmd.CopyTexture(k_PlanarReflectionFilterTex1ID, 0, 0, 0, 0, num5, num6, k_PlanarReflectionFilterTex0ID, 0, num4 + 1, 0, 0);
				cmd.CopyTexture(k_PlanarReflectionFilterDepthTex1ID, 0, 0, 0, 0, num5, num6, k_PlanarReflectionFilterDepthTex0ID, 0, num4 + 1, 0, 0);
				num >>= 1;
				num2 >>= 1;
				num5 >>= 1;
				num6 >>= 1;
				num4++;
			}
		}

		public override void FilterPlanarTexture(CommandBuffer cmd, RenderTexture source, ref PlanarTextureFilteringParameters planarTextureFilteringParameters, RenderTexture target)
		{
			int width = source.width;
			int height = source.height;
			cmd.CopyTexture(source, 0, 0, 0, 0, width, height, target, 0, 0, 0, 0);
			if (!planarTextureFilteringParameters.smoothPlanarReflection)
			{
				CreateIntermediateTextures(cmd, width, height);
				BuildColorAndDepthMipChain(cmd, source, planarTextureFilteringParameters.captureCameraDepthBuffer, ref planarTextureFilteringParameters);
				int i = 1;
				int num = 8;
				int val = (int)(Mathf.Log(width, 2f) - 1f);
				width >>= 1;
				height >>= 1;
				for (; i < 7; i++)
				{
					int threadGroupsX = (width + (num - 1)) / num;
					int threadGroupsY = (height + (num - 1)) / num;
					cmd.SetComputeTextureParam(m_PlanarReflectionFilteringCS, m_PlanarReflectionFilteringKernel, HDShaderIDs._DepthTextureMipChain, k_PlanarReflectionFilterDepthTex0ID);
					cmd.SetComputeTextureParam(m_PlanarReflectionFilteringCS, m_PlanarReflectionFilteringKernel, HDShaderIDs._ReflectionColorMipChain, k_PlanarReflectionFilterTex0ID);
					cmd.SetComputeVectorParam(m_PlanarReflectionFilteringCS, HDShaderIDs._CaptureBaseScreenSize, planarTextureFilteringParameters.captureCameraScreenSize);
					currentScreenSize.Set(width, height, 1f / (float)width, 1f / (float)height);
					cmd.SetComputeVectorParam(m_PlanarReflectionFilteringCS, HDShaderIDs._CaptureCurrentScreenSize, currentScreenSize);
					cmd.SetComputeIntParam(m_PlanarReflectionFilteringCS, HDShaderIDs._SourceMipIndex, i);
					cmd.SetComputeIntParam(m_PlanarReflectionFilteringCS, HDShaderIDs._MaxMipLevels, val);
					cmd.SetComputeFloatParam(m_PlanarReflectionFilteringCS, HDShaderIDs._RTScaleFactor, 1f);
					cmd.SetComputeVectorParam(m_PlanarReflectionFilteringCS, HDShaderIDs._ReflectionPlaneNormal, planarTextureFilteringParameters.probeNormal);
					cmd.SetComputeVectorParam(m_PlanarReflectionFilteringCS, HDShaderIDs._ReflectionPlanePosition, planarTextureFilteringParameters.probePosition);
					cmd.SetComputeVectorParam(m_PlanarReflectionFilteringCS, HDShaderIDs._CaptureCameraPositon, planarTextureFilteringParameters.captureCameraPosition);
					cmd.SetComputeMatrixParam(m_PlanarReflectionFilteringCS, HDShaderIDs._CaptureCameraIVP_NO, planarTextureFilteringParameters.captureCameraIVP_NonOblique);
					cmd.SetComputeFloatParam(m_PlanarReflectionFilteringCS, HDShaderIDs._CaptureCameraFOV, planarTextureFilteringParameters.captureFOV * MathF.PI / 180f);
					cmd.SetComputeTextureParam(m_PlanarReflectionFilteringCS, m_PlanarReflectionFilteringKernel, HDShaderIDs._FilteredPlanarReflectionBuffer, k_PlanarReflectionFilterTex1ID);
					cmd.DispatchCompute(m_PlanarReflectionFilteringCS, m_PlanarReflectionFilteringKernel, threadGroupsX, threadGroupsY, 1);
					cmd.CopyTexture(k_PlanarReflectionFilterTex1ID, 0, 0, 0, 0, width, height, target, 0, i, 0, 0);
					width >>= 1;
					height >>= 1;
				}
				ReleaseItrermediateTextures(cmd);
			}
		}
	}
}
