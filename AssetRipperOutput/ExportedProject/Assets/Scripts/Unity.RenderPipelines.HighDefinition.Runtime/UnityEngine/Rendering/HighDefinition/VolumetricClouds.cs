using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Sky/Volumetric Clouds", new Type[] { typeof(HDRenderPipeline) })]
	public sealed class VolumetricClouds : VolumeComponent, IVersionable<VolumetricClouds.Version>, ISerializationCallbackReceiver
	{
		public enum CloudControl
		{
			Simple = 0,
			Advanced = 1,
			Manual = 2
		}

		[Serializable]
		public sealed class CloudControlParameter : VolumeParameter<CloudControl>
		{
			public CloudControlParameter(CloudControl value, bool overrideState = false)
				: base(value, overrideState)
			{
			}
		}

		public enum CloudPresets
		{
			Sparse = 0,
			Cloudy = 1,
			Overcast = 2,
			Stormy = 3,
			Custom = 4
		}

		[Serializable]
		public sealed class CloudPresetsParameter : VolumeParameter<CloudPresets>
		{
			public CloudPresetsParameter(CloudPresets value, bool overrideState = false)
				: base(value, overrideState)
			{
			}
		}

		public enum CloudShadowResolution
		{
			VeryLow64 = 0x40,
			Low128 = 0x80,
			Medium256 = 0x100,
			High512 = 0x200,
			Ultra1024 = 0x400
		}

		[Serializable]
		public sealed class CloudShadowResolutionParameter : VolumeParameter<CloudShadowResolution>
		{
			public CloudShadowResolutionParameter(CloudShadowResolution value, bool overrideState = false)
				: base(value, overrideState)
			{
			}
		}

		public enum CloudMapResolution
		{
			Low32x32 = 0x20,
			Medium64x64 = 0x40,
			High128x128 = 0x80,
			Ultra256x256 = 0x100
		}

		[Serializable]
		public sealed class CloudMapResolutionParameter : VolumeParameter<CloudMapResolution>
		{
			public CloudMapResolutionParameter(CloudMapResolution value, bool overrideState = false)
				: base(value, overrideState)
			{
			}
		}

		public enum CloudErosionNoise
		{
			Worley32 = 0,
			Perlin32 = 1
		}

		[Serializable]
		public sealed class CloudErosionNoiseParameter : VolumeParameter<CloudErosionNoise>
		{
			public CloudErosionNoiseParameter(CloudErosionNoise value, bool overrideState = false)
				: base(value, overrideState)
			{
			}
		}

		public enum CloudFadeInMode
		{
			Automatic = 0,
			Manual = 1
		}

		[Serializable]
		public sealed class CloudFadeInModeParameter : VolumeParameter<CloudFadeInMode>
		{
			public CloudFadeInModeParameter(CloudFadeInMode value, bool overrideState = false)
				: base(value, overrideState)
			{
			}
		}

		private enum Version
		{
			Initial = 0,
			GlobalWind = 1,
			ShapeOffset = 2,
			Count = 3
		}

		public const int CloudShadowResolutionCount = 5;

		[Tooltip("Enable/Disable the volumetric clouds effect.")]
		public BoolParameter enable = new BoolParameter(value: false, BoolParameter.DisplayType.EnumPopup);

		[Tooltip("When enabled, clouds are part of the scene and you can interact with them. This means you can move around and inside the clouds, they can appear between the Camera and other GameObjects, and the Camera's clipping planes affect the clouds. When disabled, the clouds are part of the skybox. This means the clouds and their shadows appear relative to the Camera and always appear behind geometry.")]
		public BoolParameter localClouds = new BoolParameter(value: false);

		[Tooltip("Controls the curvature of the cloud volume which defines the distance at which the clouds intersect with the horizon.")]
		public ClampedFloatParameter earthCurvature = new ClampedFloatParameter(0f, 0f, 1f);

		[Tooltip("Tiling (x,y) of the cloud map.")]
		public Vector2Parameter cloudTiling = new Vector2Parameter(new Vector2(1f, 1f));

		[Tooltip("Offset (x,y) of the cloud map.")]
		public Vector2Parameter cloudOffset = new Vector2Parameter(new Vector2(0f, 0f));

		[Tooltip("Controls the altitude of the bottom of the volumetric clouds volume in meters.")]
		public MinFloatParameter bottomAltitude = new MinFloatParameter(1200f, 0.01f);

		[Tooltip("Controls the size of the volumetric clouds volume in meters.")]
		public MinFloatParameter altitudeRange = new MinFloatParameter(2000f, 100f);

		[Tooltip("Controls the mode in which the clouds fade in when close to the camera's near plane.")]
		public CloudFadeInModeParameter fadeInMode = new CloudFadeInModeParameter(CloudFadeInMode.Automatic);

		[Tooltip("Controls the minimal distance at which clouds start appearing.")]
		public MinFloatParameter fadeInStart = new MinFloatParameter(0f, 0f);

		[Tooltip("Controls the distance that it takes for the clouds to reach their complete density.")]
		public MinFloatParameter fadeInDistance = new MinFloatParameter(0f, 0f);

		[Tooltip("Controls the number of steps when evaluating the clouds' transmittance. A higher value may lead to a lower noise level and longer view distance, but at a higher cost.")]
		public ClampedIntParameter numPrimarySteps = new ClampedIntParameter(64, 32, 1024);

		[Tooltip("Controls the number of steps when evaluating the clouds' lighting. A higher value will lead to smoother lighting and improved self-shadowing, but at a higher cost.")]
		public ClampedIntParameter numLightSteps = new ClampedIntParameter(6, 1, 32);

		[Tooltip("Specifies the cloud map - Coverage (R), Rain (G), Type (B).")]
		public TextureParameter cloudMap = new TextureParameter(null, TextureDimension.Tex2D);

		[Tooltip("Specifies the lookup table for the clouds - Profile Coverage (R), Erosion (G), Ambient Occlusion (B).")]
		public TextureParameter cloudLut = new TextureParameter(null, TextureDimension.Tex2D);

		[Tooltip("Specifies the cloud control Mode: Simple, Advanced or Manual.")]
		public CloudControlParameter cloudControl = new CloudControlParameter(CloudControl.Simple);

		[SerializeField]
		[FormerlySerializedAs("cloudPreset")]
		private CloudPresetsParameter m_CloudPreset = new CloudPresetsParameter(CloudPresets.Cloudy);

		[Tooltip("Specifies the lower cloud layer distribution in the advanced mode.")]
		public TextureParameter cumulusMap = new TextureParameter(null, TextureDimension.Tex2D);

		[Tooltip("Overrides the coverage of the lower cloud layer specified in the cumulus map in the advanced mode.")]
		public ClampedFloatParameter cumulusMapMultiplier = new ClampedFloatParameter(1f, 0f, 1f);

		[Tooltip("Specifies the higher cloud layer distribution in the advanced mode.")]
		public TextureParameter altoStratusMap = new TextureParameter(null, TextureDimension.Tex2D);

		[Tooltip("Overrides the coverage of the higher cloud layer specified in the alto stratus map in the advanced mode.")]
		public ClampedFloatParameter altoStratusMapMultiplier = new ClampedFloatParameter(1f, 0f, 1f);

		[Tooltip("Specifies the anvil shaped clouds distribution in the advanced mode.")]
		public TextureParameter cumulonimbusMap = new TextureParameter(null, TextureDimension.Tex2D);

		[Tooltip("Overrides the coverage of the anvil shaped clouds specified in the cumulonimbus map in the advanced mode.")]
		public ClampedFloatParameter cumulonimbusMapMultiplier = new ClampedFloatParameter(1f, 0f, 1f);

		[Tooltip("Specifies the rain distribution in the advanced mode.")]
		public TextureParameter rainMap = new TextureParameter(null, TextureDimension.Tex2D);

		[Tooltip("Specifies the internal texture resolution used for the cloud map in the advanced mode. A lower value will lead to higher performance, but less precise cloud type transitions.")]
		public CloudMapResolutionParameter cloudMapResolution = new CloudMapResolutionParameter(CloudMapResolution.Medium64x64);

		[Tooltip("Controls the density (Y axis) of the volumetric clouds as a function of the height (X Axis) inside the cloud volume.")]
		public AnimationCurveParameter densityCurve = new AnimationCurveParameter(new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.15f, 1f), new Keyframe(1f, 0.1f)));

		[Tooltip("Controls the erosion (Y axis) of the volumetric clouds as a function of the height (X Axis) inside the cloud volume.")]
		public AnimationCurveParameter erosionCurve = new AnimationCurveParameter(new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.1f, 0.9f), new Keyframe(1f, 1f)));

		[Tooltip("Controls the ambient occlusion (Y axis) of the volumetric clouds as a function of the height (X Axis) inside the cloud volume.")]
		public AnimationCurveParameter ambientOcclusionCurve = new AnimationCurveParameter(new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.25f, 0.4f), new Keyframe(1f, 0f)));

		[Tooltip("Specifies the tint of the cloud scattering color.")]
		public ColorParameter scatteringTint = new ColorParameter(new Color(0f, 0f, 0f, 1f));

		[Tooltip("Controls the amount of local scattering in the clouds. A higher value may produce a more powdery or diffused aspect.")]
		[AdditionalProperty]
		public ClampedFloatParameter powderEffectIntensity = new ClampedFloatParameter(0.25f, 0f, 1f);

		[Tooltip("Controls the amount of multi-scattering inside the cloud.")]
		[AdditionalProperty]
		public ClampedFloatParameter multiScattering = new ClampedFloatParameter(0.5f, 0f, 1f);

		[Tooltip("Controls the global density of the cloud volume.")]
		public ClampedFloatParameter densityMultiplier = new ClampedFloatParameter(0.4f, 0f, 1f);

		[Tooltip("Controls the larger noise passing through the cloud coverage. A higher value will yield less cloud coverage and smaller clouds.")]
		public ClampedFloatParameter shapeFactor = new ClampedFloatParameter(0.9f, 0f, 1f);

		[Tooltip("Controls the size of the larger noise passing through the cloud coverage.")]
		public MinFloatParameter shapeScale = new MinFloatParameter(5f, 0.1f);

		[Tooltip("Controls the world space offset applied when evaluating the larger noise passing through the cloud coverage.")]
		public Vector3Parameter shapeOffset = new Vector3Parameter(Vector3.zero);

		[Tooltip("Controls the smaller noise on the edge of the clouds. A higher value will erode clouds more significantly.")]
		public ClampedFloatParameter erosionFactor = new ClampedFloatParameter(0.8f, 0f, 1f);

		[Tooltip("Controls the size of the smaller noise passing through the cloud coverage.")]
		public MinFloatParameter erosionScale = new MinFloatParameter(107f, 1f);

		[Tooltip("Controls the type of noise used to generate the smaller noise passing through the cloud coverage.")]
		[AdditionalProperty]
		public CloudErosionNoiseParameter erosionNoiseType = new CloudErosionNoiseParameter(CloudErosionNoise.Perlin32);

		[Tooltip("Controls the influence of the light probes on the cloud volume. A lower value will suppress the ambient light and produce darker clouds overall.")]
		public ClampedFloatParameter ambientLightProbeDimmer = new ClampedFloatParameter(1f, 0f, 1f);

		[Tooltip("Controls the influence of the sun light on the cloud volume. A lower value will suppress the sun light and produce darker clouds overall.")]
		public ClampedFloatParameter sunLightDimmer = new ClampedFloatParameter(1f, 0f, 1f);

		[Tooltip("Controls how much Erosion Factor is taken into account when computing ambient occlusion. The Erosion Factor parameter is editable in the custom preset, Advanced and Manual Modes.")]
		[AdditionalProperty]
		public ClampedFloatParameter erosionOcclusion = new ClampedFloatParameter(0.1f, 0f, 1f);

		[Tooltip("Sets the global horizontal wind speed in kilometers per hour.\nThis value can be relative to the Global Wind Speed defined in the Visual Environment.")]
		public WindSpeedParameter globalWindSpeed = new WindSpeedParameter();

		[Tooltip("Controls the orientation of the wind relative to the X world vector.\nThis value can be relative to the Global Wind Orientation defined in the Visual Environment.")]
		public WindOrientationParameter orientation = new WindOrientationParameter();

		[AdditionalProperty]
		[Tooltip("Controls the intensity of the wind-based altitude distortion of the clouds.")]
		public ClampedFloatParameter altitudeDistortion = new ClampedFloatParameter(0.25f, -1f, 1f);

		[Tooltip("Controls the multiplier to the speed of the cloud map.")]
		[AdditionalProperty]
		public ClampedFloatParameter cloudMapSpeedMultiplier = new ClampedFloatParameter(0.5f, 0f, 1f);

		[Tooltip("Controls the multiplier to the speed of the larger cloud shapes.")]
		[AdditionalProperty]
		public ClampedFloatParameter shapeSpeedMultiplier = new ClampedFloatParameter(1f, 0f, 1f);

		[Tooltip("Controls the multiplier to the speed of the erosion cloud shapes.")]
		[AdditionalProperty]
		public ClampedFloatParameter erosionSpeedMultiplier = new ClampedFloatParameter(0.25f, 0f, 1f);

		[Tooltip("Controls the vertical wind speed of the larger cloud shapes.")]
		[AdditionalProperty]
		public FloatParameter verticalShapeWindSpeed = new FloatParameter(0f);

		[Tooltip("Controls the vertical wind speed of the erosion cloud shapes.")]
		[AdditionalProperty]
		public FloatParameter verticalErosionWindSpeed = new FloatParameter(0f);

		[Tooltip("Temporal accumulation increases the visual quality of clouds by decreasing the noise. A higher value will give you better quality but can create ghosting.")]
		public ClampedFloatParameter temporalAccumulationFactor = new ClampedFloatParameter(0.95f, 0f, 1f);

		[Tooltip("Enable/Disable the volumetric clouds ghosting reduction. When enabled, reduces significantly the ghosting of the volumetric clouds, but may introduce some flickering at lower temporal accumulation factors.")]
		public BoolParameter ghostingReduction = new BoolParameter(value: false);

		[Tooltip("Specifies the strength of the perceptual blending for the volumetric clouds. This value should be treated as flag and only be set to 0.0 or 1.0.")]
		public ClampedFloatParameter perceptualBlending = new ClampedFloatParameter(1f, 0f, 1f);

		[Tooltip("Enable/Disable the volumetric clouds shadow. This will override the cookie of your directional light and the cloud layer shadow (if active).")]
		public BoolParameter shadows = new BoolParameter(value: false);

		[Tooltip("Specifies the resolution of the volumetric clouds shadow map.")]
		public CloudShadowResolutionParameter shadowResolution = new CloudShadowResolutionParameter(CloudShadowResolution.Medium256);

		[Tooltip("Controls the vertical offset applied to compute the volumetric clouds shadow in meters. To have accurate results, enter the average height at which the volumetric clouds shadow is received.")]
		public FloatParameter shadowPlaneHeightOffset = new FloatParameter(0f);

		[Tooltip("Sets the size of the area covered by shadow around the camera.")]
		[AdditionalProperty]
		public MinFloatParameter shadowDistance = new MinFloatParameter(8000f, 1000f);

		[Tooltip("Controls the opacity of the volumetric clouds shadow.")]
		[AdditionalProperty]
		public ClampedFloatParameter shadowOpacity = new ClampedFloatParameter(1f, 0f, 1f);

		[Tooltip("Controls the shadow opacity when outside the area covered by the volumetric clouds shadow.")]
		[AdditionalProperty]
		public ClampedFloatParameter shadowOpacityFallback = new ClampedFloatParameter(0f, 0f, 1f);

		private static readonly MigrationDescription<Version, VolumetricClouds> k_Migration = MigrationDescription.New<Version, VolumetricClouds>(MigrationStep.New(Version.GlobalWind, delegate(VolumetricClouds c)
		{
			c.globalWindSpeed.overrideState = c.m_ObsoleteWindSpeed.overrideState;
			c.globalWindSpeed.value = new WindParameter.WindParamaterValue
			{
				mode = WindParameter.WindOverrideMode.Custom,
				customValue = c.m_ObsoleteWindSpeed.value
			};
			c.orientation.overrideState = c.m_ObsoleteOrientation.overrideState;
			c.orientation.value = new WindParameter.WindParamaterValue
			{
				mode = WindParameter.WindOverrideMode.Custom,
				customValue = c.m_ObsoleteOrientation.value
			};
		}), MigrationStep.New(Version.ShapeOffset, delegate(VolumetricClouds c)
		{
			c.shapeOffset.overrideState = c.m_ObsoleteShapeOffsetX.overrideState || c.m_ObsoleteShapeOffsetY.overrideState || c.m_ObsoleteShapeOffsetZ.overrideState;
			c.shapeOffset.value = new Vector3(c.m_ObsoleteShapeOffsetX.value, c.m_ObsoleteShapeOffsetY.value, c.m_ObsoleteShapeOffsetZ.value);
		}));

		[SerializeField]
		private Version m_Version = Version.Count;

		[SerializeField]
		[FormerlySerializedAs("globalWindSpeed")]
		[Obsolete("For Data Migration")]
		private MinFloatParameter m_ObsoleteWindSpeed = new MinFloatParameter(1f, 0f);

		[SerializeField]
		[FormerlySerializedAs("orientation")]
		[Obsolete("For Data Migration")]
		private ClampedFloatParameter m_ObsoleteOrientation = new ClampedFloatParameter(0f, 0f, 360f);

		[SerializeField]
		[FormerlySerializedAs("shapeOffsetX")]
		[Obsolete("For Data Migration")]
		private FloatParameter m_ObsoleteShapeOffsetX = new FloatParameter(0f);

		[SerializeField]
		[FormerlySerializedAs("shapeOffsetY")]
		[Obsolete("For Data Migration")]
		private FloatParameter m_ObsoleteShapeOffsetY = new FloatParameter(0f);

		[SerializeField]
		[FormerlySerializedAs("shapeOffsetZ")]
		[Obsolete("For Data Migration")]
		private FloatParameter m_ObsoleteShapeOffsetZ = new FloatParameter(0f);

		public CloudPresets cloudPreset
		{
			get
			{
				return m_CloudPreset.value;
			}
			set
			{
				m_CloudPreset.value = value;
				ApplyCurrentCloudPreset();
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

		private void ApplyCurrentCloudPreset()
		{
			switch (cloudPreset)
			{
			case CloudPresets.Sparse:
				densityMultiplier.value = 0.4f;
				shapeFactor.value = 0.95f;
				shapeScale.value = 5f;
				erosionFactor.value = 0.8f;
				erosionScale.value = 107f;
				densityCurve.value = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.05f, 1f), new Keyframe(0.75f, 1f), new Keyframe(1f, 0f));
				erosionCurve.value = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.1f, 0.9f), new Keyframe(1f, 1f));
				ambientOcclusionCurve.value = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.25f, 0.5f), new Keyframe(1f, 0f));
				bottomAltitude.value = 3000f;
				altitudeRange.value = 1000f;
				break;
			case CloudPresets.Cloudy:
				densityMultiplier.value = 0.4f;
				shapeFactor.value = 0.9f;
				shapeScale.value = 5f;
				erosionFactor.value = 0.8f;
				erosionScale.value = 107f;
				densityCurve.value = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.15f, 1f), new Keyframe(1f, 0.1f));
				erosionCurve.value = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.1f, 0.9f), new Keyframe(1f, 1f));
				ambientOcclusionCurve.value = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.25f, 0.4f), new Keyframe(1f, 0f));
				bottomAltitude.value = 1200f;
				altitudeRange.value = 2000f;
				break;
			case CloudPresets.Overcast:
				densityMultiplier.value = 0.3f;
				shapeFactor.value = 0.5f;
				shapeScale.value = 5f;
				erosionFactor.value = 0.8f;
				erosionScale.value = 107f;
				densityCurve.value = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.05f, 1f), new Keyframe(0.9f, 0f), new Keyframe(1f, 0f));
				erosionCurve.value = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.1f, 0.9f), new Keyframe(1f, 1f));
				ambientOcclusionCurve.value = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 0f));
				bottomAltitude.value = 1500f;
				altitudeRange.value = 2500f;
				break;
			case CloudPresets.Stormy:
				densityMultiplier.value = 0.35f;
				shapeFactor.value = 0.85f;
				shapeScale.value = 5f;
				erosionFactor.value = 0.749f;
				erosionScale.value = 107f;
				densityCurve.value = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.037f, 1f), new Keyframe(0.6f, 1f), new Keyframe(1f, 0f));
				erosionCurve.value = new AnimationCurve(new Keyframe(0f, 1f), new Keyframe(0.05f, 0.8f), new Keyframe(0.2438f, 0.9498f), new Keyframe(0.5f, 1f), new Keyframe(0.93f, 0.9268f), new Keyframe(1f, 1f));
				ambientOcclusionCurve.value = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(0.1f, 0.4f), new Keyframe(1f, 0f));
				bottomAltitude.value = 1000f;
				altitudeRange.value = 5000f;
				break;
			}
		}

		private VolumetricClouds()
		{
			base.displayName = "Volumetric Clouds";
		}

		public void OnBeforeSerialize()
		{
			if (m_Version == Version.Count)
			{
				m_Version = Version.ShapeOffset;
			}
		}

		public void OnAfterDeserialize()
		{
			if (m_Version == Version.Count)
			{
				m_Version = Version.Initial;
			}
		}

		private void Awake()
		{
			k_Migration.Migrate(this);
		}
	}
}
