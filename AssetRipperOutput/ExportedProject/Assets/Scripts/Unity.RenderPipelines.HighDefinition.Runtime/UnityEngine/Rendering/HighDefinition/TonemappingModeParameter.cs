using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class TonemappingModeParameter : VolumeParameter<TonemappingMode>
	{
		public TonemappingModeParameter(TonemappingMode value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
