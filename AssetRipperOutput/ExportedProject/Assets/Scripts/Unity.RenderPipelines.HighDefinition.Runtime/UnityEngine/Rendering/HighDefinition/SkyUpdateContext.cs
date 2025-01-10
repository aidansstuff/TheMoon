using System;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class SkyUpdateContext
	{
		private SkySettings m_SkySettings;

		public int cachedSkyRenderingContextId = -1;

		private CloudSettings m_CloudSettings;

		public int skyParametersHash = -1;

		public float currentUpdateTime;

		private VolumetricClouds m_VolumetricClouds;

		public SkyRenderer skyRenderer { get; private set; }

		public CloudRenderer cloudRenderer { get; private set; }

		public bool settingsHadBigDifferenceWithPrev { get; private set; }

		public SkySettings skySettings
		{
			get
			{
				return m_SkySettings;
			}
			set
			{
				if (skyRenderer != null && (value == null || value.GetSkyRendererType() != skyRenderer.GetType()))
				{
					skyRenderer.Cleanup();
					skyRenderer = null;
				}
				if (m_SkySettings == null)
				{
					settingsHadBigDifferenceWithPrev = true;
				}
				else
				{
					settingsHadBigDifferenceWithPrev = m_SkySettings.SignificantlyDivergesFrom(value);
				}
				if (!(m_SkySettings == value))
				{
					skyParametersHash = -1;
					m_SkySettings = value;
					currentUpdateTime = 0f;
					if (m_SkySettings != null && skyRenderer == null)
					{
						Type skyRendererType = m_SkySettings.GetSkyRendererType();
						skyRenderer = (SkyRenderer)Activator.CreateInstance(skyRendererType);
						skyRenderer.Build();
					}
				}
			}
		}

		public CloudSettings cloudSettings
		{
			get
			{
				return m_CloudSettings;
			}
			set
			{
				if (cloudRenderer != null && (value == null || value.GetCloudRendererType() != cloudRenderer.GetType()))
				{
					cloudRenderer.Cleanup();
					cloudRenderer = null;
				}
				if (!(m_CloudSettings == value))
				{
					skyParametersHash = -1;
					m_CloudSettings = value;
					if (m_CloudSettings != null && cloudRenderer == null)
					{
						Type cloudRendererType = m_CloudSettings.GetCloudRendererType();
						cloudRenderer = (CloudRenderer)Activator.CreateInstance(cloudRendererType);
						cloudRenderer.Build();
					}
				}
			}
		}

		public VolumetricClouds volumetricClouds
		{
			get
			{
				return m_VolumetricClouds;
			}
			set
			{
				if (!(m_VolumetricClouds == value))
				{
					m_VolumetricClouds = value;
				}
			}
		}

		public void Cleanup()
		{
			if (skyRenderer != null)
			{
				skyRenderer.Cleanup();
			}
			if (cloudRenderer != null)
			{
				cloudRenderer.Cleanup();
			}
			HDRenderPipeline.currentPipeline?.skyManager.ReleaseCachedContext(cachedSkyRenderingContextId);
		}

		public bool IsValid()
		{
			return m_SkySettings != null;
		}

		public bool HasClouds()
		{
			return m_CloudSettings != null;
		}

		public bool HasVolumetricClouds()
		{
			return m_VolumetricClouds != null;
		}

		public void Reset()
		{
			if (skyRenderer != null)
			{
				skyRenderer.Reset();
			}
			if (cloudRenderer != null)
			{
				cloudRenderer.Reset();
			}
		}
	}
}
