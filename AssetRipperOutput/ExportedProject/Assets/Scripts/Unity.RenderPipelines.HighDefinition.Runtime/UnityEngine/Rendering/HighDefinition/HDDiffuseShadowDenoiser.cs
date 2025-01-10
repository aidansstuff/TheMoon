using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class HDDiffuseShadowDenoiser
	{
		private class DiffuseShadowDenoiserDirectionalPassData
		{
			public int texWidth;

			public int texHeight;

			public int viewCount;

			public float lightAngle;

			public float cameraFov;

			public int kernelSize;

			public int bilateralHKernel;

			public int bilateralVKernel;

			public ComputeShader diffuseShadowDenoiserCS;

			public TextureHandle depthStencilBuffer;

			public TextureHandle normalBuffer;

			public TextureHandle distanceBuffer;

			public TextureHandle noisyBuffer;

			public TextureHandle intermediateBuffer;

			public TextureHandle outputBuffer;
		}

		private class DiffuseShadowDenoiserSpherePassData
		{
			public int texWidth;

			public int texHeight;

			public int viewCount;

			public float cameraFov;

			public PunctualShadowProperties properties;

			public int bilateralHKernel;

			public int bilateralVKernel;

			public ComputeShader diffuseShadowDenoiserCS;

			public TextureHandle depthStencilBuffer;

			public TextureHandle normalBuffer;

			public TextureHandle distanceBuffer;

			public TextureHandle noisyBuffer;

			public TextureHandle intermediateBuffer;

			public TextureHandle outputBuffer;
		}

		private ComputeShader m_ShadowDenoiser;

		private int m_BilateralFilterHSingleDirectionalKernel;

		private int m_BilateralFilterVSingleDirectionalKernel;

		private int m_BilateralFilterHColorDirectionalKernel;

		private int m_BilateralFilterVColorDirectionalKernel;

		private int m_BilateralFilterHSinglePointKernel;

		private int m_BilateralFilterVSinglePointKernel;

		private int m_BilateralFilterHSingleSpotKernel;

		private int m_BilateralFilterVSingleSpotKernel;

		public void Init(HDRenderPipelineRayTracingResources rpRTResources)
		{
			m_ShadowDenoiser = rpRTResources.diffuseShadowDenoiserCS;
			m_BilateralFilterHSingleDirectionalKernel = m_ShadowDenoiser.FindKernel("BilateralFilterHSingleDirectional");
			m_BilateralFilterVSingleDirectionalKernel = m_ShadowDenoiser.FindKernel("BilateralFilterVSingleDirectional");
			m_BilateralFilterHColorDirectionalKernel = m_ShadowDenoiser.FindKernel("BilateralFilterHColorDirectional");
			m_BilateralFilterVColorDirectionalKernel = m_ShadowDenoiser.FindKernel("BilateralFilterVColorDirectional");
			m_BilateralFilterHSinglePointKernel = m_ShadowDenoiser.FindKernel("BilateralFilterHSinglePoint");
			m_BilateralFilterVSinglePointKernel = m_ShadowDenoiser.FindKernel("BilateralFilterVSinglePoint");
			m_BilateralFilterHSingleSpotKernel = m_ShadowDenoiser.FindKernel("BilateralFilterHSingleSpot");
			m_BilateralFilterVSingleSpotKernel = m_ShadowDenoiser.FindKernel("BilateralFilterVSingleSpot");
		}

		public void Release()
		{
		}

		public TextureHandle DenoiseBufferDirectional(RenderGraph renderGraph, HDCamera hdCamera, TextureHandle depthBuffer, TextureHandle normalBuffer, TextureHandle noisyBuffer, TextureHandle distanceBuffer, int kernelSize, float angularDiameter, bool singleChannel = true)
		{
			DiffuseShadowDenoiserDirectionalPassData passData;
			RenderGraphBuilder renderGraphBuilder = renderGraph.AddRenderPass<DiffuseShadowDenoiserDirectionalPassData>("TemporalDenoiser", out passData, ProfilingSampler.Get(HDProfileId.DiffuseFilter));
			try
			{
				renderGraphBuilder.EnableAsyncCompute(value: false);
				passData.texWidth = hdCamera.actualWidth;
				passData.texHeight = hdCamera.actualHeight;
				passData.viewCount = hdCamera.viewCount;
				passData.cameraFov = hdCamera.camera.fieldOfView * MathF.PI / 180f;
				passData.lightAngle = angularDiameter * MathF.PI / 180f;
				passData.kernelSize = kernelSize;
				passData.bilateralHKernel = (singleChannel ? m_BilateralFilterHSingleDirectionalKernel : m_BilateralFilterHColorDirectionalKernel);
				passData.bilateralVKernel = (singleChannel ? m_BilateralFilterVSingleDirectionalKernel : m_BilateralFilterVColorDirectionalKernel);
				passData.diffuseShadowDenoiserCS = m_ShadowDenoiser;
				passData.depthStencilBuffer = renderGraphBuilder.UseDepthBuffer(in depthBuffer, DepthAccess.Read);
				passData.normalBuffer = renderGraphBuilder.ReadTexture(in normalBuffer);
				passData.distanceBuffer = renderGraphBuilder.ReadTexture(in distanceBuffer);
				passData.noisyBuffer = renderGraphBuilder.ReadTexture(in noisyBuffer);
				DiffuseShadowDenoiserDirectionalPassData diffuseShadowDenoiserDirectionalPassData = passData;
				TextureDesc desc = new TextureDesc(Vector2.one, dynamicResolution: true, xrReady: true)
				{
					colorFormat = GraphicsFormat.R16G16B16A16_SFloat,
					enableRandomWrite = true,
					name = "Intermediate buffer"
				};
				diffuseShadowDenoiserDirectionalPassData.intermediateBuffer = renderGraphBuilder.CreateTransientTexture(in desc);
				DiffuseShadowDenoiserDirectionalPassData diffuseShadowDenoiserDirectionalPassData2 = passData;
				desc = new TextureDesc(Vector2.one, dynamicResolution: true, xrReady: true)
				{
					colorFormat = GraphicsFormat.R16G16B16A16_SFloat,
					enableRandomWrite = true,
					name = "Denoised Buffer"
				};
				TextureHandle input = renderGraph.CreateTexture(in desc);
				diffuseShadowDenoiserDirectionalPassData2.outputBuffer = renderGraphBuilder.ReadWriteTexture(in input);
				renderGraphBuilder.SetRenderFunc(delegate(DiffuseShadowDenoiserDirectionalPassData data, RenderGraphContext ctx)
				{
					CoreUtils.SetKeyword(ctx.cmd, "DISTANCE_BASED_DENOISER", state: true);
					int num = 8;
					int threadGroupsX = (data.texWidth + (num - 1)) / num;
					int threadGroupsY = (data.texHeight + (num - 1)) / num;
					ctx.cmd.SetComputeFloatParam(data.diffuseShadowDenoiserCS, HDShaderIDs._RaytracingLightAngle, data.lightAngle);
					ctx.cmd.SetComputeIntParam(data.diffuseShadowDenoiserCS, HDShaderIDs._DenoiserFilterRadius, data.kernelSize);
					ctx.cmd.SetComputeFloatParam(data.diffuseShadowDenoiserCS, HDShaderIDs._CameraFOV, data.cameraFov);
					ctx.cmd.SetComputeTextureParam(data.diffuseShadowDenoiserCS, data.bilateralHKernel, HDShaderIDs._DepthTexture, data.depthStencilBuffer);
					ctx.cmd.SetComputeTextureParam(data.diffuseShadowDenoiserCS, data.bilateralHKernel, HDShaderIDs._NormalBufferTexture, data.normalBuffer);
					ctx.cmd.SetComputeTextureParam(data.diffuseShadowDenoiserCS, data.bilateralHKernel, HDShaderIDs._DenoiseInputTexture, data.noisyBuffer);
					ctx.cmd.SetComputeTextureParam(data.diffuseShadowDenoiserCS, data.bilateralHKernel, HDShaderIDs._DistanceTexture, data.distanceBuffer);
					ctx.cmd.SetComputeTextureParam(data.diffuseShadowDenoiserCS, data.bilateralHKernel, HDShaderIDs._DenoiseOutputTextureRW, data.intermediateBuffer);
					ctx.cmd.DispatchCompute(data.diffuseShadowDenoiserCS, data.bilateralHKernel, threadGroupsX, threadGroupsY, data.viewCount);
					ctx.cmd.SetComputeTextureParam(data.diffuseShadowDenoiserCS, data.bilateralVKernel, HDShaderIDs._DepthTexture, data.depthStencilBuffer);
					ctx.cmd.SetComputeTextureParam(data.diffuseShadowDenoiserCS, data.bilateralVKernel, HDShaderIDs._NormalBufferTexture, data.normalBuffer);
					ctx.cmd.SetComputeTextureParam(data.diffuseShadowDenoiserCS, data.bilateralVKernel, HDShaderIDs._DenoiseInputTexture, data.intermediateBuffer);
					ctx.cmd.SetComputeTextureParam(data.diffuseShadowDenoiserCS, data.bilateralVKernel, HDShaderIDs._DistanceTexture, data.distanceBuffer);
					ctx.cmd.SetComputeTextureParam(data.diffuseShadowDenoiserCS, data.bilateralVKernel, HDShaderIDs._DenoiseOutputTextureRW, data.outputBuffer);
					ctx.cmd.DispatchCompute(data.diffuseShadowDenoiserCS, data.bilateralVKernel, threadGroupsX, threadGroupsY, data.viewCount);
					CoreUtils.SetKeyword(ctx.cmd, "DISTANCE_BASED_DENOISER", state: false);
				});
				return passData.outputBuffer;
			}
			finally
			{
				((IDisposable)renderGraphBuilder).Dispose();
			}
		}

		public TextureHandle DenoiseBufferSphere(RenderGraph renderGraph, HDCamera hdCamera, TextureHandle depthBuffer, TextureHandle normalBuffer, TextureHandle noisyBuffer, TextureHandle distanceBuffer, PunctualShadowProperties properties)
		{
			DiffuseShadowDenoiserSpherePassData passData;
			RenderGraphBuilder renderGraphBuilder = renderGraph.AddRenderPass<DiffuseShadowDenoiserSpherePassData>("DiffuseDenoiser", out passData, ProfilingSampler.Get(HDProfileId.DiffuseFilter));
			try
			{
				renderGraphBuilder.EnableAsyncCompute(value: false);
				passData.texWidth = hdCamera.actualWidth;
				passData.texHeight = hdCamera.actualHeight;
				passData.viewCount = hdCamera.viewCount;
				passData.cameraFov = hdCamera.camera.fieldOfView * MathF.PI / 180f;
				passData.properties = properties;
				if (ShaderConfig.s_CameraRelativeRendering != 0)
				{
					passData.properties.lightPosition -= hdCamera.camera.transform.position;
				}
				passData.bilateralHKernel = (properties.isSpot ? m_BilateralFilterHSingleSpotKernel : m_BilateralFilterHSinglePointKernel);
				passData.bilateralVKernel = (properties.isSpot ? m_BilateralFilterVSingleSpotKernel : m_BilateralFilterVSinglePointKernel);
				passData.diffuseShadowDenoiserCS = m_ShadowDenoiser;
				passData.depthStencilBuffer = renderGraphBuilder.UseDepthBuffer(in depthBuffer, DepthAccess.Read);
				passData.normalBuffer = renderGraphBuilder.ReadTexture(in normalBuffer);
				if (properties.distanceBasedDenoiser)
				{
					passData.distanceBuffer = renderGraphBuilder.ReadTexture(in distanceBuffer);
				}
				passData.noisyBuffer = renderGraphBuilder.ReadTexture(in noisyBuffer);
				DiffuseShadowDenoiserSpherePassData diffuseShadowDenoiserSpherePassData = passData;
				TextureDesc desc = new TextureDesc(Vector2.one, dynamicResolution: true, xrReady: true)
				{
					colorFormat = GraphicsFormat.R16G16B16A16_SFloat,
					enableRandomWrite = true,
					name = "Intermediate buffer"
				};
				diffuseShadowDenoiserSpherePassData.intermediateBuffer = renderGraphBuilder.CreateTransientTexture(in desc);
				DiffuseShadowDenoiserSpherePassData diffuseShadowDenoiserSpherePassData2 = passData;
				desc = new TextureDesc(Vector2.one, dynamicResolution: true, xrReady: true)
				{
					colorFormat = GraphicsFormat.R16G16B16A16_SFloat,
					enableRandomWrite = true,
					name = "Denoised Buffer"
				};
				TextureHandle input = renderGraph.CreateTexture(in desc);
				diffuseShadowDenoiserSpherePassData2.outputBuffer = renderGraphBuilder.ReadWriteTexture(in input);
				renderGraphBuilder.SetRenderFunc(delegate(DiffuseShadowDenoiserSpherePassData data, RenderGraphContext ctx)
				{
					int num = 8;
					int threadGroupsX = (data.texWidth + (num - 1)) / num;
					int threadGroupsY = (data.texHeight + (num - 1)) / num;
					CoreUtils.SetKeyword(ctx.cmd, "DISTANCE_BASED_DENOISER", data.properties.distanceBasedDenoiser);
					ctx.cmd.SetComputeIntParam(data.diffuseShadowDenoiserCS, HDShaderIDs._RaytracingTargetLight, data.properties.lightIndex);
					ctx.cmd.SetComputeFloatParam(data.diffuseShadowDenoiserCS, HDShaderIDs._RaytracingLightAngle, data.properties.lightConeAngle);
					ctx.cmd.SetComputeFloatParam(data.diffuseShadowDenoiserCS, HDShaderIDs._RaytracingLightRadius, data.properties.lightRadius);
					ctx.cmd.SetComputeIntParam(data.diffuseShadowDenoiserCS, HDShaderIDs._DenoiserFilterRadius, data.properties.kernelSize);
					ctx.cmd.SetComputeVectorParam(data.diffuseShadowDenoiserCS, HDShaderIDs._SphereLightPosition, data.properties.lightPosition);
					ctx.cmd.SetComputeFloatParam(data.diffuseShadowDenoiserCS, HDShaderIDs._CameraFOV, data.cameraFov);
					ctx.cmd.SetComputeTextureParam(data.diffuseShadowDenoiserCS, data.bilateralHKernel, HDShaderIDs._DepthTexture, data.depthStencilBuffer);
					ctx.cmd.SetComputeTextureParam(data.diffuseShadowDenoiserCS, data.bilateralHKernel, HDShaderIDs._NormalBufferTexture, data.normalBuffer);
					ctx.cmd.SetComputeTextureParam(data.diffuseShadowDenoiserCS, data.bilateralHKernel, HDShaderIDs._DenoiseInputTexture, data.noisyBuffer);
					if (data.properties.distanceBasedDenoiser)
					{
						ctx.cmd.SetComputeTextureParam(data.diffuseShadowDenoiserCS, data.bilateralHKernel, HDShaderIDs._DistanceTexture, data.distanceBuffer);
					}
					ctx.cmd.SetComputeTextureParam(data.diffuseShadowDenoiserCS, data.bilateralHKernel, HDShaderIDs._DenoiseOutputTextureRW, data.intermediateBuffer);
					ctx.cmd.DispatchCompute(data.diffuseShadowDenoiserCS, data.bilateralHKernel, threadGroupsX, threadGroupsY, data.viewCount);
					ctx.cmd.SetComputeTextureParam(data.diffuseShadowDenoiserCS, data.bilateralVKernel, HDShaderIDs._DepthTexture, data.depthStencilBuffer);
					ctx.cmd.SetComputeTextureParam(data.diffuseShadowDenoiserCS, data.bilateralVKernel, HDShaderIDs._NormalBufferTexture, data.normalBuffer);
					ctx.cmd.SetComputeTextureParam(data.diffuseShadowDenoiserCS, data.bilateralVKernel, HDShaderIDs._DenoiseInputTexture, data.intermediateBuffer);
					if (data.properties.distanceBasedDenoiser)
					{
						ctx.cmd.SetComputeTextureParam(data.diffuseShadowDenoiserCS, data.bilateralVKernel, HDShaderIDs._DistanceTexture, data.distanceBuffer);
					}
					ctx.cmd.SetComputeTextureParam(data.diffuseShadowDenoiserCS, data.bilateralVKernel, HDShaderIDs._DenoiseOutputTextureRW, data.outputBuffer);
					ctx.cmd.DispatchCompute(data.diffuseShadowDenoiserCS, data.bilateralVKernel, threadGroupsX, threadGroupsY, data.viewCount);
					CoreUtils.SetKeyword(ctx.cmd, "DISTANCE_BASED_DENOISER", state: false);
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
