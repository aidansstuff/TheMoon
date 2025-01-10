using System;
using System.Diagnostics;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	public sealed class FogColorParameter : VolumeParameter<FogColorMode>
	{
		public FogColorParameter(FogColorMode value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
