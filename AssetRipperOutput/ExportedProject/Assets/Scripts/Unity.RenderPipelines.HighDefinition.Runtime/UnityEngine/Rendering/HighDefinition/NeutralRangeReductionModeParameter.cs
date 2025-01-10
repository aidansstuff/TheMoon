using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class NeutralRangeReductionModeParameter : VolumeParameter<NeutralRangeReductionMode>
	{
		public NeutralRangeReductionModeParameter(NeutralRangeReductionMode value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
