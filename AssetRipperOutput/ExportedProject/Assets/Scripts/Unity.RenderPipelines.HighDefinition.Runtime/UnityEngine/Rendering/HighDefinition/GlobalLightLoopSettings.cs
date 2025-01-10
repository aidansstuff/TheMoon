using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public struct GlobalLightLoopSettings
	{
		internal static readonly GlobalLightLoopSettings @default;

		[FormerlySerializedAs("cookieSize")]
		public CookieAtlasResolution cookieAtlasSize;

		public CookieAtlasGraphicsFormat cookieFormat;

		public int cookieAtlasLastValidMip;

		[SerializeField]
		[Obsolete("There is no more texture array for cookies, use cookie atlases properties instead.", false)]
		internal int cookieTexArraySize;

		[FormerlySerializedAs("planarReflectionTextureSize")]
		[SerializeField]
		[Obsolete("There is no more planar reflection atlas, use reflection probe atlases instead.", false)]
		public PlanarReflectionAtlasResolution planarReflectionAtlasSize;

		[SerializeField]
		[Obsolete("There is no more texture array for cube reflection probes, use reflection probe atlases properties instead.", false)]
		internal int reflectionProbeCacheSize;

		[SerializeField]
		[Obsolete("There is no more cube reflection probe size, use cube reflection probe size tiers instead.", false)]
		internal CubeReflectionResolution reflectionCubemapSize;

		[SerializeField]
		[Obsolete("There is no more max env light on screen, use max planar and cube reflection probes on screen instead.", false)]
		internal int maxEnvLightsOnScreen;

		public bool reflectionCacheCompressed;

		public ReflectionAndPlanarProbeFormat reflectionProbeFormat;

		public ReflectionProbeTextureCacheResolution reflectionProbeTexCacheSize;

		public int reflectionProbeTexLastValidCubeMip;

		public int reflectionProbeTexLastValidPlanarMip;

		public bool reflectionProbeDecreaseResToFit;

		public SkyResolution skyReflectionSize;

		public LayerMask skyLightingOverrideLayerMask;

		public bool supportFabricConvolution;

		public int maxDirectionalLightsOnScreen;

		public int maxPunctualLightsOnScreen;

		public int maxAreaLightsOnScreen;

		public int maxCubeReflectionOnScreen;

		public int maxPlanarReflectionOnScreen;

		public int maxDecalsOnScreen;

		public int maxLightsPerClusterCell;

		[Obsolete("The texture resolution limit in volumetric fogs have been removed. This field is unused.")]
		public LocalVolumetricFogResolution maxLocalVolumetricFogSize;

		[Range(1f, 1024f)]
		public int maxLocalVolumetricFogOnScreen;

		internal static GlobalLightLoopSettings NewDefault()
		{
			GlobalLightLoopSettings result = default(GlobalLightLoopSettings);
			result.cookieAtlasSize = CookieAtlasResolution.CookieResolution2048;
			result.cookieFormat = CookieAtlasGraphicsFormat.R11G11B10;
			result.cookieAtlasLastValidMip = 0;
			result.cookieTexArraySize = 1;
			result.reflectionProbeFormat = ReflectionAndPlanarProbeFormat.R11G11B10;
			result.reflectionProbeTexCacheSize = ReflectionProbeTextureCacheResolution.Resolution4096x4096;
			result.reflectionProbeTexLastValidCubeMip = 3;
			result.reflectionProbeTexLastValidPlanarMip = 0;
			result.reflectionProbeDecreaseResToFit = true;
			result.skyReflectionSize = SkyResolution.SkyResolution256;
			result.skyLightingOverrideLayerMask = 0;
			result.maxDirectionalLightsOnScreen = 16;
			result.maxPunctualLightsOnScreen = 512;
			result.maxAreaLightsOnScreen = 64;
			result.maxCubeReflectionOnScreen = 32;
			result.maxPlanarReflectionOnScreen = 8;
			result.maxDecalsOnScreen = 512;
			result.maxLightsPerClusterCell = 8;
			result.maxLocalVolumetricFogOnScreen = 256;
			return result;
		}

		internal static Vector2Int GetReflectionProbeTextureCacheDim(ReflectionProbeTextureCacheResolution resolution)
		{
			if (resolution <= ReflectionProbeTextureCacheResolution.Resolution16384x16384)
			{
				return new Vector2Int((int)resolution, (int)resolution);
			}
			return new Vector2Int((int)resolution >> 16, (int)(resolution & (ReflectionProbeTextureCacheResolution)65535));
		}
	}
}
