using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class RayCountManager
	{
		private class EvaluateRayCountPassData
		{
			public TextureHandle colorBuffer;

			public TextureHandle depthBuffer;

			public TextureHandle rayCountTexture;

			public ComputeBufferHandle reducedRayCountBuffer0;

			public ComputeBufferHandle reducedRayCountBuffer1;

			public ComputeBuffer reducedRayCountBufferOutput;

			public ComputeShader rayCountCS;

			public int rayCountKernel;

			public int clearKernel;

			public int width;

			public int height;

			public Queue<AsyncGPUReadbackRequest> rayCountReadbacks;
		}

		private ComputeBuffer m_ReducedRayCountBufferOutput;

		private uint[] m_ReducedRayCountValues = new uint[9];

		private ComputeShader m_RayCountCS;

		private bool m_IsActive;

		private bool m_RayTracingSupported;

		private Queue<AsyncGPUReadbackRequest> m_RayCountReadbacks = new Queue<AsyncGPUReadbackRequest>();

		public void Init(HDRenderPipelineRayTracingResources rayTracingResources)
		{
			m_RayCountCS = rayTracingResources.countTracedRays;
			m_ReducedRayCountBufferOutput = new ComputeBuffer(10, 4);
			for (int i = 0; i < 9; i++)
			{
				m_ReducedRayCountValues[i] = 0u;
			}
			m_IsActive = false;
			m_RayTracingSupported = true;
		}

		public void Release()
		{
			CoreUtils.SafeRelease(m_ReducedRayCountBufferOutput);
		}

		public int RayCountIsEnabled()
		{
			if (!m_IsActive)
			{
				return 0;
			}
			return 1;
		}

		internal void SetRayCountEnabled(bool value)
		{
			m_IsActive = value;
		}

		public static TextureHandle CreateRayCountTexture(RenderGraph renderGraph)
		{
			TextureDesc desc = new TextureDesc(Vector2.one, dynamicResolution: true, xrReady: true)
			{
				colorFormat = GraphicsFormat.R16_UInt,
				slices = TextureXR.slices * 9,
				dimension = TextureDimension.Tex2DArray,
				clearBuffer = true,
				enableRandomWrite = true,
				name = "RayCountTextureDebug"
			};
			return renderGraph.CreateTexture(in desc);
		}

		private void PrepareEvaluateRayCountPassData(in RenderGraphBuilder builder, EvaluateRayCountPassData data, HDCamera hdCamera, TextureHandle colorBuffer, TextureHandle depthBuffer, TextureHandle rayCountTexture)
		{
			data.colorBuffer = builder.UseColorBuffer(in colorBuffer, 0);
			data.depthBuffer = builder.UseDepthBuffer(in depthBuffer, DepthAccess.ReadWrite);
			data.rayCountTexture = builder.ReadTexture(in rayCountTexture);
			RenderGraphBuilder renderGraphBuilder = builder;
			ComputeBufferDesc desc = new ComputeBufferDesc(589824, 4);
			data.reducedRayCountBuffer0 = renderGraphBuilder.CreateTransientComputeBuffer(in desc);
			renderGraphBuilder = builder;
			desc = new ComputeBufferDesc(9216, 4);
			data.reducedRayCountBuffer1 = renderGraphBuilder.CreateTransientComputeBuffer(in desc);
			data.reducedRayCountBufferOutput = m_ReducedRayCountBufferOutput;
			data.rayCountCS = m_RayCountCS;
			data.rayCountKernel = m_RayCountCS.FindKernel("TextureReduction");
			data.clearKernel = m_RayCountCS.FindKernel("ClearBuffer");
			data.width = hdCamera.actualWidth;
			data.height = hdCamera.actualHeight;
			data.rayCountReadbacks = m_RayCountReadbacks;
		}

		public void EvaluateRayCount(RenderGraph renderGraph, HDCamera hdCamera, TextureHandle colorBuffer, TextureHandle depthBuffer, TextureHandle rayCountTexture)
		{
			if (!m_IsActive)
			{
				return;
			}
			EvaluateRayCountPassData passData;
			RenderGraphBuilder builder = renderGraph.AddRenderPass<EvaluateRayCountPassData>("RenderRayCountOverlay", out passData, ProfilingSampler.Get(HDProfileId.RaytracingDebugOverlay));
			try
			{
				PrepareEvaluateRayCountPassData(in builder, passData, hdCamera, colorBuffer, depthBuffer, rayCountTexture);
				builder.SetRenderFunc(delegate(EvaluateRayCountPassData data, RenderGraphContext ctx)
				{
					int width = data.width;
					int height = data.height;
					ComputeShader rayCountCS = data.rayCountCS;
					int rayCountKernel = data.rayCountKernel;
					int num = 32;
					int num2 = Mathf.Max(1, (width + (num - 1)) / num);
					int num3 = Mathf.Max(1, (height + (num - 1)) / num);
					if (num3 > 32 || num2 > 32)
					{
						ctx.cmd.SetComputeBufferParam(rayCountCS, rayCountKernel, HDShaderIDs._OutputRayCountBuffer, data.reducedRayCountBuffer0);
						ctx.cmd.SetComputeIntParam(rayCountCS, HDShaderIDs._OutputBufferDimension, 2304);
						int num4 = 8;
						ctx.cmd.DispatchCompute(rayCountCS, rayCountKernel, num4, num4, 1);
						ctx.cmd.SetComputeTextureParam(rayCountCS, rayCountKernel, HDShaderIDs._InputRayCountTexture, data.rayCountTexture);
						ctx.cmd.SetComputeBufferParam(rayCountCS, rayCountKernel, HDShaderIDs._OutputRayCountBuffer, data.reducedRayCountBuffer0);
						ctx.cmd.SetComputeIntParam(rayCountCS, HDShaderIDs._OutputBufferDimension, 2304);
						ctx.cmd.DispatchCompute(rayCountCS, rayCountKernel, num2, num3, 1);
						width /= 32;
						height /= 32;
						rayCountKernel = rayCountCS.FindKernel("BufferReduction");
						num2 = Mathf.Max(1, (width + (num - 1)) / num);
						num3 = Mathf.Max(1, (height + (num - 1)) / num);
						ctx.cmd.SetComputeBufferParam(rayCountCS, rayCountKernel, HDShaderIDs._InputRayCountBuffer, data.reducedRayCountBuffer0);
						ctx.cmd.SetComputeBufferParam(rayCountCS, rayCountKernel, HDShaderIDs._OutputRayCountBuffer, data.reducedRayCountBuffer1);
						ctx.cmd.SetComputeIntParam(rayCountCS, HDShaderIDs._InputBufferDimension, 2304);
						ctx.cmd.SetComputeIntParam(rayCountCS, HDShaderIDs._OutputBufferDimension, 288);
						ctx.cmd.DispatchCompute(rayCountCS, rayCountKernel, num2, num3, 1);
						width /= 32;
						height /= 32;
						num2 = Mathf.Max(1, (width + (num - 1)) / num);
						num3 = Mathf.Max(1, (height + (num - 1)) / num);
						ctx.cmd.SetComputeBufferParam(rayCountCS, rayCountKernel, HDShaderIDs._InputRayCountBuffer, data.reducedRayCountBuffer1);
						ctx.cmd.SetComputeBufferParam(rayCountCS, rayCountKernel, HDShaderIDs._OutputRayCountBuffer, data.reducedRayCountBufferOutput);
						ctx.cmd.SetComputeIntParam(rayCountCS, HDShaderIDs._InputBufferDimension, 288);
						ctx.cmd.SetComputeIntParam(rayCountCS, HDShaderIDs._OutputBufferDimension, 9);
						ctx.cmd.DispatchCompute(rayCountCS, rayCountKernel, num2, num3, 1);
					}
					else
					{
						ctx.cmd.SetComputeBufferParam(rayCountCS, rayCountKernel, HDShaderIDs._OutputRayCountBuffer, data.reducedRayCountBuffer1);
						ctx.cmd.SetComputeIntParam(rayCountCS, HDShaderIDs._OutputBufferDimension, 288);
						ctx.cmd.DispatchCompute(rayCountCS, rayCountKernel, 1, 1, 1);
						ctx.cmd.SetComputeTextureParam(rayCountCS, rayCountKernel, HDShaderIDs._InputRayCountTexture, data.rayCountTexture);
						ctx.cmd.SetComputeBufferParam(rayCountCS, rayCountKernel, HDShaderIDs._OutputRayCountBuffer, data.reducedRayCountBuffer1);
						ctx.cmd.SetComputeIntParam(rayCountCS, HDShaderIDs._OutputBufferDimension, 288);
						ctx.cmd.DispatchCompute(rayCountCS, rayCountKernel, num2, num3, 1);
						width /= 32;
						height /= 32;
						rayCountKernel = rayCountCS.FindKernel("BufferReduction");
						num2 = Mathf.Max(1, (width + (num - 1)) / num);
						num3 = Mathf.Max(1, (height + (num - 1)) / num);
						ctx.cmd.SetComputeBufferParam(rayCountCS, rayCountKernel, HDShaderIDs._InputRayCountBuffer, data.reducedRayCountBuffer1);
						ctx.cmd.SetComputeBufferParam(rayCountCS, rayCountKernel, HDShaderIDs._OutputRayCountBuffer, data.reducedRayCountBufferOutput);
						ctx.cmd.SetComputeIntParam(rayCountCS, HDShaderIDs._InputBufferDimension, 288);
						ctx.cmd.SetComputeIntParam(rayCountCS, HDShaderIDs._OutputBufferDimension, 9);
						ctx.cmd.DispatchCompute(rayCountCS, rayCountKernel, num2, num3, 1);
					}
					AsyncGPUReadbackRequest item = AsyncGPUReadback.Request(data.reducedRayCountBufferOutput, 36, 0);
					data.rayCountReadbacks.Enqueue(item);
				});
			}
			finally
			{
				((IDisposable)builder).Dispose();
			}
		}

		public uint GetRaysPerFrame(RayCountValues rayCountValue)
		{
			if (!m_RayTracingSupported || !m_IsActive)
			{
				return 0u;
			}
			while (m_RayCountReadbacks.Peek().done || m_RayCountReadbacks.Peek().hasError)
			{
				if (!m_RayCountReadbacks.Peek().hasError)
				{
					NativeArray<uint> data = m_RayCountReadbacks.Peek().GetData<uint>();
					for (int i = 0; i < 9; i++)
					{
						m_ReducedRayCountValues[i] = data[i];
					}
				}
				m_RayCountReadbacks.Dequeue();
			}
			if (rayCountValue != RayCountValues.Total)
			{
				return m_ReducedRayCountValues[(int)rayCountValue];
			}
			uint num = 0u;
			for (int j = 0; j < 9; j++)
			{
				num += m_ReducedRayCountValues[j];
			}
			return num;
		}
	}
}
