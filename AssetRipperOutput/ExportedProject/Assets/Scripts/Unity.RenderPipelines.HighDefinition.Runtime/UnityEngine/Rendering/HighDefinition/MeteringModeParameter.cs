using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class MeteringModeParameter : VolumeParameter<MeteringMode>
	{
		public MeteringModeParameter(MeteringMode value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
