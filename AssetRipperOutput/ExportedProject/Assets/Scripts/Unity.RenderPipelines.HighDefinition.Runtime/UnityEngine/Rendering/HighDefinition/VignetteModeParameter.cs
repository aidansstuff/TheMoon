using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class VignetteModeParameter : VolumeParameter<VignetteMode>
	{
		public VignetteModeParameter(VignetteMode value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
