using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class LuminanceSourceParameter : VolumeParameter<LuminanceSource>
	{
		public LuminanceSourceParameter(LuminanceSource value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
