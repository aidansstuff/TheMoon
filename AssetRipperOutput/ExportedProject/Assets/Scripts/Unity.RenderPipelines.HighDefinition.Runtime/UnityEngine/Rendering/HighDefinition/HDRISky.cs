using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[VolumeComponentMenuForRenderPipeline("Sky/HDRI Sky", new Type[] { typeof(HDRenderPipeline) })]
	[SkyUniqueID(1)]
	public class HDRISky : SkySettings, IVersionable<HDRISky.Version>
	{
		public enum DistortionMode
		{
			None = 0,
			Procedural = 1,
			Flowmap = 2
		}

		protected enum Version
		{
			Initial = 0,
			GlobalWind = 1
		}

		[Tooltip("Specify the cubemap HDRP uses to render the sky.")]
		public CubemapParameter hdriSky = new CubemapParameter(null);

		[Tooltip("Distortion mode to simulate sky movement.\nIn Scene View, requires Always Refresh to be enabled.")]
		public VolumeParameter<DistortionMode> distortionMode = new VolumeParameter<DistortionMode>();

		[Tooltip("Specify the flowmap HDRP uses for sky distortion (in LatLong layout).")]
		public Texture2DParameter flowmap = new Texture2DParameter(null);

		[Tooltip("Check this box if the flowmap covers only the upper part of the sky.")]
		public BoolParameter upperHemisphereOnly = new BoolParameter(value: true);

		public WindOrientationParameter scrollOrientation = new WindOrientationParameter();

		public WindSpeedParameter scrollSpeed = new WindSpeedParameter();

		[AdditionalProperty]
		[Tooltip("Enable or disable the backplate.")]
		public BoolParameter enableBackplate = new BoolParameter(value: false);

		[AdditionalProperty]
		[Tooltip("Backplate type.")]
		public BackplateTypeParameter backplateType = new BackplateTypeParameter(BackplateType.Disc);

		[AdditionalProperty]
		[Tooltip("Define the ground level of the Backplate.")]
		public FloatParameter groundLevel = new FloatParameter(0f);

		[AdditionalProperty]
		[Tooltip("Extent of the Backplate (if circle only the X value is considered).")]
		public Vector2Parameter scale = new Vector2Parameter(Vector2.one * 32f);

		[AdditionalProperty]
		[Tooltip("Backplate's projection distance to varying the cubemap projection on the plate.")]
		public MinFloatParameter projectionDistance = new MinFloatParameter(16f, 1E-07f);

		[AdditionalProperty]
		[Tooltip("Backplate rotation parameter for the geometry.")]
		public ClampedFloatParameter plateRotation = new ClampedFloatParameter(0f, 0f, 360f);

		[AdditionalProperty]
		[Tooltip("Backplate rotation parameter for the projected texture.")]
		public ClampedFloatParameter plateTexRotation = new ClampedFloatParameter(0f, 0f, 360f);

		[AdditionalProperty]
		[Tooltip("Backplate projection offset on the plane.")]
		public Vector2Parameter plateTexOffset = new Vector2Parameter(Vector2.zero);

		[AdditionalProperty]
		[Tooltip("Backplate blend parameter to blend the edge of the backplate with the background.")]
		public ClampedFloatParameter blendAmount = new ClampedFloatParameter(0f, 0f, 100f);

		[AdditionalProperty]
		[Tooltip("Backplate Shadow Tint projected on the plane.")]
		public ColorParameter shadowTint = new ColorParameter(Color.grey);

		[AdditionalProperty]
		[Tooltip("Allow backplate to receive shadow from point light.")]
		public BoolParameter pointLightShadow = new BoolParameter(value: false);

		[AdditionalProperty]
		[Tooltip("Allow backplate to receive shadow from directional light.")]
		public BoolParameter dirLightShadow = new BoolParameter(value: false);

		[AdditionalProperty]
		[Tooltip("Allow backplate to receive shadow from Area light.")]
		public BoolParameter rectLightShadow = new BoolParameter(value: false);

		protected static readonly MigrationDescription<Version, HDRISky> k_Migration = MigrationDescription.New<Version, HDRISky>(MigrationStep.New(Version.GlobalWind, delegate(HDRISky s)
		{
			float num = 0f;
			if (s.scrollDirection.overrideState)
			{
				num += s.scrollDirection.value + 90f;
			}
			if (s.rotation.overrideState)
			{
				num += s.rotation.value;
			}
			if (num != 0f)
			{
				s.scrollOrientation.Override(new WindParameter.WindParamaterValue
				{
					mode = WindParameter.WindOverrideMode.Custom,
					customValue = num % 360f
				});
			}
			if (s.m_ObsoleteScrollSpeed.overrideState)
			{
				s.scrollSpeed.Override(new WindParameter.WindParamaterValue
				{
					mode = WindParameter.WindOverrideMode.Custom,
					customValue = s.m_ObsoleteScrollSpeed.value * 200f
				});
			}
			s.distortionMode.value = (s.enableDistortion.value ? ((s.procedural.value || !s.procedural.overrideState) ? DistortionMode.Procedural : DistortionMode.Flowmap) : DistortionMode.None);
			s.distortionMode.overrideState = s.enableDistortion.overrideState || s.procedural.overrideState;
		}));

		[SerializeField]
		private Version m_SkyVersion;

		[SerializeField]
		[Obsolete("For Data Migration")]
		public BoolParameter enableDistortion = new BoolParameter(value: false);

		[SerializeField]
		[Obsolete("For Data Migration")]
		public BoolParameter procedural = new BoolParameter(value: true);

		[SerializeField]
		[Obsolete("For Data Migration")]
		public ClampedFloatParameter scrollDirection = new ClampedFloatParameter(0f, 0f, 360f);

		[SerializeField]
		[FormerlySerializedAs("scrollSpeed")]
		[Obsolete("For Data Migration")]
		private MinFloatParameter m_ObsoleteScrollSpeed = new MinFloatParameter(1f, 0f);

		Version IVersionable<Version>.version
		{
			get
			{
				return m_SkyVersion;
			}
			set
			{
				m_SkyVersion = value;
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			upperHemisphereLuxValue.overrideState = hdriSky.overrideState;
			upperHemisphereLuxColor.overrideState = hdriSky.overrideState;
		}

		public override int GetHashCode()
		{
			return ((((((((((((((((((base.GetHashCode() * 23 + hdriSky.GetHashCode()) * 23 + flowmap.GetHashCode()) * 23 + distortionMode.GetHashCode()) * 23 + upperHemisphereOnly.GetHashCode()) * 23 + scrollOrientation.GetHashCode()) * 23 + scrollSpeed.GetHashCode()) * 23 + enableBackplate.GetHashCode()) * 23 + backplateType.GetHashCode()) * 23 + groundLevel.GetHashCode()) * 23 + scale.GetHashCode()) * 23 + projectionDistance.GetHashCode()) * 23 + plateRotation.GetHashCode()) * 23 + plateTexRotation.GetHashCode()) * 23 + plateTexOffset.GetHashCode()) * 23 + blendAmount.GetHashCode()) * 23 + shadowTint.GetHashCode()) * 23 + pointLightShadow.GetHashCode()) * 23 + dirLightShadow.GetHashCode()) * 23 + rectLightShadow.GetHashCode();
		}

		public override bool SignificantlyDivergesFrom(SkySettings otherSettings)
		{
			HDRISky hDRISky = otherSettings as HDRISky;
			if (!base.SignificantlyDivergesFrom(otherSettings))
			{
				return hdriSky.value != hDRISky.hdriSky.value;
			}
			return true;
		}

		public override Type GetSkyRendererType()
		{
			return typeof(HDRISkyRenderer);
		}

		private void Awake()
		{
			k_Migration.Migrate(this);
		}
	}
}
