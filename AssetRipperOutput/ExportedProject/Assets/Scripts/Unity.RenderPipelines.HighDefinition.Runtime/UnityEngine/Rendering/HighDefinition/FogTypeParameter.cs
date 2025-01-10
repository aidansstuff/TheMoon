using System;
using System.Diagnostics;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	internal sealed class FogTypeParameter : VolumeParameter<FogType>
	{
		public FogTypeParameter(FogType value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
