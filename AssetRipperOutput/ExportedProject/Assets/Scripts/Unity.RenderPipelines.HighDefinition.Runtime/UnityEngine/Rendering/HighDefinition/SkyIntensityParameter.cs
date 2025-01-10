using System;
using System.Diagnostics;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	public sealed class SkyIntensityParameter : VolumeParameter<SkyIntensityMode>
	{
		public SkyIntensityParameter(SkyIntensityMode value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
