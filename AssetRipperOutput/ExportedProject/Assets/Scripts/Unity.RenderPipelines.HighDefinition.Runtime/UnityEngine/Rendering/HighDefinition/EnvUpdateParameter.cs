using System;
using System.Diagnostics;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	public sealed class EnvUpdateParameter : VolumeParameter<EnvironmentUpdateMode>
	{
		public EnvUpdateParameter(EnvironmentUpdateMode value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
