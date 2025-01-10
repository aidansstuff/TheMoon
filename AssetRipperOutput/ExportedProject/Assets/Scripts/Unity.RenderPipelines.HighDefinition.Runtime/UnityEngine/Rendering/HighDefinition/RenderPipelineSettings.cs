using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public struct RenderPipelineSettings
	{
		public enum SupportedLitShaderMode
		{
			ForwardOnly = 1,
			DeferredOnly = 2,
			Both = 3
		}

		public enum LightProbeSystem
		{
			[InspectorName("Light Probe Groups")]
			LegacyLightProbes = 0,
			ProbeVolumes = 1
		}

		public enum ColorBufferFormat
		{
			R11G11B10 = 74,
			R16G16B16A16 = 48
		}

		public enum CustomBufferFormat
		{
			[InspectorName("Signed R8G8B8A8")]
			SignedR8G8B8A8 = 12,
			R8G8B8A8 = 8,
			R16G16B16A16 = 48,
			R11G11B10 = 74
		}

		public enum SupportedRayTracingMode
		{
			Performance = 1,
			Quality = 2,
			Both = 3
		}

		[Serializable]
		public struct LightSettings
		{
			public BoolScalableSetting useContactShadow;

			internal static LightSettings NewDefault()
			{
				LightSettings result = default(LightSettings);
				result.useContactShadow = new BoolScalableSetting(new bool[3] { false, false, true }, ScalableSettingSchemaId.With3Levels);
				return result;
			}
		}

		[Serializable]
		public class PlanarReflectionAtlasResolutionScalableSetting : ScalableSetting<PlanarReflectionAtlasResolution>
		{
			public PlanarReflectionAtlasResolutionScalableSetting(PlanarReflectionAtlasResolution[] values, ScalableSettingSchemaId schemaId)
				: base(values, schemaId)
			{
			}
		}

		[Serializable]
		public class ReflectionProbeResolutionScalableSetting : ScalableSetting<CubeReflectionResolution>
		{
			public ReflectionProbeResolutionScalableSetting(CubeReflectionResolution[] values, ScalableSettingSchemaId schemaId)
				: base(values, schemaId)
			{
			}
		}

		public bool supportShadowMask;

		public bool supportSSR;

		public bool supportSSRTransparent;

		public bool supportSSAO;

		public bool supportSSGI;

		public bool supportSubsurfaceScattering;

		public IntScalableSetting sssSampleBudget;

		public bool supportVolumetrics;

		public bool supportVolumetricClouds;

		public bool supportLightLayers;

		public bool supportWater;

		public WaterSimulationResolution waterSimulationResolution;

		public bool waterCPUSimulation;

		public bool supportDistortion;

		public bool supportTransparentBackface;

		public bool supportTransparentDepthPrepass;

		public bool supportTransparentDepthPostpass;

		public ColorBufferFormat colorBufferFormat;

		public bool supportCustomPass;

		public CustomBufferFormat customBufferFormat;

		public SupportedLitShaderMode supportedLitShaderMode;

		public PlanarReflectionAtlasResolutionScalableSetting planarReflectionResolution;

		public ReflectionProbeResolutionScalableSetting cubeReflectionResolution;

		public bool supportDecals;

		public bool supportDecalLayers;

		public bool supportSurfaceGradient;

		public bool decalNormalBufferHP;

		public MSAASamples msaaSampleCount;

		public bool supportMotionVectors;

		public bool supportDataDrivenLensFlare;

		public bool supportRuntimeAOVAPI;

		public bool supportDitheringCrossFade;

		public bool supportTerrainHole;

		public LightProbeSystem lightProbeSystem;

		public ProbeVolumeTextureMemoryBudget probeVolumeMemoryBudget;

		public ProbeVolumeBlendingTextureMemoryBudget probeVolumeBlendingMemoryBudget;

		public bool supportProbeVolumeStreaming;

		public ProbeVolumeSHBands probeVolumeSHBands;

		public bool supportRayTracing;

		public SupportedRayTracingMode supportedRayTracingMode;

		public GlobalLightLoopSettings lightLoopSettings;

		public HDShadowInitParameters hdShadowInitParams;

		public GlobalDecalSettings decalSettings;

		public GlobalPostProcessSettings postProcessSettings;

		public GlobalDynamicResolutionSettings dynamicResolutionSettings;

		public GlobalLowResolutionTransparencySettings lowresTransparentSettings;

		public GlobalXRSettings xrSettings;

		public GlobalPostProcessingQualitySettings postProcessQualitySettings;

		public LightSettings lightSettings;

		public IntScalableSetting maximumLODLevel;

		public FloatScalableSetting lodBias;

		public GlobalLightingQualitySettings lightingQualitySettings;

		[Obsolete("For data migration")]
		internal bool m_ObsoleteincreaseSssSampleCount;

		[SerializeField]
		[FormerlySerializedAs("lightLayerName0")]
		[Obsolete("Moved to HDGlobal Settings")]
		internal string m_ObsoleteLightLayerName0;

		[SerializeField]
		[FormerlySerializedAs("lightLayerName1")]
		[Obsolete("Moved to HDGlobal Settings")]
		internal string m_ObsoleteLightLayerName1;

		[SerializeField]
		[FormerlySerializedAs("lightLayerName2")]
		[Obsolete("Moved to HDGlobal Settings")]
		internal string m_ObsoleteLightLayerName2;

		[SerializeField]
		[FormerlySerializedAs("lightLayerName3")]
		[Obsolete("Moved to HDGlobal Settings")]
		internal string m_ObsoleteLightLayerName3;

		[SerializeField]
		[FormerlySerializedAs("lightLayerName4")]
		[Obsolete("Moved to HDGlobal Settings")]
		internal string m_ObsoleteLightLayerName4;

		[SerializeField]
		[FormerlySerializedAs("lightLayerName5")]
		[Obsolete("Moved to HDGlobal Settings")]
		internal string m_ObsoleteLightLayerName5;

		[SerializeField]
		[FormerlySerializedAs("lightLayerName6")]
		[Obsolete("Moved to HDGlobal Settings")]
		internal string m_ObsoleteLightLayerName6;

		[SerializeField]
		[FormerlySerializedAs("lightLayerName7")]
		[Obsolete("Moved to HDGlobal Settings")]
		internal string m_ObsoleteLightLayerName7;

		[SerializeField]
		[FormerlySerializedAs("decalLayerName0")]
		[Obsolete("Moved to HDGlobal Settings")]
		internal string m_ObsoleteDecalLayerName0;

		[SerializeField]
		[FormerlySerializedAs("decalLayerName1")]
		[Obsolete("Moved to HDGlobal Settings")]
		internal string m_ObsoleteDecalLayerName1;

		[SerializeField]
		[FormerlySerializedAs("decalLayerName2")]
		[Obsolete("Moved to HDGlobal Settings")]
		internal string m_ObsoleteDecalLayerName2;

		[SerializeField]
		[FormerlySerializedAs("decalLayerName3")]
		[Obsolete("Moved to HDGlobal Settings")]
		internal string m_ObsoleteDecalLayerName3;

		[SerializeField]
		[FormerlySerializedAs("decalLayerName4")]
		[Obsolete("Moved to HDGlobal Settings")]
		internal string m_ObsoleteDecalLayerName4;

		[SerializeField]
		[FormerlySerializedAs("decalLayerName5")]
		[Obsolete("Moved to HDGlobal Settings")]
		internal string m_ObsoleteDecalLayerName5;

		[SerializeField]
		[FormerlySerializedAs("decalLayerName6")]
		[Obsolete("Moved to HDGlobal Settings")]
		internal string m_ObsoleteDecalLayerName6;

		[SerializeField]
		[FormerlySerializedAs("decalLayerName7")]
		[Obsolete("Moved to HDGlobal Settings")]
		internal string m_ObsoleteDecalLayerName7;

		[SerializeField]
		[FormerlySerializedAs("supportRuntimeDebugDisplay")]
		[Obsolete("Moved to HDGlobal Settings")]
		internal bool m_ObsoleteSupportRuntimeDebugDisplay;

		public string lightLayerName0
		{
			get
			{
				return HDRenderPipelineGlobalSettings.instance.lightLayerName0;
			}
			set
			{
				HDRenderPipelineGlobalSettings.instance.lightLayerName0 = value;
			}
		}

		public string lightLayerName1
		{
			get
			{
				return HDRenderPipelineGlobalSettings.instance.lightLayerName1;
			}
			set
			{
				HDRenderPipelineGlobalSettings.instance.lightLayerName1 = value;
			}
		}

		public string lightLayerName2
		{
			get
			{
				return HDRenderPipelineGlobalSettings.instance.lightLayerName2;
			}
			set
			{
				HDRenderPipelineGlobalSettings.instance.lightLayerName2 = value;
			}
		}

		public string lightLayerName3
		{
			get
			{
				return HDRenderPipelineGlobalSettings.instance.lightLayerName3;
			}
			set
			{
				HDRenderPipelineGlobalSettings.instance.lightLayerName3 = value;
			}
		}

		public string lightLayerName4
		{
			get
			{
				return HDRenderPipelineGlobalSettings.instance.lightLayerName4;
			}
			set
			{
				HDRenderPipelineGlobalSettings.instance.lightLayerName4 = value;
			}
		}

		public string lightLayerName5
		{
			get
			{
				return HDRenderPipelineGlobalSettings.instance.lightLayerName5;
			}
			set
			{
				HDRenderPipelineGlobalSettings.instance.lightLayerName5 = value;
			}
		}

		public string lightLayerName6
		{
			get
			{
				return HDRenderPipelineGlobalSettings.instance.lightLayerName6;
			}
			set
			{
				HDRenderPipelineGlobalSettings.instance.lightLayerName6 = value;
			}
		}

		public string lightLayerName7
		{
			get
			{
				return HDRenderPipelineGlobalSettings.instance.lightLayerName7;
			}
			set
			{
				HDRenderPipelineGlobalSettings.instance.lightLayerName7 = value;
			}
		}

		public string decalLayerName0
		{
			get
			{
				return HDRenderPipelineGlobalSettings.instance.decalLayerName0;
			}
			set
			{
				HDRenderPipelineGlobalSettings.instance.decalLayerName0 = value;
			}
		}

		public string decalLayerName1
		{
			get
			{
				return HDRenderPipelineGlobalSettings.instance.decalLayerName1;
			}
			set
			{
				HDRenderPipelineGlobalSettings.instance.decalLayerName1 = value;
			}
		}

		public string decalLayerName2
		{
			get
			{
				return HDRenderPipelineGlobalSettings.instance.decalLayerName2;
			}
			set
			{
				HDRenderPipelineGlobalSettings.instance.decalLayerName2 = value;
			}
		}

		public string decalLayerName3
		{
			get
			{
				return HDRenderPipelineGlobalSettings.instance.decalLayerName3;
			}
			set
			{
				HDRenderPipelineGlobalSettings.instance.decalLayerName3 = value;
			}
		}

		public string decalLayerName4
		{
			get
			{
				return HDRenderPipelineGlobalSettings.instance.decalLayerName4;
			}
			set
			{
				HDRenderPipelineGlobalSettings.instance.decalLayerName4 = value;
			}
		}

		public string decalLayerName5
		{
			get
			{
				return HDRenderPipelineGlobalSettings.instance.decalLayerName5;
			}
			set
			{
				HDRenderPipelineGlobalSettings.instance.decalLayerName5 = value;
			}
		}

		public string decalLayerName6
		{
			get
			{
				return HDRenderPipelineGlobalSettings.instance.decalLayerName6;
			}
			set
			{
				HDRenderPipelineGlobalSettings.instance.decalLayerName6 = value;
			}
		}

		public string decalLayerName7
		{
			get
			{
				return HDRenderPipelineGlobalSettings.instance.decalLayerName7;
			}
			set
			{
				HDRenderPipelineGlobalSettings.instance.decalLayerName7 = value;
			}
		}

		[Obsolete]
		public bool supportMSAA => msaaSampleCount != MSAASamples.None;

		public bool supportRuntimeDebugDisplay
		{
			get
			{
				return HDRenderPipelineGlobalSettings.instance.supportRuntimeDebugDisplay;
			}
			set
			{
				HDRenderPipelineGlobalSettings.instance.supportRuntimeDebugDisplay = value;
			}
		}

		internal bool supportProbeVolume => lightProbeSystem == LightProbeSystem.ProbeVolumes;

		internal static RenderPipelineSettings NewDefault()
		{
			RenderPipelineSettings result = default(RenderPipelineSettings);
			result.supportShadowMask = true;
			result.supportSSAO = true;
			result.supportSubsurfaceScattering = true;
			result.sssSampleBudget = new IntScalableSetting(new int[3] { 20, 40, 80 }, ScalableSettingSchemaId.With3Levels);
			result.supportVolumetrics = true;
			result.supportDistortion = true;
			result.supportTransparentBackface = true;
			result.supportTransparentDepthPrepass = true;
			result.supportTransparentDepthPostpass = true;
			result.colorBufferFormat = ColorBufferFormat.R11G11B10;
			result.supportCustomPass = true;
			result.customBufferFormat = CustomBufferFormat.R8G8B8A8;
			result.supportedLitShaderMode = SupportedLitShaderMode.DeferredOnly;
			result.supportDecals = true;
			result.supportDecalLayers = false;
			result.supportSurfaceGradient = true;
			result.decalNormalBufferHP = false;
			result.msaaSampleCount = MSAASamples.None;
			result.supportMotionVectors = true;
			result.supportRuntimeAOVAPI = false;
			result.supportDitheringCrossFade = true;
			result.supportTerrainHole = false;
			result.supportWater = false;
			result.waterSimulationResolution = WaterSimulationResolution.Medium128;
			result.waterCPUSimulation = false;
			result.supportDataDrivenLensFlare = true;
			result.planarReflectionResolution = new PlanarReflectionAtlasResolutionScalableSetting(new PlanarReflectionAtlasResolution[3]
			{
				PlanarReflectionAtlasResolution.Resolution256,
				PlanarReflectionAtlasResolution.Resolution1024,
				PlanarReflectionAtlasResolution.Resolution2048
			}, ScalableSettingSchemaId.With3Levels);
			result.cubeReflectionResolution = new ReflectionProbeResolutionScalableSetting(new CubeReflectionResolution[3]
			{
				CubeReflectionResolution.CubeReflectionResolution128,
				CubeReflectionResolution.CubeReflectionResolution256,
				CubeReflectionResolution.CubeReflectionResolution512
			}, ScalableSettingSchemaId.With3Levels);
			result.lightLoopSettings = GlobalLightLoopSettings.NewDefault();
			result.hdShadowInitParams = HDShadowInitParameters.NewDefault();
			result.decalSettings = GlobalDecalSettings.NewDefault();
			result.postProcessSettings = GlobalPostProcessSettings.NewDefault();
			result.dynamicResolutionSettings = GlobalDynamicResolutionSettings.NewDefault();
			result.lowresTransparentSettings = GlobalLowResolutionTransparencySettings.NewDefault();
			result.xrSettings = GlobalXRSettings.NewDefault();
			result.postProcessQualitySettings = GlobalPostProcessingQualitySettings.NewDefault();
			result.lightingQualitySettings = GlobalLightingQualitySettings.NewDefault();
			result.lightSettings = LightSettings.NewDefault();
			result.supportRayTracing = false;
			result.supportedRayTracingMode = SupportedRayTracingMode.Both;
			result.lodBias = new FloatScalableSetting(new float[3] { 1f, 1f, 1f }, ScalableSettingSchemaId.With3Levels);
			result.maximumLODLevel = new IntScalableSetting(new int[3], ScalableSettingSchemaId.With3Levels);
			result.lightProbeSystem = LightProbeSystem.LegacyLightProbes;
			result.probeVolumeMemoryBudget = ProbeVolumeTextureMemoryBudget.MemoryBudgetMedium;
			result.probeVolumeBlendingMemoryBudget = ProbeVolumeBlendingTextureMemoryBudget.MemoryBudgetLow;
			result.supportProbeVolumeStreaming = false;
			result.probeVolumeSHBands = ProbeVolumeSHBands.SphericalHarmonicsL1;
			return result;
		}

		internal bool SupportsAlpha()
		{
			if (!CoreUtils.IsSceneFilteringEnabled())
			{
				return colorBufferFormat == ColorBufferFormat.R16G16B16A16;
			}
			return true;
		}
	}
}
