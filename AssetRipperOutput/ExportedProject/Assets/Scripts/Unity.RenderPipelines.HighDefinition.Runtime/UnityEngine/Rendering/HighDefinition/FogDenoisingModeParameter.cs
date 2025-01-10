using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class FogDenoisingModeParameter : VolumeParameter<FogDenoisingMode>
	{
		public FogDenoisingModeParameter(FogDenoisingMode value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
