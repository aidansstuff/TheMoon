using System;
using System.Diagnostics;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	public sealed class PhysicallyBasedSkyModelParameter : VolumeParameter<PhysicallyBasedSkyModel>
	{
		public PhysicallyBasedSkyModelParameter(PhysicallyBasedSkyModel value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
