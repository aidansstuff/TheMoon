using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Post-processing/Bloom", new Type[] { typeof(HDRenderPipeline) })]
	public sealed class Bloom : VolumeComponentWithQuality, IPostProcessComponent
	{
		[Header("Bloom")]
		[Tooltip("Set the level of brightness to filter out pixels under this level. This value is expressed in gamma-space. A value above 0 will disregard energy conservation rules.")]
		public MinFloatParameter threshold = new MinFloatParameter(0f, 0f);

		[Tooltip("Controls the strength of the bloom filter.")]
		public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

		[Tooltip("Set the radius of the bloom effect")]
		public ClampedFloatParameter scatter = new ClampedFloatParameter(0.7f, 0f, 1f);

		[Tooltip("Use the color picker to select a color for the Bloom effect to tint to.")]
		public ColorParameter tint = new ColorParameter(Color.white, hdr: false, showAlpha: false, showEyeDropper: true);

		[Header("Lens Dirt")]
		[Tooltip("Specifies a Texture to add smudges or dust to the bloom effect.")]
		public Texture2DParameter dirtTexture = new Texture2DParameter(null);

		[Tooltip("Controls the strength of the lens dirt.")]
		public MinFloatParameter dirtIntensity = new MinFloatParameter(0f, 0f);

		[Tooltip("When enabled, bloom stretches horizontally depending on the current physical Camera's Anamorphism property value.")]
		[AdditionalProperty]
		public BoolParameter anamorphic = new BoolParameter(value: true);

		[Header("Advanced Tweaks")]
		[AdditionalProperty]
		[Tooltip("Specifies the resolution at which HDRP processes the effect. Quarter resolution is less resource intensive but can result in aliasing artifacts.")]
		[SerializeField]
		[FormerlySerializedAs("resolution")]
		private BloomResolutionParameter m_Resolution = new BloomResolutionParameter(BloomResolution.Half);

		[AdditionalProperty]
		[Tooltip("When enabled, bloom uses multiple bilinear samples for the prefiltering pass.")]
		[SerializeField]
		private BoolParameter m_HighQualityPrefiltering = new BoolParameter(value: false);

		[AdditionalProperty]
		[Tooltip("When enabled, bloom uses bicubic sampling instead of bilinear sampling for the upsampling passes.")]
		[SerializeField]
		[FormerlySerializedAs("highQualityFiltering")]
		private BoolParameter m_HighQualityFiltering = new BoolParameter(value: true);

		public BloomResolution resolution
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_Resolution.value;
				}
				int item = quality.levelAndOverride.level;
				return VolumeComponentWithQuality.GetPostProcessingQualitySettings().BloomRes[item];
			}
			set
			{
				m_Resolution.value = value;
			}
		}

		public bool highQualityPrefiltering
		{
			get
			{
				if (!UsesQualitySettings())
				{
					return m_HighQualityPrefiltering.value;
				}
				int item = quality.levelAndOverride.level;
				return VolumeComponentWithQuality.GetPostProcessingQualitySettings().BloomHighQualityPrefiltering[item];
			}
			set
			{
				m_HighQualityPrefiltering.value = value;
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
				return VolumeComponentWithQuality.GetPostProcessingQualitySettings().BloomHighQualityFiltering[item];
			}
			set
			{
				m_HighQualityFiltering.value = value;
			}
		}

		public bool IsActive()
		{
			return intensity.value > 0f;
		}
	}
}
