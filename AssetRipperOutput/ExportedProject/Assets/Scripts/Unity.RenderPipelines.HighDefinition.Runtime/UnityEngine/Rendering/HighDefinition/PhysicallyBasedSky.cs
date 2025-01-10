using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[VolumeComponentMenuForRenderPipeline("Sky/Physically Based Sky", new Type[] { typeof(HDRenderPipeline) })]
	[SkyUniqueID(4)]
	public class PhysicallyBasedSky : SkySettings, IVersionable<PhysicallyBasedSky.Version>
	{
		protected enum Version
		{
			Initial = 0,
			TypeEnum = 1
		}

		private const float k_DefaultEarthRadius = 6378100f;

		private const float k_DefaultAirScatteringR = 5.8E-06f;

		private const float k_DefaultAirScatteringG = 1.35E-05f;

		private const float k_DefaultAirScatteringB = 3.3099997E-05f;

		private const float k_DefaultAirScaleHeight = 8000f;

		private const float k_DefaultAirAlbedoR = 0.9f;

		private const float k_DefaultAirAlbedoG = 0.9f;

		private const float k_DefaultAirAlbedoB = 1f;

		private const float k_DefaultAerosolScaleHeight = 1200f;

		private static readonly float k_DefaultAerosolMaximumAltitude = LayerDepthFromScaleHeight(1200f);

		public PhysicallyBasedSkyModelParameter type = new PhysicallyBasedSkyModelParameter(PhysicallyBasedSkyModel.EarthAdvanced);

		[Tooltip("When enabled, you can define the planet in terms of a world-space position and radius. Otherwise, the planet is always below the Camera in the world-space x-z plane.")]
		public BoolParameter sphericalMode = new BoolParameter(value: true);

		[Tooltip("Sets the world-space y coordinate of the planet's sea level in meters.")]
		public FloatParameter seaLevel = new FloatParameter(0f);

		[Tooltip("Sets the radius of the planet in meters. This is distance from the center of the planet to the sea level.")]
		public MinFloatParameter planetaryRadius = new MinFloatParameter(6378100f, 0f);

		[Tooltip("Sets the world-space position of the planet's center in meters.")]
		public Vector3Parameter planetCenterPosition = new Vector3Parameter(new Vector3(0f, -6378100f, 0f));

		[Tooltip("Controls the red color channel opacity of air at the point in the sky directly above the observer (zenith).")]
		public ClampedFloatParameter airDensityR = new ClampedFloatParameter(ZenithOpacityFromExtinctionAndScaleHeight(5.8E-06f, 8000f), 0f, 1f);

		[Tooltip("Controls the green color channel opacity of air at the point in the sky directly above the observer (zenith).")]
		public ClampedFloatParameter airDensityG = new ClampedFloatParameter(ZenithOpacityFromExtinctionAndScaleHeight(1.35E-05f, 8000f), 0f, 1f);

		[Tooltip("Controls the blue color channel opacity of air at the point in the sky directly above the observer (zenith).")]
		public ClampedFloatParameter airDensityB = new ClampedFloatParameter(ZenithOpacityFromExtinctionAndScaleHeight(3.3099997E-05f, 8000f), 0f, 1f);

		[Tooltip("Specifies the color that HDRP tints the air to. This controls the single scattering albedo of air molecules (per color channel). A value of 0 results in absorbing molecules, and a value of 1 results in scattering ones.")]
		public ColorParameter airTint = new ColorParameter(new Color(0.9f, 0.9f, 1f), hdr: false, showAlpha: false, showEyeDropper: true);

		[Tooltip("Sets the depth, in meters, of the atmospheric layer, from sea level, composed of air particles. Controls the rate of height-based density falloff.")]
		public MinFloatParameter airMaximumAltitude = new MinFloatParameter(LayerDepthFromScaleHeight(8000f), 0f);

		[Tooltip("Controls the opacity of aerosols at the point in the sky directly above the observer (zenith).")]
		public ClampedFloatParameter aerosolDensity = new ClampedFloatParameter(ZenithOpacityFromExtinctionAndScaleHeight(1E-05f, 1200f), 0f, 1f);

		[Tooltip("Specifies the color that HDRP tints aerosols to. This controls the single scattering albedo of aerosol molecules (per color channel). A value of 0 results in absorbing molecules, and a value of 1 results in scattering ones.")]
		public ColorParameter aerosolTint = new ColorParameter(new Color(0.9f, 0.9f, 0.9f), hdr: false, showAlpha: false, showEyeDropper: true);

		[Tooltip("Sets the depth, in meters, of the atmospheric layer, from sea level, composed of aerosol particles. Controls the rate of height-based density falloff.")]
		public MinFloatParameter aerosolMaximumAltitude = new MinFloatParameter(k_DefaultAerosolMaximumAltitude, 0f);

		[Tooltip("Controls the direction of anisotropy. Set this to a positive value for forward scattering, a negative value for backward scattering, or 0 for isotropic scattering.")]
		public ClampedFloatParameter aerosolAnisotropy = new ClampedFloatParameter(0f, -1f, 1f);

		[Tooltip("Sets the number of scattering events. This increases the quality of the sky visuals but also increases the pre-computation time.")]
		public ClampedIntParameter numberOfBounces = new ClampedIntParameter(3, 1, 10);

		[Tooltip("Specifies a color that HDRP uses to tint the Ground Color Texture.")]
		public ColorParameter groundTint = new ColorParameter(new Color(0.4f, 0.25f, 0.15f), hdr: false, showAlpha: false, showEyeDropper: false);

		[Tooltip("Specifies a Texture that represents the planet's surface. Does not affect the precomputation.")]
		public CubemapParameter groundColorTexture = new CubemapParameter(null);

		[Tooltip("Specifies a Texture that represents the emissive areas of the planet's surface. Does not affect the precomputation.")]
		public CubemapParameter groundEmissionTexture = new CubemapParameter(null);

		[Tooltip("Sets the multiplier that HDRP applies to the Ground Emission Texture.")]
		public MinFloatParameter groundEmissionMultiplier = new MinFloatParameter(1f, 0f);

		[Tooltip("Sets the orientation of the planet. Does not affect the precomputation.")]
		public Vector3Parameter planetRotation = new Vector3Parameter(Vector3.zero);

		[Tooltip("Specifies a Texture that represents the emissive areas of space. Does not affect the precomputation.")]
		public CubemapParameter spaceEmissionTexture = new CubemapParameter(null);

		[Tooltip("Sets the multiplier that HDRP applies to the Space Emission Texture. Does not affect the precomputation.")]
		public MinFloatParameter spaceEmissionMultiplier = new MinFloatParameter(1f, 0f);

		[Tooltip("Sets the orientation of space. Does not affect the precomputation.")]
		public Vector3Parameter spaceRotation = new Vector3Parameter(Vector3.zero);

		[Tooltip("Controls the saturation of the sky color. Does not affect the precomputation.")]
		public ClampedFloatParameter colorSaturation = new ClampedFloatParameter(1f, 0f, 1f);

		[Tooltip("Controls the saturation of the sky opacity. Does not affect the precomputation.")]
		public ClampedFloatParameter alphaSaturation = new ClampedFloatParameter(1f, 0f, 1f);

		[Tooltip("Sets the multiplier that HDRP applies to the opacity of the sky. Does not affect the precomputation.")]
		public ClampedFloatParameter alphaMultiplier = new ClampedFloatParameter(1f, 0f, 1f);

		[Tooltip("Specifies a color that HDRP uses to tint the sky at the horizon. Does not affect the precomputation.")]
		public ColorParameter horizonTint = new ColorParameter(Color.white, hdr: false, showAlpha: false, showEyeDropper: false);

		[Tooltip("Specifies a color that HDRP uses to tint the point in the sky directly above the observer (the zenith). Does not affect the precomputation.")]
		public ColorParameter zenithTint = new ColorParameter(Color.white, hdr: false, showAlpha: false, showEyeDropper: false);

		[Tooltip("Controls how HDRP blends between the Horizon Tint and Zenith Tint. Does not affect the precomputation.")]
		public ClampedFloatParameter horizonZenithShift = new ClampedFloatParameter(0f, -1f, 1f);

		protected static readonly MigrationDescription<Version, PhysicallyBasedSky> k_Migration = MigrationDescription.New<Version, PhysicallyBasedSky>(MigrationStep.New(Version.TypeEnum, delegate(PhysicallyBasedSky p)
		{
			p.type.value = (p.m_ObsoleteEarthPreset.value ? PhysicallyBasedSkyModel.EarthAdvanced : PhysicallyBasedSkyModel.Custom);
			p.type.overrideState = p.m_ObsoleteEarthPreset.overrideState;
		}));

		[SerializeField]
		private Version m_SkyVersion;

		[SerializeField]
		[FormerlySerializedAs("earthPreset")]
		[Obsolete("For Data Migration")]
		private BoolParameter m_ObsoleteEarthPreset = new BoolParameter(value: true);

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

		internal static float ScaleHeightFromLayerDepth(float d)
		{
			return d * 0.144765f;
		}

		internal static float LayerDepthFromScaleHeight(float H)
		{
			return H / 0.144765f;
		}

		internal static float ExtinctionFromZenithOpacityAndScaleHeight(float alpha, float H)
		{
			float num = Mathf.Min(alpha, 0.999999f);
			return (0f - Mathf.Log(1f - num, MathF.E)) / H;
		}

		internal static float ZenithOpacityFromExtinctionAndScaleHeight(float ext, float H)
		{
			float num = ext * H;
			return 1f - Mathf.Exp(0f - num);
		}

		internal float GetAirScaleHeight()
		{
			if (type.value != PhysicallyBasedSkyModel.Custom)
			{
				return 8000f;
			}
			return ScaleHeightFromLayerDepth(airMaximumAltitude.value);
		}

		internal float GetMaximumAltitude()
		{
			if (type.value == PhysicallyBasedSkyModel.Custom)
			{
				return Mathf.Max(airMaximumAltitude.value, aerosolMaximumAltitude.value);
			}
			float b = ((type.value == PhysicallyBasedSkyModel.EarthSimple) ? k_DefaultAerosolMaximumAltitude : aerosolMaximumAltitude.value);
			return Mathf.Max(LayerDepthFromScaleHeight(8000f), b);
		}

		internal float GetPlanetaryRadius()
		{
			if (type.value != PhysicallyBasedSkyModel.Custom)
			{
				return 6378100f;
			}
			return planetaryRadius.value;
		}

		internal Vector3 GetPlanetCenterPosition(Vector3 camPosWS)
		{
			if (sphericalMode.value && type.value != 0)
			{
				return planetCenterPosition.value;
			}
			float num = GetPlanetaryRadius();
			float value = seaLevel.value;
			return new Vector3(camPosWS.x, 0f - num + value, camPosWS.z);
		}

		internal Vector3 GetAirExtinctionCoefficient()
		{
			Vector3 result = default(Vector3);
			if (type.value != PhysicallyBasedSkyModel.Custom)
			{
				result.x = 5.8E-06f;
				result.y = 1.35E-05f;
				result.z = 3.3099997E-05f;
			}
			else
			{
				result.x = ExtinctionFromZenithOpacityAndScaleHeight(airDensityR.value, GetAirScaleHeight());
				result.y = ExtinctionFromZenithOpacityAndScaleHeight(airDensityG.value, GetAirScaleHeight());
				result.z = ExtinctionFromZenithOpacityAndScaleHeight(airDensityB.value, GetAirScaleHeight());
			}
			return result;
		}

		internal Vector3 GetAirAlbedo()
		{
			Vector3 result = default(Vector3);
			if (type.value != PhysicallyBasedSkyModel.Custom)
			{
				result.x = 0.9f;
				result.y = 0.9f;
				result.z = 1f;
			}
			else
			{
				result.x = airTint.value.r;
				result.y = airTint.value.g;
				result.z = airTint.value.b;
			}
			return result;
		}

		internal Vector3 GetAirScatteringCoefficient()
		{
			Vector3 airExtinctionCoefficient = GetAirExtinctionCoefficient();
			Vector3 airAlbedo = GetAirAlbedo();
			return new Vector3(airExtinctionCoefficient.x * airAlbedo.x, airExtinctionCoefficient.y * airAlbedo.y, airExtinctionCoefficient.z * airAlbedo.z);
		}

		internal float GetAerosolScaleHeight()
		{
			if (type.value == PhysicallyBasedSkyModel.EarthSimple)
			{
				return 1200f;
			}
			return ScaleHeightFromLayerDepth(aerosolMaximumAltitude.value);
		}

		internal float GetAerosolAnisotropy()
		{
			if (type.value == PhysicallyBasedSkyModel.EarthSimple)
			{
				return 0f;
			}
			return aerosolAnisotropy.value;
		}

		internal float GetAerosolExtinctionCoefficient()
		{
			return ExtinctionFromZenithOpacityAndScaleHeight(aerosolDensity.value, GetAerosolScaleHeight());
		}

		internal Vector3 GetAerosolScatteringCoefficient()
		{
			float aerosolExtinctionCoefficient = GetAerosolExtinctionCoefficient();
			return new Vector3(aerosolExtinctionCoefficient * aerosolTint.value.r, aerosolExtinctionCoefficient * aerosolTint.value.g, aerosolExtinctionCoefficient * aerosolTint.value.b);
		}

		private PhysicallyBasedSky()
		{
			base.displayName = "Physically Based Sky";
		}

		internal int GetPrecomputationHashCode()
		{
			return ((((((((((((base.GetHashCode() * 23 + type.GetHashCode()) * 23 + planetaryRadius.GetHashCode()) * 23 + groundTint.GetHashCode()) * 23 + airMaximumAltitude.GetHashCode()) * 23 + airDensityR.GetHashCode()) * 23 + airDensityG.GetHashCode()) * 23 + airDensityB.GetHashCode()) * 23 + airTint.GetHashCode()) * 23 + aerosolMaximumAltitude.GetHashCode()) * 23 + aerosolDensity.GetHashCode()) * 23 + aerosolTint.GetHashCode()) * 23 + aerosolAnisotropy.GetHashCode()) * 23 + numberOfBounces.GetHashCode();
		}

		public override int GetHashCode(Camera camera)
		{
			int hashCode = GetHashCode();
			Vector3 position = camera.transform.position;
			float num = Vector3.Distance(position, GetPlanetCenterPosition(position));
			float num2 = GetPlanetaryRadius();
			return hashCode * 23 + (num > num2).GetHashCode();
		}

		public override int GetHashCode()
		{
			int precomputationHashCode = GetPrecomputationHashCode();
			precomputationHashCode = precomputationHashCode * 23 + sphericalMode.GetHashCode();
			precomputationHashCode = precomputationHashCode * 23 + seaLevel.GetHashCode();
			precomputationHashCode = precomputationHashCode * 23 + planetCenterPosition.GetHashCode();
			precomputationHashCode = precomputationHashCode * 23 + planetRotation.GetHashCode();
			if (groundColorTexture.value != null)
			{
				precomputationHashCode = precomputationHashCode * 23 + groundColorTexture.GetHashCode();
			}
			if (groundEmissionTexture.value != null)
			{
				precomputationHashCode = precomputationHashCode * 23 + groundEmissionTexture.GetHashCode();
			}
			precomputationHashCode = precomputationHashCode * 23 + groundEmissionMultiplier.GetHashCode();
			precomputationHashCode = precomputationHashCode * 23 + spaceRotation.GetHashCode();
			if (spaceEmissionTexture.value != null)
			{
				precomputationHashCode = precomputationHashCode * 23 + spaceEmissionTexture.GetHashCode();
			}
			precomputationHashCode = precomputationHashCode * 23 + spaceEmissionMultiplier.GetHashCode();
			precomputationHashCode = precomputationHashCode * 23 + colorSaturation.GetHashCode();
			precomputationHashCode = precomputationHashCode * 23 + alphaSaturation.GetHashCode();
			precomputationHashCode = precomputationHashCode * 23 + alphaMultiplier.GetHashCode();
			precomputationHashCode = precomputationHashCode * 23 + horizonTint.GetHashCode();
			precomputationHashCode = precomputationHashCode * 23 + zenithTint.GetHashCode();
			return precomputationHashCode * 23 + horizonZenithShift.GetHashCode();
		}

		private static float Saturate(float x)
		{
			return Mathf.Max(0f, Mathf.Min(x, 1f));
		}

		private static float Rcp(float x)
		{
			return 1f / x;
		}

		private static float Rsqrt(float x)
		{
			return Rcp(Mathf.Sqrt(x));
		}

		private static float ComputeCosineOfHorizonAngle(float r, float R)
		{
			float num = R * Rcp(r);
			return 0f - Mathf.Sqrt(Saturate(1f - num * num));
		}

		private static float ChapmanUpperApprox(float z, float cosTheta)
		{
			float num = 0.761643f * (1f + 2f * z - cosTheta * cosTheta * z);
			float x = cosTheta * z + Mathf.Sqrt(z * (1.47721f + 0.273828f * (cosTheta * cosTheta * z)));
			return 0.5f * cosTheta + num * Rcp(x);
		}

		private static float ChapmanHorizontal(float z)
		{
			float num = Rsqrt(z);
			float num2 = z * num;
			return 0.626657f * (num + 2f * num2);
		}

		private static Vector3 ComputeAtmosphericOpticalDepth(float airScaleHeight, float aerosolScaleHeight, in Vector3 airExtinctionCoefficient, float aerosolExtinctionCoefficient, float R, float r, float cosTheta, bool alwaysAboveHorizon = false)
		{
			Vector2 vector = new Vector2(airScaleHeight, aerosolScaleHeight);
			Vector2 vector2 = new Vector2(Rcp(vector.x), Rcp(vector.y));
			Vector2 vector3 = r * vector2;
			Vector2 vector4 = R * vector2;
			float num = ComputeCosineOfHorizonAngle(r, R);
			float num2 = Mathf.Sqrt(Saturate(1f - cosTheta * cosTheta));
			Vector2 vector5 = default(Vector2);
			vector5.x = ChapmanUpperApprox(vector3.x, Mathf.Abs(cosTheta)) * Mathf.Exp(vector4.x - vector3.x);
			vector5.y = ChapmanUpperApprox(vector3.y, Mathf.Abs(cosTheta)) * Mathf.Exp(vector4.y - vector3.y);
			if (!alwaysAboveHorizon && cosTheta < num)
			{
				float num3 = r / R * num2;
				float cosTheta2 = Mathf.Sqrt(Saturate(1f - num3 * num3));
				Vector2 vector6 = default(Vector2);
				vector6.x = ChapmanUpperApprox(vector4.x, cosTheta2);
				vector6.y = ChapmanUpperApprox(vector4.y, cosTheta2);
				vector5 = vector6 - vector5;
			}
			else if (cosTheta < 0f)
			{
				Vector2 vector7 = vector3 * num2;
				Vector2 vector8 = new Vector2(Mathf.Exp(vector4.x - vector7.x), Mathf.Exp(vector4.x - vector7.x));
				Vector2 vector9 = default(Vector2);
				vector9.x = 2f * ChapmanHorizontal(vector7.x);
				vector9.y = 2f * ChapmanHorizontal(vector7.y);
				vector5 = vector9 * vector8 - vector5;
			}
			Vector2 vector10 = vector5 * vector;
			Vector3 vector11 = airExtinctionCoefficient;
			return new Vector3(vector10.x * vector11.x + vector10.y * aerosolExtinctionCoefficient, vector10.x * vector11.y + vector10.y * aerosolExtinctionCoefficient, vector10.x * vector11.z + vector10.y * aerosolExtinctionCoefficient);
		}

		internal static Vector3 EvaluateAtmosphericAttenuation(float airScaleHeight, float aerosolScaleHeight, in Vector3 airExtinctionCoefficient, float aerosolExtinctionCoefficient, in Vector3 C, float R, in Vector3 L, in Vector3 X)
		{
			float num = Vector3.Distance(X, C);
			float num2 = ComputeCosineOfHorizonAngle(num, R);
			float num3 = Vector3.Dot(X - C, L) * Rcp(num);
			if (num3 > num2)
			{
				Vector3 vector = ComputeAtmosphericOpticalDepth(airScaleHeight, aerosolScaleHeight, in airExtinctionCoefficient, aerosolExtinctionCoefficient, R, num, num3, alwaysAboveHorizon: true);
				Vector3 result = default(Vector3);
				result.x = Mathf.Exp(0f - vector.x);
				result.y = Mathf.Exp(0f - vector.y);
				result.z = Mathf.Exp(0f - vector.z);
				return result;
			}
			return Vector3.zero;
		}

		internal override Vector3 EvaluateAtmosphericAttenuation(Vector3 sunDirection, Vector3 cameraPosition)
		{
			float airScaleHeight = GetAirScaleHeight();
			float aerosolScaleHeight = GetAerosolScaleHeight();
			Vector3 airExtinctionCoefficient = GetAirExtinctionCoefficient();
			float aerosolExtinctionCoefficient = GetAerosolExtinctionCoefficient();
			Vector3 C = GetPlanetCenterPosition(cameraPosition);
			return EvaluateAtmosphericAttenuation(airScaleHeight, aerosolScaleHeight, in airExtinctionCoefficient, aerosolExtinctionCoefficient, in C, GetPlanetaryRadius(), in sunDirection, in cameraPosition);
		}

		public override Type GetSkyRendererType()
		{
			return typeof(PhysicallyBasedSkyRenderer);
		}

		private void Awake()
		{
			k_Migration.Migrate(this);
		}
	}
}
