using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class FocusDistanceModeParameter : VolumeParameter<FocusDistanceMode>
	{
		public FocusDistanceModeParameter(FocusDistanceMode value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
