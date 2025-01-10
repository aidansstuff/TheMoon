using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Post-processing/Depth Of Field", new Type[] { typeof(HDRenderPipeline) })]
	public sealed class DepthOfField : VolumeComponentWithQuality, IPostProcessComponent
	{
		internal static Vector2 s_HighQualityAdaptiveSamplingWeights = new Vector2(4f, 1f);

		internal static Vector2 s_LowQualityAdaptiveSamplingWeights = new Vector2(1f, 0.75f);

		[Tooltip("Specifies the mode that HDRP uses to set the focus for the depth of field effect.")]
		public DepthOfFieldModeParameter focusMode = new DepthOfFieldModeParameter(DepthOfFieldMode.Off);

		[Tooltip("The distance to the focus plane from the Camera.")]
		public MinFloatParameter focusDistance = new MinFloatParameter(10f, 0.1f);

		[Tooltip("Specifies where to read the focus distance from..")]
		public FocusDistanceModeParameter focusDistanceMode = new FocusDistanceModeParameter(FocusDistanceMode.Volume);

		[Header("Near Range")]
		[Tooltip("Sets the distance from the Camera at which the near field blur begins to decrease in intensity.")]
		public MinFloatParameter nearFocusStart = new MinFloatParameter(0f, 0f);

		[Tooltip("Sets the distance from the Camera at which the near field does not blur anymore.")]
		public MinFloatParameter nearFocusEnd = new MinFloatParameter(4f, 0f);

		[Header("Far Range")]
		[Tooltip("Sets the distance from the Camera at which the far field starts blurring.")]
		public MinFloatParameter farFocusStart = new MinFloatParameter(10f, 0f);

		[Tooltip("Sets the distance from the Camera at which the far field blur reaches its maximum blur radius.")]
		public MinFloatParameter farFocusEnd = new MinFloatParameter(20f, 0f);

		[Header("Near Blur")]
		[Tooltip("Sets the number of samples to use for the near field.")]
		[SerializeField]
		[FormerlySerializedAs("nearSampleCount")]
		private ClampedIntParameter m_NearSampleCount = new ClampedIntParameter(5, 3, 8);

		[SerializeField]
		[FormerlySerializedAs("nearMaxBlur")]
		[Tooltip("Sets the maximum radius the near blur can reach.")]
		private ClampedFloatParameter m_NearMaxBlur = new ClampedFloatParameter(4f, 0f, 8f);

		[Header("Far Blur")]
		[Tooltip("Sets the number of samples to use for the far field.")]
		[SerializeField]
		[FormerlySerializedAs("farSampleCount")]
		private ClampedIntParameter m_FarSampleCount = new ClampedIntParameter(7, 3, 16);

		[Tooltip("Sets the maximum radius the far blur can reach.")]
		[SerializeField]
		[FormerlySerializedAs("farMaxBlur")]
		private ClampedFloatParameter m_FarMaxBlur = new ClampedFloatParameter(8f, 0f, 16f);

		[Header("Advanced Tweaks")]
		[AdditionalProperty]
		[Tooltip("Specifies the resolution at which HDRP processes the depth of field effect.")]
		[SerializeField]
		[FormerlySerializedAs("resolution")]
		private DepthOfFieldResolutionParameter m_Resolution = new DepthOfFieldResolutionParameter(DepthOfFieldResolution.Half);

		[AdditionalProperty]
		[Tooltip("When enabled, HDRP uses bicubic instead of bilinear filtering for the depth of field effect. Also conceals tiling artifacts in the physically-based mode.")]
		[SerializeField]
		[FormerlySerializedAs("highQualityFiltering")]
		private BoolParameter m_HighQualityFiltering = new BoolParameter(value: true);

		[AdditionalProperty]
		[Tooltip("When enabled, HDRP uses a more accurate but slower physically based algorithm to compute the depth of field effect.")]
		[SerializeField]
		private BoolParameter m_PhysicallyBased = new BoolParameter(value: false);

		[AdditionalProperty]
		[Tooltip("Adjust near blur CoC based on depth distance when manual, non-physical mode is used.")]
		[SerializeField]
		private BoolParameter m_LimitManualRangeNearBlur = new BoolParameter(value: false);

		public int nearSampleCount
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_NearSampleCount.value;
				}
				int item = quality.levelAndOverride.level;
				return VolumeComponentWithQuality.GetPostProcessingQualitySettings().NearBlurSampleCount[item];
			}
			set
			{
				m_NearSampleCount.value = value;
			}
		}

		public float nearMaxBlur
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_NearMaxBlur.value;
				}
				int item = quality.levelAndOverride.level;
				return VolumeComponentWithQuality.GetPostProcessingQualitySettings().NearBlurMaxRadius[item];
			}
			set
			{
				m_NearMaxBlur.value = value;
			}
		}

		public int farSampleCount
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_FarSampleCount.value;
				}
				int item = quality.levelAndOverride.level;
				return VolumeComponentWithQuality.GetPostProcessingQualitySettings().FarBlurSampleCount[item];
			}
			set
			{
				m_FarSampleCount.value = value;
			}
		}

		public float farMaxBlur
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_FarMaxBlur.value;
				}
				int item = quality.levelAndOverride.level;
				return VolumeComponentWithQuality.GetPostProcessingQualitySettings().FarBlurMaxRadius[item];
			}
			set
			{
				m_FarMaxBlur.value = value;
			}
		}

		public bool highQualityFiltering
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_HighQualityFiltering.value;
				}
				int item = quality.levelAndOverride.level;
				return VolumeComponentWithQuality.GetPostProcessingQualitySettings().DoFHighQualityFiltering[item];
			}
			set
			{
				m_HighQualityFiltering.value = value;
			}
		}

		public bool physicallyBased
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_PhysicallyBased.value;
				}
				int item = quality.levelAndOverride.level;
				return VolumeComponentWithQuality.GetPostProcessingQualitySettings().DoFPhysicallyBased[item];
			}
			set
			{
				m_PhysicallyBased.value = value;
			}
		}

		public bool limitManualRangeNearBlur
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_LimitManualRangeNearBlur.value;
				}
				return VolumeComponentWithQuality.GetPostProcessingQualitySettings().LimitManualRangeNearBlur[quality.levelAndOverride.level];
			}
			set
			{
				m_LimitManualRangeNearBlur.value = value;
			}
		}

		public DepthOfFieldResolution resolution
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_Resolution.value;
				}
				int item = quality.levelAndOverride.level;
				return VolumeComponentWithQuality.GetPostProcessingQualitySettings().DoFResolution[item];
			}
			set
			{
				m_Resolution.value = value;
			}
		}

		public bool IsActive()
		{
			if (focusMode.value != 0)
			{
				if (!IsNearLayerActive())
				{
					return IsFarLayerActive();
				}
				return true;
			}
			return false;
		}

		public bool IsNearLayerActive()
		{
			if (nearMaxBlur > 0f)
			{
				return nearFocusEnd.value > 0f;
			}
			return false;
		}

		public bool IsFarLayerActive()
		{
			return farMaxBlur > 0f;
		}
	}
}
