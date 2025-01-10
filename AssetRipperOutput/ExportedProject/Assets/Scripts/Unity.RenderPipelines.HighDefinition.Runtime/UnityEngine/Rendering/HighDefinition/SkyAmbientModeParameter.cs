using System;
using System.Diagnostics;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	public sealed class SkyAmbientModeParameter : VolumeParameter<SkyAmbientMode>
	{
		public SkyAmbientModeParameter(SkyAmbientMode value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
