using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Lighting/Screen Space Refraction", new Type[] { typeof(HDRenderPipeline) })]
	public class ScreenSpaceRefraction : VolumeComponent
	{
		internal enum RefractionModel
		{
			None = 0,
			Planar = 1,
			Sphere = 2,
			Thin = 3
		}

		[Tooltip("Controls the distance at which HDRP fades out SSR near the edge of the screen.")]
		public ClampedFloatParameter screenFadeDistance = new ClampedFloatParameter(0.1f, 0.001f, 1f);
	}
}
