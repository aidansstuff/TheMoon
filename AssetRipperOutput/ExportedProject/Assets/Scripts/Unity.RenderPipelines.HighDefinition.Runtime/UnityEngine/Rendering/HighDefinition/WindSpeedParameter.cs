using System;
using System.Diagnostics;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	public sealed class WindSpeedParameter : WindParameter
	{
		public WindSpeedParameter(float value = 100f, WindOverrideMode mode = WindOverrideMode.Global, bool overrideState = false)
			: base(value, mode, overrideState)
		{
		}

		protected override float GetGlobalValue(HDCamera camera)
		{
			return camera.volumeStack.GetComponent<VisualEnvironment>().windSpeed.value;
		}
	}
}
