using System;
using System.Collections.Generic;

namespace UnityEngine.Rendering.HighDefinition
{
	public abstract class SkySettings : VolumeComponent
	{
		[Tooltip("Sets the rotation of the sky.")]
		public ClampedFloatParameter rotation = new ClampedFloatParameter(0f, 0f, 360f);

		[Tooltip("Specifies the intensity mode HDRP uses for the sky.")]
		public SkyIntensityParameter skyIntensityMode = new SkyIntensityParameter(SkyIntensityMode.Exposure);

		[Tooltip("Sets the exposure of the sky in EV.")]
		public FloatParameter exposure = new FloatParameter(0f);

		[Tooltip("Sets the intensity multiplier for the sky.")]
		public MinFloatParameter multiplier = new MinFloatParameter(1f, 0f);

		[Tooltip("Informative helper that displays the relative intensity (in Lux) for the current HDR texture set in HDRI Sky.")]
		public MinFloatParameter upperHemisphereLuxValue = new MinFloatParameter(1f, 0f);

		[Tooltip("Informative helper that displays Show the color of Shadow.")]
		public Vector3Parameter upperHemisphereLuxColor = new Vector3Parameter(new Vector3(0f, 0f, 0f));

		[Tooltip("Sets the absolute intensity (in Lux) of the current HDR texture set in HDRI Sky. Functions as a Lux intensity multiplier for the sky.")]
		public FloatParameter desiredLuxValue = new FloatParameter(20000f);

		[Tooltip("Specifies when HDRP updates the environment lighting. When set to OnDemand, use HDRenderPipeline.RequestSkyEnvironmentUpdate() to request an update.")]
		public EnvUpdateParameter updateMode = new EnvUpdateParameter(EnvironmentUpdateMode.OnChanged);

		[Tooltip("Sets the period, in seconds, at which HDRP updates the environment ligting (0 means HDRP updates it every frame).")]
		public MinFloatParameter updatePeriod = new MinFloatParameter(0f, 0f);

		[Tooltip("When enabled, HDRP uses the Sun Disk in baked lighting.")]
		public BoolParameter includeSunInBaking = new BoolParameter(value: false);

		private static Dictionary<Type, int> skyUniqueIDs = new Dictionary<Type, int>();

		public virtual int GetHashCode(Camera camera)
		{
			return GetHashCode();
		}

		public override int GetHashCode()
		{
			return (((((13 * 23 + rotation.GetHashCode()) * 23 + exposure.GetHashCode()) * 23 + multiplier.GetHashCode()) * 23 + desiredLuxValue.GetHashCode()) * 23 + skyIntensityMode.GetHashCode()) * 23 + includeSunInBaking.GetHashCode();
		}

		public static int GetUniqueID<T>()
		{
			return GetUniqueID(typeof(T));
		}

		public static int GetUniqueID(Type type)
		{
			if (!skyUniqueIDs.TryGetValue(type, out var value))
			{
				object[] customAttributes = type.GetCustomAttributes(typeof(SkyUniqueID), inherit: false);
				value = ((customAttributes.Length == 0) ? (-1) : ((SkyUniqueID)customAttributes[0]).uniqueID);
				skyUniqueIDs[type] = value;
			}
			return value;
		}

		public float GetIntensityFromSettings()
		{
			float num = 1f;
			switch (skyIntensityMode.value)
			{
			case SkyIntensityMode.Exposure:
				num *= ColorUtils.ConvertEV100ToExposure(0f - exposure.value);
				break;
			case SkyIntensityMode.Multiplier:
				num *= multiplier.value;
				break;
			case SkyIntensityMode.Lux:
				num *= desiredLuxValue.value / Mathf.Max(upperHemisphereLuxValue.value, 1E-05f);
				break;
			}
			return num;
		}

		public virtual bool SignificantlyDivergesFrom(SkySettings otherSettings)
		{
			if (otherSettings == null || otherSettings.GetSkyRendererType() != GetSkyRendererType())
			{
				return true;
			}
			float intensityFromSettings = GetIntensityFromSettings();
			float intensityFromSettings2 = otherSettings.GetIntensityFromSettings();
			return ((intensityFromSettings > intensityFromSettings2) ? (intensityFromSettings / intensityFromSettings2) : (intensityFromSettings2 / intensityFromSettings)) > 3f;
		}

		public abstract Type GetSkyRendererType();

		internal virtual Vector3 EvaluateAtmosphericAttenuation(Vector3 sunDirection, Vector3 cameraPosition)
		{
			return Vector3.one;
		}
	}
}
