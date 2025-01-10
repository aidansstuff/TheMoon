using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Post-processing/Motion Blur", new Type[] { typeof(HDRenderPipeline) })]
	public sealed class MotionBlur : VolumeComponentWithQuality, IPostProcessComponent
	{
		[Tooltip("Sets the intensity of the motion blur effect. Acts as a multiplier for velocities.")]
		public MinFloatParameter intensity = new MinFloatParameter(0f, 0f);

		[Tooltip("Controls the maximum velocity, in pixels, that HDRP allows for all sources of motion blur except Camera rotation.")]
		public ClampedFloatParameter maximumVelocity = new ClampedFloatParameter(200f, 0f, 1500f);

		[Tooltip("Controls the minimum velocity, in pixels, that a GameObject must have to contribute to the motion blur effect.")]
		public ClampedFloatParameter minimumVelocity = new ClampedFloatParameter(2f, 0f, 64f);

		[Header("Camera Velocity")]
		[AdditionalProperty]
		[Tooltip("If toggled off, the motion caused by the camera is not considered when doing motion blur.")]
		public BoolParameter cameraMotionBlur = new BoolParameter(value: true);

		[AdditionalProperty]
		[Tooltip("Determine if and how the component of the motion vectors coming from the camera is clamped in a special fashion.")]
		public CameraClampModeParameter specialCameraClampMode = new CameraClampModeParameter(CameraClampMode.None);

		[AdditionalProperty]
		[Tooltip("Sets the maximum length, as a fraction of the screen's full resolution, that the motion vectors resulting from Camera can have.")]
		public ClampedFloatParameter cameraVelocityClamp = new ClampedFloatParameter(0.05f, 0f, 0.3f);

		[AdditionalProperty]
		[Tooltip("Sets the maximum length, as a fraction of the screen's full resolution, that the motion vectors resulting from Camera can have.")]
		public ClampedFloatParameter cameraTranslationVelocityClamp = new ClampedFloatParameter(0.05f, 0f, 0.3f);

		[AdditionalProperty]
		[Tooltip("Sets the maximum length, as a fraction of the screen's full resolution, that the motion vectors resulting from Camera rotation can have.")]
		public ClampedFloatParameter cameraRotationVelocityClamp = new ClampedFloatParameter(0.03f, 0f, 0.3f);

		[AdditionalProperty]
		[Tooltip("Value used for the depth based weighting of samples. Tweak if unwanted leak of background onto foreground or viceversa is detected.")]
		public ClampedFloatParameter depthComparisonExtent = new ClampedFloatParameter(1f, 0f, 20f);

		[Tooltip("Sets the maximum number of sample points that HDRP uses to compute motion blur.")]
		[SerializeField]
		[FormerlySerializedAs("sampleCount")]
		private MinIntParameter m_SampleCount = new MinIntParameter(8, 2);

		public int sampleCount
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_SampleCount.value;
				}
				int item = quality.levelAndOverride.level;
				return VolumeComponentWithQuality.GetPostProcessingQualitySettings().MotionBlurSampleCount[item];
			}
			set
			{
				m_SampleCount.value = value;
			}
		}

		public bool IsActive()
		{
			return intensity.value > 0f;
		}
	}
}
