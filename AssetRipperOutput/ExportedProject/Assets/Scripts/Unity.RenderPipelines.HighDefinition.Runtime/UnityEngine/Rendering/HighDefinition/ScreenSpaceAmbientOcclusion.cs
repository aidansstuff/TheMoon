using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Lighting/Ambient Occlusion", new Type[] { typeof(HDRenderPipeline) })]
	public sealed class ScreenSpaceAmbientOcclusion : VolumeComponentWithQuality
	{
		public BoolParameter rayTracing = new BoolParameter(value: false);

		public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 4f);

		public ClampedFloatParameter directLightingStrength = new ClampedFloatParameter(0f, 0f, 1f);

		public ClampedFloatParameter radius = new ClampedFloatParameter(2f, 0.25f, 5f);

		public ClampedFloatParameter spatialBilateralAggressiveness = new ClampedFloatParameter(0.15f, 0f, 1f);

		public BoolParameter temporalAccumulation = new BoolParameter(value: true);

		public ClampedFloatParameter ghostingReduction = new ClampedFloatParameter(0.5f, 0f, 1f);

		public ClampedFloatParameter blurSharpness = new ClampedFloatParameter(0.1f, 0f, 1f);

		public LayerMaskParameter layerMask = new LayerMaskParameter(-1);

		[AdditionalProperty]
		public ClampedFloatParameter specularOcclusion = new ClampedFloatParameter(0.5f, 0f, 1f);

		[AdditionalProperty]
		public BoolParameter occluderMotionRejection = new BoolParameter(value: true);

		[AdditionalProperty]
		public BoolParameter receiverMotionRejection = new BoolParameter(value: true);

		[SerializeField]
		[FormerlySerializedAs("stepCount")]
		private ClampedIntParameter m_StepCount = new ClampedIntParameter(6, 2, 32);

		[SerializeField]
		[FormerlySerializedAs("fullResolution")]
		private BoolParameter m_FullResolution = new BoolParameter(value: false);

		[SerializeField]
		[FormerlySerializedAs("maximumRadiusInPixels")]
		private ClampedIntParameter m_MaximumRadiusInPixels = new ClampedIntParameter(40, 16, 256);

		[AdditionalProperty]
		[SerializeField]
		[FormerlySerializedAs("bilateralUpsample")]
		private BoolParameter m_BilateralUpsample = new BoolParameter(value: true);

		[SerializeField]
		[FormerlySerializedAs("directionCount")]
		private ClampedIntParameter m_DirectionCount = new ClampedIntParameter(2, 1, 6);

		[SerializeField]
		[FormerlySerializedAs("rayLength")]
		private MinFloatParameter m_RayLength = new MinFloatParameter(50f, 0.01f);

		[SerializeField]
		[FormerlySerializedAs("sampleCount")]
		private ClampedIntParameter m_SampleCount = new ClampedIntParameter(1, 1, 64);

		[SerializeField]
		[FormerlySerializedAs("denoise")]
		private BoolParameter m_Denoise = new BoolParameter(value: true);

		[SerializeField]
		[FormerlySerializedAs("denoiserRadius")]
		private ClampedFloatParameter m_DenoiserRadius = new ClampedFloatParameter(1f, 0.001f, 1f);

		public float rayLength
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_RayLength.value;
				}
				return VolumeComponentWithQuality.GetLightingQualitySettings().RTAORayLength[quality.value];
			}
			set
			{
				m_RayLength.value = value;
			}
		}

		public int sampleCount
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_SampleCount.value;
				}
				return VolumeComponentWithQuality.GetLightingQualitySettings().RTAOSampleCount[quality.value];
			}
			set
			{
				m_SampleCount.value = value;
			}
		}

		public bool denoise
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_Denoise.value;
				}
				return VolumeComponentWithQuality.GetLightingQualitySettings().RTAODenoise[quality.value];
			}
			set
			{
				m_Denoise.value = value;
			}
		}

		public float denoiserRadius
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_DenoiserRadius.value;
				}
				return VolumeComponentWithQuality.GetLightingQualitySettings().RTAODenoiserRadius[quality.value];
			}
			set
			{
				m_DenoiserRadius.value = value;
			}
		}

		public int stepCount
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_StepCount.value;
				}
				return VolumeComponentWithQuality.GetLightingQualitySettings().AOStepCount[quality.value];
			}
			set
			{
				m_StepCount.value = value;
			}
		}

		public bool fullResolution
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_FullResolution.value;
				}
				return VolumeComponentWithQuality.GetLightingQualitySettings().AOFullRes[quality.value];
			}
			set
			{
				m_FullResolution.value = value;
			}
		}

		public int maximumRadiusInPixels
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_MaximumRadiusInPixels.value;
				}
				return VolumeComponentWithQuality.GetLightingQualitySettings().AOMaximumRadiusPixels[quality.value];
			}
			set
			{
				m_MaximumRadiusInPixels.value = value;
			}
		}

		public bool bilateralUpsample
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_BilateralUpsample.value;
				}
				return VolumeComponentWithQuality.GetLightingQualitySettings().AOBilateralUpsample[quality.value];
			}
			set
			{
				m_BilateralUpsample.value = value;
			}
		}

		public int directionCount
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_DirectionCount.value;
				}
				return VolumeComponentWithQuality.GetLightingQualitySettings().AODirectionCount[quality.value];
			}
			set
			{
				m_DirectionCount.value = value;
			}
		}
	}
}
