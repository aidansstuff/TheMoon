using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class HDReflectionDenoiser
	{
		private class ReflectionDenoiserPassData
		{
			public int texWidth;

			public int texHeight;

			public int viewCount;

			public int maxKernelSize;

			public float historyValidity;

			public Vector4 historySizeAndScale;

			public Vector2 historyBufferSize;

			public Vector4 currentEffectResolution;

			public float pixelSpreadTangent;

			public int affectSmoothSurfaces;

			public int singleReflectionBounce;

			public float roughnessBasedDenoising;

			public ComputeShader reflectionDenoiserCS;

			public int temporalAccumulationKernel;

			public int copyHistoryKernel;

			public int bilateralFilterHKernel;

			public int bilateralFilterVKernel;

			public Texture2D reflectionFilterMapping;

			public TextureHandle depthBuffer;

			public TextureHandle historyDepth;

			public TextureHandle normalBuffer;

			public TextureHandle motionVectorBuffer;

			public TextureHandle intermediateBuffer0;

			public TextureHandle intermediateBuffer1;

			public TextureHandle historySignal;

			public TextureHandle noisyToOutputSignal;
		}

		private ComputeShader m_ReflectionDenoiserCS;

		private Texture2D m_ReflectionFilterMapping;

		private int s_TemporalAccumulationFullResKernel;

		private int s_TemporalAccumulationHalfResKernel;

		private int s_CopyHistoryKernel;

		private int s_BilateralFilterH_FRKernel;

		private int s_BilateralFilterV_FRKernel;

		private int s_BilateralFilterH_HRKernel;

		private int s_BilateralFilterV_HRKernel;

		public void Init(HDRenderPipelineRayTracingResources rpRTResources)
		{
			m_ReflectionDenoiserCS = rpRTResources.reflectionDenoiserCS;
			m_ReflectionFilterMapping = rpRTResources.reflectionFilterMapping;
			s_TemporalAccumulationFullResKernel = m_ReflectionDenoiserCS.FindKernel("TemporalAccumulationFullRes");
			s_TemporalAccumulationHalfResKernel = m_ReflectionDenoiserCS.FindKernel("TemporalAccumulationHalfRes");
			s_CopyHistoryKernel = m_ReflectionDenoiserCS.FindKernel("CopyHistory");
			s_BilateralFilterH_FRKernel = m_ReflectionDenoiserCS.FindKernel("BilateralFilterH_FR");
			s_BilateralFilterV_FRKernel = m_ReflectionDenoiserCS.FindKernel("BilateralFilterV_FR");
			s_BilateralFilterH_HRKernel = m_ReflectionDenoiserCS.FindKernel("BilateralFilterH_HR");
			s_BilateralFilterV_HRKernel = m_ReflectionDenoiserCS.FindKernel("BilateralFilterV_HR");
		}

		public void Release()
		{
		}

		public TextureHandle DenoiseRTR(RenderGraph renderGraph, HDCamera hdCamera, float historyValidity, int maxKernelSize, bool fullResolution, bool singleReflectionBounce, bool affectSmoothSurfaces, TextureHandle depthPyramid, TextureHandle normalBuffer, TextureHandle motionVectorBuffer, TextureHandle clearCoatTexture, TextureHandle lightingTexture, RTHandle historyBuffer)
		{
			ReflectionDenoiserPassData passData;
			RenderGraphBuilder renderGraphBuilder = renderGraph.AddRenderPass<ReflectionDenoiserPassData>("Denoise ray traced reflections", out passData, ProfilingSampler.Get(HDProfileId.RaytracingReflectionFilter));
			try
			{
				renderGraphBuilder.EnableAsyncCompute(value: false);
				passData.texWidth = (fullResolution ? hdCamera.actualWidth : (hdCamera.actualWidth / 2));
				passData.texHeight = (fullResolution ? hdCamera.actualHeight : (hdCamera.actualHeight / 2));
				passData.viewCount = hdCamera.viewCount;
				passData.historyValidity = historyValidity;
				passData.historySizeAndScale = HDRenderPipeline.EvaluateRayTracingHistorySizeAndScale(hdCamera, historyBuffer);
				passData.maxKernelSize = (fullResolution ? maxKernelSize : (maxKernelSize / 2));
				passData.historyBufferSize = new Vector2(1f / (float)hdCamera.historyRTHandleProperties.currentRenderTargetSize.x, 1f / (float)hdCamera.historyRTHandleProperties.currentRenderTargetSize.y);
				passData.currentEffectResolution = new Vector4(passData.texWidth, passData.texHeight, 1f / (float)passData.texWidth, 1f / (float)passData.texHeight);
				passData.pixelSpreadTangent = HDRenderPipeline.GetPixelSpreadTangent(hdCamera.camera.fieldOfView, passData.texWidth, passData.texHeight);
				passData.affectSmoothSurfaces = (affectSmoothSurfaces ? 1 : 0);
				passData.singleReflectionBounce = (singleReflectionBounce ? 1 : 0);
				passData.roughnessBasedDenoising = (singleReflectionBounce ? 1 : 0);
				passData.reflectionDenoiserCS = m_ReflectionDenoiserCS;
				passData.temporalAccumulationKernel = (fullResolution ? s_TemporalAccumulationFullResKernel : s_TemporalAccumulationHalfResKernel);
				passData.copyHistoryKernel = s_CopyHistoryKernel;
				passData.bilateralFilterHKernel = (fullResolution ? s_BilateralFilterH_FRKernel : s_BilateralFilterH_HRKernel);
				passData.bilateralFilterVKernel = (fullResolution ? s_BilateralFilterV_FRKernel : s_BilateralFilterV_HRKernel);
				passData.reflectionFilterMapping = m_ReflectionFilterMapping;
				passData.depthBuffer = renderGraphBuilder.ReadTexture(in depthPyramid);
				passData.normalBuffer = renderGraphBuilder.ReadTexture(in normalBuffer);
				passData.motionVectorBuffer = renderGraphBuilder.ReadTexture(in motionVectorBuffer);
				RTHandle currentFrameRT = hdCamera.GetCurrentFrameRT(6);
				passData.historyDepth = ((currentFrameRT != null) ? renderGraph.ImportTexture(hdCamera.GetCurrentFrameRT(6)) : renderGraph.defaultResources.blackTextureXR);
				ReflectionDenoiserPassData reflectionDenoiserPassData = passData;
				TextureDesc desc = new TextureDesc(Vector2.one, dynamicResolution: true, xrReady: true)
				{
					colorFormat = GraphicsFormat.R16G16B16A16_SFloat,
					enableRandomWrite = true,
					name = "IntermediateTexture0"
				};
				reflectionDenoiserPassData.intermediateBuffer0 = renderGraphBuilder.CreateTransientTexture(in desc);
				ReflectionDenoiserPassData reflectionDenoiserPassData2 = passData;
				desc = new TextureDesc(Vector2.one, dynamicResolution: true, xrReady: true)
				{
					colorFormat = GraphicsFormat.R16G16B16A16_SFloat,
					enableRandomWrite = true,
					name = "IntermediateTexture1"
				};
				reflectionDenoiserPassData2.intermediateBuffer1 = renderGraphBuilder.CreateTransientTexture(in desc);
				ReflectionDenoiserPassData reflectionDenoiserPassData3 = passData;
				TextureHandle input = renderGraph.ImportTexture(historyBuffer);
				reflectionDenoiserPassData3.historySignal = renderGraphBuilder.ReadWriteTexture(in input);
				passData.noisyToOutputSignal = renderGraphBuilder.ReadWriteTexture(in lightingTexture);
				renderGraphBuilder.SetRenderFunc(delegate(ReflectionDenoiserPassData data, RenderGraphContext ctx)
				{
					int num = 8;
					int threadGroupsX = (data.texWidth + (num - 1)) / num;
					int threadGroupsY = (data.texHeight + (num - 1)) / num;
					ctx.cmd.SetComputeFloatParam(data.reflectionDenoiserCS, HDShaderIDs._HistoryValidity, data.historyValidity);
					ctx.cmd.SetComputeFloatParam(data.reflectionDenoiserCS, HDShaderIDs._PixelSpreadAngleTangent, data.pixelSpreadTangent);
					ctx.cmd.SetComputeVectorParam(data.reflectionDenoiserCS, HDShaderIDs._HistoryBufferSize, data.historyBufferSize);
					ctx.cmd.SetComputeVectorParam(data.reflectionDenoiserCS, HDShaderIDs._CurrentEffectResolution, data.currentEffectResolution);
					ctx.cmd.SetComputeVectorParam(data.reflectionDenoiserCS, HDShaderIDs._HistorySizeAndScale, data.historySizeAndScale);
					ctx.cmd.SetComputeIntParam(data.reflectionDenoiserCS, HDShaderIDs._AffectSmoothSurfaces, data.affectSmoothSurfaces);
					ctx.cmd.SetComputeIntParam(data.reflectionDenoiserCS, HDShaderIDs._SingleReflectionBounce, data.singleReflectionBounce);
					ctx.cmd.SetComputeTextureParam(data.reflectionDenoiserCS, data.temporalAccumulationKernel, HDShaderIDs._DenoiseInputTexture, data.noisyToOutputSignal);
					ctx.cmd.SetComputeTextureParam(data.reflectionDenoiserCS, data.temporalAccumulationKernel, HDShaderIDs._DepthTexture, data.depthBuffer);
					ctx.cmd.SetComputeTextureParam(data.reflectionDenoiserCS, data.temporalAccumulationKernel, HDShaderIDs._HistoryDepthTexture, data.historyDepth);
					ctx.cmd.SetComputeTextureParam(data.reflectionDenoiserCS, data.temporalAccumulationKernel, HDShaderIDs._NormalBufferTexture, data.normalBuffer);
					ctx.cmd.SetComputeTextureParam(data.reflectionDenoiserCS, data.temporalAccumulationKernel, HDShaderIDs._CameraMotionVectorsTexture, data.motionVectorBuffer);
					ctx.cmd.SetComputeTextureParam(data.reflectionDenoiserCS, data.temporalAccumulationKernel, HDShaderIDs._HistoryBuffer, data.historySignal);
					ctx.cmd.SetComputeTextureParam(data.reflectionDenoiserCS, data.temporalAccumulationKernel, HDShaderIDs._DenoiseOutputTextureRW, data.intermediateBuffer0);
					ctx.cmd.SetComputeTextureParam(data.reflectionDenoiserCS, data.temporalAccumulationKernel, HDShaderIDs._SampleCountTextureRW, data.intermediateBuffer1);
					ctx.cmd.DispatchCompute(data.reflectionDenoiserCS, data.temporalAccumulationKernel, threadGroupsX, threadGroupsY, data.viewCount);
					ctx.cmd.SetComputeTextureParam(data.reflectionDenoiserCS, data.copyHistoryKernel, HDShaderIDs._DenoiseInputTexture, data.intermediateBuffer0);
					ctx.cmd.SetComputeTextureParam(data.reflectionDenoiserCS, data.copyHistoryKernel, HDShaderIDs._DenoiseOutputTextureRW, data.historySignal);
					ctx.cmd.SetComputeTextureParam(data.reflectionDenoiserCS, data.copyHistoryKernel, HDShaderIDs._SampleCountTextureRW, data.intermediateBuffer1);
					ctx.cmd.DispatchCompute(data.reflectionDenoiserCS, data.copyHistoryKernel, threadGroupsX, threadGroupsY, data.viewCount);
					ctx.cmd.SetComputeIntParam(data.reflectionDenoiserCS, HDShaderIDs._DenoiserFilterRadius, data.maxKernelSize);
					ctx.cmd.SetComputeFloatParam(data.reflectionDenoiserCS, HDShaderIDs._RoughnessBasedDenoising, data.roughnessBasedDenoising);
					ctx.cmd.SetComputeTextureParam(data.reflectionDenoiserCS, data.bilateralFilterHKernel, HDShaderIDs._DenoiseInputTexture, data.intermediateBuffer0);
					ctx.cmd.SetComputeTextureParam(data.reflectionDenoiserCS, data.bilateralFilterHKernel, HDShaderIDs._DepthTexture, data.depthBuffer);
					ctx.cmd.SetComputeTextureParam(data.reflectionDenoiserCS, data.bilateralFilterHKernel, HDShaderIDs._NormalBufferTexture, data.normalBuffer);
					ctx.cmd.SetComputeTextureParam(data.reflectionDenoiserCS, data.bilateralFilterHKernel, HDShaderIDs._DenoiseOutputTextureRW, data.intermediateBuffer1);
					ctx.cmd.SetComputeTextureParam(data.reflectionDenoiserCS, data.bilateralFilterHKernel, HDShaderIDs._ReflectionFilterMapping, data.reflectionFilterMapping);
					ctx.cmd.DispatchCompute(data.reflectionDenoiserCS, data.bilateralFilterHKernel, threadGroupsX, threadGroupsY, data.viewCount);
					ctx.cmd.SetComputeIntParam(data.reflectionDenoiserCS, HDShaderIDs._DenoiserFilterRadius, data.maxKernelSize);
					ctx.cmd.SetComputeFloatParam(data.reflectionDenoiserCS, HDShaderIDs._RoughnessBasedDenoising, data.roughnessBasedDenoising);
					ctx.cmd.SetComputeTextureParam(data.reflectionDenoiserCS, data.bilateralFilterVKernel, HDShaderIDs._DenoiseInputTexture, data.intermediateBuffer1);
					ctx.cmd.SetComputeTextureParam(data.reflectionDenoiserCS, data.bilateralFilterVKernel, HDShaderIDs._DepthTexture, data.depthBuffer);
					ctx.cmd.SetComputeTextureParam(data.reflectionDenoiserCS, data.bilateralFilterVKernel, HDShaderIDs._NormalBufferTexture, data.normalBuffer);
					ctx.cmd.SetComputeTextureParam(data.reflectionDenoiserCS, data.bilateralFilterVKernel, HDShaderIDs._DenoiseOutputTextureRW, data.noisyToOutputSignal);
					ctx.cmd.SetComputeTextureParam(data.reflectionDenoiserCS, data.bilateralFilterVKernel, HDShaderIDs._ReflectionFilterMapping, data.reflectionFilterMapping);
					ctx.cmd.DispatchCompute(data.reflectionDenoiserCS, data.bilateralFilterVKernel, threadGroupsX, threadGroupsY, data.viewCount);
				});
				return passData.noisyToOutputSignal;
			}
			finally
			{
				((IDisposable)renderGraphBuilder).Dispose();
			}
		}
	}
}
