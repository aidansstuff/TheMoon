using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Post-processing/Chromatic Aberration", new Type[] { typeof(HDRenderPipeline) })]
	public sealed class ChromaticAberration : VolumeComponentWithQuality, IPostProcessComponent
	{
		[Tooltip("Specifies a Texture which HDRP uses to shift the hue of chromatic aberrations.")]
		public Texture2DParameter spectralLut = new Texture2DParameter(null);

		[Tooltip("Use the slider to set the strength of the Chromatic Aberration effect.")]
		public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

		[Tooltip("Controls the maximum number of samples HDRP uses to render the effect. A lower sample number results in better performance.")]
		[SerializeField]
		[FormerlySerializedAs("maxSamples")]
		private ClampedIntParameter m_MaxSamples = new ClampedIntParameter(6, 3, 24);

		public int maxSamples
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_MaxSamples.value;
				}
				int item = quality.levelAndOverride.level;
				return VolumeComponentWithQuality.GetPostProcessingQualitySettings().ChromaticAberrationMaxSamples[item];
			}
			set
			{
				m_MaxSamples.value = value;
			}
		}

		public bool IsActive()
		{
			return intensity.value > 0f;
		}
	}
}
