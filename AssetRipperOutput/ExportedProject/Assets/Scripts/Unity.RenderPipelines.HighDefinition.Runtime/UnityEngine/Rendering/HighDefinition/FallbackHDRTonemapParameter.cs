using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class FallbackHDRTonemapParameter : VolumeParameter<FallbackHDRTonemap>
	{
		public FallbackHDRTonemapParameter(FallbackHDRTonemap value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
