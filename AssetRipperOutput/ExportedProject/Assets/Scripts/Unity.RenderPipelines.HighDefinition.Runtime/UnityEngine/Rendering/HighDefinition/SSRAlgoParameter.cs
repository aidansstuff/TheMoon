using System;
using System.Diagnostics;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	public sealed class SSRAlgoParameter : VolumeParameter<ScreenSpaceReflectionAlgorithm>
	{
		public SSRAlgoParameter(ScreenSpaceReflectionAlgorithm value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
