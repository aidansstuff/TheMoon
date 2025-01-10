using System;
using System.Diagnostics;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	public sealed class BackplateTypeParameter : VolumeParameter<BackplateType>
	{
		public BackplateTypeParameter(BackplateType value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
