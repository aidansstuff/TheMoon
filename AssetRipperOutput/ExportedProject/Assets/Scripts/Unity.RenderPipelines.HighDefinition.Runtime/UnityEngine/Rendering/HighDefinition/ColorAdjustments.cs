using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Post-processing/Color Adjustments", new Type[] { typeof(HDRenderPipeline) })]
	public sealed class ColorAdjustments : VolumeComponent, IPostProcessComponent
	{
		[Tooltip("Adjusts the brightness of the image just before color grading, in EV.")]
		public FloatParameter postExposure = new FloatParameter(0f);

		[Tooltip("Controls the overall range of the tonal values.")]
		public ClampedFloatParameter contrast = new ClampedFloatParameter(0f, -100f, 100f);

		[Tooltip("Specifies the color that HDRP tints the render to.")]
		public ColorParameter colorFilter = new ColorParameter(Color.white, hdr: true, showAlpha: false, showEyeDropper: true);

		[Tooltip("Controls the hue of all colors in the render.")]
		public ClampedFloatParameter hueShift = new ClampedFloatParameter(0f, -180f, 180f);

		[Tooltip("Controls the intensity of all colors in the render.")]
		public ClampedFloatParameter saturation = new ClampedFloatParameter(0f, -100f, 100f);

		public bool IsActive()
		{
			if (postExposure.value == 0f && contrast.value == 0f && !(colorFilter != Color.white) && !(hueShift != 0f))
			{
				return saturation != 0f;
			}
			return true;
		}
	}
}
