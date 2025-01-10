using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Post-processing/Tonemapping", new Type[] { typeof(HDRenderPipeline) })]
	public sealed class Tonemapping : VolumeComponent, IPostProcessComponent
	{
		[Tooltip("Specifies the tonemapping algorithm to use for the color grading process.")]
		public TonemappingModeParameter mode = new TonemappingModeParameter(TonemappingMode.None);

		[AdditionalProperty]
		[Tooltip("Whether to use full ACES tonemap instead of an approximation. When outputting to an HDR display, full ACES is always used regardless of this checkbox.")]
		public BoolParameter useFullACES = new BoolParameter(value: false);

		[Tooltip("Controls the transition between the toe and the mid section of the curve. A value of 0 results in no transition and a value of 1 results in a very hard transition.")]
		public ClampedFloatParameter toeStrength = new ClampedFloatParameter(0f, 0f, 1f);

		[Tooltip("Controls how much of the dynamic range is in the toe. Higher values result in longer toes and therefore contain more of the dynamic range.")]
		public ClampedFloatParameter toeLength = new ClampedFloatParameter(0.5f, 0f, 1f);

		[Tooltip("Controls the transition between the midsection and the shoulder of the curve. A value of 0 results in no transition and a value of 1 results in a very hard transition.")]
		public ClampedFloatParameter shoulderStrength = new ClampedFloatParameter(0f, 0f, 1f);

		[Tooltip("Sets how many F-stops (EV) to add to the dynamic range of the curve.")]
		public MinFloatParameter shoulderLength = new MinFloatParameter(0.5f, 0f);

		[Tooltip("Controls how much overshoot to add to the shoulder.")]
		public ClampedFloatParameter shoulderAngle = new ClampedFloatParameter(0f, 0f, 1f);

		[Tooltip("Sets a gamma correction value that HDRP applies to the whole curve.")]
		public MinFloatParameter gamma = new MinFloatParameter(1f, 0.001f);

		[Tooltip("A custom 3D texture lookup table to apply.")]
		public Texture3DParameter lutTexture = new Texture3DParameter(null);

		[Tooltip("How much of the lookup texture will contribute to the color grading effect.")]
		public ClampedFloatParameter lutContribution = new ClampedFloatParameter(1f, 0f, 1f);

		[AdditionalProperty]
		[Tooltip("Specifies the range reduction mode used when HDR output is enabled and Neutral tonemapping is enabled.")]
		public NeutralRangeReductionModeParameter neutralHDRRangeReductionMode = new NeutralRangeReductionModeParameter(NeutralRangeReductionMode.BT2390);

		[Tooltip("Specifies the ACES preset to be used for HDR displays.")]
		public HDRACESPresetParameter acesPreset = new HDRACESPresetParameter(HDRACESPreset.ACES1000Nits);

		[Tooltip("Specifies the fallback tonemapping algorithm to use when outputting to an HDR device, when the main mode is not supported.")]
		public FallbackHDRTonemapParameter fallbackMode = new FallbackHDRTonemapParameter(FallbackHDRTonemap.Neutral);

		[Tooltip("How much hue we want to preserve. Values closer to 0 try to preserve hue, while as values get closer to 1 hue shifts are reintroduced.")]
		public ClampedFloatParameter hueShiftAmount = new ClampedFloatParameter(0f, 0f, 1f);

		[Tooltip("Whether to use values detected from the output device as paperwhite. This value will often not lead to equivalent images between SDR and HDR. It is suggested to manually set this value.")]
		public BoolParameter detectPaperWhite = new BoolParameter(value: false);

		[Tooltip("It controls how bright a paper white surface should be, it also determines the maximum brightness of UI. The scene is also scaled relative to this value. Value in nits.")]
		public ClampedFloatParameter paperWhite = new ClampedFloatParameter(300f, 0f, 400f);

		[Tooltip("Whether to use the minimum and maximum brightness values detected from the output device. It might be worth considering calibrating this values manually if the results are not the desired ones.")]
		public BoolParameter detectBrightnessLimits = new BoolParameter(value: true);

		[Tooltip("The minimum brightness (in nits) of the screen. Note that this is assumed to be 0.005 with ACES Tonemap.")]
		public ClampedFloatParameter minNits = new ClampedFloatParameter(0.005f, 0f, 50f);

		[Tooltip("The maximum brightness (in nits) of the screen. Note that this is assumed to be defined by the preset when ACES Tonemap is used.")]
		public ClampedFloatParameter maxNits = new ClampedFloatParameter(1000f, 0f, 5000f);

		public bool IsActive()
		{
			if (mode.value == TonemappingMode.External)
			{
				if (ValidateLUT())
				{
					return lutContribution.value > 0f;
				}
				return false;
			}
			return mode.value != TonemappingMode.None;
		}

		internal TonemappingMode GetHDRTonemappingMode()
		{
			if (mode.value == TonemappingMode.Custom || mode.value == TonemappingMode.External)
			{
				if (fallbackMode.value == FallbackHDRTonemap.None)
				{
					return TonemappingMode.None;
				}
				if (fallbackMode.value == FallbackHDRTonemap.Neutral)
				{
					return TonemappingMode.Neutral;
				}
				if (fallbackMode.value == FallbackHDRTonemap.ACES)
				{
					return TonemappingMode.ACES;
				}
			}
			return mode.value;
		}

		public bool ValidateLUT()
		{
			HDRenderPipelineAsset currentAsset = HDRenderPipeline.currentAsset;
			if (currentAsset == null || lutTexture.value == null)
			{
				return false;
			}
			if (lutTexture.value.width != currentAsset.currentPlatformRenderPipelineSettings.postProcessSettings.lutSize)
			{
				return false;
			}
			bool flag = false;
			Texture value = lutTexture.value;
			if (!(value is Texture3D texture3D))
			{
				if (value is RenderTexture renderTexture)
				{
					flag |= renderTexture.dimension == TextureDimension.Tex3D && renderTexture.width == renderTexture.height && renderTexture.height == renderTexture.volumeDepth;
				}
			}
			else
			{
				flag |= texture3D.width == texture3D.height && texture3D.height == texture3D.depth;
			}
			return flag;
		}
	}
}
