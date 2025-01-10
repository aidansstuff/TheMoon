namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\ShaderLibrary\\ShaderVariablesGlobal.cs", needAccessors = false, generateCBuffer = true, constantRegister = 0)]
	internal struct ShaderVariablesGlobal
	{
		public const int RenderingLightLayersMask = 255;

		public const int RenderingLightLayersMaskShift = 0;

		public const int RenderingDecalLayersMask = 65280;

		public const int RenderingDecalLayersMaskShift = 8;

		public const int DefaultRenderingLayerMask = 257;

		public const int DefaultDecalLayers = 255;

		public Matrix4x4 _ViewMatrix;

		public Matrix4x4 _CameraViewMatrix;

		public Matrix4x4 _InvViewMatrix;

		public Matrix4x4 _ProjMatrix;

		public Matrix4x4 _InvProjMatrix;

		public Matrix4x4 _ViewProjMatrix;

		public Matrix4x4 _CameraViewProjMatrix;

		public Matrix4x4 _InvViewProjMatrix;

		public Matrix4x4 _NonJitteredViewProjMatrix;

		public Matrix4x4 _PrevViewProjMatrix;

		public Matrix4x4 _PrevInvViewProjMatrix;

		public Vector4 _WorldSpaceCameraPos_Internal;

		public Vector4 _PrevCamPosRWS_Internal;

		public Vector4 _ScreenSize;

		public Vector4 _PostProcessScreenSize;

		public Vector4 _RTHandleScale;

		public Vector4 _RTHandleScaleHistory;

		public Vector4 _RTHandlePostProcessScale;

		public Vector4 _RTHandlePostProcessScaleHistory;

		public Vector4 _DynamicResolutionFullscreenScale;

		public Vector4 _ZBufferParams;

		public Vector4 _ProjectionParams;

		public Vector4 unity_OrthoParams;

		public Vector4 _ScreenParams;

		[HLSLArray(6, typeof(Vector4))]
		public unsafe fixed float _FrustumPlanes[24];

		[HLSLArray(6, typeof(Vector4))]
		public unsafe fixed float _ShadowFrustumPlanes[24];

		public Vector4 _TaaFrameInfo;

		public Vector4 _TaaJitterStrength;

		public Vector4 _Time;

		public Vector4 _SinTime;

		public Vector4 _CosTime;

		public Vector4 unity_DeltaTime;

		public Vector4 _TimeParameters;

		public Vector4 _LastTimeParameters;

		public int _FogEnabled;

		public int _PBRFogEnabled;

		public int _EnableVolumetricFog;

		public float _MaxFogDistance;

		public Vector4 _FogColor;

		public float _FogColorMode;

		public float _GlobalMipBias;

		public float _GlobalMipBiasPow2;

		public float _Pad0;

		public Vector4 _MipFogParameters;

		public Vector4 _HeightFogBaseScattering;

		public float _HeightFogBaseExtinction;

		public float _HeightFogBaseHeight;

		public float _GlobalFogAnisotropy;

		public int _VolumetricFilteringEnabled;

		public Vector2 _HeightFogExponents;

		public int _FogDirectionalOnly;

		public float _FogGIDimmer;

		public Vector4 _VBufferViewportSize;

		public Vector4 _VBufferLightingViewportScale;

		public Vector4 _VBufferLightingViewportLimit;

		public Vector4 _VBufferDistanceEncodingParams;

		public Vector4 _VBufferDistanceDecodingParams;

		public uint _VBufferSliceCount;

		public float _VBufferRcpSliceCount;

		public float _VBufferRcpInstancedViewCount;

		public float _VBufferLastSliceDist;

		public Vector4 _ShadowAtlasSize;

		public Vector4 _CascadeShadowAtlasSize;

		public Vector4 _AreaShadowAtlasSize;

		public Vector4 _CachedShadowAtlasSize;

		public Vector4 _CachedAreaShadowAtlasSize;

		public int _ReflectionsMode;

		public int _UnusedPadding0;

		public int _UnusedPadding1;

		public int _UnusedPadding2;

		public uint _DirectionalLightCount;

		public uint _PunctualLightCount;

		public uint _AreaLightCount;

		public uint _EnvLightCount;

		public int _EnvLightSkyEnabled;

		public uint _CascadeShadowCount;

		public int _DirectionalShadowIndex;

		public uint _EnableLightLayers;

		public uint _EnableSkyReflection;

		public uint _EnableSSRefraction;

		public float _SSRefractionInvScreenWeightDistance;

		public float _ColorPyramidLodCount;

		public float _DirectionalTransmissionMultiplier;

		public float _ProbeExposureScale;

		public float _ContactShadowOpacity;

		public float _ReplaceDiffuseForIndirect;

		public Vector4 _AmbientOcclusionParam;

		public float _IndirectDiffuseLightingMultiplier;

		public uint _IndirectDiffuseLightingLayers;

		public float _ReflectionLightingMultiplier;

		public uint _ReflectionLightingLayers;

		public float _MicroShadowOpacity;

		public uint _EnableProbeVolumes;

		public uint _ProbeVolumeCount;

		public float _SlopeScaleDepthBias;

		public Vector4 _CookieAtlasSize;

		public Vector4 _CookieAtlasData;

		public Vector4 _ReflectionAtlasCubeData;

		public Vector4 _ReflectionAtlasPlanarData;

		public uint _NumTileFtplX;

		public uint _NumTileFtplY;

		public float g_fClustScale;

		public float g_fClustBase;

		public float g_fNearPlane;

		public float g_fFarPlane;

		public int g_iLog2NumClusters;

		public uint g_isLogBaseBufferEnabled;

		public uint _NumTileClusteredX;

		public uint _NumTileClusteredY;

		public int _EnvSliceSize;

		public uint _EnableDecalLayers;

		[HLSLArray(16, typeof(Vector4))]
		public unsafe fixed float _ShapeParamsAndMaxScatterDists[64];

		[HLSLArray(16, typeof(Vector4))]
		public unsafe fixed float _TransmissionTintsAndFresnel0[64];

		[HLSLArray(16, typeof(Vector4))]
		public unsafe fixed float _WorldScalesAndFilterRadiiAndThicknessRemaps[64];

		[HLSLArray(16, typeof(ShaderGenUInt4))]
		public unsafe fixed uint _DiffusionProfileHashTable[64];

		public uint _EnableSubsurfaceScattering;

		public uint _TexturingModeFlags;

		public uint _TransmissionFlags;

		public uint _DiffusionProfileCount;

		public Vector2 _DecalAtlasResolution;

		public uint _EnableDecals;

		public uint _DecalCount;

		public float _OffScreenDownsampleFactor;

		public uint _OffScreenRendering;

		public uint _XRViewCount;

		public int _FrameCount;

		public Vector4 _CoarseStencilBufferSize;

		public int _IndirectDiffuseMode;

		public int _EnableRayTracedReflections;

		public int _RaytracingFrameIndex;

		public uint _EnableRecursiveRayTracing;

		public int _TransparentCameraOnlyMotionVectors;

		public float _GlobalTessellationFactorMultiplier;

		public float _SpecularOcclusionBlend;

		public float _DeExposureMultiplier;

		public Vector4 _ScreenSizeOverride;

		public Vector4 _ScreenCoordScaleBias;
	}
}
