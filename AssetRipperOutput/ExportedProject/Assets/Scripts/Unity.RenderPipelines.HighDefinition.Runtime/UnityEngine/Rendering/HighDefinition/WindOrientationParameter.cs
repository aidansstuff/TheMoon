using System;
using System.Diagnostics;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	public sealed class WindOrientationParameter : WindParameter
	{
		public WindOrientationParameter(float value = 0f, WindOverrideMode mode = WindOverrideMode.Global, bool overrideState = false)
			: base(value, mode, overrideState)
		{
		}

		protected override float GetGlobalValue(HDCamera camera)
		{
			return camera.volumeStack.GetComponent<VisualEnvironment>().windOrientation.value;
		}

		public override void Interp(WindParamaterValue from, WindParamaterValue to, float t)
		{
			m_Value.multiplyValue = 0f;
			m_Value.mode = ((t > 0f) ? to.mode : from.mode);
			m_Value.additiveValue = from.additiveValue + (to.additiveValue - from.additiveValue) * t;
			m_Value.customValue = HDUtils.InterpolateOrientation(from.customValue, to.customValue, t);
		}

		public override float GetValue(HDCamera camera)
		{
			if (value.mode == WindOverrideMode.Multiply)
			{
				throw new NotSupportedException("Texture format not supported");
			}
			return base.GetValue(camera);
		}
	}
}
