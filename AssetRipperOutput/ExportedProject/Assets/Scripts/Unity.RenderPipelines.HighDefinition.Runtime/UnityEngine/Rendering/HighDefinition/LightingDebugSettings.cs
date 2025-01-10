using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public class LightingDebugSettings
	{
		public DebugLightFilterMode debugLightFilterMode;

		public DebugLightingMode debugLightingMode;

		public bool debugLightLayers;

		public DebugLightLayersMask debugLightLayersFilterMask = (DebugLightLayersMask)(-1);

		public bool debugSelectionLightLayers;

		public bool debugSelectionShadowLayers;

		public Vector4[] debugRenderingLayersColors = GetDefaultRenderingLayersColorPalette();

		public ShadowMapDebugMode shadowDebugMode;

		public bool shadowDebugUseSelection;

		public uint shadowMapIndex;

		public float shadowMinValue;

		public float shadowMaxValue = 1f;

		public float shadowResolutionScaleFactor = 1f;

		public bool clearShadowAtlas;

		public bool overrideSmoothness;

		public float overrideSmoothnessValue = 0.5f;

		public bool overrideAlbedo;

		public Color overrideAlbedoValue = new Color(0.5f, 0.5f, 0.5f);

		public bool overrideNormal;

		public bool overrideAmbientOcclusion;

		public float overrideAmbientOcclusionValue = 1f;

		public bool overrideSpecularColor;

		public Color overrideSpecularColorValue = new Color(1f, 1f, 1f);

		public bool overrideEmissiveColor;

		public Color overrideEmissiveColorValue = new Color(1f, 1f, 1f);

		public bool displaySkyReflection;

		public float skyReflectionMipmap;

		public bool displayLightVolumes;

		public LightVolumeDebug lightVolumeDebugByCategory;

		public uint maxDebugLightCount = 24u;

		public ExposureDebugMode exposureDebugMode;

		public float debugExposure;

		[Obsolete("Please use the lens attenuation mode in HDRP Global Settings", true)]
		public float debugLensAttenuation = 0.65f;

		public bool showTonemapCurveAlongHistogramView = true;

		public bool centerHistogramAroundMiddleGrey;

		public bool displayFinalImageHistogramAsRGB;

		public bool displayMaskOnly;

		public bool displayOnSceneOverlay = true;

		public HDRDebugMode hdrDebugMode;

		public bool displayCookieAtlas;

		public bool displayCookieCubeArray;

		public uint cubeArraySliceIndex;

		public uint cookieAtlasMipLevel;

		public bool clearCookieAtlas;

		public bool displayReflectionProbeAtlas;

		public uint reflectionProbeMipLevel;

		public uint reflectionProbeSlice;

		public bool reflectionProbeApplyExposure;

		public bool clearReflectionProbeAtlas;

		public bool showPunctualLight = true;

		public bool showDirectionalLight = true;

		public bool showAreaLight = true;

		public bool showReflectionProbe = true;

		[Obsolete("The local volumetric fog atlas was removed. This field is unused.")]
		public bool displayLocalVolumetricFogAtlas;

		public uint localVolumetricFogAtlasSlice;

		public bool localVolumetricFogUseSelection;

		public TileClusterDebug tileClusterDebug;

		public TileClusterCategoryDebug tileClusterDebugByCategory = TileClusterCategoryDebug.Punctual;

		public ClusterDebugMode clusterDebugMode;

		public float clusterDebugDistance = 1f;

		public bool IsDebugDisplayEnabled()
		{
			if (debugLightingMode == DebugLightingMode.None && debugLightFilterMode == DebugLightFilterMode.None && !debugLightLayers && !overrideSmoothness && !overrideAlbedo && !overrideNormal && !overrideAmbientOcclusion && !overrideSpecularColor && !overrideEmissiveColor)
			{
				return shadowDebugMode == ShadowMapDebugMode.SingleShadow;
			}
			return true;
		}

		internal bool IsDebugDisplayRemovePostprocess()
		{
			if (debugLightingMode != DebugLightingMode.LuxMeter && debugLightingMode != DebugLightingMode.LuminanceMeter && debugLightingMode != DebugLightingMode.VisualizeShadowMasks && debugLightingMode != DebugLightingMode.IndirectDiffuseOcclusion && debugLightingMode != DebugLightingMode.IndirectSpecularOcclusion)
			{
				return debugLightingMode == DebugLightingMode.ProbeVolumeSampledSubdivision;
			}
			return true;
		}

		internal static Vector4[] GetDefaultRenderingLayersColorPalette()
		{
			Vector4[] array = new Vector4[32];
			Vector4[] array2 = new Vector4[8]
			{
				new Vector4(230f, 159f, 0f) / 255f,
				new Vector4(86f, 180f, 233f) / 255f,
				new Vector4(255f, 182f, 291f) / 255f,
				new Vector4(0f, 158f, 115f) / 255f,
				new Vector4(240f, 228f, 66f) / 255f,
				new Vector4(0f, 114f, 178f) / 255f,
				new Vector4(213f, 94f, 0f) / 255f,
				new Vector4(170f, 68f, 170f) / 255f
			};
			int i;
			for (i = 0; i < array2.Length; i++)
			{
				array[i] = array2[i];
			}
			for (; i < array.Length; i++)
			{
				array[i] = new Vector4(0f, 0f, 0f);
			}
			return array;
		}
	}
}
