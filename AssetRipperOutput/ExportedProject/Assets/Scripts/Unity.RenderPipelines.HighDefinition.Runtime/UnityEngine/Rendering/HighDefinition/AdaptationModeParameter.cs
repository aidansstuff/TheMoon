using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class AdaptationModeParameter : VolumeParameter<AdaptationMode>
	{
		public AdaptationModeParameter(AdaptationMode value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
