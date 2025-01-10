using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class HDDiffuseDenoiser
	{
		private class DiffuseDenoiserPassData
		{
			public int texWidth;

			public int texHeight;

			public int viewCount;

			public bool needInit;

			public float pixelSpreadTangent;

			public float kernelSize;

			public bool halfResolutionFilter;

			public bool jitterFilter;

			public int frameIndex;

			public bool fullResolutionInput;

			public int bilateralFilterKernel;

			public int gatherKernel;

			public ComputeBufferHandle pointDistribution;

			public ComputeShader diffuseDenoiserCS;

			public Texture2D owenScrambledTexture;

			public TextureHandle depthStencilBuffer;

			public TextureHandle normalBuffer;

			public TextureHandle noisyBuffer;

			public TextureHandle intermediateBuffer;

			public TextureHandle outputBuffer;
		}

		internal struct DiffuseDenoiserParameters
		{
			public bool singleChannel;

			public float kernelSize;

			public bool halfResolutionFilter;

			public bool jitterFilter;

			public bool fullResolutionInput;
		}

		private ComputeShader m_DiffuseDenoiser;

		private bool m_DenoiserInitialized;

		private Texture2D m_OwnenScrambledTexture;

		private ComputeBuffer m_PointDistribution;

		private int m_BilateralFilterSingleKernel;

		private int m_BilateralFilterColorKernel;

		private int m_GatherSingleKernel;

		private int m_GatherColorKernel;

		public void Init(HDRenderPipelineRuntimeResources rpResources, HDRenderPipeline renderPipeline)
		{
			m_DiffuseDenoiser = rpResources.shaders.diffuseDenoiserCS;
			m_BilateralFilterSingleKernel = m_DiffuseDenoiser.FindKernel("BilateralFilterSingle");
			m_BilateralFilterColorKernel = m_DiffuseDenoiser.FindKernel("BilateralFilterColor");
			m_GatherSingleKernel = m_DiffuseDenoiser.FindKernel("GatherSingle");
			m_GatherColorKernel = m_DiffuseDenoiser.FindKernel("GatherColor");
			m_DenoiserInitialized = false;
			m_OwnenScrambledTexture = rpResources.textures.owenScrambledRGBATex;
			m_PointDistribution = new ComputeBuffer(64, 8);
		}

		public void Release()
		{
			CoreUtils.SafeRelease(m_PointDistribution);
		}

		public TextureHandle Denoise(RenderGraph renderGraph, HDCamera hdCamera, DiffuseDenoiserParameters denoiserParams, TextureHandle noisyBuffer, TextureHandle depthBuffer, TextureHandle normalBuffer, TextureHandle outputBuffer)
		{
			DiffuseDenoiserPassData passData;
			RenderGraphBuilder renderGraphBuilder = renderGraph.AddRenderPass<DiffuseDenoiserPassData>("DiffuseDenoiser", out passData, ProfilingSampler.Get(HDProfileId.DiffuseFilter));
			try
			{
				renderGraphBuilder.EnableAsyncCompute(value: false);
				passData.needInit = !m_DenoiserInitialized;
				m_DenoiserInitialized = true;
				passData.owenScrambledTexture = m_OwnenScrambledTexture;
				if (denoiserParams.fullResolutionInput)
				{
					passData.texWidth = hdCamera.actualWidth;
					passData.texHeight = hdCamera.actualHeight;
				}
				else
				{
					passData.texWidth = hdCamera.actualWidth / 2;
					passData.texHeight = hdCamera.actualHeight / 2;
				}
				passData.viewCount = hdCamera.viewCount;
				passData.pixelSpreadTangent = HDRenderPipeline.GetPixelSpreadTangent(hdCamera.camera.fieldOfView, passData.texWidth, passData.texHeight);
				passData.kernelSize = denoiserParams.kernelSize;
				passData.halfResolutionFilter = denoiserParams.halfResolutionFilter;
				passData.jitterFilter = denoiserParams.jitterFilter;
				passData.frameIndex = HDRenderPipeline.RayTracingFrameIndex(hdCamera);
				passData.fullResolutionInput = denoiserParams.fullResolutionInput;
				passData.bilateralFilterKernel = (denoiserParams.singleChannel ? m_BilateralFilterSingleKernel : m_BilateralFilterColorKernel);
				passData.gatherKernel = (denoiserParams.singleChannel ? m_GatherSingleKernel : m_GatherColorKernel);
				passData.diffuseDenoiserCS = m_DiffuseDenoiser;
				DiffuseDenoiserPassData diffuseDenoiserPassData = passData;
				ComputeBufferHandle input = renderGraph.ImportComputeBuffer(m_PointDistribution);
				diffuseDenoiserPassData.pointDistribution = renderGraphBuilder.ReadComputeBuffer(in input);
				passData.depthStencilBuffer = renderGraphBuilder.ReadTexture(in depthBuffer);
				passData.normalBuffer = renderGraphBuilder.ReadTexture(in normalBuffer);
				passData.noisyBuffer = renderGraphBuilder.ReadTexture(in noisyBuffer);
				DiffuseDenoiserPassData diffuseDenoiserPassData2 = passData;
				TextureDesc desc = new TextureDesc(Vector2.one, dynamicResolution: true, xrReady: true)
				{
					colorFormat = GraphicsFormat.B10G11R11_UFloatPack32,
					enableRandomWrite = true,
					name = "DiffuseDenoiserIntermediate"
				};
				diffuseDenoiserPassData2.intermediateBuffer = renderGraphBuilder.CreateTransientTexture(in desc);
				passData.outputBuffer = renderGraphBuilder.WriteTexture(in outputBuffer);
				renderGraphBuilder.SetRenderFunc(delegate(DiffuseDenoiserPassData data, RenderGraphContext ctx)
				{
					if (passData.needInit)
					{
						int kernelIndex = data.diffuseDenoiserCS.FindKernel("GeneratePointDistribution");
						ctx.cmd.SetComputeTextureParam(data.diffuseDenoiserCS, kernelIndex, HDShaderIDs._OwenScrambledRGTexture, data.owenScrambledTexture);
						ctx.cmd.SetComputeBufferParam(data.diffuseDenoiserCS, kernelIndex, "_PointDistributionRW", data.pointDistribution);
						ctx.cmd.DispatchCompute(data.diffuseDenoiserCS, kernelIndex, 1, 1, 1);
					}
					int num = 8;
					int threadGroupsX = (data.texWidth + (num - 1)) / num;
					int threadGroupsY = (data.texHeight + (num - 1)) / num;
					ctx.cmd.SetComputeFloatParam(data.diffuseDenoiserCS, HDShaderIDs._DenoiserFilterRadius, data.kernelSize);
					ctx.cmd.SetComputeBufferParam(data.diffuseDenoiserCS, data.bilateralFilterKernel, HDShaderIDs._PointDistribution, data.pointDistribution);
					ctx.cmd.SetComputeTextureParam(data.diffuseDenoiserCS, data.bilateralFilterKernel, HDShaderIDs._DenoiseInputTexture, data.noisyBuffer);
					ctx.cmd.SetComputeTextureParam(data.diffuseDenoiserCS, data.bilateralFilterKernel, HDShaderIDs._DepthTexture, data.depthStencilBuffer);
					ctx.cmd.SetComputeTextureParam(data.diffuseDenoiserCS, data.bilateralFilterKernel, HDShaderIDs._StencilTexture, data.depthStencilBuffer, 0, RenderTextureSubElement.Stencil);
					ctx.cmd.SetComputeTextureParam(data.diffuseDenoiserCS, data.bilateralFilterKernel, HDShaderIDs._NormalBufferTexture, data.normalBuffer);
					ctx.cmd.SetComputeTextureParam(data.diffuseDenoiserCS, data.bilateralFilterKernel, HDShaderIDs._DenoiseOutputTextureRW, data.halfResolutionFilter ? data.intermediateBuffer : data.outputBuffer);
					ctx.cmd.SetComputeIntParam(data.diffuseDenoiserCS, HDShaderIDs._HalfResolutionFilter, data.halfResolutionFilter ? 1 : 0);
					ctx.cmd.SetComputeFloatParam(data.diffuseDenoiserCS, HDShaderIDs._PixelSpreadAngleTangent, data.pixelSpreadTangent);
					if (data.jitterFilter)
					{
						ctx.cmd.SetComputeIntParam(data.diffuseDenoiserCS, HDShaderIDs._JitterFramePeriod, data.frameIndex % 4);
					}
					else
					{
						ctx.cmd.SetComputeIntParam(data.diffuseDenoiserCS, HDShaderIDs._JitterFramePeriod, -1);
					}
					CoreUtils.SetKeyword(ctx.cmd, "FULL_RESOLUTION_INPUT", data.fullResolutionInput);
					ctx.cmd.DispatchCompute(data.diffuseDenoiserCS, data.bilateralFilterKernel, threadGroupsX, threadGroupsY, data.viewCount);
					if (data.halfResolutionFilter)
					{
						ctx.cmd.SetComputeTextureParam(data.diffuseDenoiserCS, data.gatherKernel, HDShaderIDs._DenoiseInputTexture, data.intermediateBuffer);
						ctx.cmd.SetComputeTextureParam(data.diffuseDenoiserCS, data.gatherKernel, HDShaderIDs._DepthTexture, data.depthStencilBuffer);
						ctx.cmd.SetComputeTextureParam(data.diffuseDenoiserCS, data.gatherKernel, HDShaderIDs._DenoiseOutputTextureRW, data.outputBuffer);
						ctx.cmd.DispatchCompute(data.diffuseDenoiserCS, data.gatherKernel, threadGroupsX, threadGroupsY, data.viewCount);
					}
					CoreUtils.SetKeyword(ctx.cmd, "FULL_RESOLUTION_INPUT", state: false);
				});
				return passData.outputBuffer;
			}
			finally
			{
				((IDisposable)renderGraphBuilder).Dispose();
			}
		}
	}
}
