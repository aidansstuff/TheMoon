using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[ExecuteAlways]
	[AddComponentMenu("")]
	public class StaticLightingSky : MonoBehaviour
	{
		[SerializeField]
		private VolumeProfile m_Profile;

		private bool m_NeedUpdateStaticLightingSky;

		[SerializeField]
		[FormerlySerializedAs("m_BakingSkyUniqueID")]
		private int m_StaticLightingSkyUniqueID;

		private int m_LastComputedHash;

		[SerializeField]
		private int m_StaticLightingCloudsUniqueID;

		private int m_LastComputedCloudHash;

		private SkySettings m_SkySettings;

		private SkySettings m_SkySettingsFromProfile;

		private CloudSettings m_CloudSettings;

		private CloudSettings m_CloudSettingsFromProfile;

		[SerializeField]
		private bool m_StaticLightingVolumetricClouds;

		private int m_LastComputedVolumetricCloudHash;

		private VolumetricClouds m_VolumetricClouds;

		private VolumetricClouds m_VolumetricCloudSettingsFromProfile;

		private List<SkySettings> m_VolumeSkyList = new List<SkySettings>();

		private List<CloudSettings> m_VolumeCloudsList = new List<CloudSettings>();

		internal SkySettings skySettings
		{
			get
			{
				GetSkyFromIDAndVolume(m_StaticLightingSkyUniqueID, m_Profile, out var skySetting, out var _);
				if (skySetting != null)
				{
					int hashCode = skySetting.GetHashCode();
					if (m_LastComputedHash != hashCode)
					{
						UpdateCurrentStaticLightingSky();
					}
				}
				else
				{
					ResetSky();
				}
				return m_SkySettings;
			}
		}

		internal CloudSettings cloudSettings
		{
			get
			{
				GetCloudFromIDAndVolume(m_StaticLightingCloudsUniqueID, m_Profile, out var cloudSetting, out var _);
				if (cloudSetting != null)
				{
					int hashCode = cloudSetting.GetHashCode();
					if (m_LastComputedCloudHash != hashCode)
					{
						UpdateCurrentStaticLightingClouds();
					}
				}
				else
				{
					ResetCloud();
				}
				return m_CloudSettings;
			}
		}

		internal VolumetricClouds volumetricClouds
		{
			get
			{
				if (!m_StaticLightingVolumetricClouds)
				{
					return null;
				}
				return m_VolumetricClouds;
			}
		}

		public VolumeProfile profile
		{
			get
			{
				return m_Profile;
			}
			set
			{
				if (value != m_Profile)
				{
					m_StaticLightingSkyUniqueID = 0;
					if (m_Profile == null)
					{
						SkyManager.RegisterStaticLightingSky(this);
					}
					if (value == null)
					{
						SkyManager.UnRegisterStaticLightingSky(this);
					}
				}
				m_Profile = value;
			}
		}

		public int staticLightingSkyUniqueID
		{
			get
			{
				return m_StaticLightingSkyUniqueID;
			}
			set
			{
				m_StaticLightingSkyUniqueID = value;
				UpdateCurrentStaticLightingSky();
			}
		}

		public int staticLightingCloudsUniqueID
		{
			get
			{
				return m_StaticLightingCloudsUniqueID;
			}
			set
			{
				m_StaticLightingCloudsUniqueID = value;
				UpdateCurrentStaticLightingClouds();
			}
		}

		private void GetSkyFromIDAndVolume(int skyUniqueID, VolumeProfile profile, out SkySettings skySetting, out Type skyType)
		{
			skySetting = null;
			skyType = typeof(SkySettings);
			if (!(profile != null) || skyUniqueID == 0)
			{
				return;
			}
			m_VolumeSkyList.Clear();
			if (!profile.TryGetAllSubclassOf(typeof(SkySettings), m_VolumeSkyList))
			{
				return;
			}
			foreach (SkySettings volumeSky in m_VolumeSkyList)
			{
				if (skyUniqueID == SkySettings.GetUniqueID(volumeSky.GetType()) && volumeSky.active)
				{
					skyType = volumeSky.GetType();
					skySetting = volumeSky;
				}
			}
		}

		private void GetCloudFromIDAndVolume(int cloudUniqueID, VolumeProfile profile, out CloudSettings cloudSetting, out Type cloudType)
		{
			cloudSetting = null;
			cloudType = typeof(CloudSettings);
			if (!(profile != null) || cloudUniqueID == 0)
			{
				return;
			}
			m_VolumeCloudsList.Clear();
			if (!profile.TryGetAllSubclassOf(typeof(CloudSettings), m_VolumeCloudsList))
			{
				return;
			}
			foreach (CloudSettings volumeClouds in m_VolumeCloudsList)
			{
				if (cloudUniqueID == CloudSettings.GetUniqueID(volumeClouds.GetType()) && volumeClouds.active)
				{
					cloudType = volumeClouds.GetType();
					cloudSetting = volumeClouds;
				}
			}
		}

		private void GetVolumetricCloudVolume(VolumeProfile profile, out VolumetricClouds volumetricClouds)
		{
			volumetricClouds = null;
			if (profile != null)
			{
				profile.TryGet<VolumetricClouds>(out volumetricClouds);
			}
		}

		private int InitComponentFromProfile<T>(T component, T componentFromProfile, Type type) where T : VolumeComponent
		{
			ReadOnlyCollection<VolumeParameter> parameters = component.parameters;
			ReadOnlyCollection<VolumeParameter> parameters2 = componentFromProfile.parameters;
			Volume orCreateDefaultVolume = HDRenderPipelineGlobalSettings.instance.GetOrCreateDefaultVolume();
			T component2 = null;
			if (orCreateDefaultVolume.sharedProfile != null)
			{
				orCreateDefaultVolume.sharedProfile.TryGet<T>(type, out component2);
			}
			ReadOnlyCollection<VolumeParameter> readOnlyCollection = ((component2 != null) ? component2.parameters : null);
			if (parameters2 == null)
			{
				return 0;
			}
			int count = parameters.Count;
			for (int i = 0; i < count; i++)
			{
				if (parameters2[i].overrideState)
				{
					parameters[i].SetValue(parameters2[i]);
				}
				else if (readOnlyCollection != null && readOnlyCollection[i].overrideState)
				{
					parameters[i].SetValue(readOnlyCollection[i]);
				}
			}
			return componentFromProfile.GetHashCode();
		}

		private void UpdateCurrentStaticLightingSky()
		{
			if (RenderPipelineManager.currentPipeline is HDRenderPipeline)
			{
				CoreUtils.Destroy(m_SkySettings);
				m_SkySettings = null;
				m_LastComputedHash = 0;
				GetSkyFromIDAndVolume(m_StaticLightingSkyUniqueID, m_Profile, out m_SkySettingsFromProfile, out var skyType);
				if (m_SkySettingsFromProfile != null)
				{
					m_SkySettings = (SkySettings)ScriptableObject.CreateInstance(skyType);
					m_LastComputedHash = InitComponentFromProfile(m_SkySettings, m_SkySettingsFromProfile, skyType);
				}
			}
		}

		private void UpdateCurrentStaticLightingClouds()
		{
			CoreUtils.Destroy(m_CloudSettings);
			m_CloudSettings = null;
			m_LastComputedCloudHash = 0;
			GetCloudFromIDAndVolume(m_StaticLightingCloudsUniqueID, m_Profile, out m_CloudSettingsFromProfile, out var cloudType);
			if (m_CloudSettingsFromProfile != null)
			{
				m_CloudSettings = (CloudSettings)ScriptableObject.CreateInstance(cloudType);
				m_LastComputedCloudHash = InitComponentFromProfile(m_CloudSettings, m_CloudSettingsFromProfile, cloudType);
			}
		}

		private void UpdateCurrentStaticLightingVolumetricClouds()
		{
			CoreUtils.Destroy(m_VolumetricClouds);
			m_VolumetricClouds = null;
			m_LastComputedVolumetricCloudHash = 0;
			GetVolumetricCloudVolume(m_Profile, out m_VolumetricCloudSettingsFromProfile);
			if (m_VolumetricCloudSettingsFromProfile != null)
			{
				m_VolumetricClouds = (VolumetricClouds)ScriptableObject.CreateInstance(typeof(VolumetricClouds));
				m_LastComputedVolumetricCloudHash = InitComponentFromProfile(m_VolumetricClouds, m_VolumetricCloudSettingsFromProfile, typeof(VolumetricClouds));
			}
		}

		private void OnValidate()
		{
			if (!base.isActiveAndEnabled)
			{
				return;
			}
			if (m_Profile == null)
			{
				m_StaticLightingSkyUniqueID = 0;
				m_StaticLightingCloudsUniqueID = 0;
				m_StaticLightingVolumetricClouds = false;
			}
			if (profile != null)
			{
				if (m_SkySettingsFromProfile != null && !profile.components.Find((VolumeComponent x) => x == m_SkySettingsFromProfile))
				{
					m_StaticLightingSkyUniqueID = 0;
				}
				if (m_CloudSettingsFromProfile != null && !profile.components.Find((VolumeComponent x) => x == m_CloudSettingsFromProfile))
				{
					m_StaticLightingCloudsUniqueID = 0;
				}
			}
			m_NeedUpdateStaticLightingSky = true;
		}

		private bool VerifyProfileComponentsInitialized()
		{
			if (m_Profile != null)
			{
				foreach (VolumeComponent component in m_Profile.components)
				{
					if (component.parameters == null || component.parameters.Count == 0)
					{
						return false;
					}
				}
			}
			return true;
		}

		private void OnEnable()
		{
			if (VerifyProfileComponentsInitialized())
			{
				UpdateCurrentStaticLightingSky();
				UpdateCurrentStaticLightingClouds();
				UpdateCurrentStaticLightingVolumetricClouds();
			}
			else
			{
				m_NeedUpdateStaticLightingSky = true;
			}
			if (m_Profile != null)
			{
				SkyManager.RegisterStaticLightingSky(this);
			}
		}

		private void OnDisable()
		{
			if (m_Profile != null)
			{
				SkyManager.UnRegisterStaticLightingSky(this);
			}
			ResetSky();
			ResetCloud();
			ResetVolumetricCloud();
		}

		private void Update()
		{
			if (m_NeedUpdateStaticLightingSky)
			{
				UpdateCurrentStaticLightingSky();
				UpdateCurrentStaticLightingClouds();
				UpdateCurrentStaticLightingVolumetricClouds();
				m_NeedUpdateStaticLightingSky = false;
			}
		}

		private void ResetSky()
		{
			CoreUtils.Destroy(m_SkySettings);
			m_SkySettings = null;
			m_SkySettingsFromProfile = null;
			m_LastComputedHash = 0;
		}

		private void ResetCloud()
		{
			CoreUtils.Destroy(m_CloudSettings);
			m_CloudSettings = null;
			m_CloudSettingsFromProfile = null;
			m_LastComputedCloudHash = 0;
		}

		private void ResetVolumetricCloud()
		{
			CoreUtils.Destroy(m_VolumetricClouds);
			m_VolumetricClouds = null;
			m_CloudSettingsFromProfile = null;
			m_LastComputedVolumetricCloudHash = 0;
		}
	}
}
