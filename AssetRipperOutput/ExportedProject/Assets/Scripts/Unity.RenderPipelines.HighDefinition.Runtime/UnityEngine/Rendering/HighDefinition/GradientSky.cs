using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[VolumeComponentMenuForRenderPipeline("Sky/Gradient Sky", new Type[] { typeof(HDRenderPipeline) })]
	[SkyUniqueID(3)]
	public class GradientSky : SkySettings
	{
		[Tooltip("Specifies the color of the upper hemisphere of the sky.")]
		public ColorParameter top = new ColorParameter(Color.blue, hdr: true, showAlpha: false, showEyeDropper: true);

		[Tooltip("Specifies the color at the horizon.")]
		public ColorParameter middle = new ColorParameter(new Color(0.3f, 0.7f, 1f), hdr: true, showAlpha: false, showEyeDropper: true);

		[Tooltip("Specifies the color of the lower hemisphere of the sky. This is below the horizon.")]
		public ColorParameter bottom = new ColorParameter(Color.white, hdr: true, showAlpha: false, showEyeDropper: true);

		[Tooltip("Sets the size of the horizon (Middle color).")]
		public MinFloatParameter gradientDiffusion = new MinFloatParameter(1f, 0f);

		public override int GetHashCode()
		{
			return (((base.GetHashCode() * 23 + bottom.GetHashCode()) * 23 + top.GetHashCode()) * 23 + middle.GetHashCode()) * 23 + gradientDiffusion.GetHashCode();
		}

		public override Type GetSkyRendererType()
		{
			return typeof(GradientSkyRenderer);
		}
	}
}
