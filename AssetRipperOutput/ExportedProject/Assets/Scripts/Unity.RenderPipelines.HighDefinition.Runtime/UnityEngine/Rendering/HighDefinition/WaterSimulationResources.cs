using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class WaterSimulationResources
	{
		private float m_Time;

		public float simulationTime;

		public float deltaTime;

		public int simulationResolution;

		public int maxNumBands;

		public WaterSurfaceType surfaceType;

		public WaterSpectrumParameters spectrum;

		public WaterRenderingParameters rendering;

		public WaterSimulationResourcesGPU gpuBuffers;

		public WaterSimulationResourcesCPU cpuBuffers;

		public void AllocateSimulationBuffersGPU()
		{
			gpuBuffers = new WaterSimulationResourcesGPU();
			gpuBuffers.phillipsSpectrumBuffer = RTHandles.Alloc(simulationResolution, simulationResolution, maxNumBands, DepthBits.None, GraphicsFormat.R16G16_SFloat, FilterMode.Point, TextureWrapMode.Repeat, TextureDimension.Tex2DArray, enableRandomWrite: true);
			gpuBuffers.displacementBuffer = RTHandles.Alloc(simulationResolution, simulationResolution, maxNumBands, DepthBits.None, GraphicsFormat.R16G16B16A16_SFloat, FilterMode.Point, TextureWrapMode.Repeat, TextureDimension.Tex2DArray, enableRandomWrite: true);
			gpuBuffers.additionalDataBuffer = RTHandles.Alloc(simulationResolution, simulationResolution, maxNumBands, DepthBits.None, GraphicsFormat.R16G16B16A16_SFloat, FilterMode.Point, TextureWrapMode.Repeat, TextureDimension.Tex2DArray, enableRandomWrite: true, useMipMap: true, autoGenerateMips: false);
		}

		public void ReleaseSimulationBuffersGPU()
		{
			if (gpuBuffers != null)
			{
				RTHandles.Release(gpuBuffers.additionalDataBuffer);
				RTHandles.Release(gpuBuffers.displacementBuffer);
				RTHandles.Release(gpuBuffers.phillipsSpectrumBuffer);
				RTHandles.Release(gpuBuffers.causticsBuffer);
				gpuBuffers = null;
			}
		}

		public void AllocateSimulationBuffersCPU()
		{
			cpuBuffers = new WaterSimulationResourcesCPU();
			cpuBuffers.h0BufferCPU = new NativeArray<float2>(simulationResolution * simulationResolution * maxNumBands, Allocator.Persistent);
			cpuBuffers.displacementBufferCPU = new NativeArray<float4>(simulationResolution * simulationResolution * maxNumBands, Allocator.Persistent);
		}

		public void ReleaseSimulationBuffersCPU()
		{
			if (cpuBuffers != null)
			{
				cpuBuffers.h0BufferCPU.Dispose();
				cpuBuffers.displacementBufferCPU.Dispose();
				cpuBuffers = null;
			}
		}

		public void InitializeSimulationResources(int simulationRes, int nbBands)
		{
			simulationResolution = simulationRes;
			maxNumBands = nbBands;
			m_Time = Time.realtimeSinceStartup;
		}

		public bool ValidResources(int simulationRes, int nbBands)
		{
			if (simulationRes == simulationResolution && nbBands == maxNumBands)
			{
				return AllocatedTextures();
			}
			return false;
		}

		public bool AllocatedTextures()
		{
			return gpuBuffers != null;
		}

		public void CheckCausticsResources(bool used, int causticsResolution)
		{
			if (used)
			{
				bool flag = true;
				if (gpuBuffers.causticsBuffer != null)
				{
					flag = gpuBuffers.causticsBuffer.rt.width != causticsResolution;
					if (flag)
					{
						RTHandles.Release(gpuBuffers.causticsBuffer);
					}
				}
				if (flag)
				{
					gpuBuffers.causticsBuffer = RTHandles.Alloc(causticsResolution, causticsResolution, 1, DepthBits.None, GraphicsFormat.R16_SFloat, FilterMode.Bilinear, TextureWrapMode.Repeat, TextureDimension.Tex2D, enableRandomWrite: true, useMipMap: true, autoGenerateMips: false);
				}
			}
			else if (gpuBuffers.causticsBuffer != null)
			{
				RTHandles.Release(gpuBuffers.causticsBuffer);
				gpuBuffers.causticsBuffer = null;
			}
		}

		public void Update(float timeMultiplier)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			float num = realtimeSinceStartup - m_Time;
			m_Time = realtimeSinceStartup;
			deltaTime = num * timeMultiplier;
			simulationTime += deltaTime;
		}

		public void ReleaseSimulationResources()
		{
			ReleaseSimulationBuffersGPU();
			ReleaseSimulationBuffersCPU();
			spectrum.numActiveBands = 0;
			spectrum.patchSizes = Vector4.zero;
			spectrum.patchWindSpeed = Vector4.zero;
			spectrum.patchWindOrientation = Vector4.zero;
			spectrum.patchWindDirDampener = Vector4.zero;
			rendering.patchAmplitudeMultiplier = Vector4.zero;
			rendering.patchCurrentSpeed = Vector4.zero;
			rendering.patchCurrentOrientation = Vector4.zero;
			rendering.patchFadeStart = Vector4.zero;
			rendering.patchFadeDistance = Vector4.zero;
			rendering.patchFadeValue = Vector4.zero;
			simulationResolution = 0;
			maxNumBands = 0;
			simulationTime = 0f;
			deltaTime = 0f;
		}
	}
}
