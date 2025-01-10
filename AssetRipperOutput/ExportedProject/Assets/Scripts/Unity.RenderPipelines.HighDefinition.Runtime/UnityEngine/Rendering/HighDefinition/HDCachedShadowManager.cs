using UnityEngine.Experimental.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.HighDefinition
{
	public class HDCachedShadowManager
	{
		private static HDCachedShadowManager s_Instance = new HDCachedShadowManager();

		private const int m_MaxShadowCascades = 4;

		private bool[] m_DirectionalShadowPendingUpdate = new bool[4];

		private bool[] m_DirectionalShadowHasRendered = new bool[4];

		private Vector3 m_CachedDirectionalForward;

		private Vector3 m_CachedDirectionalAngles;

		private bool m_AllowDirectionalMixedCached;

		internal const int k_MinSlotSize = 64;

		private (int, int)[] m_TempFilled = new(int, int)[6];

		internal HDCachedShadowAtlas punctualShadowAtlas;

		internal HDCachedShadowAtlas areaShadowAtlas;

		internal HDShadowAtlas directionalLightAtlas;

		private int m_DirectionalLightCacheSize = 1;

		private HDShadowInitParameters m_InitParams;

		public static HDCachedShadowManager instance => s_Instance;

		public bool WouldFitInAtlas(int shadowResolution, HDLightType lightType)
		{
			bool flag = true;
			int x = 0;
			int y = 0;
			if (lightType == HDLightType.Point)
			{
				int num = 0;
				for (int i = 0; i < 6; i++)
				{
					flag = flag && HDShadowManager.cachedShadowManager.punctualShadowAtlas.FindSlotInAtlas(shadowResolution, tempFill: true, out x, out y);
					if (flag)
					{
						m_TempFilled[num++] = (x, y);
						continue;
					}
					for (int j = 0; j < num; j++)
					{
						HDShadowManager.cachedShadowManager.punctualShadowAtlas.FreeTempFilled(m_TempFilled[j].Item1, m_TempFilled[j].Item2, shadowResolution);
					}
					return false;
				}
				for (int k = 0; k < num; k++)
				{
					HDShadowManager.cachedShadowManager.punctualShadowAtlas.FreeTempFilled(m_TempFilled[k].Item1, m_TempFilled[k].Item2, shadowResolution);
				}
			}
			if (lightType == HDLightType.Spot)
			{
				flag = flag && HDShadowManager.cachedShadowManager.punctualShadowAtlas.FindSlotInAtlas(shadowResolution, out x, out y);
			}
			if (lightType == HDLightType.Area)
			{
				flag = flag && HDShadowManager.cachedShadowManager.areaShadowAtlas.FindSlotInAtlas(shadowResolution, out x, out y);
			}
			return flag;
		}

		public bool WouldFitInAtlas(HDAdditionalLightData lightData)
		{
			if (lightData.legacyLight.shadows != 0)
			{
				HDLightType type = lightData.type;
				int resolutionFromSettings = lightData.GetResolutionFromSettings(lightData.GetShadowMapType(type), m_InitParams);
				return WouldFitInAtlas(resolutionFromSettings, type);
			}
			return false;
		}

		public void DefragAtlas(HDLightType lightType)
		{
			if (lightType == HDLightType.Area)
			{
				instance.areaShadowAtlas.DefragmentAtlasAndReRender(instance.m_InitParams);
			}
			if (lightType == HDLightType.Point || lightType == HDLightType.Spot)
			{
				instance.punctualShadowAtlas.DefragmentAtlasAndReRender(instance.m_InitParams);
			}
		}

		public void ForceEvictLight(HDAdditionalLightData lightData)
		{
			EvictLight(lightData);
			lightData.lightIdxForCachedShadows = -1;
		}

		public void ForceRegisterLight(HDAdditionalLightData lightData)
		{
			RegisterLight(lightData);
		}

		public bool LightHasBeenPlacedInAtlas(HDAdditionalLightData lightData)
		{
			switch (lightData.type)
			{
			case HDLightType.Area:
				return instance.areaShadowAtlas.LightIsPlaced(lightData);
			case HDLightType.Spot:
			case HDLightType.Point:
				return instance.punctualShadowAtlas.LightIsPlaced(lightData);
			case HDLightType.Directional:
				return !lightData.ShadowIsUpdatedEveryFrame();
			default:
				return false;
			}
		}

		public bool LightHasBeenPlaceAndRenderedAtLeastOnce(HDAdditionalLightData lightData, int numberOfCascades = 0)
		{
			switch (lightData.type)
			{
			case HDLightType.Area:
				if (instance.areaShadowAtlas.LightIsPlaced(lightData))
				{
					return instance.areaShadowAtlas.FullLightShadowHasRenderedAtLeastOnce(lightData);
				}
				return false;
			case HDLightType.Spot:
			case HDLightType.Point:
				if (instance.punctualShadowAtlas.LightIsPlaced(lightData))
				{
					return instance.punctualShadowAtlas.FullLightShadowHasRenderedAtLeastOnce(lightData);
				}
				return false;
			case HDLightType.Directional:
			{
				bool flag = true;
				for (int i = 0; i < numberOfCascades; i++)
				{
					flag = flag && m_DirectionalShadowHasRendered[i];
				}
				return !lightData.ShadowIsUpdatedEveryFrame() && flag;
			}
			default:
				return false;
			}
		}

		public bool ShadowHasBeenPlaceAndRenderedAtLeastOnce(HDAdditionalLightData lightData, int shadowIndex)
		{
			HDLightType type = lightData.type;
			switch (type)
			{
			case HDLightType.Area:
				if (instance.areaShadowAtlas.LightIsPlaced(lightData))
				{
					return instance.areaShadowAtlas.ShadowHasRenderedAtLeastOnce(lightData.lightIdxForCachedShadows);
				}
				return false;
			case HDLightType.Spot:
				if (instance.punctualShadowAtlas.LightIsPlaced(lightData))
				{
					return instance.punctualShadowAtlas.ShadowHasRenderedAtLeastOnce(lightData.lightIdxForCachedShadows);
				}
				return false;
			default:
				switch (type)
				{
				case HDLightType.Spot:
					break;
				case HDLightType.Directional:
					if (!lightData.ShadowIsUpdatedEveryFrame())
					{
						return m_DirectionalShadowHasRendered[shadowIndex];
					}
					return false;
				default:
					return false;
				}
				break;
			case HDLightType.Point:
				break;
			}
			_ = 2;
			if (instance.punctualShadowAtlas.LightIsPlaced(lightData))
			{
				return instance.punctualShadowAtlas.ShadowHasRenderedAtLeastOnce(lightData.lightIdxForCachedShadows + shadowIndex);
			}
			return false;
		}

		private void MarkAllDirectionalShadowsForUpdate()
		{
			for (int i = 0; i < 4; i++)
			{
				m_DirectionalShadowPendingUpdate[i] = true;
				m_DirectionalShadowHasRendered[i] = false;
			}
		}

		private HDCachedShadowManager()
		{
			punctualShadowAtlas = new HDCachedShadowAtlas(ShadowMapType.PunctualAtlas);
			if (ShaderConfig.s_AreaLights == 1)
			{
				areaShadowAtlas = new HDCachedShadowAtlas(ShadowMapType.AreaLightAtlas);
			}
			directionalLightAtlas = new HDShadowAtlas();
		}

		internal void InitDirectionalState(HDShadowAtlas.HDShadowAtlasInitParameters atlasInitParams, bool allowMixedCachedShadows)
		{
			m_AllowDirectionalMixedCached = allowMixedCachedShadows;
			if (m_AllowDirectionalMixedCached)
			{
				m_DirectionalLightCacheSize = atlasInitParams.width;
				atlasInitParams.isShadowCache = true;
				atlasInitParams.useSharedTexture = true;
				directionalLightAtlas.InitAtlas(atlasInitParams);
			}
		}

		internal void InitPunctualShadowAtlas(HDShadowAtlas.HDShadowAtlasInitParameters atlasInitParams)
		{
			m_InitParams = atlasInitParams.initParams;
			atlasInitParams.isShadowCache = true;
			punctualShadowAtlas.InitAtlas(atlasInitParams);
		}

		internal void InitAreaLightShadowAtlas(HDShadowAtlas.HDShadowAtlasInitParameters atlasInitParams)
		{
			m_InitParams = atlasInitParams.initParams;
			atlasInitParams.isShadowCache = true;
			areaShadowAtlas.InitAtlas(atlasInitParams);
		}

		internal bool DirectionalHasCachedAtlas()
		{
			return m_AllowDirectionalMixedCached;
		}

		internal void UpdateDirectionalCacheTexture(RenderGraph renderGraph)
		{
			TextureHandle outputTexture = directionalLightAtlas.GetOutputTexture(renderGraph);
			TextureDesc desc = directionalLightAtlas.GetAtlasDesc();
			if (m_DirectionalLightCacheSize != desc.width)
			{
				renderGraph.RefreshSharedTextureDesc(outputTexture, in desc);
				m_DirectionalLightCacheSize = desc.width;
			}
		}

		internal void RegisterLight(HDAdditionalLightData lightData)
		{
			if (lightData.legacyLight.bakingOutput.lightmapBakeType != LightmapBakeType.Baked)
			{
				HDLightType type = lightData.type;
				if (type == HDLightType.Directional)
				{
					lightData.lightIdxForCachedShadows = 0;
					MarkAllDirectionalShadowsForUpdate();
				}
				if (type == HDLightType.Spot || type == HDLightType.Point)
				{
					punctualShadowAtlas.RegisterLight(lightData);
				}
				if (ShaderConfig.s_AreaLights == 1 && type == HDLightType.Area && lightData.areaLightShape == AreaLightShape.Rectangle)
				{
					areaShadowAtlas.RegisterLight(lightData);
				}
			}
		}

		internal void EvictLight(HDAdditionalLightData lightData)
		{
			HDLightType type = lightData.type;
			if (type == HDLightType.Directional)
			{
				lightData.lightIdxForCachedShadows = -1;
				MarkAllDirectionalShadowsForUpdate();
			}
			if (type == HDLightType.Spot || type == HDLightType.Point)
			{
				punctualShadowAtlas.EvictLight(lightData);
			}
			if (ShaderConfig.s_AreaLights == 1 && type == HDLightType.Area)
			{
				areaShadowAtlas.EvictLight(lightData);
			}
		}

		internal void RegisterTransformToCache(HDAdditionalLightData lightData)
		{
			HDLightType type = lightData.type;
			if (type == HDLightType.Spot || type == HDLightType.Point)
			{
				punctualShadowAtlas.RegisterTransformCacheSlot(lightData);
			}
			if (ShaderConfig.s_AreaLights == 1 && type == HDLightType.Area)
			{
				areaShadowAtlas.RegisterTransformCacheSlot(lightData);
			}
			if (type == HDLightType.Directional)
			{
				m_CachedDirectionalAngles = lightData.transform.eulerAngles;
			}
		}

		internal void RemoveTransformFromCache(HDAdditionalLightData lightData)
		{
			HDLightType type = lightData.type;
			if (type == HDLightType.Spot || type == HDLightType.Point)
			{
				punctualShadowAtlas.RemoveTransformFromCache(lightData);
			}
			if (ShaderConfig.s_AreaLights == 1 && type == HDLightType.Area)
			{
				areaShadowAtlas.RemoveTransformFromCache(lightData);
			}
		}

		internal void AssignSlotsInAtlases()
		{
			punctualShadowAtlas.AssignOffsetsInAtlas(m_InitParams);
			if (ShaderConfig.s_AreaLights == 1)
			{
				areaShadowAtlas.AssignOffsetsInAtlas(m_InitParams);
			}
		}

		internal bool NeedRenderingDueToTransformChange(HDAdditionalLightData lightData, HDLightType lightType)
		{
			int num;
			if (lightData.updateUponLightMovement)
			{
				switch (lightType)
				{
				case HDLightType.Directional:
				{
					float cachedShadowAngleUpdateThreshold = lightData.cachedShadowAngleUpdateThreshold;
					Vector3 vector = m_CachedDirectionalAngles - lightData.transform.eulerAngles;
					if (!(Mathf.Abs(vector.x) > cachedShadowAngleUpdateThreshold) && !(Mathf.Abs(vector.y) > cachedShadowAngleUpdateThreshold))
					{
						num = ((Mathf.Abs(vector.z) > cachedShadowAngleUpdateThreshold) ? 1 : 0);
						if (num == 0)
						{
							goto IL_006e;
						}
					}
					else
					{
						num = 1;
					}
					m_CachedDirectionalAngles = lightData.transform.eulerAngles;
					goto IL_006e;
				}
				case HDLightType.Area:
					return areaShadowAtlas.NeedRenderingDueToTransformChange(lightData, lightType);
				default:
					{
						return punctualShadowAtlas.NeedRenderingDueToTransformChange(lightData, lightType);
					}
					IL_006e:
					return (byte)num != 0;
				}
			}
			return false;
		}

		internal bool ShadowIsPendingUpdate(int shadowIdx, ShadowMapType shadowMapType)
		{
			return shadowMapType switch
			{
				ShadowMapType.PunctualAtlas => punctualShadowAtlas.ShadowIsPendingRendering(shadowIdx), 
				ShadowMapType.AreaLightAtlas => areaShadowAtlas.ShadowIsPendingRendering(shadowIdx), 
				ShadowMapType.CascadedDirectional => m_DirectionalShadowPendingUpdate[shadowIdx], 
				_ => false, 
			};
		}

		internal void MarkShadowAsRendered(int shadowIdx, ShadowMapType shadowMapType)
		{
			if (shadowMapType == ShadowMapType.PunctualAtlas)
			{
				punctualShadowAtlas.MarkAsRendered(shadowIdx);
			}
			if (shadowMapType == ShadowMapType.AreaLightAtlas)
			{
				areaShadowAtlas.MarkAsRendered(shadowIdx);
			}
			if (shadowMapType == ShadowMapType.CascadedDirectional)
			{
				m_DirectionalShadowPendingUpdate[shadowIdx] = false;
				m_DirectionalShadowHasRendered[shadowIdx] = true;
			}
		}

		internal void UpdateResolutionRequest(ref HDShadowResolutionRequest request, int shadowIdx, ShadowMapType shadowMapType)
		{
			switch (shadowMapType)
			{
			case ShadowMapType.PunctualAtlas:
				punctualShadowAtlas.UpdateResolutionRequest(ref request, shadowIdx);
				break;
			case ShadowMapType.AreaLightAtlas:
				areaShadowAtlas.UpdateResolutionRequest(ref request, shadowIdx);
				break;
			case ShadowMapType.CascadedDirectional:
				request.cachedAtlasViewport = request.dynamicAtlasViewport;
				break;
			}
		}

		internal void UpdateDebugSettings(LightingDebugSettings lightingDebugSettings)
		{
			punctualShadowAtlas.UpdateDebugSettings(lightingDebugSettings);
			if (ShaderConfig.s_AreaLights == 1)
			{
				areaShadowAtlas.UpdateDebugSettings(lightingDebugSettings);
			}
			if (m_AllowDirectionalMixedCached)
			{
				directionalLightAtlas.UpdateDebugSettings(lightingDebugSettings);
			}
		}

		internal void ScheduleShadowUpdate(HDAdditionalLightData light)
		{
			switch (light.type)
			{
			case HDLightType.Spot:
			case HDLightType.Point:
				punctualShadowAtlas.ScheduleShadowUpdate(light);
				break;
			case HDLightType.Area:
				areaShadowAtlas.ScheduleShadowUpdate(light);
				break;
			case HDLightType.Directional:
				MarkAllDirectionalShadowsForUpdate();
				break;
			}
		}

		internal void ScheduleShadowUpdate(HDAdditionalLightData light, int subShadowIndex)
		{
			HDLightType type = light.type;
			if (type == HDLightType.Spot)
			{
				punctualShadowAtlas.ScheduleShadowUpdate(light);
			}
			if (type == HDLightType.Area)
			{
				areaShadowAtlas.ScheduleShadowUpdate(light);
			}
			if (type == HDLightType.Point)
			{
				punctualShadowAtlas.ScheduleShadowUpdate(light.lightIdxForCachedShadows + subShadowIndex);
			}
			if (type == HDLightType.Directional)
			{
				m_DirectionalShadowPendingUpdate[subShadowIndex] = true;
			}
		}

		internal bool LightIsPendingPlacement(HDAdditionalLightData light, ShadowMapType shadowMapType)
		{
			return shadowMapType switch
			{
				ShadowMapType.PunctualAtlas => punctualShadowAtlas.LightIsPendingPlacement(light), 
				ShadowMapType.AreaLightAtlas => areaShadowAtlas.LightIsPendingPlacement(light), 
				_ => false, 
			};
		}

		internal void ClearShadowRequests()
		{
			punctualShadowAtlas.Clear();
			if (ShaderConfig.s_AreaLights == 1)
			{
				areaShadowAtlas.Clear();
			}
			if (m_AllowDirectionalMixedCached)
			{
				directionalLightAtlas.Clear();
			}
		}

		internal void Cleanup(RenderGraph renderGraph)
		{
			if (m_AllowDirectionalMixedCached)
			{
				directionalLightAtlas.Release(renderGraph);
			}
			punctualShadowAtlas.Release(renderGraph);
			if (ShaderConfig.s_AreaLights == 1)
			{
				areaShadowAtlas.Release(renderGraph);
			}
		}
	}
}
