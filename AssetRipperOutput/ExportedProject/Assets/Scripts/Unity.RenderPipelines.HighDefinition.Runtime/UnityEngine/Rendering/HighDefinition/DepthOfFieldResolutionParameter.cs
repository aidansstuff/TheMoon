using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class DepthOfFieldResolutionParameter : VolumeParameter<DepthOfFieldResolution>
	{
		public DepthOfFieldResolutionParameter(DepthOfFieldResolution value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
