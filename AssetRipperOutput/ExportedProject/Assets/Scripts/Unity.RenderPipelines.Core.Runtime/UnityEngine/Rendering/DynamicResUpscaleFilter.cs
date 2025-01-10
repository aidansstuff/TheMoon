using System;

namespace UnityEngine.Rendering
{
	public enum DynamicResUpscaleFilter : byte
	{
		[Obsolete("Bilinear upscale filter is considered obsolete and is not supported anymore, please use CatmullRom for a very cheap, but blurry filter.", false)]
		Bilinear = 0,
		CatmullRom = 1,
		[Obsolete("Lanczos upscale filter is considered obsolete and is not supported anymore, please use Contrast Adaptive Sharpening for very sharp filter or FidelityFX Super Resolution 1.0.", false)]
		Lanczos = 2,
		ContrastAdaptiveSharpen = 3,
		[InspectorName("FidelityFX Super Resolution 1.0")]
		EdgeAdaptiveScalingUpres = 4,
		[InspectorName("TAA Upscale")]
		TAAU = 5
	}
}
