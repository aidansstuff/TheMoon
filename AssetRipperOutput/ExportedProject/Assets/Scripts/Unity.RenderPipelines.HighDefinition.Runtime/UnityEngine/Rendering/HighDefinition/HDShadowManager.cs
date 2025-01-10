using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.Experimental.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class HDShadowManager
	{
		private class BindShadowGlobalResourcesPassData
		{
			public ShadowResult shadowResult;
		}

		public const int k_DirectionalShadowCascadeCount = 4;

		public const int k_MinShadowMapResolution = 16;

		public const int k_MaxShadowMapResolution = 16384;

		private List<HDShadowData> m_ShadowDatas = new List<HDShadowData>();

		private HDShadowRequest[] m_ShadowRequests;

		private HDShadowResolutionRequest[] m_ShadowResolutionRequests;

		private HDDirectionalShadowData[] m_CachedDirectionalShadowData;

		private HDDirectionalShadowData m_DirectionalShadowData;

		private ComputeBuffer m_ShadowDataBuffer;

		private ComputeBuffer m_DirectionalShadowDataBuffer;

		private HDDynamicShadowAtlas m_CascadeAtlas;

		private HDDynamicShadowAtlas m_Atlas;

		private HDDynamicShadowAtlas m_AreaLightShadowAtlas;

		private int m_MaxShadowRequests;

		private int m_ShadowRequestCount;

		private int m_CascadeCount;

		private int m_ShadowResolutionRequestCounter;

		private Material m_ClearShadowMaterial;

		private Material m_BlitShadowMaterial;

		private ConstantBuffer<ShaderVariablesGlobal> m_GlobalShaderVariables;

		public static HDCachedShadowManager cachedShadowManager => HDCachedShadowManager.instance;

		public void InitShadowManager(HDRenderPipelineRuntimeResources renderPipelineResources, HDShadowInitParameters initParams, RenderGraph renderGraph, Shader clearShader)
		{
			m_ShadowDataBuffer = new ComputeBuffer(Mathf.Max(initParams.maxShadowRequests, 1), Marshal.SizeOf(typeof(HDShadowData)));
			m_DirectionalShadowDataBuffer = new ComputeBuffer(1, Marshal.SizeOf(typeof(HDDirectionalShadowData)));
			m_MaxShadowRequests = initParams.maxShadowRequests;
			m_ShadowRequestCount = 0;
			if (initParams.maxShadowRequests != 0)
			{
				m_ClearShadowMaterial = CoreUtils.CreateEngineMaterial(clearShader);
				m_BlitShadowMaterial = CoreUtils.CreateEngineMaterial(renderPipelineResources.shaders.shadowBlitPS);
				m_ShadowDatas.Capacity = Math.Max(initParams.maxShadowRequests, m_ShadowDatas.Capacity);
				m_ShadowResolutionRequests = new HDShadowResolutionRequest[initParams.maxShadowRequests];
				m_ShadowRequests = new HDShadowRequest[initParams.maxShadowRequests];
				m_CachedDirectionalShadowData = new HDDirectionalShadowData[1];
				m_GlobalShaderVariables = new ConstantBuffer<ShaderVariablesGlobal>();
				for (int i = 0; i < initParams.maxShadowRequests; i++)
				{
					m_ShadowResolutionRequests[i] = new HDShadowResolutionRequest();
				}
				HDShadowAtlas.HDShadowAtlasInitParameters hDShadowAtlasInitParameters = new HDShadowAtlas.HDShadowAtlasInitParameters(renderPipelineResources, renderGraph, useSharedTexture: false, initParams.punctualLightShadowAtlas.shadowAtlasResolution, initParams.punctualLightShadowAtlas.shadowAtlasResolution, HDShaderIDs._ShadowmapAtlas, m_ClearShadowMaterial, initParams.maxShadowRequests, initParams, m_GlobalShaderVariables);
				hDShadowAtlasInitParameters.name = "Shadow Map Atlas";
				m_Atlas = new HDDynamicShadowAtlas(hDShadowAtlasInitParameters);
				HDShadowAtlas.BlurAlgorithm blurAlgorithm = ((GetDirectionalShadowAlgorithm() == DirectionalShadowAlgorithm.IMS) ? HDShadowAtlas.BlurAlgorithm.IM : HDShadowAtlas.BlurAlgorithm.None);
				HDShadowAtlas.HDShadowAtlasInitParameters hDShadowAtlasInitParameters2 = hDShadowAtlasInitParameters;
				hDShadowAtlasInitParameters2.useSharedTexture = true;
				hDShadowAtlasInitParameters2.width = 1;
				hDShadowAtlasInitParameters2.height = 1;
				hDShadowAtlasInitParameters2.atlasShaderID = HDShaderIDs._ShadowmapCascadeAtlas;
				hDShadowAtlasInitParameters2.blurAlgorithm = blurAlgorithm;
				hDShadowAtlasInitParameters2.depthBufferBits = initParams.directionalShadowsDepthBits;
				hDShadowAtlasInitParameters2.name = "Cascade Shadow Map Atlas";
				m_CascadeAtlas = new HDDynamicShadowAtlas(hDShadowAtlasInitParameters2);
				HDShadowAtlas.HDShadowAtlasInitParameters hDShadowAtlasInitParameters3 = hDShadowAtlasInitParameters;
				if (ShaderConfig.s_AreaLights == 1)
				{
					hDShadowAtlasInitParameters3.useSharedTexture = false;
					hDShadowAtlasInitParameters3.width = initParams.areaLightShadowAtlas.shadowAtlasResolution;
					hDShadowAtlasInitParameters3.height = initParams.areaLightShadowAtlas.shadowAtlasResolution;
					hDShadowAtlasInitParameters3.atlasShaderID = HDShaderIDs._ShadowmapAreaAtlas;
					hDShadowAtlasInitParameters3.blurAlgorithm = GetAreaLightShadowBlurAlgorithm();
					hDShadowAtlasInitParameters3.depthBufferBits = initParams.areaLightShadowAtlas.shadowAtlasDepthBits;
					hDShadowAtlasInitParameters3.name = "Area Light Shadow Map Atlas";
					m_AreaLightShadowAtlas = new HDDynamicShadowAtlas(hDShadowAtlasInitParameters3);
				}
				HDShadowAtlas.HDShadowAtlasInitParameters atlasInitParams = hDShadowAtlasInitParameters;
				atlasInitParams.useSharedTexture = true;
				atlasInitParams.width = initParams.cachedPunctualLightShadowAtlas;
				atlasInitParams.height = initParams.cachedPunctualLightShadowAtlas;
				atlasInitParams.atlasShaderID = HDShaderIDs._CachedShadowmapAtlas;
				atlasInitParams.name = "Cached Shadow Map Atlas";
				atlasInitParams.isShadowCache = true;
				cachedShadowManager.InitPunctualShadowAtlas(atlasInitParams);
				if (ShaderConfig.s_AreaLights == 1)
				{
					HDShadowAtlas.HDShadowAtlasInitParameters atlasInitParams2 = hDShadowAtlasInitParameters3;
					atlasInitParams2.useSharedTexture = true;
					atlasInitParams2.width = initParams.cachedAreaLightShadowAtlas;
					atlasInitParams2.height = initParams.cachedAreaLightShadowAtlas;
					atlasInitParams2.atlasShaderID = HDShaderIDs._CachedAreaLightShadowmapAtlas;
					atlasInitParams2.name = "Cached Area Light Shadow Map Atlas";
					atlasInitParams2.isShadowCache = true;
					cachedShadowManager.InitAreaLightShadowAtlas(atlasInitParams2);
				}
				cachedShadowManager.InitDirectionalState(hDShadowAtlasInitParameters2, initParams.allowDirectionalMixedCachedShadows);
			}
		}

		public void Cleanup(RenderGraph renderGraph)
		{
			m_ShadowDataBuffer.Dispose();
			m_DirectionalShadowDataBuffer.Dispose();
			if (m_MaxShadowRequests != 0)
			{
				m_Atlas.Release(renderGraph);
				if (ShaderConfig.s_AreaLights == 1)
				{
					m_AreaLightShadowAtlas.Release(renderGraph);
				}
				m_CascadeAtlas.Release(renderGraph);
				CoreUtils.Destroy(m_ClearShadowMaterial);
				cachedShadowManager.Cleanup(renderGraph);
				m_GlobalShaderVariables.Release();
			}
		}

		public static DirectionalShadowAlgorithm GetDirectionalShadowAlgorithm()
		{
			return HDRenderPipeline.currentAsset.currentPlatformRenderPipelineSettings.hdShadowInitParams.shadowFilteringQuality switch
			{
				HDShadowFilteringQuality.Low => DirectionalShadowAlgorithm.PCF5x5, 
				HDShadowFilteringQuality.Medium => DirectionalShadowAlgorithm.PCF7x7, 
				HDShadowFilteringQuality.High => DirectionalShadowAlgorithm.PCSS, 
				_ => DirectionalShadowAlgorithm.PCF5x5, 
			};
		}

		public static HDShadowAtlas.BlurAlgorithm GetAreaLightShadowBlurAlgorithm()
		{
			if (HDRenderPipeline.currentAsset.currentPlatformRenderPipelineSettings.hdShadowInitParams.areaShadowFilteringQuality != HDAreaShadowFilteringQuality.High)
			{
				return HDShadowAtlas.BlurAlgorithm.EVSM;
			}
			return HDShadowAtlas.BlurAlgorithm.None;
		}

		public void UpdateShaderVariablesGlobalCB(ref ShaderVariablesGlobal cb)
		{
			if (m_MaxShadowRequests != 0)
			{
				cb._CascadeShadowCount = (uint)(m_CascadeCount + 1);
				cb._ShadowAtlasSize = new Vector4(m_Atlas.width, m_Atlas.height, 1f / (float)m_Atlas.width, 1f / (float)m_Atlas.height);
				cb._CascadeShadowAtlasSize = new Vector4(m_CascadeAtlas.width, m_CascadeAtlas.height, 1f / (float)m_CascadeAtlas.width, 1f / (float)m_CascadeAtlas.height);
				cb._CachedShadowAtlasSize = new Vector4(cachedShadowManager.punctualShadowAtlas.width, cachedShadowManager.punctualShadowAtlas.height, 1f / (float)cachedShadowManager.punctualShadowAtlas.width, 1f / (float)cachedShadowManager.punctualShadowAtlas.height);
				if (ShaderConfig.s_AreaLights == 1)
				{
					cb._AreaShadowAtlasSize = new Vector4(m_AreaLightShadowAtlas.width, m_AreaLightShadowAtlas.height, 1f / (float)m_AreaLightShadowAtlas.width, 1f / (float)m_AreaLightShadowAtlas.height);
					cb._CachedAreaShadowAtlasSize = new Vector4(cachedShadowManager.areaShadowAtlas.width, cachedShadowManager.areaShadowAtlas.height, 1f / (float)cachedShadowManager.areaShadowAtlas.width, 1f / (float)cachedShadowManager.areaShadowAtlas.height);
				}
			}
		}

		public void UpdateDirectionalShadowResolution(int resolution, int cascadeCount)
		{
			Vector2Int size = new Vector2Int(resolution, resolution);
			if (cascadeCount > 1)
			{
				size.x *= 2;
			}
			if (cascadeCount > 2)
			{
				size.y *= 2;
			}
			m_CascadeAtlas.UpdateSize(size);
			if (cachedShadowManager.DirectionalHasCachedAtlas())
			{
				cachedShadowManager.directionalLightAtlas.UpdateSize(size);
			}
		}

		internal int ReserveShadowResolutions(Vector2 resolution, ShadowMapType shadowMapType, int lightID, int index, ShadowMapUpdateType updateType)
		{
			if (m_ShadowRequestCount >= m_MaxShadowRequests)
			{
				Debug.LogWarning("Max shadow requests count reached, dropping all exceeding requests. You can increase this limit by changing the Maximum Shadows on Screen property in the HDRP asset.");
				return -1;
			}
			m_ShadowResolutionRequests[m_ShadowResolutionRequestCounter].shadowMapType = shadowMapType;
			if (updateType != ShadowMapUpdateType.Cached || shadowMapType == ShadowMapType.CascadedDirectional)
			{
				m_ShadowResolutionRequests[m_ShadowResolutionRequestCounter].resolution = resolution;
				m_ShadowResolutionRequests[m_ShadowResolutionRequestCounter].dynamicAtlasViewport.width = resolution.x;
				m_ShadowResolutionRequests[m_ShadowResolutionRequestCounter].dynamicAtlasViewport.height = resolution.y;
				switch (shadowMapType)
				{
				case ShadowMapType.PunctualAtlas:
					m_Atlas.ReserveResolution(m_ShadowResolutionRequests[m_ShadowResolutionRequestCounter]);
					break;
				case ShadowMapType.AreaLightAtlas:
					m_AreaLightShadowAtlas.ReserveResolution(m_ShadowResolutionRequests[m_ShadowResolutionRequestCounter]);
					break;
				case ShadowMapType.CascadedDirectional:
					m_CascadeAtlas.ReserveResolution(m_ShadowResolutionRequests[m_ShadowResolutionRequestCounter]);
					break;
				}
			}
			m_ShadowResolutionRequestCounter++;
			m_ShadowRequestCount = m_ShadowResolutionRequestCounter;
			return m_ShadowResolutionRequestCounter - 1;
		}

		internal HDShadowResolutionRequest GetResolutionRequest(int index)
		{
			if (index < 0 || index >= m_ShadowRequestCount)
			{
				return null;
			}
			return m_ShadowResolutionRequests[index];
		}

		public Vector2 GetReservedResolution(int index)
		{
			if (index < 0 || index >= m_ShadowRequestCount)
			{
				return Vector2.zero;
			}
			return m_ShadowResolutionRequests[index].resolution;
		}

		internal void UpdateShadowRequest(int index, HDShadowRequest shadowRequest, ShadowMapUpdateType updateType)
		{
			if (index >= m_ShadowRequestCount)
			{
				return;
			}
			m_ShadowRequests[index] = shadowRequest;
			bool flag = updateType == ShadowMapUpdateType.Cached || updateType == ShadowMapUpdateType.Mixed;
			bool flag2 = updateType == ShadowMapUpdateType.Dynamic || updateType == ShadowMapUpdateType.Mixed;
			switch (shadowRequest.shadowMapType)
			{
			case ShadowMapType.PunctualAtlas:
				if (flag)
				{
					cachedShadowManager.punctualShadowAtlas.AddShadowRequest(shadowRequest);
				}
				if (flag2)
				{
					m_Atlas.AddShadowRequest(shadowRequest);
					if (updateType == ShadowMapUpdateType.Mixed)
					{
						m_Atlas.AddRequestToPendingBlitFromCache(shadowRequest);
					}
				}
				break;
			case ShadowMapType.CascadedDirectional:
				if (updateType == ShadowMapUpdateType.Mixed && cachedShadowManager.DirectionalHasCachedAtlas())
				{
					cachedShadowManager.directionalLightAtlas.AddShadowRequest(shadowRequest);
					m_CascadeAtlas.AddRequestToPendingBlitFromCache(shadowRequest);
				}
				m_CascadeAtlas.AddShadowRequest(shadowRequest);
				break;
			case ShadowMapType.AreaLightAtlas:
				if (flag)
				{
					cachedShadowManager.areaShadowAtlas.AddShadowRequest(shadowRequest);
				}
				if (flag2)
				{
					m_AreaLightShadowAtlas.AddShadowRequest(shadowRequest);
					if (updateType == ShadowMapUpdateType.Mixed)
					{
						m_AreaLightShadowAtlas.AddRequestToPendingBlitFromCache(shadowRequest);
					}
				}
				break;
			}
		}

		public unsafe void UpdateCascade(int cascadeIndex, Vector4 cullingSphere, float border)
		{
			if (cullingSphere.w != float.NegativeInfinity)
			{
				cullingSphere.w *= cullingSphere.w;
			}
			m_CascadeCount = Mathf.Max(m_CascadeCount, cascadeIndex);
			fixed (float* ptr = m_DirectionalShadowData.sphereCascades)
			{
				*(Vector4*)((byte*)ptr + (nint)cascadeIndex * (nint)sizeof(Vector4)) = cullingSphere;
			}
			fixed (float* ptr = m_DirectionalShadowData.cascadeBorders)
			{
				ptr[cascadeIndex] = border;
			}
		}

		private HDShadowData CreateShadowData(HDShadowRequest shadowRequest, HDShadowAtlas atlas)
		{
			HDShadowData result = default(HDShadowData);
			Matrix4x4 deviceProjection = shadowRequest.deviceProjection;
			Matrix4x4 view = shadowRequest.view;
			result.proj = new Vector4(deviceProjection.m00, deviceProjection.m11, deviceProjection.m22, deviceProjection.m23);
			result.pos = shadowRequest.position;
			result.rot0 = new Vector3(view.m00, view.m01, view.m02);
			result.rot1 = new Vector3(view.m10, view.m11, view.m12);
			result.rot2 = new Vector3(view.m20, view.m21, view.m22);
			result.shadowToWorld = shadowRequest.shadowToWorld;
			result.cacheTranslationDelta = new Vector3(0f, 0f, 0f);
			Rect rect = (shadowRequest.isInCachedAtlas ? shadowRequest.cachedAtlasViewport : shadowRequest.dynamicAtlasViewport);
			float x = 1f / (float)atlas.width;
			float y = 1f / (float)atlas.height;
			result.atlasOffset = Vector2.Scale(new Vector2(x, y), new Vector2(rect.x, rect.y));
			result.shadowMapSize = new Vector4(rect.width, rect.height, 1f / rect.width, 1f / rect.height);
			result.normalBias = shadowRequest.normalBias;
			result.worldTexelSize = shadowRequest.worldTexelSize;
			result.shadowFilterParams0.x = shadowRequest.shadowSoftness;
			result.shadowFilterParams0.y = HDShadowUtils.Asfloat(shadowRequest.blockerSampleCount);
			result.shadowFilterParams0.z = HDShadowUtils.Asfloat(shadowRequest.filterSampleCount);
			result.shadowFilterParams0.w = shadowRequest.minFilterSize;
			result.zBufferParam = shadowRequest.zBufferParam;
			if (atlas.HasBlurredEVSM())
			{
				result.shadowFilterParams0 = shadowRequest.evsmParams;
			}
			result.isInCachedAtlas = (shadowRequest.isInCachedAtlas ? 1f : 0f);
			return result;
		}

		private unsafe Vector4 GetCascadeSphereAtIndex(int index)
		{
			fixed (float* ptr = m_DirectionalShadowData.sphereCascades)
			{
				return *(Vector4*)((byte*)ptr + (nint)index * (nint)sizeof(Vector4));
			}
		}

		public void UpdateCullingParameters(ref ScriptableCullingParameters cullingParams, float maxShadowDistance)
		{
			cullingParams.shadowDistance = Mathf.Min(maxShadowDistance, cullingParams.shadowDistance);
		}

		public void LayoutShadowMaps(LightingDebugSettings lightingDebugSettings)
		{
			if (m_MaxShadowRequests == 0)
			{
				return;
			}
			cachedShadowManager.UpdateDebugSettings(lightingDebugSettings);
			m_Atlas.UpdateDebugSettings(lightingDebugSettings);
			if (m_CascadeAtlas != null)
			{
				m_CascadeAtlas.UpdateDebugSettings(lightingDebugSettings);
			}
			if (ShaderConfig.s_AreaLights == 1)
			{
				m_AreaLightShadowAtlas.UpdateDebugSettings(lightingDebugSettings);
			}
			if (lightingDebugSettings.shadowResolutionScaleFactor != 1f)
			{
				HDShadowResolutionRequest[] shadowResolutionRequests = m_ShadowResolutionRequests;
				foreach (HDShadowResolutionRequest hDShadowResolutionRequest in shadowResolutionRequests)
				{
					if (hDShadowResolutionRequest.shadowMapType != 0)
					{
						hDShadowResolutionRequest.resolution *= lightingDebugSettings.shadowResolutionScaleFactor;
					}
				}
			}
			if (m_CascadeAtlas != null && !m_CascadeAtlas.Layout(allowResize: false))
			{
				Debug.LogError("Cascade Shadow atlasing has failed, only one directional light can cast shadows at a time");
			}
			m_Atlas.Layout();
			if (ShaderConfig.s_AreaLights == 1)
			{
				m_AreaLightShadowAtlas.Layout();
			}
		}

		public unsafe void PrepareGPUShadowDatas(CullingResults cullResults, HDCamera camera)
		{
			if (m_MaxShadowRequests == 0)
			{
				return;
			}
			int num = 0;
			m_ShadowDatas.Clear();
			for (int i = 0; i < m_ShadowRequestCount; i++)
			{
				HDShadowAtlas atlas = m_Atlas;
				if (m_ShadowRequests[i].isInCachedAtlas)
				{
					atlas = cachedShadowManager.punctualShadowAtlas;
				}
				if (m_ShadowRequests[i].shadowMapType == ShadowMapType.CascadedDirectional)
				{
					atlas = m_CascadeAtlas;
				}
				else if (m_ShadowRequests[i].shadowMapType == ShadowMapType.AreaLightAtlas)
				{
					atlas = m_AreaLightShadowAtlas;
					if (m_ShadowRequests[i].isInCachedAtlas)
					{
						atlas = cachedShadowManager.areaShadowAtlas;
					}
				}
				HDShadowData hDShadowData;
				if (m_ShadowRequests[i].shouldUseCachedShadowData)
				{
					hDShadowData = m_ShadowRequests[i].cachedShadowData;
				}
				else
				{
					hDShadowData = CreateShadowData(m_ShadowRequests[i], atlas);
					m_ShadowRequests[i].cachedShadowData = hDShadowData;
				}
				m_ShadowDatas.Add(hDShadowData);
				m_ShadowRequests[i].shadowIndex = num++;
			}
			int num2 = 4;
			int num3 = 4;
			fixed (float* ptr = m_DirectionalShadowData.sphereCascades)
			{
				Vector4* ptr2 = (Vector4*)ptr;
				for (int j = 0; j < 4; j++)
				{
					num2 = ((num2 == 4 && ptr2[j].w > 0f) ? j : num2);
					num3 = (((num3 == 4 || num3 == num2) && ptr2[j].w > 0f) ? j : num3);
				}
			}
			if (num3 != 4)
			{
				m_DirectionalShadowData.cascadeDirection = (GetCascadeSphereAtIndex(num3) - GetCascadeSphereAtIndex(num2)).normalized;
			}
			else
			{
				m_DirectionalShadowData.cascadeDirection = Vector4.zero;
			}
			m_DirectionalShadowData.cascadeDirection.w = camera.volumeStack.GetComponent<HDShadowSettings>().cascadeShadowSplitCount.value;
			if (m_ShadowRequestCount > 0)
			{
				m_ShadowDataBuffer.SetData(m_ShadowDatas);
				m_CachedDirectionalShadowData[0] = m_DirectionalShadowData;
				m_DirectionalShadowDataBuffer.SetData(m_CachedDirectionalShadowData);
			}
		}

		public void PushGlobalParameters(CommandBuffer cmd)
		{
			cmd.SetGlobalBuffer(HDShaderIDs._HDShadowDatas, m_ShadowDataBuffer);
			cmd.SetGlobalBuffer(HDShaderIDs._HDDirectionalShadowData, m_DirectionalShadowDataBuffer);
		}

		public int GetShadowRequestCount()
		{
			return m_ShadowRequestCount;
		}

		public void Clear()
		{
			if (m_MaxShadowRequests != 0)
			{
				m_Atlas.Clear();
				m_CascadeAtlas.Clear();
				if (ShaderConfig.s_AreaLights == 1)
				{
					m_AreaLightShadowAtlas.Clear();
				}
				cachedShadowManager.ClearShadowRequests();
				m_ShadowResolutionRequestCounter = 0;
				m_ShadowRequestCount = 0;
				m_CascadeCount = 0;
			}
		}

		public void DisplayShadowAtlas(RTHandle atlasTexture, CommandBuffer cmd, Material debugMaterial, float screenX, float screenY, float screenSizeX, float screenSizeY, float minValue, float maxValue, MaterialPropertyBlock mpb)
		{
			m_Atlas.DisplayAtlas(atlasTexture, cmd, debugMaterial, new Rect(0f, 0f, m_Atlas.width, m_Atlas.height), screenX, screenY, screenSizeX, screenSizeY, minValue, maxValue, mpb);
		}

		public void DisplayShadowCascadeAtlas(RTHandle atlasTexture, CommandBuffer cmd, Material debugMaterial, float screenX, float screenY, float screenSizeX, float screenSizeY, float minValue, float maxValue, MaterialPropertyBlock mpb)
		{
			m_CascadeAtlas.DisplayAtlas(atlasTexture, cmd, debugMaterial, new Rect(0f, 0f, m_CascadeAtlas.width, m_CascadeAtlas.height), screenX, screenY, screenSizeX, screenSizeY, minValue, maxValue, mpb);
		}

		public void DisplayAreaLightShadowAtlas(RTHandle atlasTexture, CommandBuffer cmd, Material debugMaterial, float screenX, float screenY, float screenSizeX, float screenSizeY, float minValue, float maxValue, MaterialPropertyBlock mpb)
		{
			if (ShaderConfig.s_AreaLights == 1)
			{
				m_AreaLightShadowAtlas.DisplayAtlas(atlasTexture, cmd, debugMaterial, new Rect(0f, 0f, m_AreaLightShadowAtlas.width, m_AreaLightShadowAtlas.height), screenX, screenY, screenSizeX, screenSizeY, minValue, maxValue, mpb);
			}
		}

		public void DisplayCachedPunctualShadowAtlas(RTHandle atlasTexture, CommandBuffer cmd, Material debugMaterial, float screenX, float screenY, float screenSizeX, float screenSizeY, float minValue, float maxValue, MaterialPropertyBlock mpb)
		{
			cachedShadowManager.punctualShadowAtlas.DisplayAtlas(atlasTexture, cmd, debugMaterial, new Rect(0f, 0f, cachedShadowManager.punctualShadowAtlas.width, cachedShadowManager.punctualShadowAtlas.height), screenX, screenY, screenSizeX, screenSizeY, minValue, maxValue, mpb);
		}

		public void DisplayCachedAreaShadowAtlas(RTHandle atlasTexture, CommandBuffer cmd, Material debugMaterial, float screenX, float screenY, float screenSizeX, float screenSizeY, float minValue, float maxValue, MaterialPropertyBlock mpb)
		{
			if (ShaderConfig.s_AreaLights == 1)
			{
				cachedShadowManager.areaShadowAtlas.DisplayAtlas(atlasTexture, cmd, debugMaterial, new Rect(0f, 0f, cachedShadowManager.areaShadowAtlas.width, cachedShadowManager.areaShadowAtlas.height), screenX, screenY, screenSizeX, screenSizeY, minValue, maxValue, mpb);
			}
		}

		public void DisplayShadowMap(in ShadowResult atlasTextures, int shadowIndex, CommandBuffer cmd, Material debugMaterial, float screenX, float screenY, float screenSizeX, float screenSizeY, float minValue, float maxValue, MaterialPropertyBlock mpb)
		{
			if (shadowIndex >= m_ShadowRequestCount)
			{
				return;
			}
			HDShadowRequest hDShadowRequest = m_ShadowRequests[shadowIndex];
			switch (hDShadowRequest.shadowMapType)
			{
			case ShadowMapType.PunctualAtlas:
				if (hDShadowRequest.isInCachedAtlas)
				{
					cachedShadowManager.punctualShadowAtlas.DisplayAtlas(atlasTextures.cachedPunctualShadowResult, cmd, debugMaterial, hDShadowRequest.cachedAtlasViewport, screenX, screenY, screenSizeX, screenSizeY, minValue, maxValue, mpb);
				}
				else
				{
					m_Atlas.DisplayAtlas(atlasTextures.punctualShadowResult, cmd, debugMaterial, hDShadowRequest.dynamicAtlasViewport, screenX, screenY, screenSizeX, screenSizeY, minValue, maxValue, mpb);
				}
				break;
			case ShadowMapType.CascadedDirectional:
				m_CascadeAtlas.DisplayAtlas(atlasTextures.directionalShadowResult, cmd, debugMaterial, hDShadowRequest.dynamicAtlasViewport, screenX, screenY, screenSizeX, screenSizeY, minValue, maxValue, mpb);
				break;
			case ShadowMapType.AreaLightAtlas:
				if (ShaderConfig.s_AreaLights == 1)
				{
					if (hDShadowRequest.isInCachedAtlas)
					{
						cachedShadowManager.areaShadowAtlas.DisplayAtlas(atlasTextures.cachedAreaShadowResult, cmd, debugMaterial, hDShadowRequest.cachedAtlasViewport, screenX, screenY, screenSizeX, screenSizeY, minValue, maxValue, mpb);
					}
					else
					{
						m_AreaLightShadowAtlas.DisplayAtlas(atlasTextures.areaShadowResult, cmd, debugMaterial, hDShadowRequest.dynamicAtlasViewport, screenX, screenY, screenSizeX, screenSizeY, minValue, maxValue, mpb);
					}
				}
				break;
			}
		}

		internal static ShadowResult ReadShadowResult(in ShadowResult shadowResult, RenderGraphBuilder builder)
		{
			ShadowResult result = default(ShadowResult);
			if (shadowResult.punctualShadowResult.IsValid())
			{
				result.punctualShadowResult = builder.ReadTexture(in shadowResult.punctualShadowResult);
			}
			if (shadowResult.directionalShadowResult.IsValid())
			{
				result.directionalShadowResult = builder.ReadTexture(in shadowResult.directionalShadowResult);
			}
			if (shadowResult.areaShadowResult.IsValid())
			{
				result.areaShadowResult = builder.ReadTexture(in shadowResult.areaShadowResult);
			}
			if (shadowResult.cachedPunctualShadowResult.IsValid())
			{
				result.cachedPunctualShadowResult = builder.ReadTexture(in shadowResult.cachedPunctualShadowResult);
			}
			if (shadowResult.cachedAreaShadowResult.IsValid())
			{
				result.cachedAreaShadowResult = builder.ReadTexture(in shadowResult.cachedAreaShadowResult);
			}
			return result;
		}

		internal void RenderShadows(RenderGraph renderGraph, in ShaderVariablesGlobal globalCB, HDCamera hdCamera, CullingResults cullResults, ref ShadowResult result)
		{
			InvalidateAtlasOutputsIfNeeded();
			if (m_ShadowRequestCount != 0 && (hdCamera.frameSettings.IsEnabled(FrameSettingsField.OpaqueObjects) || hdCamera.frameSettings.IsEnabled(FrameSettingsField.TransparentObjects)))
			{
				result.cachedPunctualShadowResult = cachedShadowManager.punctualShadowAtlas.RenderShadows(renderGraph, cullResults, in globalCB, hdCamera.frameSettings, "Cached Punctual Lights Shadows rendering");
				cachedShadowManager.punctualShadowAtlas.AddBlitRequestsForUpdatedShadows(m_Atlas);
				BlitCachedShadows(renderGraph, ShadowMapType.PunctualAtlas);
				result.punctualShadowResult = m_Atlas.RenderShadows(renderGraph, cullResults, in globalCB, hdCamera.frameSettings, "Punctual Lights Shadows rendering");
				if (ShaderConfig.s_AreaLights == 1)
				{
					cachedShadowManager.areaShadowAtlas.RenderShadowMaps(renderGraph, cullResults, in globalCB, hdCamera.frameSettings, "Cached Area Lights Shadows rendering");
					cachedShadowManager.areaShadowAtlas.AddBlitRequestsForUpdatedShadows(m_AreaLightShadowAtlas);
					BlitCachedShadows(renderGraph, ShadowMapType.AreaLightAtlas);
					m_AreaLightShadowAtlas.RenderShadowMaps(renderGraph, cullResults, in globalCB, hdCamera.frameSettings, "Area Light Shadows rendering");
					result.areaShadowResult = m_AreaLightShadowAtlas.BlurShadows(renderGraph);
					result.cachedAreaShadowResult = cachedShadowManager.areaShadowAtlas.BlurShadows(renderGraph);
				}
				if (cachedShadowManager.DirectionalHasCachedAtlas())
				{
					if (cachedShadowManager.directionalLightAtlas.HasShadowRequests())
					{
						cachedShadowManager.UpdateDirectionalCacheTexture(renderGraph);
						cachedShadowManager.directionalLightAtlas.RenderShadows(renderGraph, cullResults, in globalCB, hdCamera.frameSettings, "Cached Directional Lights Shadows rendering");
						cachedShadowManager.directionalLightAtlas.AddBlitRequestsForUpdatedShadows(m_CascadeAtlas);
					}
					BlitCachedShadows(renderGraph, ShadowMapType.CascadedDirectional);
				}
				result.directionalShadowResult = m_CascadeAtlas.RenderShadows(renderGraph, cullResults, in globalCB, hdCamera.frameSettings, "Directional Light Shadows rendering");
			}
			BindShadowGlobalResources(renderGraph, in result);
		}

		internal void ReleaseSharedShadowAtlases(RenderGraph renderGraph)
		{
			if (cachedShadowManager.DirectionalHasCachedAtlas())
			{
				cachedShadowManager.directionalLightAtlas.CleanupRenderGraphOutput(renderGraph);
			}
			cachedShadowManager.punctualShadowAtlas.CleanupRenderGraphOutput(renderGraph);
			if (ShaderConfig.s_AreaLights == 1)
			{
				cachedShadowManager.areaShadowAtlas.CleanupRenderGraphOutput(renderGraph);
			}
			cachedShadowManager.DefragAtlas(HDLightType.Point);
			cachedShadowManager.DefragAtlas(HDLightType.Spot);
			if (ShaderConfig.s_AreaLights == 1)
			{
				cachedShadowManager.DefragAtlas(HDLightType.Area);
			}
		}

		private void InvalidateAtlasOutputsIfNeeded()
		{
			cachedShadowManager.punctualShadowAtlas.InvalidateOutputIfNeeded();
			m_Atlas.InvalidateOutputIfNeeded();
			m_CascadeAtlas.InvalidateOutputIfNeeded();
			if (cachedShadowManager.DirectionalHasCachedAtlas())
			{
				cachedShadowManager.directionalLightAtlas.InvalidateOutputIfNeeded();
			}
			if (ShaderConfig.s_AreaLights == 1)
			{
				cachedShadowManager.areaShadowAtlas.InvalidateOutputIfNeeded();
				m_AreaLightShadowAtlas.InvalidateOutputIfNeeded();
			}
		}

		private static void BindAtlasTexture(RenderGraphContext ctx, TextureHandle texture, int shaderId)
		{
			if (texture.IsValid())
			{
				ctx.cmd.SetGlobalTexture(shaderId, texture);
			}
			else
			{
				ctx.cmd.SetGlobalTexture(shaderId, ctx.defaultResources.defaultShadowTexture);
			}
		}

		private void BindShadowGlobalResources(RenderGraph renderGraph, in ShadowResult shadowResult)
		{
			BindShadowGlobalResourcesPassData passData;
			using RenderGraphBuilder builder = renderGraph.AddRenderPass<BindShadowGlobalResourcesPassData>("BindShadowGlobalResources", out passData);
			passData.shadowResult = ReadShadowResult(in shadowResult, builder);
			builder.AllowPassCulling(value: false);
			builder.SetRenderFunc(delegate(BindShadowGlobalResourcesPassData data, RenderGraphContext ctx)
			{
				BindAtlasTexture(ctx, data.shadowResult.punctualShadowResult, HDShaderIDs._ShadowmapAtlas);
				BindAtlasTexture(ctx, data.shadowResult.directionalShadowResult, HDShaderIDs._ShadowmapCascadeAtlas);
				BindAtlasTexture(ctx, data.shadowResult.areaShadowResult, HDShaderIDs._ShadowmapAreaAtlas);
				BindAtlasTexture(ctx, data.shadowResult.cachedPunctualShadowResult, HDShaderIDs._CachedShadowmapAtlas);
				BindAtlasTexture(ctx, data.shadowResult.cachedAreaShadowResult, HDShaderIDs._CachedAreaLightShadowmapAtlas);
			});
		}

		internal static void BindDefaultShadowGlobalResources(RenderGraph renderGraph)
		{
			BindShadowGlobalResourcesPassData passData;
			using RenderGraphBuilder renderGraphBuilder = renderGraph.AddRenderPass<BindShadowGlobalResourcesPassData>("BindDefaultShadowGlobalResources", out passData);
			renderGraphBuilder.AllowPassCulling(value: false);
			renderGraphBuilder.SetRenderFunc(delegate(BindShadowGlobalResourcesPassData data, RenderGraphContext ctx)
			{
				BindAtlasTexture(ctx, ctx.defaultResources.defaultShadowTexture, HDShaderIDs._ShadowmapAtlas);
				BindAtlasTexture(ctx, ctx.defaultResources.defaultShadowTexture, HDShaderIDs._ShadowmapCascadeAtlas);
				BindAtlasTexture(ctx, ctx.defaultResources.defaultShadowTexture, HDShaderIDs._ShadowmapAreaAtlas);
				BindAtlasTexture(ctx, ctx.defaultResources.defaultShadowTexture, HDShaderIDs._CachedShadowmapAtlas);
				BindAtlasTexture(ctx, ctx.defaultResources.defaultShadowTexture, HDShaderIDs._CachedAreaLightShadowmapAtlas);
			});
		}

		private void BlitCachedShadows(RenderGraph renderGraph)
		{
			m_Atlas.BlitCachedIntoAtlas(renderGraph, cachedShadowManager.punctualShadowAtlas.GetOutputTexture(renderGraph), cachedShadowManager.punctualShadowAtlas.width, m_BlitShadowMaterial, "Blit Punctual Mixed Cached Shadows", HDProfileId.BlitPunctualMixedCachedShadowMaps);
			if (cachedShadowManager.DirectionalHasCachedAtlas())
			{
				m_CascadeAtlas.BlitCachedIntoAtlas(renderGraph, cachedShadowManager.directionalLightAtlas.GetOutputTexture(renderGraph), cachedShadowManager.directionalLightAtlas.width, m_BlitShadowMaterial, "Blit Directional Mixed Cached Shadows", HDProfileId.BlitDirectionalMixedCachedShadowMaps);
			}
			if (ShaderConfig.s_AreaLights == 1)
			{
				m_AreaLightShadowAtlas.BlitCachedIntoAtlas(renderGraph, cachedShadowManager.areaShadowAtlas.GetOutputTexture(renderGraph), cachedShadowManager.areaShadowAtlas.width, m_BlitShadowMaterial, "Blit Area Mixed Cached Shadows", HDProfileId.BlitAreaMixedCachedShadowMaps);
			}
		}

		private void BlitCachedShadows(RenderGraph renderGraph, ShadowMapType shadowAtlas)
		{
			if (shadowAtlas == ShadowMapType.PunctualAtlas)
			{
				m_Atlas.BlitCachedIntoAtlas(renderGraph, cachedShadowManager.punctualShadowAtlas.GetOutputTexture(renderGraph), cachedShadowManager.punctualShadowAtlas.width, m_BlitShadowMaterial, "Blit Punctual Mixed Cached Shadows", HDProfileId.BlitPunctualMixedCachedShadowMaps);
			}
			if (shadowAtlas == ShadowMapType.CascadedDirectional && cachedShadowManager.DirectionalHasCachedAtlas())
			{
				m_CascadeAtlas.BlitCachedIntoAtlas(renderGraph, cachedShadowManager.directionalLightAtlas.GetOutputTexture(renderGraph), cachedShadowManager.directionalLightAtlas.width, m_BlitShadowMaterial, "Blit Directional Mixed Cached Shadows", HDProfileId.BlitDirectionalMixedCachedShadowMaps);
			}
			if (shadowAtlas == ShadowMapType.AreaLightAtlas && ShaderConfig.s_AreaLights == 1)
			{
				m_AreaLightShadowAtlas.BlitCachedIntoAtlas(renderGraph, cachedShadowManager.areaShadowAtlas.GetShadowMapDepthTexture(renderGraph), cachedShadowManager.areaShadowAtlas.width, m_BlitShadowMaterial, "Blit Area Mixed Cached Shadows", HDProfileId.BlitAreaMixedCachedShadowMaps);
			}
		}
	}
}
