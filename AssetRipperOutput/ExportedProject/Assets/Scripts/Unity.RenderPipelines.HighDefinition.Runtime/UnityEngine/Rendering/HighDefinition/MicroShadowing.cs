using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Shadowing/Micro Shadows", new Type[] { typeof(HDRenderPipeline) })]
	public class MicroShadowing : VolumeComponent
	{
		[Tooltip("Enables micro shadows for directional lights.")]
		[DisplayInfo(name = "State")]
		public BoolParameter enable = new BoolParameter(value: false, BoolParameter.DisplayType.EnumPopup);

		[Tooltip("Controls the opacity of the micro shadows.")]
		public ClampedFloatParameter opacity = new ClampedFloatParameter(1f, 0f, 1f);

		private MicroShadowing()
		{
			base.displayName = "Micro Shadows";
		}
	}
}
