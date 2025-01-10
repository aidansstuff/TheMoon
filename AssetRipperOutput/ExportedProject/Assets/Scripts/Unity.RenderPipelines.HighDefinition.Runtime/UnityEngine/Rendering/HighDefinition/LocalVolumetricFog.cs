using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[ExecuteAlways]
	[AddComponentMenu("Rendering/Local Volumetric Fog")]
	public class LocalVolumetricFog : MonoBehaviour, IVersionable<LocalVolumetricFog.Version>
	{
		private enum Version
		{
			First = 0,
			ScaleIndependent = 1,
			FixUniformBlendDistanceToBeMetric = 2
		}

		public LocalVolumetricFogArtistParameters parameters = new LocalVolumetricFogArtistParameters(Color.white, 10f, 0f);

		public Action OnTextureUpdated;

		private static readonly MigrationDescription<Version, LocalVolumetricFog> k_Migration = MigrationDescription.New<Version, LocalVolumetricFog>(MigrationStep.New(Version.ScaleIndependent, delegate(LocalVolumetricFog data)
		{
			data.parameters.size = data.transform.lossyScale;
			data.parameters.m_EditorAdvancedFade = true;
		}), MigrationStep.New(Version.FixUniformBlendDistanceToBeMetric, delegate(LocalVolumetricFog data)
		{
			data.parameters.MigrateToFixUniformBlendDistanceToBeMetric();
		}));

		[SerializeField]
		private Version m_Version = MigrationDescription.LastVersion<Version>();

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

		internal void PrepareParameters(float time)
		{
			parameters.Update(time);
		}

		private void NotifyUpdatedTexure()
		{
			if (OnTextureUpdated != null)
			{
				OnTextureUpdated();
			}
		}

		private void OnEnable()
		{
			LocalVolumetricFogManager.manager.RegisterVolume(this);
		}

		private void OnDisable()
		{
			LocalVolumetricFogManager.manager.DeRegisterVolume(this);
		}

		private void OnValidate()
		{
			parameters.Constrain();
		}

		private void Awake()
		{
			k_Migration.Migrate(this);
		}
	}
}
