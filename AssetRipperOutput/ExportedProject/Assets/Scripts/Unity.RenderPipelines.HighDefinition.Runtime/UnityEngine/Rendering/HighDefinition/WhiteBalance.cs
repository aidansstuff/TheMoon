using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Post-processing/White Balance", new Type[] { typeof(HDRenderPipeline) })]
	public sealed class WhiteBalance : VolumeComponent, IPostProcessComponent
	{
		[Tooltip("Controls the color temperature HDRP uses for white balancing.")]
		public ClampedFloatParameter temperature = new ClampedFloatParameter(0f, -100f, 100f);

		[Tooltip("Controls the white balance color to compensate for a green or magenta tint.")]
		public ClampedFloatParameter tint = new ClampedFloatParameter(0f, -100f, 100f);

		public bool IsActive()
		{
			if (Mathf.Approximately(temperature.value, 0f))
			{
				return !Mathf.Approximately(tint.value, 0f);
			}
			return true;
		}
	}
}
