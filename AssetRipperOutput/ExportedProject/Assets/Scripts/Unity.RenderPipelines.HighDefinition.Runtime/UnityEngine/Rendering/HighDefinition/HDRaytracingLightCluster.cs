using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Experimental.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class HDRaytracingLightCluster
	{
		private class LightClusterDebugPassData
		{
			public int texWidth;

			public int texHeight;

			public int lightClusterDebugKernel;

			public Vector3 clusterCellSize;

			public Material debugMaterial;

			public ComputeBufferHandle lightCluster;

			public ComputeShader lightClusterDebugCS;

			public TextureHandle depthStencilBuffer;

			public TextureHandle depthPyramid;

			public TextureHandle outputBuffer;
		}

		private HDRenderPipelineRuntimeResources m_RenderPipelineResources;

		private HDRenderPipelineRayTracingResources m_RenderPipelineRayTracingResources;

		private HDRenderPipeline m_RenderPipeline;

		private LightVolume[] m_LightVolumesCPUArray;

		private ComputeBuffer m_LightVolumeGPUArray;

		private ComputeBuffer m_LightCullResult;

		private ComputeBuffer m_LightCluster;

		private List<LightData> m_LightDataCPUArray = new List<LightData>();

		private ComputeBuffer m_LightDataGPUArray;

		private List<EnvLightData> m_EnvLightDataCPUArray = new List<EnvLightData>();

		private ComputeBuffer m_EnvLightDataGPUArray;

		private Material m_DebugMaterial;

		private const string m_LightClusterKernelName = "RaytracingLightCluster";

		private const string m_LightCullKernelName = "RaytracingLightCull";

		public static readonly int _ClusterCellSize = Shader.PropertyToID("_ClusterCellSize");

		public static readonly int _LightVolumes = Shader.PropertyToID("_LightVolumes");

		public static readonly int _LightVolumeCount = Shader.PropertyToID("_LightVolumeCount");

		public static readonly int _DebugColorGradientTexture = Shader.PropertyToID("_DebugColorGradientTexture");

		public static readonly int _DebutLightClusterTexture = Shader.PropertyToID("_DebutLightClusterTexture");

		public static readonly int _RaytracingLightCullResult = Shader.PropertyToID("_RaytracingLightCullResult");

		public static readonly int _ClusterCenterPosition = Shader.PropertyToID("_ClusterCenterPosition");

		public static readonly int _ClusterDimension = Shader.PropertyToID("_ClusterDimension");

		private int m_NumLightsPerCell;

		private Vector3 minClusterPos = new Vector3(0f, 0f, 0f);

		private Vector3 maxClusterPos = new Vector3(0f, 0f, 0f);

		private Vector3 clusterCellSize = new Vector3(0f, 0f, 0f);

		private Vector3 clusterCenter = new Vector3(0f, 0f, 0f);

		private Vector3 clusterDimension = new Vector3(0f, 0f, 0f);

		private int punctualLightCount;

		private int areaLightCount;

		private int envLightCount;

		private int totalLightCount;

		private Bounds bounds;

		private Vector3 minBounds = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);

		private Vector3 maxBounds = new Vector3(float.MinValue, float.MinValue, float.MinValue);

		private Matrix4x4 localToWorldMatrix;

		private VisibleLight visibleLight;

		private Light lightComponent;

		internal const int k_MaxPlanarReflectionsOnScreen = 16;

		internal const int k_MaxCubeReflectionsOnScreen = 64;

		private EnvLightReflectionDataRT m_EnvLightReflectionDataRT;

		public void Initialize(HDRenderPipeline renderPipeline)
		{
			m_RenderPipelineResources = HDRenderPipelineGlobalSettings.instance.renderPipelineResources;
			m_RenderPipelineRayTracingResources = HDRenderPipelineGlobalSettings.instance.renderPipelineRayTracingResources;
			m_RenderPipeline = renderPipeline;
			m_LightDataGPUArray = new ComputeBuffer(1, Marshal.SizeOf(typeof(LightData)));
			m_EnvLightDataGPUArray = new ComputeBuffer(1, Marshal.SizeOf(typeof(EnvLightData)));
			m_NumLightsPerCell = renderPipeline.asset.currentPlatformRenderPipelineSettings.lightLoopSettings.maxLightsPerClusterCell;
			int bufferSize = 131072 * (renderPipeline.asset.currentPlatformRenderPipelineSettings.lightLoopSettings.maxLightsPerClusterCell + 4);
			ResizeClusterBuffer(bufferSize);
			m_DebugMaterial = CoreUtils.CreateEngineMaterial(m_RenderPipelineRayTracingResources.lightClusterDebugS);
		}

		public void ReleaseResources()
		{
			CoreUtils.SafeRelease(m_LightVolumeGPUArray);
			m_LightVolumeGPUArray = null;
			CoreUtils.SafeRelease(m_LightCluster);
			m_LightCluster = null;
			CoreUtils.SafeRelease(m_LightCullResult);
			m_LightCullResult = null;
			CoreUtils.SafeRelease(m_LightDataGPUArray);
			m_LightDataGPUArray = null;
			CoreUtils.SafeRelease(m_EnvLightDataGPUArray);
			m_EnvLightDataGPUArray = null;
			CoreUtils.Destroy(m_DebugMaterial);
			m_DebugMaterial = null;
		}

		private void ResizeClusterBuffer(int bufferSize)
		{
			if (m_LightCluster != null)
			{
				if (m_LightCluster.count == bufferSize)
				{
					return;
				}
				CoreUtils.SafeRelease(m_LightCluster);
				m_LightCluster = null;
			}
			if (bufferSize > 0)
			{
				m_LightCluster = new ComputeBuffer(bufferSize, 4);
			}
		}

		private void ResizeCullResultBuffer(int numLights)
		{
			if (m_LightCullResult != null)
			{
				if (m_LightCullResult.count == numLights)
				{
					return;
				}
				CoreUtils.SafeRelease(m_LightCullResult);
				m_LightCullResult = null;
			}
			if (numLights > 0)
			{
				m_LightCullResult = new ComputeBuffer(numLights, 4);
			}
		}

		private void ResizeVolumeBuffer(int numLights)
		{
			if (m_LightVolumeGPUArray != null)
			{
				if (m_LightVolumeGPUArray.count == numLights)
				{
					return;
				}
				CoreUtils.SafeRelease(m_LightVolumeGPUArray);
				m_LightVolumeGPUArray = null;
			}
			if (numLights > 0)
			{
				m_LightVolumesCPUArray = new LightVolume[numLights];
				m_LightVolumeGPUArray = new ComputeBuffer(numLights, Marshal.SizeOf(typeof(LightVolume)));
			}
		}

		private void ResizeLightDataBuffer(int numLights)
		{
			if (m_LightDataGPUArray != null)
			{
				if (m_LightDataGPUArray.count == numLights)
				{
					return;
				}
				CoreUtils.SafeRelease(m_LightDataGPUArray);
				m_LightDataGPUArray = null;
			}
			if (numLights > 0)
			{
				m_LightDataGPUArray = new ComputeBuffer(numLights, Marshal.SizeOf(typeof(LightData)));
			}
		}

		private void ResizeEnvLightDataBuffer(int numEnvLights)
		{
			if (m_EnvLightDataGPUArray != null)
			{
				if (m_EnvLightDataGPUArray.count == numEnvLights)
				{
					return;
				}
				CoreUtils.SafeRelease(m_EnvLightDataGPUArray);
				m_EnvLightDataGPUArray = null;
			}
			if (numEnvLights > 0)
			{
				m_EnvLightDataGPUArray = new ComputeBuffer(numEnvLights, Marshal.SizeOf(typeof(EnvLightData)));
			}
		}

		private void OOBBToAABBBounds(Vector3 centerWS, Vector3 extents, Vector3 up, Vector3 right, Vector3 forward, ref Bounds outBounds)
		{
			bounds.min = minBounds;
			bounds.max = maxBounds;
			bounds.Encapsulate(centerWS + right * extents.x + up * extents.y + forward * extents.z);
			bounds.Encapsulate(centerWS + right * extents.x + up * extents.y - forward * extents.z);
			bounds.Encapsulate(centerWS + right * extents.x - up * extents.y + forward * extents.z);
			bounds.Encapsulate(centerWS + right * extents.x - up * extents.y - forward * extents.z);
			bounds.Encapsulate(centerWS - right * extents.x + up * extents.y + forward * extents.z);
			bounds.Encapsulate(centerWS - right * extents.x + up * extents.y - forward * extents.z);
			bounds.Encapsulate(centerWS - right * extents.x - up * extents.y + forward * extents.z);
			bounds.Encapsulate(centerWS - right * extents.x - up * extents.y - forward * extents.z);
		}

		private void BuildGPULightVolumes(HDCamera hdCamera, HDRayTracingLights rayTracingLights)
		{
			int lightCount = rayTracingLights.lightCount;
			if (m_LightVolumesCPUArray == null || lightCount != m_LightVolumesCPUArray.Length)
			{
				ResizeVolumeBuffer(lightCount);
			}
			punctualLightCount = 0;
			areaLightCount = 0;
			envLightCount = 0;
			totalLightCount = 0;
			int num = 0;
			HDLightRenderDatabase instance = HDLightRenderDatabase.instance;
			for (int i = 0; i < rayTracingLights.hdLightEntityArray.Count; i++)
			{
				int entityDataIndex = instance.GetEntityDataIndex(rayTracingLights.hdLightEntityArray[i]);
				HDAdditionalLightData hDAdditionalLightData = instance.hdAdditionalLightData[entityDataIndex];
				if (!(hDAdditionalLightData != null))
				{
					continue;
				}
				Light component = hDAdditionalLightData.gameObject.GetComponent<Light>();
				if (component == null)
				{
					continue;
				}
				m_RenderPipeline.ReserveCookieAtlasTexture(hDAdditionalLightData, component, hDAdditionalLightData.type);
				Vector3 position = hDAdditionalLightData.gameObject.transform.position;
				if (ShaderConfig.s_CameraRelativeRendering != 0)
				{
					position -= hdCamera.camera.transform.position;
				}
				float range = component.range;
				m_LightVolumesCPUArray[num].active = (hDAdditionalLightData.gameObject.activeInHierarchy ? 1 : 0);
				m_LightVolumesCPUArray[num].lightIndex = (uint)i;
				bool flag = hDAdditionalLightData.type == HDLightType.Area;
				bool flag2 = hDAdditionalLightData.type == HDLightType.Spot && hDAdditionalLightData.spotLightShape == SpotLightShape.Box;
				if (!flag && !flag2)
				{
					m_LightVolumesCPUArray[num].range = new Vector3(range, range, range);
					m_LightVolumesCPUArray[num].position = position;
					m_LightVolumesCPUArray[num].shape = 0;
					m_LightVolumesCPUArray[num].lightType = 0u;
					punctualLightCount++;
				}
				else
				{
					Vector3 vector = new Vector3(hDAdditionalLightData.shapeWidth + 2f * range, hDAdditionalLightData.shapeHeight + 2f * range, range);
					Vector3 extents = 0.5f * vector;
					Vector3 centerWS = position + extents.z * hDAdditionalLightData.gameObject.transform.forward;
					OOBBToAABBBounds(centerWS, extents, hDAdditionalLightData.gameObject.transform.up, hDAdditionalLightData.gameObject.transform.right, hDAdditionalLightData.gameObject.transform.forward, ref bounds);
					m_LightVolumesCPUArray[num].range = bounds.extents;
					m_LightVolumesCPUArray[num].position = bounds.center;
					m_LightVolumesCPUArray[num].shape = 1;
					if (flag)
					{
						m_LightVolumesCPUArray[num].lightType = 1u;
						areaLightCount++;
					}
					else
					{
						m_LightVolumesCPUArray[num].lightType = 0u;
						punctualLightCount++;
					}
				}
				num++;
			}
			int num2 = num;
			for (int j = 0; j < rayTracingLights.reflectionProbeArray.Count; j++)
			{
				HDProbe hDProbe = rayTracingLights.reflectionProbeArray[j];
				if (hDProbe != null && hDProbe.enabled && hDProbe.HasValidRenderedData())
				{
					Vector3 position2 = hDProbe.influenceToWorld.GetColumn(3);
					if (ShaderConfig.s_CameraRelativeRendering != 0)
					{
						position2 -= hdCamera.camera.transform.position;
					}
					if (hDProbe.influenceVolume.shape == InfluenceShape.Sphere)
					{
						m_LightVolumesCPUArray[j + num2].shape = 0;
						m_LightVolumesCPUArray[j + num2].range = new Vector3(hDProbe.influenceVolume.sphereRadius, hDProbe.influenceVolume.sphereRadius, hDProbe.influenceVolume.sphereRadius);
						m_LightVolumesCPUArray[j + num2].position = position2;
					}
					else
					{
						m_LightVolumesCPUArray[j + num2].shape = 1;
						m_LightVolumesCPUArray[j + num2].range = new Vector3(hDProbe.influenceVolume.boxSize.x / 2f, hDProbe.influenceVolume.boxSize.y / 2f, hDProbe.influenceVolume.boxSize.z / 2f);
						m_LightVolumesCPUArray[j + num2].position = position2;
					}
					m_LightVolumesCPUArray[j + num2].active = (hDProbe.gameObject.activeInHierarchy ? 1 : 0);
					m_LightVolumesCPUArray[j + num2].lightIndex = (uint)j;
					m_LightVolumesCPUArray[j + num2].lightType = 2u;
					envLightCount++;
				}
			}
			totalLightCount = punctualLightCount + areaLightCount + envLightCount;
			m_LightVolumeGPUArray.SetData(m_LightVolumesCPUArray);
		}

		private void EvaluateClusterVolume(HDCamera hdCamera)
		{
			LightCluster component = hdCamera.volumeStack.GetComponent<LightCluster>();
			if (ShaderConfig.s_CameraRelativeRendering != 0)
			{
				clusterCenter.Set(0f, 0f, 0f);
			}
			else
			{
				clusterCenter = hdCamera.camera.gameObject.transform.position;
			}
			minClusterPos.Set(float.MaxValue, float.MaxValue, float.MaxValue);
			maxClusterPos.Set(float.MinValue, float.MinValue, float.MinValue);
			for (int i = 0; i < totalLightCount; i++)
			{
				minClusterPos.x = Mathf.Min(m_LightVolumesCPUArray[i].position.x - m_LightVolumesCPUArray[i].range.x, minClusterPos.x);
				minClusterPos.y = Mathf.Min(m_LightVolumesCPUArray[i].position.y - m_LightVolumesCPUArray[i].range.y, minClusterPos.y);
				minClusterPos.z = Mathf.Min(m_LightVolumesCPUArray[i].position.z - m_LightVolumesCPUArray[i].range.z, minClusterPos.z);
				maxClusterPos.x = Mathf.Max(m_LightVolumesCPUArray[i].position.x + m_LightVolumesCPUArray[i].range.x, maxClusterPos.x);
				maxClusterPos.y = Mathf.Max(m_LightVolumesCPUArray[i].position.y + m_LightVolumesCPUArray[i].range.y, maxClusterPos.y);
				maxClusterPos.z = Mathf.Max(m_LightVolumesCPUArray[i].position.z + m_LightVolumesCPUArray[i].range.z, maxClusterPos.z);
			}
			minClusterPos.x = ((minClusterPos.x < clusterCenter.x - component.cameraClusterRange.value) ? (clusterCenter.x - component.cameraClusterRange.value) : minClusterPos.x);
			minClusterPos.y = ((minClusterPos.y < clusterCenter.y - component.cameraClusterRange.value) ? (clusterCenter.y - component.cameraClusterRange.value) : minClusterPos.y);
			minClusterPos.z = ((minClusterPos.z < clusterCenter.z - component.cameraClusterRange.value) ? (clusterCenter.z - component.cameraClusterRange.value) : minClusterPos.z);
			maxClusterPos.x = ((maxClusterPos.x > clusterCenter.x + component.cameraClusterRange.value) ? (clusterCenter.x + component.cameraClusterRange.value) : maxClusterPos.x);
			maxClusterPos.y = ((maxClusterPos.y > clusterCenter.y + component.cameraClusterRange.value) ? (clusterCenter.y + component.cameraClusterRange.value) : maxClusterPos.y);
			maxClusterPos.z = ((maxClusterPos.z > clusterCenter.z + component.cameraClusterRange.value) ? (clusterCenter.z + component.cameraClusterRange.value) : maxClusterPos.z);
			clusterCellSize = maxClusterPos - minClusterPos;
			clusterCellSize.x /= 64f;
			clusterCellSize.y /= 64f;
			clusterCellSize.z /= 32f;
			clusterCenter = (maxClusterPos + minClusterPos) / 2f;
			clusterDimension = maxClusterPos - minClusterPos;
		}

		private void CullLights(CommandBuffer cmd)
		{
			using (new ProfilingScope(cmd, ProfilingSampler.Get(HDProfileId.RaytracingCullLights)))
			{
				if (m_LightCullResult == null || m_LightCullResult.count != totalLightCount)
				{
					ResizeCullResultBuffer(totalLightCount);
				}
				ComputeShader lightClusterBuildCS = m_RenderPipelineRayTracingResources.lightClusterBuildCS;
				int kernelIndex = lightClusterBuildCS.FindKernel("RaytracingLightCull");
				cmd.SetComputeVectorParam(lightClusterBuildCS, _ClusterCenterPosition, clusterCenter);
				cmd.SetComputeVectorParam(lightClusterBuildCS, _ClusterDimension, clusterDimension);
				cmd.SetComputeFloatParam(lightClusterBuildCS, _LightVolumeCount, totalLightCount);
				cmd.SetComputeBufferParam(lightClusterBuildCS, kernelIndex, _LightVolumes, m_LightVolumeGPUArray);
				cmd.SetComputeBufferParam(lightClusterBuildCS, kernelIndex, _RaytracingLightCullResult, m_LightCullResult);
				int threadGroupsX = totalLightCount / 16 + 1;
				cmd.DispatchCompute(lightClusterBuildCS, kernelIndex, threadGroupsX, 1, 1);
			}
		}

		private void BuildLightCluster(HDCamera hdCamera, CommandBuffer cmd)
		{
			using (new ProfilingScope(cmd, ProfilingSampler.Get(HDProfileId.RaytracingBuildCluster)))
			{
				ComputeShader lightClusterBuildCS = m_RenderPipelineRayTracingResources.lightClusterBuildCS;
				int kernelIndex = lightClusterBuildCS.FindKernel("RaytracingLightCluster");
				cmd.SetComputeBufferParam(lightClusterBuildCS, kernelIndex, HDShaderIDs._RaytracingLightClusterRW, m_LightCluster);
				cmd.SetComputeVectorParam(lightClusterBuildCS, _ClusterCellSize, clusterCellSize);
				cmd.SetComputeBufferParam(lightClusterBuildCS, kernelIndex, _LightVolumes, m_LightVolumeGPUArray);
				cmd.SetComputeFloatParam(lightClusterBuildCS, _LightVolumeCount, totalLightCount);
				cmd.SetComputeBufferParam(lightClusterBuildCS, kernelIndex, _RaytracingLightCullResult, m_LightCullResult);
				int threadGroupsX = 8;
				int threadGroupsY = 8;
				int threadGroupsZ = 4;
				cmd.DispatchCompute(lightClusterBuildCS, kernelIndex, threadGroupsX, threadGroupsY, threadGroupsZ);
			}
		}

		private void BuildLightData(CommandBuffer cmd, HDCamera hdCamera, HDRayTracingLights rayTracingLights, DebugDisplaySettings debugDisplaySettings)
		{
			if (rayTracingLights.lightCount == 0)
			{
				ResizeLightDataBuffer(1);
				return;
			}
			if (m_LightDataGPUArray == null || m_LightDataGPUArray.count != rayTracingLights.lightCount)
			{
				ResizeLightDataBuffer(rayTracingLights.lightCount);
			}
			m_LightDataCPUArray.Clear();
			HDShadowSettings component = hdCamera.volumeStack.GetComponent<HDShadowSettings>();
			HDAdditionalLightData.ScalableSettings.UseContactShadow(m_RenderPipeline.asset);
			HDLightRenderDatabase instance = HDLightRenderDatabase.instance;
			HDProcessedVisibleLight hDProcessedVisibleLight = default(HDProcessedVisibleLight);
			hDProcessedVisibleLight.shadowMapFlags = HDProcessedVisibleLightsBuilder.ShadowMapFlags.None;
			HDProcessedVisibleLight processedEntity = hDProcessedVisibleLight;
			HDGpuLightsBuilder.CreateGpuLightDataJobGlobalConfig globalConfig = HDGpuLightsBuilder.CreateGpuLightDataJobGlobalConfig.Create(hdCamera, component);
			HDShadowInitParameters shadowInitParams = m_RenderPipeline.currentPlatformRenderPipelineSettings.hdShadowInitParams;
			for (int i = 0; i < rayTracingLights.hdLightEntityArray.Count; i++)
			{
				int entityDataIndex = instance.GetEntityDataIndex(rayTracingLights.hdLightEntityArray[i]);
				HDAdditionalLightData hDAdditionalLightData = instance.hdAdditionalLightData[entityDataIndex];
				LightData lightData = default(LightData);
				if (hDAdditionalLightData == null)
				{
					m_LightDataCPUArray.Add(lightData);
					continue;
				}
				LightCategory lightCategory = LightCategory.Count;
				GPULightType gpuLightType = GPULightType.Point;
				LightVolumeType lightVolumeType = LightVolumeType.Count;
				HDLightType type = hDAdditionalLightData.type;
				HDRenderPipeline.EvaluateGPULightType(type, hDAdditionalLightData.spotLightShape, hDAdditionalLightData.areaLightShape, ref lightCategory, ref gpuLightType, ref lightVolumeType);
				hDAdditionalLightData.gameObject.TryGetComponent<Light>(out lightComponent);
				ref HDLightRenderData lightDataAsRef = ref instance.GetLightDataAsRef(entityDataIndex);
				processedEntity.dataIndex = entityDataIndex;
				processedEntity.gpuLightType = gpuLightType;
				processedEntity.lightType = hDAdditionalLightData.type;
				Vector3 lightDimensions = hDAdditionalLightData.transform.position - hdCamera.camera.transform.position;
				processedEntity.distanceToCamera = lightDimensions.magnitude;
				processedEntity.lightDistanceFade = HDUtils.ComputeLinearDistanceFade(processedEntity.distanceToCamera, lightDataAsRef.fadeDistance);
				processedEntity.lightVolumetricDistanceFade = HDUtils.ComputeLinearDistanceFade(processedEntity.distanceToCamera, lightDataAsRef.volumetricFadeDistance);
				processedEntity.isBakedShadowMask = HDRenderPipeline.IsBakedShadowMaskLight(lightComponent);
				visibleLight.finalColor = LightUtils.EvaluateLightColor(lightComponent, hDAdditionalLightData);
				visibleLight.range = lightComponent.range;
				localToWorldMatrix.SetColumn(3, lightComponent.gameObject.transform.position);
				localToWorldMatrix.SetColumn(2, lightComponent.transform.forward);
				localToWorldMatrix.SetColumn(1, lightComponent.transform.up);
				localToWorldMatrix.SetColumn(0, lightComponent.transform.right);
				visibleLight.localToWorldMatrix = localToWorldMatrix;
				visibleLight.spotAngle = lightComponent.spotAngle;
				int shadowIndex = hDAdditionalLightData.shadowIndex;
				new Vector3(0f, 0f, 0f);
				LightCategory lightCategory2 = lightCategory;
				GPULightType gpuLightType2 = gpuLightType;
				LightShadowCasterMode lightShadowCasterMode = lightComponent.lightShadowCasterMode;
				LightBakingOutput visibleLightBakingOutput = lightComponent.bakingOutput;
				HDGpuLightsBuilder.CreateGpuLightDataJob.ConvertLightToGPUFormat(lightCategory2, gpuLightType2, in globalConfig, lightShadowCasterMode, in visibleLightBakingOutput, in visibleLight, in processedEntity, in lightDataAsRef, out lightDimensions, ref lightData);
				m_RenderPipeline.gpuLightList.ProcessLightDataShadowIndex(cmd, in shadowInitParams, type, lightComponent, hDAdditionalLightData, shadowIndex, ref lightData);
				Vector3 worldSpaceCameraPos = hdCamera.mainViewConstants.worldSpaceCameraPos;
				HDRenderPipeline.UpdateLightCameraRelativetData(ref lightData, worldSpaceCameraPos);
				m_LightDataCPUArray.Add(lightData);
			}
			m_LightDataGPUArray.SetData(m_LightDataCPUArray);
		}

		private unsafe void SetPlanarReflectionDataRT(int index, ref Matrix4x4 vp, ref Vector4 scaleOffset)
		{
			for (int i = 0; i < 16; i++)
			{
				m_EnvLightReflectionDataRT._PlanarCaptureVPRT[index * 16 + i] = vp[i];
			}
			for (int j = 0; j < 4; j++)
			{
				m_EnvLightReflectionDataRT._PlanarScaleOffsetRT[index * 4 + j] = scaleOffset[j];
			}
		}

		private unsafe void SetCubeReflectionDataRT(int index, ref Vector4 scaleOffset)
		{
			for (int i = 0; i < 4; i++)
			{
				m_EnvLightReflectionDataRT._CubeScaleOffsetRT[index * 4 + i] = scaleOffset[i];
			}
		}

		private void BuildEnvLightData(CommandBuffer cmd, HDCamera hdCamera, HDRayTracingLights lights)
		{
			int count = lights.reflectionProbeArray.Count;
			if (count == 0)
			{
				ResizeEnvLightDataBuffer(1);
				return;
			}
			if (m_EnvLightDataCPUArray == null || m_EnvLightDataGPUArray == null || m_EnvLightDataGPUArray.count != count)
			{
				ResizeEnvLightDataBuffer(count);
			}
			m_EnvLightDataCPUArray.Clear();
			ProcessedProbeData processedData = default(ProcessedProbeData);
			EnvLightData envLightData = default(EnvLightData);
			for (int i = 0; i < lights.reflectionProbeArray.Count; i++)
			{
				HDProbe probe = lights.reflectionProbeArray[i];
				HDRenderPipeline.PreprocessProbeData(ref processedData, probe, hdCamera);
				m_RenderPipeline.GetEnvLightData(cmd, hdCamera, in processedData, ref envLightData, out var fetchIndex, out var scaleOffset, out var vp);
				HDProbe hdProbe = processedData.hdProbe;
				if (!(hdProbe is PlanarReflectionProbe))
				{
					if (hdProbe is HDAdditionalReflectionData)
					{
						SetCubeReflectionDataRT(fetchIndex, ref scaleOffset);
					}
				}
				else
				{
					SetPlanarReflectionDataRT(fetchIndex, ref vp, ref scaleOffset);
				}
				Vector3 worldSpaceCameraPos = hdCamera.mainViewConstants.worldSpaceCameraPos;
				HDRenderPipeline.UpdateEnvLighCameraRelativetData(ref envLightData, worldSpaceCameraPos);
				m_EnvLightDataCPUArray.Add(envLightData);
			}
			m_EnvLightDataGPUArray.SetData(m_EnvLightDataCPUArray);
		}

		public void EvaluateClusterDebugView(RenderGraph renderGraph, HDCamera hdCamera, TextureHandle depthStencilBuffer, TextureHandle depthPyramid)
		{
			if (FullScreenDebugMode.LightCluster != m_RenderPipeline.m_CurrentDebugDisplaySettings.data.fullScreenDebugMode)
			{
				return;
			}
			LightClusterDebugPassData passData;
			RenderGraphBuilder renderGraphBuilder = renderGraph.AddRenderPass<LightClusterDebugPassData>("Debug Texture for the Light Cluster", out passData, ProfilingSampler.Get(HDProfileId.RaytracingDebugCluster));
			TextureHandle outputBuffer;
			try
			{
				renderGraphBuilder.EnableAsyncCompute(value: false);
				passData.texWidth = hdCamera.actualWidth;
				passData.texHeight = hdCamera.actualHeight;
				passData.clusterCellSize = clusterCellSize;
				LightClusterDebugPassData lightClusterDebugPassData = passData;
				ComputeBufferHandle input = renderGraph.ImportComputeBuffer(m_LightCluster);
				lightClusterDebugPassData.lightCluster = renderGraphBuilder.ReadComputeBuffer(in input);
				passData.lightClusterDebugCS = m_RenderPipelineRayTracingResources.lightClusterDebugCS;
				passData.lightClusterDebugKernel = passData.lightClusterDebugCS.FindKernel("DebugLightCluster");
				passData.debugMaterial = m_DebugMaterial;
				passData.depthStencilBuffer = renderGraphBuilder.UseDepthBuffer(in depthStencilBuffer, DepthAccess.Read);
				passData.depthPyramid = renderGraphBuilder.ReadTexture(in depthStencilBuffer);
				LightClusterDebugPassData lightClusterDebugPassData2 = passData;
				TextureDesc desc = new TextureDesc(Vector2.one, dynamicResolution: true, xrReady: true)
				{
					colorFormat = GraphicsFormat.R16G16B16A16_SFloat,
					enableRandomWrite = true,
					name = "Light Cluster Debug Texture"
				};
				TextureHandle input2 = renderGraph.CreateTexture(in desc);
				lightClusterDebugPassData2.outputBuffer = renderGraphBuilder.WriteTexture(in input2);
				renderGraphBuilder.SetRenderFunc(delegate(LightClusterDebugPassData data, RenderGraphContext ctx)
				{
					MaterialPropertyBlock tempMaterialPropertyBlock = ctx.renderGraphPool.GetTempMaterialPropertyBlock();
					CoreUtils.SetRenderTarget(ctx.cmd, data.outputBuffer, data.depthStencilBuffer, ClearFlag.Color, Color.black);
					ctx.cmd.SetComputeBufferParam(data.lightClusterDebugCS, data.lightClusterDebugKernel, HDShaderIDs._RaytracingLightCluster, data.lightCluster);
					ctx.cmd.SetComputeVectorParam(data.lightClusterDebugCS, _ClusterCellSize, data.clusterCellSize);
					ctx.cmd.SetComputeTextureParam(data.lightClusterDebugCS, data.lightClusterDebugKernel, HDShaderIDs._CameraDepthTexture, data.depthStencilBuffer);
					ctx.cmd.SetComputeTextureParam(data.lightClusterDebugCS, data.lightClusterDebugKernel, _DebutLightClusterTexture, data.outputBuffer);
					int num = 8;
					int threadGroupsX = (data.texWidth + (num - 1)) / num;
					int threadGroupsY = (data.texHeight + (num - 1)) / num;
					ctx.cmd.DispatchCompute(data.lightClusterDebugCS, data.lightClusterDebugKernel, threadGroupsX, threadGroupsY, 1);
					tempMaterialPropertyBlock.SetBuffer(HDShaderIDs._RaytracingLightCluster, data.lightCluster);
					tempMaterialPropertyBlock.SetVector(_ClusterCellSize, data.clusterCellSize);
					tempMaterialPropertyBlock.SetTexture(HDShaderIDs._CameraDepthTexture, data.depthPyramid);
					ctx.cmd.DrawProcedural(Matrix4x4.identity, data.debugMaterial, 1, MeshTopology.Lines, 48, 131072, tempMaterialPropertyBlock);
					ctx.cmd.DrawProcedural(Matrix4x4.identity, data.debugMaterial, 0, MeshTopology.Triangles, 36, 131072, tempMaterialPropertyBlock);
				});
				outputBuffer = passData.outputBuffer;
			}
			finally
			{
				((IDisposable)renderGraphBuilder).Dispose();
			}
			m_RenderPipeline.PushFullScreenDebugTexture(renderGraph, outputBuffer, FullScreenDebugMode.LightCluster);
		}

		public ComputeBuffer GetCluster()
		{
			return m_LightCluster;
		}

		public ComputeBuffer GetLightDatas()
		{
			return m_LightDataGPUArray;
		}

		public ComputeBuffer GetEnvLightDatas()
		{
			return m_EnvLightDataGPUArray;
		}

		public Vector3 GetMinClusterPos()
		{
			return minClusterPos;
		}

		public Vector3 GetMaxClusterPos()
		{
			return maxClusterPos;
		}

		public Vector3 GetClusterCellSize()
		{
			return clusterCellSize;
		}

		public int GetPunctualLightCount()
		{
			return punctualLightCount;
		}

		public int GetAreaLightCount()
		{
			return areaLightCount;
		}

		public int GetEnvLightCount()
		{
			return envLightCount;
		}

		public int GetLightPerCellCount()
		{
			return m_NumLightsPerCell;
		}

		private void InvalidateCluster()
		{
			minClusterPos.Set(float.MaxValue, float.MaxValue, float.MaxValue);
			maxClusterPos.Set(float.MinValue, float.MinValue, float.MinValue);
			punctualLightCount = 0;
			areaLightCount = 0;
			envLightCount = 0;
		}

		public void CullForRayTracing(HDCamera hdCamera, HDRayTracingLights rayTracingLights)
		{
			if (rayTracingLights.lightCount == 0 || !m_RenderPipeline.GetRayTracingState())
			{
				InvalidateCluster();
				return;
			}
			BuildGPULightVolumes(hdCamera, rayTracingLights);
			if (totalLightCount == 0)
			{
				InvalidateCluster();
			}
			else
			{
				EvaluateClusterVolume(hdCamera);
			}
		}

		public void BuildLightClusterBuffer(CommandBuffer cmd, HDCamera hdCamera, HDRayTracingLights rayTracingLights)
		{
			if (totalLightCount != 0 && rayTracingLights.lightCount != 0 && m_RenderPipeline.GetRayTracingState())
			{
				CullLights(cmd);
				BuildLightCluster(hdCamera, cmd);
			}
		}

		public void ReserveCookieAtlasSlots(HDRayTracingLights rayTracingLights)
		{
			HDLightRenderDatabase instance = HDLightRenderDatabase.instance;
			for (int i = 0; i < rayTracingLights.hdLightEntityArray.Count; i++)
			{
				int entityDataIndex = instance.GetEntityDataIndex(rayTracingLights.hdLightEntityArray[i]);
				HDAdditionalLightData hDAdditionalLightData = instance.hdAdditionalLightData[entityDataIndex];
				hDAdditionalLightData.gameObject.TryGetComponent<Light>(out lightComponent);
				m_RenderPipeline.ReserveCookieAtlasTexture(hDAdditionalLightData, lightComponent, hDAdditionalLightData.type);
			}
		}

		public void BuildRayTracingLightData(CommandBuffer cmd, HDCamera hdCamera, HDRayTracingLights rayTracingLights, DebugDisplaySettings debugDisplaySettings)
		{
			BuildLightData(cmd, hdCamera, rayTracingLights, debugDisplaySettings);
			BuildEnvLightData(cmd, hdCamera, rayTracingLights);
		}

		public void BindLightClusterData(CommandBuffer cmd)
		{
			ConstantBuffer.PushGlobal(cmd, in m_EnvLightReflectionDataRT, HDShaderIDs._EnvLightReflectionDataRT);
			cmd.SetGlobalBuffer(HDShaderIDs._RaytracingLightCluster, GetCluster());
			cmd.SetGlobalBuffer(HDShaderIDs._LightDatasRT, GetLightDatas());
			cmd.SetGlobalBuffer(HDShaderIDs._EnvLightDatasRT, GetEnvLightDatas());
		}
	}
}
