using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class TargetMidGrayParameter : VolumeParameter<TargetMidGray>
	{
		public TargetMidGrayParameter(TargetMidGray value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
