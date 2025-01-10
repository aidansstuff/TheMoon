using System;
using System.Collections.Generic;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class HDRenderPipelineGlobalSettings : RenderPipelineGlobalSettings, IVersionable<HDRenderPipelineGlobalSettings.Version>, IMigratableAsset, IShaderVariantSettings
	{
		private enum Version
		{
			First = 0,
			UpdateMSAA = 1,
			UpdateLensFlare = 2,
			MovedSupportRuntimeDebugDisplayToGlobalSettings = 3,
			DisableAutoRegistration = 4,
			MoveDiffusionProfilesToVolume = 5
		}

		private static HDRenderPipelineGlobalSettings cachedInstance = null;

		private Volume s_DefaultVolume;

		[SerializeField]
		[FormerlySerializedAs("m_VolumeProfileDefault")]
		private VolumeProfile m_DefaultVolumeProfile;

		[SerializeField]
		private FrameSettings m_RenderingPathDefaultCameraFrameSettings = FrameSettings.NewDefaultCamera();

		[SerializeField]
		private FrameSettings m_RenderingPathDefaultBakedOrCustomReflectionFrameSettings = FrameSettings.NewDefaultCustomOrBakeReflectionProbe();

		[SerializeField]
		private FrameSettings m_RenderingPathDefaultRealtimeReflectionFrameSettings = FrameSettings.NewDefaultRealtimeReflectionProbe();

		[SerializeField]
		private HDRenderPipelineRuntimeResources m_RenderPipelineResources;

		[SerializeField]
		private HDRenderPipelineRayTracingResources m_RenderPipelineRayTracingResources;

		[SerializeField]
		internal List<string> beforeTransparentCustomPostProcesses = new List<string>();

		[SerializeField]
		internal List<string> beforePostProcessCustomPostProcesses = new List<string>();

		[SerializeField]
		internal List<string> afterPostProcessBlursCustomPostProcesses = new List<string>();

		[SerializeField]
		internal List<string> afterPostProcessCustomPostProcesses = new List<string>();

		[SerializeField]
		internal List<string> beforeTAACustomPostProcesses = new List<string>();

		private static readonly string[] k_DefaultLightLayerNames = new string[8] { "Light Layer default", "Light Layer 1", "Light Layer 2", "Light Layer 3", "Light Layer 4", "Light Layer 5", "Light Layer 6", "Light Layer 7" };

		public string lightLayerName0 = k_DefaultLightLayerNames[0];

		public string lightLayerName1 = k_DefaultLightLayerNames[1];

		public string lightLayerName2 = k_DefaultLightLayerNames[2];

		public string lightLayerName3 = k_DefaultLightLayerNames[3];

		public string lightLayerName4 = k_DefaultLightLayerNames[4];

		public string lightLayerName5 = k_DefaultLightLayerNames[5];

		public string lightLayerName6 = k_DefaultLightLayerNames[6];

		public string lightLayerName7 = k_DefaultLightLayerNames[7];

		[NonSerialized]
		private string[] m_LightLayerNames;

		[NonSerialized]
		private string[] m_PrefixedLightLayerNames;

		private static readonly string[] k_DefaultDecalLayerNames = new string[8] { "Decal Layer default", "Decal Layer 1", "Decal Layer 2", "Decal Layer 3", "Decal Layer 4", "Decal Layer 5", "Decal Layer 6", "Decal Layer 7" };

		public string decalLayerName0 = k_DefaultDecalLayerNames[0];

		public string decalLayerName1 = k_DefaultDecalLayerNames[1];

		public string decalLayerName2 = k_DefaultDecalLayerNames[2];

		public string decalLayerName3 = k_DefaultDecalLayerNames[3];

		public string decalLayerName4 = k_DefaultDecalLayerNames[4];

		public string decalLayerName5 = k_DefaultDecalLayerNames[5];

		public string decalLayerName6 = k_DefaultDecalLayerNames[6];

		public string decalLayerName7 = k_DefaultDecalLayerNames[7];

		[NonSerialized]
		private string[] m_DecalLayerNames;

		[NonSerialized]
		private string[] m_PrefixedDecalLayerNames;

		[NonSerialized]
		private string[] m_RenderingLayerNames;

		[NonSerialized]
		private string[] m_PrefixedRenderingLayerNames;

		[SerializeField]
		internal LensAttenuationMode lensAttenuationMode;

		[SerializeField]
		internal ColorGradingSpace colorGradingSpace;

		[SerializeField]
		[FormerlySerializedAs("diffusionProfileSettingsList")]
		internal DiffusionProfileSettings[] m_ObsoleteDiffusionProfileSettingsList;

		[SerializeField]
		internal bool rendererListCulling;

		private static readonly DiffusionProfileSettings[] kEmptyProfiles = new DiffusionProfileSettings[0];

		[SerializeField]
		internal string DLSSProjectId = "000000";

		[SerializeField]
		internal bool useDLSSCustomProjectId;

		[SerializeField]
		internal bool supportProbeVolumes;

		public bool supportRuntimeDebugDisplay;

		public bool autoRegisterDiffusionProfiles = true;

		[SerializeField]
		internal ProbeVolumeSceneData apvScenesData;

		private static Version[] skipedStepWhenCreatedFromHDRPAsset = new Version[0];

		[SerializeField]
		private Version m_Version = MigrationDescription.LastVersion<Version>();

		[SerializeField]
		[FormerlySerializedAs("shaderVariantLogLevel")]
		internal ShaderVariantLogLevel m_ShaderVariantLogLevel;

		[SerializeField]
		internal bool m_ExportShaderVariants = true;

		public static HDRenderPipelineGlobalSettings instance
		{
			get
			{
				if (cachedInstance == null)
				{
					cachedInstance = GraphicsSettings.GetSettingsForRenderPipeline<HDRenderPipeline>() as HDRenderPipelineGlobalSettings;
				}
				return cachedInstance;
			}
		}

		internal VolumeProfile volumeProfile
		{
			get
			{
				return m_DefaultVolumeProfile;
			}
			set
			{
				m_DefaultVolumeProfile = value;
			}
		}

		internal HDRenderPipelineRuntimeResources renderPipelineResources => m_RenderPipelineResources;

		internal HDRenderPipelineRayTracingResources renderPipelineRayTracingResources => m_RenderPipelineRayTracingResources;

		public string[] lightLayerNames
		{
			get
			{
				if (m_LightLayerNames == null)
				{
					m_LightLayerNames = new string[8];
				}
				m_LightLayerNames[0] = lightLayerName0;
				m_LightLayerNames[1] = lightLayerName1;
				m_LightLayerNames[2] = lightLayerName2;
				m_LightLayerNames[3] = lightLayerName3;
				m_LightLayerNames[4] = lightLayerName4;
				m_LightLayerNames[5] = lightLayerName5;
				m_LightLayerNames[6] = lightLayerName6;
				m_LightLayerNames[7] = lightLayerName7;
				return m_LightLayerNames;
			}
		}

		public string[] prefixedLightLayerNames
		{
			get
			{
				if (m_PrefixedLightLayerNames == null)
				{
					UpdateRenderingLayerNames();
				}
				return m_PrefixedLightLayerNames;
			}
		}

		public string[] decalLayerNames
		{
			get
			{
				if (m_DecalLayerNames == null)
				{
					m_DecalLayerNames = new string[8];
				}
				m_DecalLayerNames[0] = decalLayerName0;
				m_DecalLayerNames[1] = decalLayerName1;
				m_DecalLayerNames[2] = decalLayerName2;
				m_DecalLayerNames[3] = decalLayerName3;
				m_DecalLayerNames[4] = decalLayerName4;
				m_DecalLayerNames[5] = decalLayerName5;
				m_DecalLayerNames[6] = decalLayerName6;
				m_DecalLayerNames[7] = decalLayerName7;
				return m_DecalLayerNames;
			}
		}

		public string[] prefixedDecalLayerNames
		{
			get
			{
				if (m_PrefixedDecalLayerNames == null)
				{
					UpdateRenderingLayerNames();
				}
				return m_PrefixedDecalLayerNames;
			}
		}

		private string[] renderingLayerNames
		{
			get
			{
				if (m_RenderingLayerNames == null)
				{
					UpdateRenderingLayerNames();
				}
				return m_RenderingLayerNames;
			}
		}

		private string[] prefixedRenderingLayerNames
		{
			get
			{
				if (m_PrefixedRenderingLayerNames == null)
				{
					UpdateRenderingLayerNames();
				}
				return m_PrefixedRenderingLayerNames;
			}
		}

		public string[] renderingLayerMaskNames => renderingLayerNames;

		public string[] prefixedRenderingLayerMaskNames => prefixedRenderingLayerNames;

		internal DiffusionProfileSettings[] diffusionProfileSettingsList
		{
			get
			{
				if (instance.volumeProfile != null && instance.volumeProfile.TryGet<DiffusionProfileList>(out var component))
				{
					return component.diffusionProfiles.value ?? kEmptyProfiles;
				}
				return kEmptyProfiles;
			}
			set
			{
				GetOrCreateDiffusionProfileList().diffusionProfiles.value = value;
			}
		}

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

		public ShaderVariantLogLevel shaderVariantLogLevel
		{
			get
			{
				return m_ShaderVariantLogLevel;
			}
			set
			{
				m_ShaderVariantLogLevel = value;
			}
		}

		public bool exportShaderVariants
		{
			get
			{
				return m_ExportShaderVariants;
			}
			set
			{
				m_ExportShaderVariants = true;
			}
		}

		internal static void UpdateGraphicsSettings(HDRenderPipelineGlobalSettings newSettings)
		{
			if (!(newSettings == cachedInstance))
			{
				if (newSettings != null)
				{
					GraphicsSettings.RegisterRenderPipelineSettings<HDRenderPipeline>(newSettings);
				}
				else
				{
					GraphicsSettings.UnregisterRenderPipelineSettings<HDRenderPipeline>();
				}
				cachedInstance = newSettings;
			}
		}

		internal Volume GetOrCreateDefaultVolume()
		{
			if (s_DefaultVolume == null || s_DefaultVolume.Equals(null))
			{
				GameObject gameObject = new GameObject("Default Volume")
				{
					hideFlags = HideFlags.HideAndDontSave
				};
				s_DefaultVolume = gameObject.AddComponent<Volume>();
				s_DefaultVolume.isGlobal = true;
				s_DefaultVolume.priority = float.MinValue;
				s_DefaultVolume.sharedProfile = GetOrCreateDefaultVolumeProfile();
			}
			if (s_DefaultVolume.sharedProfile == null || s_DefaultVolume.sharedProfile.Equals(null))
			{
				s_DefaultVolume.sharedProfile = volumeProfile;
			}
			if (s_DefaultVolume.sharedProfile != volumeProfile)
			{
				s_DefaultVolume.sharedProfile = volumeProfile;
			}
			if (s_DefaultVolume == null)
			{
				Debug.LogError("[HDRP] Cannot Create Default Volume.");
			}
			return s_DefaultVolume;
		}

		internal VolumeProfile GetOrCreateDefaultVolumeProfile()
		{
			return volumeProfile;
		}

		internal ref FrameSettings GetDefaultFrameSettings(FrameSettingsRenderType type)
		{
			return type switch
			{
				FrameSettingsRenderType.Camera => ref m_RenderingPathDefaultCameraFrameSettings, 
				FrameSettingsRenderType.CustomOrBakedReflection => ref m_RenderingPathDefaultBakedOrCustomReflectionFrameSettings, 
				FrameSettingsRenderType.RealtimeReflection => ref m_RenderingPathDefaultRealtimeReflectionFrameSettings, 
				_ => throw new ArgumentException("Unknown FrameSettingsRenderType"), 
			};
		}

		public bool IsCustomPostProcessRegistered(Type customPostProcessType)
		{
			string assemblyQualifiedName = customPostProcessType.AssemblyQualifiedName;
			if (!beforeTransparentCustomPostProcesses.Contains(assemblyQualifiedName) && !beforePostProcessCustomPostProcesses.Contains(assemblyQualifiedName) && !afterPostProcessBlursCustomPostProcesses.Contains(assemblyQualifiedName) && !afterPostProcessCustomPostProcesses.Contains(assemblyQualifiedName))
			{
				return beforeTAACustomPostProcesses.Contains(assemblyQualifiedName);
			}
			return true;
		}

		internal void UpdateRenderingLayerNames()
		{
			if (m_RenderingLayerNames == null)
			{
				m_RenderingLayerNames = new string[32];
			}
			m_RenderingLayerNames[0] = lightLayerName0;
			m_RenderingLayerNames[1] = lightLayerName1;
			m_RenderingLayerNames[2] = lightLayerName2;
			m_RenderingLayerNames[3] = lightLayerName3;
			m_RenderingLayerNames[4] = lightLayerName4;
			m_RenderingLayerNames[5] = lightLayerName5;
			m_RenderingLayerNames[6] = lightLayerName6;
			m_RenderingLayerNames[7] = lightLayerName7;
			m_RenderingLayerNames[8] = decalLayerName0;
			m_RenderingLayerNames[9] = decalLayerName1;
			m_RenderingLayerNames[10] = decalLayerName2;
			m_RenderingLayerNames[11] = decalLayerName3;
			m_RenderingLayerNames[12] = decalLayerName4;
			m_RenderingLayerNames[13] = decalLayerName5;
			m_RenderingLayerNames[14] = decalLayerName6;
			m_RenderingLayerNames[15] = decalLayerName7;
			for (int i = 16; i < m_RenderingLayerNames.Length; i++)
			{
				m_RenderingLayerNames[i] = $"Unused {i}";
			}
			if (m_PrefixedRenderingLayerNames == null)
			{
				m_PrefixedRenderingLayerNames = new string[32];
			}
			if (m_PrefixedLightLayerNames == null)
			{
				m_PrefixedLightLayerNames = new string[8];
			}
			if (m_PrefixedDecalLayerNames == null)
			{
				m_PrefixedDecalLayerNames = new string[8];
			}
			for (int j = 0; j < m_PrefixedRenderingLayerNames.Length; j++)
			{
				m_PrefixedRenderingLayerNames[j] = $"{j}: {m_RenderingLayerNames[j]}";
				if (j < 8)
				{
					m_PrefixedLightLayerNames[j] = m_PrefixedRenderingLayerNames[j];
				}
				else if (j < 16)
				{
					m_PrefixedDecalLayerNames[j - 8] = $"{j - 8}: {m_RenderingLayerNames[j]}";
				}
			}
		}

		internal void ResetRenderingLayerNames(bool lightLayers, bool decalLayers)
		{
			if (lightLayers)
			{
				lightLayerName0 = k_DefaultLightLayerNames[0];
				lightLayerName1 = k_DefaultLightLayerNames[1];
				lightLayerName2 = k_DefaultLightLayerNames[2];
				lightLayerName3 = k_DefaultLightLayerNames[3];
				lightLayerName4 = k_DefaultLightLayerNames[4];
				lightLayerName5 = k_DefaultLightLayerNames[5];
				lightLayerName6 = k_DefaultLightLayerNames[6];
				lightLayerName7 = k_DefaultLightLayerNames[7];
			}
			if (decalLayers)
			{
				decalLayerName0 = k_DefaultDecalLayerNames[0];
				decalLayerName1 = k_DefaultDecalLayerNames[1];
				decalLayerName2 = k_DefaultDecalLayerNames[2];
				decalLayerName3 = k_DefaultDecalLayerNames[3];
				decalLayerName4 = k_DefaultDecalLayerNames[4];
				decalLayerName5 = k_DefaultDecalLayerNames[5];
				decalLayerName6 = k_DefaultDecalLayerNames[6];
				decalLayerName7 = k_DefaultDecalLayerNames[7];
			}
			UpdateRenderingLayerNames();
		}

		internal DiffusionProfileList GetOrCreateDiffusionProfileList()
		{
			VolumeProfile orCreateDefaultVolumeProfile = instance.GetOrCreateDefaultVolumeProfile();
			if (!orCreateDefaultVolumeProfile.TryGet<DiffusionProfileList>(out var component))
			{
				component = orCreateDefaultVolumeProfile.Add<DiffusionProfileList>(overrides: true);
			}
			if (component.diffusionProfiles.value == null)
			{
				component.diffusionProfiles.value = new DiffusionProfileSettings[0];
			}
			return component;
		}

		internal ProbeVolumeSceneData GetOrCreateAPVSceneData()
		{
			if (apvScenesData == null)
			{
				apvScenesData = new ProbeVolumeSceneData(this, "apvScenesData");
			}
			apvScenesData.SetParentObject(this, "apvScenesData");
			return apvScenesData;
		}
	}
}
