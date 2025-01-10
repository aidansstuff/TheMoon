using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	public class HDRenderPipelineAsset : RenderPipelineAsset, IVirtualTexturingEnabledRenderPipeline, IVersionable<HDRenderPipelineAsset.Version>, IMigratableAsset
	{
		private enum Version
		{
			None = 0,
			First = 1,
			UpgradeFrameSettingsToStruct = 2,
			AddAfterPostProcessFrameSetting = 3,
			AddFrameSettingSpecularLighting = 5,
			AddReflectionSettings = 6,
			AddPostProcessFrameSettings = 7,
			AddRayTracingFrameSettings = 8,
			AddFrameSettingDirectSpecularLighting = 9,
			AddCustomPostprocessAndCustomPass = 10,
			ScalableSettingsRefactor = 11,
			ShadowFilteringVeryHighQualityRemoval = 12,
			SeparateColorGradingAndTonemappingFrameSettings = 13,
			ReplaceTextureArraysByAtlasForCookieAndPlanar = 14,
			AddedAdaptiveSSS = 15,
			RemoveCookieCubeAtlasToOctahedral2D = 16,
			RoughDistortion = 17,
			VirtualTexturing = 18,
			AddedHDRenderPipelineGlobalSettings = 19,
			DecalSurfaceGradient = 20,
			RemovalOfUpscaleFilter = 21,
			CombinedPlanarAndCubemapReflectionAtlases = 22
		}

		[NonSerialized]
		internal bool isInOnValidateCall;

		[SerializeField]
		[FormerlySerializedAs("renderPipelineSettings")]
		private RenderPipelineSettings m_RenderPipelineSettings = RenderPipelineSettings.NewDefault();

		[SerializeField]
		internal bool allowShaderVariantStripping = true;

		[SerializeField]
		internal bool enableSRPBatcher = true;

		[FormerlySerializedAs("materialQualityLevels")]
		public MaterialQuality availableMaterialQualityLevels = (MaterialQuality)(-1);

		[SerializeField]
		[FormerlySerializedAs("m_CurrentMaterialQualityLevel")]
		private MaterialQuality m_DefaultMaterialQualityLevel = MaterialQuality.High;

		[SerializeField]
		[Obsolete("Use HDRP Global Settings' diffusionProfileSettingsList instead")]
		internal DiffusionProfileSettings diffusionProfileSettings;

		[SerializeField]
		internal VirtualTexturingSettingsSRP virtualTexturingSettings = new VirtualTexturingSettingsSRP();

		[SerializeField]
		private bool m_UseRenderGraph = true;

		private static readonly MigrationDescription<Version, HDRenderPipelineAsset> k_Migration = MigrationDescription.New<Version, HDRenderPipelineAsset>(MigrationStep.New(Version.UpgradeFrameSettingsToStruct, delegate(HDRenderPipelineAsset data)
		{
			FrameSettingsOverrideMask newFrameSettingsOverrideMask = default(FrameSettingsOverrideMask);
			if (data.m_ObsoleteFrameSettings != null)
			{
				FrameSettings.MigrateFromClassVersion(ref data.m_ObsoleteFrameSettings, ref data.m_ObsoleteFrameSettingsMovedToDefaultSettings, ref newFrameSettingsOverrideMask);
			}
			if (data.m_ObsoleteBakedOrCustomReflectionFrameSettings != null)
			{
				FrameSettings.MigrateFromClassVersion(ref data.m_ObsoleteBakedOrCustomReflectionFrameSettings, ref data.m_ObsoleteBakedOrCustomReflectionFrameSettingsMovedToDefaultSettings, ref newFrameSettingsOverrideMask);
			}
			if (data.m_ObsoleteRealtimeReflectionFrameSettings != null)
			{
				FrameSettings.MigrateFromClassVersion(ref data.m_ObsoleteRealtimeReflectionFrameSettings, ref data.m_ObsoleteRealtimeReflectionFrameSettingsMovedToDefaultSettings, ref newFrameSettingsOverrideMask);
			}
		}), MigrationStep.New(Version.AddAfterPostProcessFrameSetting, delegate(HDRenderPipelineAsset data)
		{
			FrameSettings.MigrateToAfterPostprocess(ref data.m_ObsoleteFrameSettingsMovedToDefaultSettings);
		}), MigrationStep.New(Version.AddReflectionSettings, delegate(HDRenderPipelineAsset data)
		{
			FrameSettings.MigrateToDefaultReflectionSettings(ref data.m_ObsoleteFrameSettingsMovedToDefaultSettings);
			FrameSettings.MigrateToNoReflectionSettings(ref data.m_ObsoleteBakedOrCustomReflectionFrameSettingsMovedToDefaultSettings);
			FrameSettings.MigrateToNoReflectionRealtimeSettings(ref data.m_ObsoleteRealtimeReflectionFrameSettingsMovedToDefaultSettings);
		}), MigrationStep.New(Version.AddPostProcessFrameSettings, delegate(HDRenderPipelineAsset data)
		{
			FrameSettings.MigrateToPostProcess(ref data.m_ObsoleteFrameSettingsMovedToDefaultSettings);
		}), MigrationStep.New(Version.AddRayTracingFrameSettings, delegate(HDRenderPipelineAsset data)
		{
			FrameSettings.MigrateToRayTracing(ref data.m_ObsoleteFrameSettingsMovedToDefaultSettings);
		}), MigrationStep.New(Version.AddFrameSettingDirectSpecularLighting, delegate(HDRenderPipelineAsset data)
		{
			FrameSettings.MigrateToDirectSpecularLighting(ref data.m_ObsoleteFrameSettingsMovedToDefaultSettings);
			FrameSettings.MigrateToNoDirectSpecularLighting(ref data.m_ObsoleteBakedOrCustomReflectionFrameSettingsMovedToDefaultSettings);
			FrameSettings.MigrateToDirectSpecularLighting(ref data.m_ObsoleteRealtimeReflectionFrameSettingsMovedToDefaultSettings);
		}), MigrationStep.New(Version.AddCustomPostprocessAndCustomPass, delegate(HDRenderPipelineAsset data)
		{
			FrameSettings.MigrateToCustomPostprocessAndCustomPass(ref data.m_ObsoleteFrameSettingsMovedToDefaultSettings);
		}), MigrationStep.New(Version.ScalableSettingsRefactor, delegate(HDRenderPipelineAsset data)
		{
			ref HDShadowInitParameters hdShadowInitParams2 = ref data.m_RenderPipelineSettings.hdShadowInitParams;
			hdShadowInitParams2.shadowResolutionArea.schemaId = ScalableSettingSchemaId.With4Levels;
			hdShadowInitParams2.shadowResolutionDirectional.schemaId = ScalableSettingSchemaId.With4Levels;
			hdShadowInitParams2.shadowResolutionPunctual.schemaId = ScalableSettingSchemaId.With4Levels;
		}), MigrationStep.New(Version.ShadowFilteringVeryHighQualityRemoval, delegate(HDRenderPipelineAsset data)
		{
			ref HDShadowInitParameters hdShadowInitParams = ref data.m_RenderPipelineSettings.hdShadowInitParams;
			hdShadowInitParams.shadowFilteringQuality = ((hdShadowInitParams.shadowFilteringQuality > HDShadowFilteringQuality.High) ? HDShadowFilteringQuality.High : hdShadowInitParams.shadowFilteringQuality);
		}), MigrationStep.New(Version.SeparateColorGradingAndTonemappingFrameSettings, delegate(HDRenderPipelineAsset data)
		{
			FrameSettings.MigrateToSeparateColorGradingAndTonemapping(ref data.m_ObsoleteFrameSettingsMovedToDefaultSettings);
		}), MigrationStep.New(Version.ReplaceTextureArraysByAtlasForCookieAndPlanar, delegate(HDRenderPipelineAsset data)
		{
			ref GlobalLightLoopSettings lightLoopSettings3 = ref data.m_RenderPipelineSettings.lightLoopSettings;
			float num7 = Mathf.Sqrt((int)lightLoopSettings3.cookieAtlasSize * (int)lightLoopSettings3.cookieAtlasSize * lightLoopSettings3.cookieTexArraySize);
			float num8 = Mathf.Sqrt((int)lightLoopSettings3.planarReflectionAtlasSize * (int)lightLoopSettings3.planarReflectionAtlasSize * lightLoopSettings3.maxPlanarReflectionOnScreen);
			num7 = Mathf.NextPowerOfTwo((int)num7);
			num8 = Mathf.NextPowerOfTwo((int)num8);
			num7 = Mathf.Clamp(num7, 256f, 8192f);
			num8 = Mathf.Clamp(num8, 256f, 8192f);
			lightLoopSettings3.cookieAtlasSize = (CookieAtlasResolution)num7;
			lightLoopSettings3.planarReflectionAtlasSize = (PlanarReflectionAtlasResolution)num8;
		}), MigrationStep.New(Version.AddedAdaptiveSSS, delegate(HDRenderPipelineAsset data)
		{
			bool obsoleteincreaseSssSampleCount = data.m_RenderPipelineSettings.m_ObsoleteincreaseSssSampleCount;
			FrameSettings.MigrateSubsurfaceParams(ref data.m_ObsoleteFrameSettingsMovedToDefaultSettings, obsoleteincreaseSssSampleCount);
			FrameSettings.MigrateSubsurfaceParams(ref data.m_ObsoleteBakedOrCustomReflectionFrameSettingsMovedToDefaultSettings, obsoleteincreaseSssSampleCount);
			FrameSettings.MigrateSubsurfaceParams(ref data.m_ObsoleteRealtimeReflectionFrameSettingsMovedToDefaultSettings, obsoleteincreaseSssSampleCount);
		}), MigrationStep.New(Version.RemoveCookieCubeAtlasToOctahedral2D, delegate(HDRenderPipelineAsset data)
		{
			ref GlobalLightLoopSettings lightLoopSettings2 = ref data.m_RenderPipelineSettings.lightLoopSettings;
			Mathf.Sqrt((int)lightLoopSettings2.cookieAtlasSize * (int)lightLoopSettings2.cookieAtlasSize * lightLoopSettings2.cookieTexArraySize);
			Mathf.Sqrt((int)lightLoopSettings2.planarReflectionAtlasSize * (int)lightLoopSettings2.planarReflectionAtlasSize * lightLoopSettings2.maxPlanarReflectionOnScreen);
			Debug.Log("HDRP Internally changed the storage of Cube Cookie to use Octahedral Projection inside the 2D Cookie Atlas. It is recommended that you increase the size of the 2D Cookie Atlas if your cookies no longer fit. To fix this, select your HDRP Asset and in the Inspector, go to Lighting > Cookies. In the 2D Atlas Size drop-down, select a larger cookie resolution.");
		}), MigrationStep.New(Version.RoughDistortion, delegate(HDRenderPipelineAsset data)
		{
			FrameSettings.MigrateRoughDistortion(ref data.m_ObsoleteFrameSettingsMovedToDefaultSettings);
			FrameSettings.MigrateRoughDistortion(ref data.m_ObsoleteBakedOrCustomReflectionFrameSettingsMovedToDefaultSettings);
			FrameSettings.MigrateRoughDistortion(ref data.m_ObsoleteRealtimeReflectionFrameSettingsMovedToDefaultSettings);
		}), MigrationStep.New(Version.VirtualTexturing, delegate(HDRenderPipelineAsset data)
		{
			FrameSettings.MigrateVirtualTexturing(ref data.m_ObsoleteFrameSettingsMovedToDefaultSettings);
			FrameSettings.MigrateVirtualTexturing(ref data.m_ObsoleteBakedOrCustomReflectionFrameSettingsMovedToDefaultSettings);
			FrameSettings.MigrateVirtualTexturing(ref data.m_ObsoleteRealtimeReflectionFrameSettingsMovedToDefaultSettings);
		}), MigrationStep.New(Version.AddedHDRenderPipelineGlobalSettings, delegate(HDRenderPipelineAsset data)
		{
			data.m_ObsoleteDefaultVolumeProfile = null;
			data.m_ObsoleteDefaultLookDevProfile = null;
			data.m_ObsoleteRenderPipelineResources = null;
			data.m_ObsoleteRenderPipelineRayTracingResources = null;
			data.m_ObsoleteBeforeTransparentCustomPostProcesses = null;
			data.m_ObsoleteBeforePostProcessCustomPostProcesses = null;
			data.m_ObsoleteAfterPostProcessCustomPostProcesses = null;
			data.m_ObsoleteBeforeTAACustomPostProcesses = null;
			data.m_ObsoleteDiffusionProfileSettingsList = null;
			data.m_RenderPipelineSettings.m_ObsoleteLightLayerName0 = null;
			data.m_RenderPipelineSettings.m_ObsoleteLightLayerName1 = null;
			data.m_RenderPipelineSettings.m_ObsoleteLightLayerName2 = null;
			data.m_RenderPipelineSettings.m_ObsoleteLightLayerName3 = null;
			data.m_RenderPipelineSettings.m_ObsoleteLightLayerName4 = null;
			data.m_RenderPipelineSettings.m_ObsoleteLightLayerName5 = null;
			data.m_RenderPipelineSettings.m_ObsoleteLightLayerName6 = null;
			data.m_RenderPipelineSettings.m_ObsoleteLightLayerName7 = null;
			data.m_RenderPipelineSettings.m_ObsoleteDecalLayerName0 = null;
			data.m_RenderPipelineSettings.m_ObsoleteDecalLayerName1 = null;
			data.m_RenderPipelineSettings.m_ObsoleteDecalLayerName2 = null;
			data.m_RenderPipelineSettings.m_ObsoleteDecalLayerName3 = null;
			data.m_RenderPipelineSettings.m_ObsoleteDecalLayerName4 = null;
			data.m_RenderPipelineSettings.m_ObsoleteDecalLayerName5 = null;
			data.m_RenderPipelineSettings.m_ObsoleteDecalLayerName6 = null;
			data.m_RenderPipelineSettings.m_ObsoleteDecalLayerName7 = null;
		}), MigrationStep.New(Version.DecalSurfaceGradient, delegate(HDRenderPipelineAsset data)
		{
			data.m_RenderPipelineSettings.supportSurfaceGradient = false;
		}), MigrationStep.New(Version.RemovalOfUpscaleFilter, delegate(HDRenderPipelineAsset data)
		{
			if (data.m_RenderPipelineSettings.dynamicResolutionSettings.upsampleFilter == DynamicResUpscaleFilter.Bilinear)
			{
				data.m_RenderPipelineSettings.dynamicResolutionSettings.upsampleFilter = DynamicResUpscaleFilter.CatmullRom;
			}
			if (data.m_RenderPipelineSettings.dynamicResolutionSettings.upsampleFilter == DynamicResUpscaleFilter.Lanczos)
			{
				data.m_RenderPipelineSettings.dynamicResolutionSettings.upsampleFilter = DynamicResUpscaleFilter.ContrastAdaptiveSharpen;
			}
		}), MigrationStep.New(Version.CombinedPlanarAndCubemapReflectionAtlases, delegate(HDRenderPipelineAsset data)
		{
			ref GlobalLightLoopSettings lightLoopSettings = ref data.m_RenderPipelineSettings.lightLoopSettings;
			CubeReflectionResolution reflectionCubemapSize = lightLoopSettings.reflectionCubemapSize;
			CubeReflectionResolution[] array = (CubeReflectionResolution[])Enum.GetValues(typeof(CubeReflectionResolution));
			int num = Mathf.Max(Array.IndexOf(array, reflectionCubemapSize), 0);
			CubeReflectionResolution[] values = new CubeReflectionResolution[3]
			{
				array[Mathf.Min(num, array.Length - 1)],
				array[Mathf.Min(num + 1, array.Length - 1)],
				array[Mathf.Min(num + 2, array.Length - 1)]
			};
			data.m_RenderPipelineSettings.cubeReflectionResolution = new RenderPipelineSettings.ReflectionProbeResolutionScalableSetting(values, ScalableSettingSchemaId.With3Levels);
			int reflectionProbeSizeInAtlas = ReflectionProbeTextureCache.GetReflectionProbeSizeInAtlas((int)lightLoopSettings.reflectionCubemapSize);
			int num2 = lightLoopSettings.reflectionProbeCacheSize * reflectionProbeSizeInAtlas * reflectionProbeSizeInAtlas;
			int num3 = (int)lightLoopSettings.planarReflectionAtlasSize * (int)lightLoopSettings.planarReflectionAtlasSize;
			int num4 = num2 + num3;
			lightLoopSettings.reflectionProbeTexCacheSize = ReflectionProbeTextureCacheResolution.Resolution16384x16384;
			foreach (ReflectionProbeTextureCacheResolution item in from ReflectionProbeTextureCacheResolution r in Enum.GetValues(typeof(ReflectionProbeTextureCacheResolution))
				orderby (int)(r & (ReflectionProbeTextureCacheResolution)65535)
				select r)
			{
				int num5 = (int)(item & (ReflectionProbeTextureCacheResolution)65535);
				int num6 = (int)item >> 16;
				if (num6 == 0)
				{
					num6 = num5;
				}
				if (num6 * num5 >= num4)
				{
					lightLoopSettings.reflectionProbeTexCacheSize = item;
					break;
				}
			}
			lightLoopSettings.maxCubeReflectionOnScreen = Mathf.Clamp(lightLoopSettings.maxEnvLightsOnScreen - lightLoopSettings.maxPlanarReflectionOnScreen, 32, 64);
		}));

		[SerializeField]
		private Version m_Version = MigrationDescription.LastVersion<Version>();

		[SerializeField]
		[FormerlySerializedAs("serializedFrameSettings")]
		[FormerlySerializedAs("m_FrameSettings")]
		[Obsolete("For data migration")]
		private ObsoleteFrameSettings m_ObsoleteFrameSettings;

		[SerializeField]
		[FormerlySerializedAs("m_BakedOrCustomReflectionFrameSettings")]
		[Obsolete("For data migration")]
		private ObsoleteFrameSettings m_ObsoleteBakedOrCustomReflectionFrameSettings;

		[SerializeField]
		[FormerlySerializedAs("m_RealtimeReflectionFrameSettings")]
		[Obsolete("For data migration")]
		private ObsoleteFrameSettings m_ObsoleteRealtimeReflectionFrameSettings;

		[SerializeField]
		[FormerlySerializedAs("m_DefaultVolumeProfile")]
		[Obsolete("Moved from HDRPAsset to HDGlobal Settings")]
		internal VolumeProfile m_ObsoleteDefaultVolumeProfile;

		[SerializeField]
		[FormerlySerializedAs("m_DefaultLookDevProfile")]
		[Obsolete("Moved from HDRPAsset to HDGlobal Settings")]
		internal VolumeProfile m_ObsoleteDefaultLookDevProfile;

		[SerializeField]
		[FormerlySerializedAs("m_RenderingPathDefaultCameraFrameSettings")]
		[Obsolete("Moved from HDRPAsset to HDGlobal Settings")]
		internal FrameSettings m_ObsoleteFrameSettingsMovedToDefaultSettings;

		[SerializeField]
		[FormerlySerializedAs("m_RenderingPathDefaultBakedOrCustomReflectionFrameSettings")]
		[Obsolete("Moved from HDRPAsset to HDGlobal Settings")]
		internal FrameSettings m_ObsoleteBakedOrCustomReflectionFrameSettingsMovedToDefaultSettings;

		[SerializeField]
		[FormerlySerializedAs("m_RenderingPathDefaultRealtimeReflectionFrameSettings")]
		[Obsolete("Moved from HDRPAsset to HDGlobal Settings")]
		internal FrameSettings m_ObsoleteRealtimeReflectionFrameSettingsMovedToDefaultSettings;

		[SerializeField]
		[FormerlySerializedAs("m_RenderPipelineResources")]
		[Obsolete("Moved from HDRPAsset to HDGlobal Settings")]
		internal HDRenderPipelineRuntimeResources m_ObsoleteRenderPipelineResources;

		[SerializeField]
		[FormerlySerializedAs("m_RenderPipelineRayTracingResources")]
		[Obsolete("Moved from HDRPAsset to HDGlobal Settings")]
		internal HDRenderPipelineRayTracingResources m_ObsoleteRenderPipelineRayTracingResources;

		[SerializeField]
		[FormerlySerializedAs("beforeTransparentCustomPostProcesses")]
		[Obsolete("Moved from HDRPAsset to HDGlobal Settings")]
		internal List<string> m_ObsoleteBeforeTransparentCustomPostProcesses;

		[SerializeField]
		[FormerlySerializedAs("beforePostProcessCustomPostProcesses")]
		[Obsolete("Moved from HDRPAsset to HDGlobal Settings")]
		internal List<string> m_ObsoleteBeforePostProcessCustomPostProcesses;

		[SerializeField]
		[FormerlySerializedAs("afterPostProcessCustomPostProcesses")]
		[Obsolete("Moved from HDRPAsset to HDGlobal Settings")]
		internal List<string> m_ObsoleteAfterPostProcessCustomPostProcesses;

		[SerializeField]
		[FormerlySerializedAs("beforeTAACustomPostProcesses")]
		[Obsolete("Moved from HDRPAsset to HDGlobal Settings")]
		internal List<string> m_ObsoleteBeforeTAACustomPostProcesses;

		[SerializeField]
		[FormerlySerializedAs("shaderVariantLogLevel")]
		[Obsolete("Moved from HDRPAsset to HDGlobal Settings")]
		internal int m_ObsoleteShaderVariantLogLevel;

		[SerializeField]
		[FormerlySerializedAs("m_LensAttenuation")]
		[Obsolete("Moved from HDRPAsset to HDGlobal Settings")]
		internal LensAttenuationMode m_ObsoleteLensAttenuation;

		[SerializeField]
		[FormerlySerializedAs("diffusionProfileSettingsList")]
		[Obsolete("Moved from HDRPAsset to HDGlobal Settings")]
		internal DiffusionProfileSettings[] m_ObsoleteDiffusionProfileSettingsList;

		private HDRenderPipelineGlobalSettings globalSettings => HDRenderPipelineGlobalSettings.instance;

		internal HDRenderPipelineRuntimeResources renderPipelineResources => globalSettings.renderPipelineResources;

		internal bool frameSettingsHistory { get; set; }

		internal ReflectionSystemParameters reflectionSystemParameters
		{
			get
			{
				ReflectionSystemParameters result = default(ReflectionSystemParameters);
				result.maxActivePlanarReflectionProbe = 512;
				result.maxActiveEnvReflectionProbe = 512;
				return result;
			}
		}

		public RenderPipelineSettings currentPlatformRenderPipelineSettings
		{
			get
			{
				return m_RenderPipelineSettings;
			}
			set
			{
				m_RenderPipelineSettings = value;
				OnValidate();
			}
		}

		public MaterialQuality defaultMaterialQualityLevel => m_DefaultMaterialQualityLevel;

		public override string[] renderingLayerMaskNames => globalSettings.renderingLayerMaskNames;

		public override string[] prefixedRenderingLayerMaskNames => globalSettings.prefixedRenderingLayerMaskNames;

		public string[] lightLayerNames => globalSettings.lightLayerNames;

		public string[] decalLayerNames => globalSettings.decalLayerNames;

		public override Shader defaultShader => globalSettings?.renderPipelineResources?.shaders.defaultPS;

		internal bool useRenderGraph
		{
			get
			{
				return m_UseRenderGraph;
			}
			set
			{
				m_UseRenderGraph = value;
			}
		}

		public bool virtualTexturingEnabled => true;

		Version IVersionable<Version>.version
		{
			get
			{
				return m_Version;
			}
			set
			{
				m_Version = value;
			}
		}

		private HDRenderPipelineAsset()
		{
		}

		private void OnEnable()
		{
			Migrate();
			HDRenderPipeline.SetupDLSSFeature(HDRenderPipelineGlobalSettings.instance);
		}

		private void Reset()
		{
			OnValidate();
		}

		protected override RenderPipeline CreatePipeline()
		{
			return new HDRenderPipeline(this);
		}

		protected override void OnValidate()
		{
			isInOnValidateCall = true;
			if (GraphicsSettings.currentRenderPipeline == this)
			{
				base.OnValidate();
			}
			isInOnValidateCall = false;
		}

		internal void TurnOffRayTracing()
		{
			m_RenderPipelineSettings.supportRayTracing = false;
		}

		private bool Migrate()
		{
			return k_Migration.Migrate(this);
		}
	}
}
