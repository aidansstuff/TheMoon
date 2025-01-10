using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class SkyImportanceSamplingParameter : VolumeParameter<SkyImportanceSamplingMode>
	{
		public SkyImportanceSamplingParameter(SkyImportanceSamplingMode value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
