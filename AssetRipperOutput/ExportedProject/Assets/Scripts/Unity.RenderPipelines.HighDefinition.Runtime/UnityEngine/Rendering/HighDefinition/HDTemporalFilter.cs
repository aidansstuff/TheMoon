using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class HDTemporalFilter
	{
		[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\RenderPipeline\\Raytracing\\HDTemporalFilter.cs")]
		private enum HistoryRejectionFlags
		{
			Depth = 1,
			Reprojection = 2,
			PreviousDepth = 4,
			Position = 8,
			Normal = 16,
			Motion = 32,
			Combined = 63,
			CombinedNoMotion = 31
		}

		internal struct TemporalFilterParameters
		{
			public bool singleChannel;

			public float historyValidity;

			public bool occluderMotionRejection;

			public bool receiverMotionRejection;

			public bool exposureControl;

			public bool fullResolution;
		}

		private class HistoryValidityPassData
		{
			public int texWidth;

			public int texHeight;

			public int viewCount;

			public Vector4 historySizeAndScale;

			public float historyValidity;

			public float pixelSpreadTangent;

			public int validateHistoryKernel;

			public ComputeShader temporalFilterCS;

			public TextureHandle depthStencilBuffer;

			public TextureHandle normalBuffer;

			public TextureHandle motionVectorBuffer;

			public TextureHandle historyDepthTexture;

			public TextureHandle historyNormalTexture;

			public TextureHandle validationBuffer;
		}

		private class TemporalFilterPassData
		{
			public int texWidth;

			public int texHeight;

			public int viewCount;

			public float historyValidity;

			public float pixelSpreadTangent;

			public bool occluderMotionRejection;

			public bool receiverMotionRejection;

			public int exposureControl;

			public bool fullResolution;

			public int temporalAccKernel;

			public int copyHistoryKernel;

			public ComputeShader temporalFilterCS;

			public TextureHandle depthStencilBuffer;

			public TextureHandle normalBuffer;

			public TextureHandle motionVectorBuffer;

			public TextureHandle velocityBuffer;

			public TextureHandle noisyBuffer;

			public TextureHandle validationBuffer;

			public TextureHandle historyBuffer;

			public TextureHandle outputBuffer;
		}

		internal struct TemporalDenoiserArrayOutputData
		{
			public TextureHandle outputSignal;

			public TextureHandle outputSignalDistance;
		}

		private class TemporalFilterArrayPassData
		{
			public int texWidth;

			public int texHeight;

			public int viewCount;

			public bool distanceBasedDenoiser;

			public float historyValidity;

			public float pixelSpreadTangent;

			public int sliceIndex;

			public Vector4 channelMask;

			public Vector4 distanceChannelMask;

			public int temporalAccKernel;

			public int blendHistoryKernel;

			public int temporalAccSingleKernel;

			public int blendHistoryNoValidityKernel;

			public int outputHistoryKernel;

			public ComputeShader temporalFilterCS;

			public TextureHandle depthStencilBuffer;

			public TextureHandle normalBuffer;

			public TextureHandle motionVectorBuffer;

			public TextureHandle noisyBuffer;

			public TextureHandle distanceBuffer;

			public TextureHandle validationBuffer;

			public TextureHandle velocityBuffer;

			public TextureHandle inputHistoryBuffer;

			public TextureHandle outputHistoryBuffer;

			public TextureHandle validationHistoryBuffer;

			public TextureHandle distanceHistorySignal;

			public TextureHandle intermediateSignalOutput;

			public TextureHandle intermediateValidityOutput;

			public TextureHandle outputBuffer;

			public TextureHandle outputDistanceSignal;
		}

		private ComputeShader m_TemporalFilterCS;

		private int m_ValidateHistoryKernel;

		private int m_TemporalAccumulationSingleKernel;

		private int m_TemporalAccumulationColorKernel;

		private int m_CopyHistoryKernel;

		private int m_TemporalAccumulationSingleArrayKernel;

		private int m_TemporalAccumulationColorArrayKernel;

		private int m_BlendHistorySingleArrayKernel;

		private int m_BlendHistoryColorArrayKernel;

		private int m_BlendHistorySingleArrayNoValidityKernel;

		private int m_OutputHistoryArrayKernel;

		public void Init(HDRenderPipelineRuntimeResources rpResources)
		{
			m_TemporalFilterCS = rpResources.shaders.temporalFilterCS;
			m_ValidateHistoryKernel = m_TemporalFilterCS.FindKernel("ValidateHistory");
			m_TemporalAccumulationSingleKernel = m_TemporalFilterCS.FindKernel("TemporalAccumulationSingle");
			m_TemporalAccumulationColorKernel = m_TemporalFilterCS.FindKernel("TemporalAccumulationColor");
			m_CopyHistoryKernel = m_TemporalFilterCS.FindKernel("CopyHistory");
			m_TemporalAccumulationSingleArrayKernel = m_TemporalFilterCS.FindKernel("TemporalAccumulationSingleArray");
			m_TemporalAccumulationColorArrayKernel = m_TemporalFilterCS.FindKernel("TemporalAccumulationColorArray");
			m_BlendHistorySingleArrayKernel = m_TemporalFilterCS.FindKernel("BlendHistorySingleArray");
			m_BlendHistoryColorArrayKernel = m_TemporalFilterCS.FindKernel("BlendHistoryColorArray");
			m_BlendHistorySingleArrayNoValidityKernel = m_TemporalFilterCS.FindKernel("BlendHistorySingleArrayNoValidity");
			m_OutputHistoryArrayKernel = m_TemporalFilterCS.FindKernel("OutputHistoryArray");
		}

		public void Release()
		{
		}

		public TextureHandle HistoryValidity(RenderGraph renderGraph, HDCamera hdCamera, float historyValidity, TextureHandle depthBuffer, TextureHandle normalBuffer, TextureHandle motionVectorBuffer)
		{
			HistoryValidityPassData passData;
			RenderGraphBuilder renderGraphBuilder = renderGraph.AddRenderPass<HistoryValidityPassData>("History Validity Evaluation", out passData, ProfilingSampler.Get(HDProfileId.HistoryValidity));
			try
			{
				renderGraphBuilder.EnableAsyncCompute(value: false);
				passData.texWidth = hdCamera.actualWidth;
				passData.texHeight = hdCamera.actualHeight;
				passData.viewCount = hdCamera.viewCount;
				passData.pixelSpreadTangent = HDRenderPipeline.GetPixelSpreadTangent(hdCamera.camera.fieldOfView, hdCamera.actualWidth, hdCamera.actualHeight);
				passData.historyValidity = historyValidity;
				passData.validateHistoryKernel = m_ValidateHistoryKernel;
				passData.temporalFilterCS = m_TemporalFilterCS;
				passData.depthStencilBuffer = renderGraphBuilder.ReadTexture(in depthBuffer);
				passData.normalBuffer = renderGraphBuilder.ReadTexture(in normalBuffer);
				TextureHandle input;
				if (hdCamera.frameSettings.IsEnabled(FrameSettingsField.MotionVectors))
				{
					passData.motionVectorBuffer = renderGraphBuilder.ReadTexture(in motionVectorBuffer);
				}
				else
				{
					HistoryValidityPassData historyValidityPassData = passData;
					input = renderGraph.defaultResources.blackTextureXR;
					historyValidityPassData.motionVectorBuffer = renderGraphBuilder.ReadTexture(in input);
				}
				RTHandle currentFrameRT = hdCamera.GetCurrentFrameRT(6);
				RTHandle currentFrameRT2 = hdCamera.GetCurrentFrameRT(5);
				HistoryValidityPassData historyValidityPassData2 = passData;
				input = renderGraph.ImportTexture(currentFrameRT);
				historyValidityPassData2.historyDepthTexture = renderGraphBuilder.ReadTexture(in input);
				HistoryValidityPassData historyValidityPassData3 = passData;
				input = renderGraph.ImportTexture(currentFrameRT2);
				historyValidityPassData3.historyNormalTexture = renderGraphBuilder.ReadTexture(in input);
				passData.historySizeAndScale = ((currentFrameRT != null && currentFrameRT2 != null) ? HDRenderPipeline.EvaluateRayTracingHistorySizeAndScale(hdCamera, currentFrameRT) : Vector4.one);
				HistoryValidityPassData historyValidityPassData4 = passData;
				TextureDesc desc = new TextureDesc(Vector2.one, dynamicResolution: true, xrReady: true)
				{
					colorFormat = GraphicsFormat.R8_UInt,
					enableRandomWrite = true,
					name = "ValidationTexture"
				};
				input = renderGraph.CreateTexture(in desc);
				historyValidityPassData4.validationBuffer = renderGraphBuilder.WriteTexture(in input);
				renderGraphBuilder.SetRenderFunc(delegate(HistoryValidityPassData data, RenderGraphContext ctx)
				{
					RTHandle rTHandle = data.historyDepthTexture;
					RTHandle rTHandle2 = data.historyNormalTexture;
					if (rTHandle == null || rTHandle2 == null)
					{
						CoreUtils.SetRenderTarget(ctx.cmd, data.validationBuffer, ClearFlag.Color, Color.black);
					}
					else
					{
						int num = 8;
						int threadGroupsX = (data.texWidth + (num - 1)) / num;
						int threadGroupsY = (data.texHeight + (num - 1)) / num;
						ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.validateHistoryKernel, HDShaderIDs._DepthTexture, data.depthStencilBuffer);
						ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.validateHistoryKernel, HDShaderIDs._HistoryDepthTexture, data.historyDepthTexture);
						ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.validateHistoryKernel, HDShaderIDs._NormalBufferTexture, data.normalBuffer);
						ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.validateHistoryKernel, HDShaderIDs._HistoryNormalTexture, data.historyNormalTexture);
						ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.validateHistoryKernel, HDShaderIDs._CameraMotionVectorsTexture, data.motionVectorBuffer);
						ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.validateHistoryKernel, HDShaderIDs._StencilTexture, data.depthStencilBuffer, 0, RenderTextureSubElement.Stencil);
						ctx.cmd.SetComputeFloatParam(data.temporalFilterCS, HDShaderIDs._HistoryValidity, data.historyValidity);
						ctx.cmd.SetComputeFloatParam(data.temporalFilterCS, HDShaderIDs._PixelSpreadAngleTangent, data.pixelSpreadTangent);
						ctx.cmd.SetComputeIntParam(data.temporalFilterCS, HDShaderIDs._ObjectMotionStencilBit, 32);
						ctx.cmd.SetComputeVectorParam(data.temporalFilterCS, HDShaderIDs._HistorySizeAndScale, data.historySizeAndScale);
						ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.validateHistoryKernel, HDShaderIDs._ValidationBufferRW, data.validationBuffer);
						ctx.cmd.DispatchCompute(data.temporalFilterCS, data.validateHistoryKernel, threadGroupsX, threadGroupsY, data.viewCount);
					}
				});
				return passData.validationBuffer;
			}
			finally
			{
				((IDisposable)renderGraphBuilder).Dispose();
			}
		}

		internal TextureHandle Denoise(RenderGraph renderGraph, HDCamera hdCamera, TemporalFilterParameters filterParams, TextureHandle noisyBuffer, TextureHandle velocityBuffer, TextureHandle historyBuffer, TextureHandle depthBuffer, TextureHandle normalBuffer, TextureHandle motionVectorBuffer, TextureHandle historyValidationBuffer)
		{
			TemporalFilterPassData passData;
			RenderGraphBuilder renderGraphBuilder = renderGraph.AddRenderPass<TemporalFilterPassData>("TemporalDenoiser", out passData, ProfilingSampler.Get(HDProfileId.TemporalFilter));
			try
			{
				renderGraphBuilder.EnableAsyncCompute(value: false);
				if (filterParams.fullResolution)
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
				passData.historyValidity = filterParams.historyValidity;
				passData.receiverMotionRejection = filterParams.receiverMotionRejection;
				passData.occluderMotionRejection = filterParams.occluderMotionRejection;
				passData.exposureControl = (filterParams.exposureControl ? 1 : 0);
				passData.fullResolution = filterParams.fullResolution;
				passData.temporalAccKernel = (filterParams.singleChannel ? m_TemporalAccumulationSingleKernel : m_TemporalAccumulationColorKernel);
				passData.copyHistoryKernel = m_CopyHistoryKernel;
				passData.temporalFilterCS = m_TemporalFilterCS;
				passData.depthStencilBuffer = renderGraphBuilder.ReadTexture(in depthBuffer);
				passData.normalBuffer = renderGraphBuilder.ReadTexture(in normalBuffer);
				passData.motionVectorBuffer = renderGraphBuilder.ReadTexture(in motionVectorBuffer);
				passData.velocityBuffer = renderGraphBuilder.ReadTexture(in velocityBuffer);
				passData.noisyBuffer = renderGraphBuilder.ReadTexture(in noisyBuffer);
				passData.validationBuffer = renderGraphBuilder.ReadTexture(in historyValidationBuffer);
				passData.historyBuffer = renderGraphBuilder.ReadWriteTexture(in historyBuffer);
				TemporalFilterPassData temporalFilterPassData = passData;
				TextureDesc desc = new TextureDesc(Vector2.one, dynamicResolution: true, xrReady: true)
				{
					colorFormat = GraphicsFormat.R16G16B16A16_SFloat,
					enableRandomWrite = true,
					name = "Temporal Filter Output"
				};
				TextureHandle input = renderGraph.CreateTexture(in desc);
				temporalFilterPassData.outputBuffer = renderGraphBuilder.ReadWriteTexture(in input);
				renderGraphBuilder.SetRenderFunc(delegate(TemporalFilterPassData data, RenderGraphContext ctx)
				{
					int num = 8;
					int threadGroupsX = (data.texWidth + (num - 1)) / num;
					int threadGroupsY = (data.texHeight + (num - 1)) / num;
					ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.temporalAccKernel, HDShaderIDs._DenoiseInputTexture, data.noisyBuffer);
					ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.temporalAccKernel, HDShaderIDs._HistoryBuffer, data.historyBuffer);
					ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.temporalAccKernel, HDShaderIDs._DepthTexture, data.depthStencilBuffer);
					ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.temporalAccKernel, HDShaderIDs._ValidationBuffer, data.validationBuffer);
					ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.temporalAccKernel, HDShaderIDs._VelocityBuffer, data.velocityBuffer);
					ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.temporalAccKernel, HDShaderIDs._CameraMotionVectorsTexture, data.motionVectorBuffer);
					ctx.cmd.SetComputeFloatParam(data.temporalFilterCS, HDShaderIDs._HistoryValidity, data.historyValidity);
					ctx.cmd.SetComputeIntParam(data.temporalFilterCS, HDShaderIDs._ReceiverMotionRejection, data.receiverMotionRejection ? 1 : 0);
					ctx.cmd.SetComputeIntParam(data.temporalFilterCS, HDShaderIDs._OccluderMotionRejection, data.occluderMotionRejection ? 1 : 0);
					ctx.cmd.SetComputeFloatParam(data.temporalFilterCS, HDShaderIDs._PixelSpreadAngleTangent, data.pixelSpreadTangent);
					ctx.cmd.SetComputeIntParam(data.temporalFilterCS, HDShaderIDs._EnableExposureControl, data.exposureControl);
					ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.temporalAccKernel, HDShaderIDs._AccumulationOutputTextureRW, data.outputBuffer);
					CoreUtils.SetKeyword(ctx.cmd, "FULL_RESOLUTION_FILTER", data.fullResolution);
					ctx.cmd.DispatchCompute(data.temporalFilterCS, data.temporalAccKernel, threadGroupsX, threadGroupsY, data.viewCount);
					ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.copyHistoryKernel, HDShaderIDs._DenoiseInputTexture, data.outputBuffer);
					ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.copyHistoryKernel, HDShaderIDs._DenoiseOutputTextureRW, data.historyBuffer);
					ctx.cmd.DispatchCompute(data.temporalFilterCS, data.copyHistoryKernel, threadGroupsX, threadGroupsY, data.viewCount);
					CoreUtils.SetKeyword(ctx.cmd, "FULL_RESOLUTION_FILTER", state: true);
				});
				return passData.outputBuffer;
			}
			finally
			{
				((IDisposable)renderGraphBuilder).Dispose();
			}
		}

		public TemporalDenoiserArrayOutputData DenoiseBuffer(RenderGraph renderGraph, HDCamera hdCamera, TextureHandle depthBuffer, TextureHandle normalBuffer, TextureHandle motionVectorBuffer, TextureHandle historyValidationBuffer, TextureHandle noisyBuffer, RTHandle historyBuffer, TextureHandle distanceBuffer, RTHandle distanceHistorySignal, TextureHandle velocityBuffer, RTHandle validationHistoryBuffer, int sliceIndex, Vector4 channelMask, Vector4 distanceChannelMask, bool distanceBased, bool singleChannel, float historyValidity)
		{
			TemporalDenoiserArrayOutputData result = default(TemporalDenoiserArrayOutputData);
			TemporalFilterArrayPassData passData;
			RenderGraphBuilder renderGraphBuilder = renderGraph.AddRenderPass<TemporalFilterArrayPassData>("TemporalDenoiser", out passData, ProfilingSampler.Get(HDProfileId.TemporalFilter));
			try
			{
				renderGraphBuilder.EnableAsyncCompute(value: false);
				passData.texWidth = hdCamera.actualWidth;
				passData.texHeight = hdCamera.actualHeight;
				passData.viewCount = hdCamera.viewCount;
				passData.distanceBasedDenoiser = distanceBased;
				passData.historyValidity = historyValidity;
				passData.pixelSpreadTangent = HDRenderPipeline.GetPixelSpreadTangent(hdCamera.camera.fieldOfView, hdCamera.actualWidth, hdCamera.actualHeight);
				passData.sliceIndex = sliceIndex;
				passData.channelMask = channelMask;
				passData.distanceChannelMask = distanceChannelMask;
				passData.temporalAccKernel = (singleChannel ? m_TemporalAccumulationSingleArrayKernel : m_TemporalAccumulationColorArrayKernel);
				passData.blendHistoryKernel = (singleChannel ? m_BlendHistorySingleArrayKernel : m_BlendHistoryColorArrayKernel);
				passData.temporalAccSingleKernel = m_TemporalAccumulationSingleArrayKernel;
				passData.blendHistoryNoValidityKernel = m_BlendHistorySingleArrayNoValidityKernel;
				passData.outputHistoryKernel = m_OutputHistoryArrayKernel;
				passData.temporalFilterCS = m_TemporalFilterCS;
				passData.depthStencilBuffer = renderGraphBuilder.ReadTexture(in depthBuffer);
				passData.normalBuffer = renderGraphBuilder.ReadTexture(in normalBuffer);
				passData.motionVectorBuffer = renderGraphBuilder.ReadTexture(in motionVectorBuffer);
				passData.velocityBuffer = renderGraphBuilder.ReadTexture(in velocityBuffer);
				passData.noisyBuffer = renderGraphBuilder.ReadTexture(in noisyBuffer);
				passData.distanceBuffer = (distanceBased ? renderGraphBuilder.ReadTexture(in distanceBuffer) : renderGraph.defaultResources.blackTextureXR);
				passData.validationBuffer = renderGraphBuilder.ReadTexture(in historyValidationBuffer);
				TemporalFilterArrayPassData temporalFilterArrayPassData = passData;
				TextureHandle input = renderGraph.ImportTexture(historyBuffer);
				temporalFilterArrayPassData.outputHistoryBuffer = renderGraphBuilder.ReadWriteTexture(in input);
				passData.inputHistoryBuffer = passData.outputHistoryBuffer;
				TemporalFilterArrayPassData temporalFilterArrayPassData2 = passData;
				input = renderGraph.ImportTexture(validationHistoryBuffer);
				temporalFilterArrayPassData2.validationHistoryBuffer = renderGraphBuilder.ReadWriteTexture(in input);
				TemporalFilterArrayPassData temporalFilterArrayPassData3 = passData;
				TextureHandle distanceHistorySignal2;
				if (!distanceBased)
				{
					distanceHistorySignal2 = renderGraph.defaultResources.blackTextureXR;
				}
				else
				{
					input = renderGraph.ImportTexture(distanceHistorySignal);
					distanceHistorySignal2 = renderGraphBuilder.ReadWriteTexture(in input);
				}
				temporalFilterArrayPassData3.distanceHistorySignal = distanceHistorySignal2;
				TemporalFilterArrayPassData temporalFilterArrayPassData4 = passData;
				TextureDesc desc = new TextureDesc(Vector2.one, dynamicResolution: true, xrReady: true)
				{
					colorFormat = GraphicsFormat.R16G16B16A16_SFloat,
					enableRandomWrite = true,
					name = "Intermediate Filter Output"
				};
				temporalFilterArrayPassData4.intermediateSignalOutput = renderGraphBuilder.CreateTransientTexture(in desc);
				TemporalFilterArrayPassData temporalFilterArrayPassData5 = passData;
				desc = new TextureDesc(Vector2.one, dynamicResolution: true, xrReady: true)
				{
					colorFormat = GraphicsFormat.R16G16B16A16_SFloat,
					enableRandomWrite = true,
					name = "Intermediate Validity output"
				};
				temporalFilterArrayPassData5.intermediateValidityOutput = renderGraphBuilder.CreateTransientTexture(in desc);
				TemporalFilterArrayPassData temporalFilterArrayPassData6 = passData;
				desc = new TextureDesc(Vector2.one, dynamicResolution: true, xrReady: true)
				{
					colorFormat = GraphicsFormat.R16G16B16A16_SFloat,
					enableRandomWrite = true,
					name = "Temporal Filter Output"
				};
				input = renderGraph.CreateTexture(in desc);
				temporalFilterArrayPassData6.outputBuffer = renderGraphBuilder.ReadWriteTexture(in input);
				TemporalFilterArrayPassData temporalFilterArrayPassData7 = passData;
				TextureHandle outputDistanceSignal;
				if (!distanceBased)
				{
					outputDistanceSignal = default(TextureHandle);
				}
				else
				{
					desc = new TextureDesc(Vector2.one, dynamicResolution: true, xrReady: true)
					{
						colorFormat = GraphicsFormat.R16G16B16A16_SFloat,
						enableRandomWrite = true,
						name = "Temporal Filter Distance output"
					};
					input = renderGraph.CreateTexture(in desc);
					outputDistanceSignal = renderGraphBuilder.ReadWriteTexture(in input);
				}
				temporalFilterArrayPassData7.outputDistanceSignal = outputDistanceSignal;
				renderGraphBuilder.SetRenderFunc(delegate(TemporalFilterArrayPassData data, RenderGraphContext ctx)
				{
					int num = 8;
					int threadGroupsX = (data.texWidth + (num - 1)) / num;
					int threadGroupsY = (data.texHeight + (num - 1)) / num;
					CoreUtils.SetKeyword(ctx.cmd, "FULL_RESOLUTION_FILTER", state: true);
					ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.temporalAccKernel, HDShaderIDs._DenoiseInputTexture, data.noisyBuffer);
					ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.temporalAccKernel, HDShaderIDs._HistoryBuffer, data.inputHistoryBuffer);
					ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.temporalAccKernel, HDShaderIDs._HistoryValidityBuffer, data.validationHistoryBuffer);
					ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.temporalAccKernel, HDShaderIDs._DepthTexture, data.depthStencilBuffer);
					ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.temporalAccKernel, HDShaderIDs._CameraMotionVectorsTexture, data.motionVectorBuffer);
					ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.temporalAccKernel, HDShaderIDs._ValidationBuffer, data.validationBuffer);
					ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.temporalAccKernel, HDShaderIDs._VelocityBuffer, data.velocityBuffer);
					ctx.cmd.SetComputeIntParam(data.temporalFilterCS, HDShaderIDs._DenoisingHistorySlice, data.sliceIndex);
					ctx.cmd.SetComputeVectorParam(data.temporalFilterCS, HDShaderIDs._DenoisingHistoryMask, data.channelMask);
					ctx.cmd.SetComputeFloatParam(data.temporalFilterCS, HDShaderIDs._HistoryValidity, data.historyValidity);
					ctx.cmd.SetComputeIntParam(data.temporalFilterCS, HDShaderIDs._ReceiverMotionRejection, 1);
					ctx.cmd.SetComputeIntParam(data.temporalFilterCS, HDShaderIDs._OccluderMotionRejection, 1);
					ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.temporalAccKernel, HDShaderIDs._AccumulationOutputTextureRW, data.outputBuffer);
					ctx.cmd.DispatchCompute(data.temporalFilterCS, data.temporalAccKernel, threadGroupsX, threadGroupsY, data.viewCount);
					ctx.cmd.SetComputeIntParam(data.temporalFilterCS, HDShaderIDs._DenoisingHistorySlice, data.sliceIndex);
					ctx.cmd.SetComputeVectorParam(data.temporalFilterCS, HDShaderIDs._DenoisingHistoryMask, data.channelMask);
					ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.blendHistoryKernel, HDShaderIDs._DenoiseInputTexture, data.outputBuffer);
					ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.blendHistoryKernel, HDShaderIDs._DenoiseInputArrayTexture, data.inputHistoryBuffer);
					ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.blendHistoryKernel, HDShaderIDs._ValidityInputArrayTexture, data.validationHistoryBuffer);
					ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.blendHistoryKernel, HDShaderIDs._IntermediateDenoiseOutputTextureRW, data.intermediateSignalOutput);
					ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.blendHistoryKernel, HDShaderIDs._IntermediateValidityOutputTextureRW, data.intermediateValidityOutput);
					ctx.cmd.DispatchCompute(data.temporalFilterCS, data.blendHistoryKernel, threadGroupsX, threadGroupsY, data.viewCount);
					ctx.cmd.SetComputeIntParam(data.temporalFilterCS, HDShaderIDs._DenoisingHistorySlice, data.sliceIndex);
					ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.outputHistoryKernel, HDShaderIDs._IntermediateDenoiseOutputTexture, data.intermediateSignalOutput);
					ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.outputHistoryKernel, HDShaderIDs._IntermediateValidityOutputTexture, data.intermediateValidityOutput);
					ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.outputHistoryKernel, HDShaderIDs._DenoiseOutputArrayTextureRW, data.outputHistoryBuffer);
					ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.outputHistoryKernel, HDShaderIDs._ValidityOutputTextureRW, data.validationHistoryBuffer);
					ctx.cmd.DispatchCompute(data.temporalFilterCS, data.outputHistoryKernel, threadGroupsX, threadGroupsY, data.viewCount);
					if (data.distanceBasedDenoiser)
					{
						ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.temporalAccSingleKernel, HDShaderIDs._DenoiseInputTexture, data.distanceBuffer);
						ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.temporalAccSingleKernel, HDShaderIDs._HistoryBuffer, data.distanceHistorySignal);
						ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.temporalAccSingleKernel, HDShaderIDs._HistoryValidityBuffer, data.validationHistoryBuffer);
						ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.temporalAccSingleKernel, HDShaderIDs._DepthTexture, data.depthStencilBuffer);
						ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.temporalAccSingleKernel, HDShaderIDs._ValidationBuffer, data.validationBuffer);
						ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.temporalAccSingleKernel, HDShaderIDs._VelocityBuffer, data.velocityBuffer);
						ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.temporalAccSingleKernel, HDShaderIDs._CameraMotionVectorsTexture, data.motionVectorBuffer);
						ctx.cmd.SetComputeIntParam(data.temporalFilterCS, HDShaderIDs._DenoisingHistorySlice, data.sliceIndex);
						ctx.cmd.SetComputeVectorParam(data.temporalFilterCS, HDShaderIDs._DenoisingHistoryMask, data.distanceChannelMask);
						ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.temporalAccSingleKernel, HDShaderIDs._AccumulationOutputTextureRW, data.outputDistanceSignal);
						ctx.cmd.DispatchCompute(data.temporalFilterCS, data.temporalAccSingleKernel, threadGroupsX, threadGroupsY, data.viewCount);
						ctx.cmd.SetComputeIntParam(data.temporalFilterCS, HDShaderIDs._DenoisingHistorySlice, data.sliceIndex);
						ctx.cmd.SetComputeVectorParam(data.temporalFilterCS, HDShaderIDs._DenoisingHistoryMask, data.distanceChannelMask);
						ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.blendHistoryNoValidityKernel, HDShaderIDs._DenoiseInputTexture, data.outputDistanceSignal);
						ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.blendHistoryNoValidityKernel, HDShaderIDs._DenoiseInputArrayTexture, data.distanceHistorySignal);
						ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.blendHistoryNoValidityKernel, HDShaderIDs._IntermediateDenoiseOutputTextureRW, data.intermediateSignalOutput);
						ctx.cmd.DispatchCompute(data.temporalFilterCS, data.blendHistoryNoValidityKernel, threadGroupsX, threadGroupsY, data.viewCount);
						ctx.cmd.SetComputeIntParam(data.temporalFilterCS, HDShaderIDs._DenoisingHistorySlice, data.sliceIndex);
						ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.outputHistoryKernel, HDShaderIDs._IntermediateDenoiseOutputTexture, data.intermediateSignalOutput);
						ctx.cmd.SetComputeTextureParam(data.temporalFilterCS, data.outputHistoryKernel, HDShaderIDs._DenoiseOutputArrayTextureRW, data.distanceHistorySignal);
						ctx.cmd.DispatchCompute(data.temporalFilterCS, data.outputHistoryKernel, threadGroupsX, threadGroupsY, data.viewCount);
					}
				});
				result.outputSignal = passData.outputBuffer;
				result.outputSignalDistance = passData.outputDistanceSignal;
				return result;
			}
			finally
			{
				((IDisposable)renderGraphBuilder).Dispose();
			}
		}
	}
}
