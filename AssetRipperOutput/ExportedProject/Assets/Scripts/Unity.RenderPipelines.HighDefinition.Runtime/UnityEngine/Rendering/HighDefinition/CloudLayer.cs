using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[VolumeComponentMenuForRenderPipeline("Sky/Cloud Layer", new Type[] { typeof(HDRenderPipeline) })]
	[CloudUniqueID(1)]
	public class CloudLayer : CloudSettings
	{
		[Serializable]
		public class CloudMap
		{
			internal static Texture s_DefaultTexture;

			[Tooltip("Specify the texture HDRP uses to render the clouds (in LatLong layout).")]
			public Texture2DParameter cloudMap = new Texture2DParameter(s_DefaultTexture);

			[Tooltip("Opacity multiplier for the red channel.")]
			public ClampedFloatParameter opacityR = new ClampedFloatParameter(1f, 0f, 1f);

			[Tooltip("Opacity multiplier for the green channel.")]
			public ClampedFloatParameter opacityG = new ClampedFloatParameter(0f, 0f, 1f);

			[Tooltip("Opacity multiplier for the blue channel.")]
			public ClampedFloatParameter opacityB = new ClampedFloatParameter(0f, 0f, 1f);

			[Tooltip("Opacity multiplier for the alpha channel.")]
			public ClampedFloatParameter opacityA = new ClampedFloatParameter(0f, 0f, 1f);

			[Tooltip("Altitude of the bottom of the cloud layer in meters.")]
			public MinFloatParameter altitude = new MinFloatParameter(2000f, 0f);

			[Tooltip("Sets the rotation of the clouds (in degrees).")]
			public ClampedFloatParameter rotation = new ClampedFloatParameter(0f, 0f, 360f);

			[Tooltip("Specifies the color HDRP uses to tint the clouds.")]
			public ColorParameter tint = new ColorParameter(Color.white, hdr: false, showAlpha: false, showEyeDropper: true);

			[InspectorName("Exposure Compensation")]
			[Tooltip("Sets the exposure compensation of the clouds in EV.")]
			public FloatParameter exposure = new FloatParameter(0f);

			[InspectorName("Wind")]
			[Tooltip("Distortion mode used to simulate cloud movement.\nIn Scene View, requires Always Refresh to be enabled.")]
			public VolumeParameter<CloudDistortionMode> distortionMode = new VolumeParameter<CloudDistortionMode>();

			[InspectorName("Orientation")]
			[Tooltip("Controls the orientation of the wind relative to the X world vector.\nThis value can be relative to the Global Wind Orientation defined in the Visual Environment.")]
			public WindOrientationParameter scrollOrientation = new WindOrientationParameter();

			[InspectorName("Speed")]
			[Tooltip("Sets the wind speed in kilometers per hour.\nThis value can be relative to the Global Wind Speed defined in the Visual Environment.")]
			public WindSpeedParameter scrollSpeed = new WindSpeedParameter();

			[Tooltip("Specify the flowmap HDRP uses for cloud distortion (in LatLong layout).")]
			public Texture2DParameter flowmap = new Texture2DParameter(null);

			[InspectorName("Raymarching")]
			[Tooltip("Simulates cloud self-shadowing using raymarching.")]
			public BoolParameter lighting = new BoolParameter(value: true);

			[Tooltip("Number of raymarching steps.")]
			public ClampedIntParameter steps = new ClampedIntParameter(6, 2, 32);

			[InspectorName("Density")]
			[Tooltip("Density of the cloud layer.")]
			public ClampedFloatParameter thickness = new ClampedFloatParameter(0.5f, 0f, 1f);

			[Tooltip("Controls the influence of the ambient probe on the cloud layer volume. A lower value will suppress the ambient light and produce darker clouds overall.")]
			public ClampedFloatParameter ambientProbeDimmer = new ClampedFloatParameter(1f, 0f, 1f);

			[Tooltip("Projects a portion of the clouds around the sun light to simulate cloud shadows. This will override the cookie of your directional light.")]
			public BoolParameter castShadows = new BoolParameter(value: false);

			internal float scrollFactor;

			internal int NumSteps
			{
				get
				{
					if (!lighting.value)
					{
						return 0;
					}
					return steps.value;
				}
			}

			internal Vector4 Opacities => new Vector4(opacityR.value, opacityG.value, opacityB.value, opacityA.value);

			internal Color Color => tint.value * ColorUtils.ConvertEV100ToExposure(0f - exposure.value);

			internal Vector4 GetRenderingParameters(HDCamera camera)
			{
				float f = MathF.PI / 180f * scrollOrientation.GetValue(camera);
				return new Vector3(0f - Mathf.Cos(f), 0f - Mathf.Sin(f), scrollFactor);
			}

			internal (Vector4, Vector4) GetBakingParameters()
			{
				return new ValueTuple<Vector4, Vector4>(item2: new Vector4(rotation.value / 360f, NumSteps, thickness.value * 0.095f + 0.005f, altitude.value), item1: Opacities);
			}

			internal int GetBakingHashCode()
			{
				int num = 17;
				num = num * 23 + cloudMap.GetHashCode();
				num = num * 23 + opacityR.GetHashCode();
				num = num * 23 + opacityG.GetHashCode();
				num = num * 23 + opacityB.GetHashCode();
				num = num * 23 + opacityA.GetHashCode();
				num = num * 23 + rotation.GetHashCode();
				num = num * 23 + castShadows.GetHashCode();
				if (lighting.value)
				{
					num = num * 23 + lighting.GetHashCode();
					num = num * 23 + steps.GetHashCode();
					num = num * 23 + altitude.GetHashCode();
					num = num * 23 + thickness.GetHashCode();
				}
				return num;
			}

			public override int GetHashCode()
			{
				return (((((((((((((((((17 * 23 + cloudMap.GetHashCode()) * 23 + opacityR.GetHashCode()) * 23 + opacityG.GetHashCode()) * 23 + opacityB.GetHashCode()) * 23 + opacityA.GetHashCode()) * 23 + altitude.GetHashCode()) * 23 + rotation.GetHashCode()) * 23 + tint.GetHashCode()) * 23 + exposure.GetHashCode()) * 23 + distortionMode.GetHashCode()) * 23 + scrollOrientation.GetHashCode()) * 23 + scrollSpeed.GetHashCode()) * 23 + flowmap.GetHashCode()) * 23 + lighting.GetHashCode()) * 23 + steps.GetHashCode()) * 23 + thickness.GetHashCode()) * 23 + ambientProbeDimmer.GetHashCode()) * 23 + castShadows.GetHashCode();
			}
		}

		[Tooltip("Controls the global opacity of the cloud layer.")]
		public ClampedFloatParameter opacity = new ClampedFloatParameter(1f, 0f, 1f);

		[AdditionalProperty]
		[Tooltip("Check this box if the cloud layer covers only the upper part of the sky.")]
		public BoolParameter upperHemisphereOnly = new BoolParameter(value: true);

		public VolumeParameter<CloudMapMode> layers = new VolumeParameter<CloudMapMode>();

		[AdditionalProperty]
		[Tooltip("Specifies the resolution of the texture HDRP uses to represent the clouds.")]
		public CloudLayerEnumParameter<CloudResolution> resolution = new CloudLayerEnumParameter<CloudResolution>(CloudResolution.CloudResolution1024);

		[Header("Cloud Shadows")]
		[Tooltip("Controls the opacity of the cloud shadows.")]
		public MinFloatParameter shadowMultiplier = new MinFloatParameter(1f, 0f);

		[Tooltip("Controls the tint of the cloud shadows.")]
		public ColorParameter shadowTint = new ColorParameter(Color.black, hdr: false, showAlpha: false, showEyeDropper: true);

		[AdditionalProperty]
		[Tooltip("Specifies the resolution of the texture HDRP uses to represent the cloud shadows.")]
		public CloudLayerEnumParameter<CloudShadowsResolution> shadowResolution = new CloudLayerEnumParameter<CloudShadowsResolution>(CloudShadowsResolution.Medium);

		[Tooltip("Specifies the size of the projected shadows.")]
		public MinFloatParameter shadowSize = new MinFloatParameter(500f, 0f);

		public CloudMap layerA = new CloudMap();

		public CloudMap layerB = new CloudMap();

		internal int NumLayers
		{
			get
			{
				if (!(layers == CloudMapMode.Single))
				{
					return 2;
				}
				return 1;
			}
		}

		internal bool CastShadows
		{
			get
			{
				if (!layerA.castShadows.value)
				{
					if (layers.value == CloudMapMode.Double)
					{
						return layerB.castShadows.value;
					}
					return false;
				}
				return true;
			}
		}

		private Vector3Int CastToInt3(Vector3 vec)
		{
			return new Vector3Int((int)vec.x, (int)vec.y, (int)vec.z);
		}

		internal int GetBakingHashCode(Light sunLight)
		{
			int num = 17;
			bool flag = layerA.lighting.value;
			bool flag2 = sunLight != null && layerA.castShadows.value;
			num = num * 23 + upperHemisphereOnly.GetHashCode();
			num = num * 23 + layers.GetHashCode();
			num = num * 23 + resolution.GetHashCode();
			num = num * 23 + layerA.GetBakingHashCode();
			if (layers.value == CloudMapMode.Double)
			{
				num = num * 23 + layerB.GetBakingHashCode();
				flag |= layerB.lighting.value;
				flag2 |= layerB.castShadows.value;
			}
			if (flag && sunLight != null)
			{
				num = num * 23 + CastToInt3(sunLight.transform.rotation.eulerAngles).GetHashCode();
			}
			if (flag2)
			{
				num = num * 23 + shadowResolution.GetHashCode();
			}
			return num;
		}

		public override int GetHashCode()
		{
			int num = 17;
			num = num * 23 + opacity.GetHashCode();
			num = num * 23 + upperHemisphereOnly.GetHashCode();
			num = num * 23 + layers.GetHashCode();
			num = num * 23 + resolution.GetHashCode();
			num = num * 23 + layerA.GetHashCode();
			if (layers.value == CloudMapMode.Double)
			{
				num = num * 23 + layerB.GetHashCode();
			}
			return num;
		}

		public override Type GetCloudRendererType()
		{
			return typeof(CloudLayerRenderer);
		}

		private static void Init()
		{
			HDRenderPipelineGlobalSettings instance = HDRenderPipelineGlobalSettings.instance;
			if (instance != null)
			{
				CloudMap.s_DefaultTexture = instance.renderPipelineResources?.textures.defaultCloudMap;
			}
		}
	}
}
