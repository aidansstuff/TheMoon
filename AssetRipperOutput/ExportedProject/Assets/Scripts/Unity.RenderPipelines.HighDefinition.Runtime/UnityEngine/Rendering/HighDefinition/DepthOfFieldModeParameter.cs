using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class DepthOfFieldModeParameter : VolumeParameter<DepthOfFieldMode>
	{
		public DepthOfFieldModeParameter(DepthOfFieldMode value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
