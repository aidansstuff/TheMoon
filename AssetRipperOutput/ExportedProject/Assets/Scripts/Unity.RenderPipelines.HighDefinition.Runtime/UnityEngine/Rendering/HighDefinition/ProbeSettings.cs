using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public struct ProbeSettings
	{
		public enum ProbeType
		{
			ReflectionProbe = 0,
			PlanarProbe = 1
		}

		public enum Mode
		{
			Baked = 0,
			Realtime = 1,
			Custom = 2
		}

		public enum RealtimeMode
		{
			EveryFrame = 0,
			OnEnable = 1,
			OnDemand = 2
		}

		[Serializable]
		public struct Lighting
		{
			[Obsolete("Since 2019.3, use Lighting.NewDefault() instead.")]
			public static readonly Lighting @default;

			public float multiplier;

			[Range(0f, 1f)]
			public float weight;

			public LightLayerEnum lightLayer;

			public float fadeDistance;

			[Min(1E-06f)]
			public float rangeCompressionFactor;

			public static Lighting NewDefault()
			{
				Lighting result = default(Lighting);
				result.multiplier = 1f;
				result.weight = 1f;
				result.lightLayer = LightLayerEnum.LightLayerDefault;
				result.fadeDistance = 10000f;
				result.rangeCompressionFactor = 1f;
				return result;
			}
		}

		[Serializable]
		public struct ProxySettings
		{
			[Obsolete("Since 2019.3, use ProxySettings.NewDefault() instead.")]
			public static readonly ProxySettings @default;

			public bool useInfluenceVolumeAsProxyVolume;

			public Vector3 capturePositionProxySpace;

			public Quaternion captureRotationProxySpace;

			public Vector3 mirrorPositionProxySpace;

			public Quaternion mirrorRotationProxySpace;

			public static ProxySettings NewDefault()
			{
				ProxySettings result = default(ProxySettings);
				result.capturePositionProxySpace = Vector3.zero;
				result.captureRotationProxySpace = Quaternion.identity;
				result.useInfluenceVolumeAsProxyVolume = true;
				return result;
			}
		}

		[Serializable]
		public struct Frustum
		{
			public enum FOVMode
			{
				Fixed = 0,
				Viewer = 1,
				Automatic = 2
			}

			[Obsolete("Since 2019.3, use Frustum.NewDefault() instead.")]
			public static readonly Frustum @default;

			public FOVMode fieldOfViewMode;

			[Range(0f, 179f)]
			public float fixedValue;

			[Min(0f)]
			public float automaticScale;

			[Min(0f)]
			public float viewerScale;

			public static Frustum NewDefault()
			{
				Frustum result = default(Frustum);
				result.fieldOfViewMode = FOVMode.Viewer;
				result.fixedValue = 90f;
				result.automaticScale = 1f;
				result.viewerScale = 1f;
				return result;
			}
		}

		[Serializable]
		public class CubeReflectionResolutionScalableSettingValue : ScalableSettingValue<CubeReflectionResolution>
		{
		}

		[Serializable]
		public class PlanarReflectionAtlasResolutionScalableSettingValue : ScalableSettingValue<PlanarReflectionAtlasResolution>
		{
		}

		internal const CubeReflectionResolution k_DefaultCubeResolution = CubeReflectionResolution.CubeReflectionResolution128;

		[Obsolete("Since 2019.3, use ProbeSettings.NewDefault() instead.")]
		public static ProbeSettings @default;

		public Frustum frustum;

		public ProbeType type;

		public Mode mode;

		public RealtimeMode realtimeMode;

		public bool timeSlicing;

		public Lighting lighting;

		public InfluenceVolume influence;

		public ProxyVolume proxy;

		public ProxySettings proxySettings;

		public PlanarReflectionAtlasResolutionScalableSettingValue resolutionScalable;

		[SerializeField]
		internal PlanarReflectionAtlasResolution resolution;

		[SerializeField]
		internal CubeReflectionResolutionScalableSettingValue cubeResolution;

		[FormerlySerializedAs("camera")]
		public CameraSettings cameraSettings;

		public bool roughReflections;

		public bool distanceBasedRoughness;

		public static ProbeSettings NewDefault()
		{
			ProbeSettings probeSettings = default(ProbeSettings);
			probeSettings.type = ProbeType.ReflectionProbe;
			probeSettings.realtimeMode = RealtimeMode.EveryFrame;
			probeSettings.timeSlicing = false;
			probeSettings.mode = Mode.Baked;
			probeSettings.cameraSettings = CameraSettings.NewDefault();
			probeSettings.influence = null;
			probeSettings.lighting = Lighting.NewDefault();
			probeSettings.proxy = null;
			probeSettings.proxySettings = ProxySettings.NewDefault();
			probeSettings.frustum = Frustum.NewDefault();
			probeSettings.resolutionScalable = new PlanarReflectionAtlasResolutionScalableSettingValue();
			probeSettings.cubeResolution = new CubeReflectionResolutionScalableSettingValue();
			probeSettings.roughReflections = true;
			probeSettings.distanceBasedRoughness = false;
			ProbeSettings result = probeSettings;
			result.resolutionScalable.@override = PlanarReflectionAtlasResolution.Resolution512;
			result.cubeResolution.@override = CubeReflectionResolution.CubeReflectionResolution128;
			return result;
		}

		public Hash128 ComputeHash()
		{
			Hash128 hash = default(Hash128);
			Hash128 hash2 = default(Hash128);
			HashUtilities.ComputeHash128(ref type, ref hash);
			HashUtilities.ComputeHash128(ref mode, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			HashUtilities.ComputeHash128(ref lighting, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			HashUtilities.ComputeHash128(ref proxySettings, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			hash2 = cameraSettings.GetHash();
			HashUtilities.AppendHash(ref hash2, ref hash);
			CubeReflectionResolution value = CubeReflectionResolution.CubeReflectionResolution128;
			if (RenderPipelineManager.currentPipeline is HDRenderPipeline hDRenderPipeline)
			{
				value = cubeResolution.Value(hDRenderPipeline.asset.currentPlatformRenderPipelineSettings.cubeReflectionResolution);
			}
			HashUtilities.ComputeHash128(ref value, ref hash2);
			HashUtilities.AppendHash(ref hash2, ref hash);
			if (influence != null)
			{
				hash2 = influence.ComputeHash();
				HashUtilities.AppendHash(ref hash2, ref hash);
			}
			if (proxy != null)
			{
				hash2 = proxy.ComputeHash();
				HashUtilities.AppendHash(ref hash2, ref hash);
			}
			return hash;
		}
	}
}
