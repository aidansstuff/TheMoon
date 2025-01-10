using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Post-processing/Lens Distortion", new Type[] { typeof(HDRenderPipeline) })]
	public sealed class LensDistortion : VolumeComponent, IPostProcessComponent
	{
		[Tooltip("Controls the overall strength of the distortion effect.")]
		public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, -1f, 1f);

		[Tooltip("Controls the distortion intensity on the x-axis. Acts as a multiplier.")]
		public ClampedFloatParameter xMultiplier = new ClampedFloatParameter(1f, 0f, 1f);

		[Tooltip("Controls the distortion intensity on the x-axis. Acts as a multiplier.")]
		public ClampedFloatParameter yMultiplier = new ClampedFloatParameter(1f, 0f, 1f);

		[Tooltip("Distortion center point. 0.5,0.5 is center of the screen.")]
		public Vector2Parameter center = new Vector2Parameter(new Vector2(0.5f, 0.5f));

		[Tooltip("Controls global screen scaling for the distortion effect. Use this to hide the screen borders when using a high \"Intensity\".")]
		public ClampedFloatParameter scale = new ClampedFloatParameter(1f, 0.01f, 5f);

		public bool IsActive()
		{
			if (Mathf.Abs(intensity.value) > 0f)
			{
				if (!(xMultiplier.value > 0f))
				{
					return yMultiplier.value > 0f;
				}
				return true;
			}
			return false;
		}
	}
}
