using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class RayCastingModeParameter : VolumeParameter<RayCastingMode>
	{
		public RayCastingModeParameter(RayCastingMode value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
