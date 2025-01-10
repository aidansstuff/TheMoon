using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Post-processing/Film Grain", new Type[] { typeof(HDRenderPipeline) })]
	public sealed class FilmGrain : VolumeComponent, IPostProcessComponent
	{
		[Tooltip("Specifies the type of grain to use. Select a preset or select \"Custom\" to provide your own Texture.")]
		public FilmGrainLookupParameter type = new FilmGrainLookupParameter(FilmGrainLookup.Thin1);

		[Tooltip("Use the slider to set the strength of the Film Grain effect.")]
		public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 1f);

		[Tooltip("Controls the noisiness response curve. The higher you set this value, the less noise there is in brighter areas.")]
		public ClampedFloatParameter response = new ClampedFloatParameter(0.8f, 0f, 1f);

		[Tooltip("Specifies a tileable Texture to use for the grain. The neutral value for this Texture is 0.5 which means that HDRP does not apply grain at this value.")]
		public Texture2DParameter texture = new Texture2DParameter(null);

		public bool IsActive()
		{
			if (intensity.value > 0f)
			{
				if (type.value == FilmGrainLookup.Custom)
				{
					return texture.value != null;
				}
				return true;
			}
			return false;
		}
	}
}
