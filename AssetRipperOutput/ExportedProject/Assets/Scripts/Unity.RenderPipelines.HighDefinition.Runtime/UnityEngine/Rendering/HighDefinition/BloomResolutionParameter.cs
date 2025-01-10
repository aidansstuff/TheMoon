using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class BloomResolutionParameter : VolumeParameter<BloomResolution>
	{
		public BloomResolutionParameter(BloomResolution value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
