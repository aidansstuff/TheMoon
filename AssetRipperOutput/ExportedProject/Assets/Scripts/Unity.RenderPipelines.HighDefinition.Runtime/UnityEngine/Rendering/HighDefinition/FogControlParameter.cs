using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class FogControlParameter : VolumeParameter<FogControl>
	{
		public FogControlParameter(FogControl value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
