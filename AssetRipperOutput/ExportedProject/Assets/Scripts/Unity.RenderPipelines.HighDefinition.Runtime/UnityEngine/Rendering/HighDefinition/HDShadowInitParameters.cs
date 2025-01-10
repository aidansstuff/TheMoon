using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public struct HDShadowInitParameters
	{
		[Serializable]
		public struct HDShadowAtlasInitParams
		{
			public int shadowAtlasResolution;

			public DepthBits shadowAtlasDepthBits;

			public bool useDynamicViewportRescale;

			internal static HDShadowAtlasInitParams GetDefault()
			{
				HDShadowAtlasInitParams result = default(HDShadowAtlasInitParams);
				result.shadowAtlasResolution = 4096;
				result.shadowAtlasDepthBits = DepthBits.Depth32;
				result.useDynamicViewportRescale = true;
				return result;
			}
		}

		internal const int k_DefaultShadowAtlasResolution = 4096;

		internal const int k_DefaultMaxShadowRequests = 128;

		internal const DepthBits k_DefaultShadowMapDepthBits = DepthBits.Depth32;

		public int maxShadowRequests;

		public DepthBits directionalShadowsDepthBits;

		[FormerlySerializedAs("shadowQuality")]
		public HDShadowFilteringQuality shadowFilteringQuality;

		public HDAreaShadowFilteringQuality areaShadowFilteringQuality;

		public HDShadowAtlasInitParams punctualLightShadowAtlas;

		public HDShadowAtlasInitParams areaLightShadowAtlas;

		public int cachedPunctualLightShadowAtlas;

		public int cachedAreaLightShadowAtlas;

		public bool allowDirectionalMixedCachedShadows;

		public IntScalableSetting shadowResolutionDirectional;

		public IntScalableSetting shadowResolutionPunctual;

		public IntScalableSetting shadowResolutionArea;

		public int maxDirectionalShadowMapResolution;

		public int maxPunctualShadowMapResolution;

		public int maxAreaShadowMapResolution;

		public bool supportScreenSpaceShadows;

		public int maxScreenSpaceShadowSlots;

		public ScreenSpaceShadowFormat screenSpaceShadowBufferFormat;

		internal static HDShadowInitParameters NewDefault()
		{
			HDShadowInitParameters result = default(HDShadowInitParameters);
			result.maxShadowRequests = 128;
			result.directionalShadowsDepthBits = DepthBits.Depth32;
			result.punctualLightShadowAtlas = HDShadowAtlasInitParams.GetDefault();
			result.areaLightShadowAtlas = HDShadowAtlasInitParams.GetDefault();
			result.cachedPunctualLightShadowAtlas = 2048;
			result.cachedAreaLightShadowAtlas = 1024;
			result.allowDirectionalMixedCachedShadows = false;
			result.shadowResolutionDirectional = new IntScalableSetting(new int[4] { 256, 512, 1024, 2048 }, ScalableSettingSchemaId.With4Levels);
			result.shadowResolutionArea = new IntScalableSetting(new int[4] { 256, 512, 1024, 2048 }, ScalableSettingSchemaId.With4Levels);
			result.shadowResolutionPunctual = new IntScalableSetting(new int[4] { 256, 512, 1024, 2048 }, ScalableSettingSchemaId.With4Levels);
			result.shadowFilteringQuality = HDShadowFilteringQuality.Medium;
			result.areaShadowFilteringQuality = HDAreaShadowFilteringQuality.Medium;
			result.supportScreenSpaceShadows = false;
			result.maxScreenSpaceShadowSlots = 4;
			result.screenSpaceShadowBufferFormat = ScreenSpaceShadowFormat.R16G16B16A16;
			result.maxDirectionalShadowMapResolution = 2048;
			result.maxAreaShadowMapResolution = 2048;
			result.maxPunctualShadowMapResolution = 2048;
			return result;
		}
	}
}
