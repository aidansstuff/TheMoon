using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Shadowing/Contact Shadows", new Type[] { typeof(HDRenderPipeline) })]
	public class ContactShadows : VolumeComponentWithQuality
	{
		public BoolParameter enable = new BoolParameter(value: false, BoolParameter.DisplayType.EnumPopup);

		public ClampedFloatParameter length = new ClampedFloatParameter(0.15f, 0f, 1f);

		public ClampedFloatParameter opacity = new ClampedFloatParameter(1f, 0f, 1f);

		public ClampedFloatParameter distanceScaleFactor = new ClampedFloatParameter(0.5f, 0f, 1f);

		public MinFloatParameter maxDistance = new MinFloatParameter(50f, 0f);

		public MinFloatParameter minDistance = new MinFloatParameter(0f, 0f);

		public MinFloatParameter fadeDistance = new MinFloatParameter(5f, 0f);

		public MinFloatParameter fadeInDistance = new MinFloatParameter(0f, 0f);

		public ClampedFloatParameter rayBias = new ClampedFloatParameter(0.2f, 0f, 1f);

		public ClampedFloatParameter thicknessScale = new ClampedFloatParameter(0.15f, 0.02f, 1f);

		[SerializeField]
		[FormerlySerializedAs("sampleCount")]
		private NoInterpClampedIntParameter m_SampleCount = new NoInterpClampedIntParameter(10, 4, 64);

		public int sampleCount
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_SampleCount.value;
				}
				int value = quality.value;
				return VolumeComponentWithQuality.GetLightingQualitySettings().ContactShadowSampleCount[value];
			}
			set
			{
				m_SampleCount.value = value;
			}
		}

		private ContactShadows()
		{
			base.displayName = "Contact Shadows";
		}
	}
}
