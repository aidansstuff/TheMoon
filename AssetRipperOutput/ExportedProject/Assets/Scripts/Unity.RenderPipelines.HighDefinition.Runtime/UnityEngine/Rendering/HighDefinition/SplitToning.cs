using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Post-processing/Split Toning", new Type[] { typeof(HDRenderPipeline) })]
	public sealed class SplitToning : VolumeComponent, IPostProcessComponent
	{
		[Tooltip("Specifies the color to use for shadows.")]
		public ColorParameter shadows = new ColorParameter(Color.grey, hdr: false, showAlpha: false, showEyeDropper: true);

		[Tooltip("Specifies the color to use for highlights.")]
		public ColorParameter highlights = new ColorParameter(Color.grey, hdr: false, showAlpha: false, showEyeDropper: true);

		[Tooltip("Controls the balance between the colors in the highlights and shadows.")]
		public ClampedFloatParameter balance = new ClampedFloatParameter(0f, -100f, 100f);

		public bool IsActive()
		{
			if (!(shadows != Color.grey))
			{
				return highlights != Color.grey;
			}
			return true;
		}
	}
}
