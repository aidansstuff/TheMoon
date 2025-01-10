using System;
using System.Diagnostics;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	public sealed class CloudLayerEnumParameter<T> : VolumeParameter<T>
	{
		public CloudLayerEnumParameter(T value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
