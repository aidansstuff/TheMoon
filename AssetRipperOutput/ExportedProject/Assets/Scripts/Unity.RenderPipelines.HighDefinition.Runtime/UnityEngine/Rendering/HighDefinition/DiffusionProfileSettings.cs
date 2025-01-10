using System;

namespace UnityEngine.Rendering.HighDefinition
{
	public sealed class DiffusionProfileSettings : ScriptableObject, IVersionable<DiffusionProfileSettings.Version>
	{
		private enum Version
		{
			Initial = 0,
			DiffusionProfileRework = 1,
			SplitScatteringDistance = 2
		}

		[SerializeField]
		internal DiffusionProfile profile;

		[NonSerialized]
		internal Vector4 worldScaleAndFilterRadiusAndThicknessRemap;

		[NonSerialized]
		internal Vector4 shapeParamAndMaxScatterDist;

		[NonSerialized]
		internal Vector4 transmissionTintAndFresnel0;

		[NonSerialized]
		internal Vector4 disabledTransmissionTintAndFresnel0;

		[NonSerialized]
		internal int updateCount;

		[SerializeField]
		private Version m_Version = MigrationDescription.LastVersion<Version>();

		[Obsolete("Profiles are obsolete, only one diffusion profile per asset is allowed.")]
		internal DiffusionProfile[] profiles;

		private static readonly MigrationDescription<Version, DiffusionProfileSettings> k_Migration = MigrationDescription.New<Version, DiffusionProfileSettings>(MigrationStep.New<Version, DiffusionProfileSettings>(Version.DiffusionProfileRework, delegate
		{
		}), MigrationStep.New(Version.SplitScatteringDistance, delegate(DiffusionProfileSettings d)
		{
			d.scatteringDistance = d.profile.scatteringDistance;
		}));

		public Color scatteringDistance
		{
			get
			{
				return profile.scatteringDistance * profile.scatteringDistanceMultiplier;
			}
			set
			{
				HDUtils.ConvertHDRColorToLDR(value, out profile.scatteringDistance, out profile.scatteringDistanceMultiplier);
				profile.Validate();
				UpdateCache();
			}
		}

		public float maximumRadius => profile.filterRadius;

		public float indexOfRefraction
		{
			get
			{
				return profile.ior;
			}
			set
			{
				profile.ior = value;
				profile.Validate();
				UpdateCache();
			}
		}

		public float worldScale
		{
			get
			{
				return profile.worldScale;
			}
			set
			{
				profile.worldScale = value;
				profile.Validate();
				UpdateCache();
			}
		}

		public Color transmissionTint
		{
			get
			{
				return profile.transmissionTint;
			}
			set
			{
				profile.transmissionTint = value;
				profile.Validate();
				UpdateCache();
			}
		}

		[Obsolete("Profiles are obsolete, only one diffusion profile per asset is allowed.")]
		internal DiffusionProfile this[int index] => profile;

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

		private void OnEnable()
		{
			if (profile == null)
			{
				profile = new DiffusionProfile(dontUseDefaultConstructor: true);
			}
			profile.Validate();
			UpdateCache();
		}

		internal void UpdateCache()
		{
			worldScaleAndFilterRadiusAndThicknessRemap = new Vector4(profile.worldScale, profile.filterRadius, profile.thicknessRemap.x, profile.thicknessRemap.y - profile.thicknessRemap.x);
			shapeParamAndMaxScatterDist = profile.shapeParam;
			shapeParamAndMaxScatterDist.w = profile.maxScatteringDistance;
			float num = (profile.ior - 1f) / (profile.ior + 1f);
			num *= num;
			transmissionTintAndFresnel0 = new Vector4(profile.transmissionTint.r * 0.25f, profile.transmissionTint.g * 0.25f, profile.transmissionTint.b * 0.25f, num);
			disabledTransmissionTintAndFresnel0 = new Vector4(0f, 0f, 0f, num);
			updateCount++;
		}

		internal bool HasChanged(int update)
		{
			return update == updateCount;
		}

		internal void SetDefaultParams()
		{
			worldScaleAndFilterRadiusAndThicknessRemap = new Vector4(1f, 0f, 0f, 1f);
			shapeParamAndMaxScatterDist = new Vector4(16777216f, 16777216f, 16777216f, 0f);
			transmissionTintAndFresnel0.w = 0.04f;
		}
	}
}
