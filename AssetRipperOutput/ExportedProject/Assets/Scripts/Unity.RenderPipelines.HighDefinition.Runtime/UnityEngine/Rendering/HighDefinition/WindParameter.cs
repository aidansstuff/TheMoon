using System;
using System.Diagnostics;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	public abstract class WindParameter : VolumeParameter<WindParameter.WindParamaterValue>
	{
		public enum WindOverrideMode
		{
			Custom = 0,
			Global = 1,
			Additive = 2,
			Multiply = 3
		}

		[Serializable]
		public struct WindParamaterValue
		{
			public WindOverrideMode mode;

			public float customValue;

			public float additiveValue;

			public float multiplyValue;

			public override string ToString()
			{
				if (mode == WindOverrideMode.Global)
				{
					return mode.ToString();
				}
				string text = null;
				if (mode == WindOverrideMode.Custom)
				{
					text = customValue.ToString();
				}
				if (mode == WindOverrideMode.Additive)
				{
					text = additiveValue.ToString();
				}
				if (mode == WindOverrideMode.Multiply)
				{
					text = multiplyValue.ToString();
				}
				return text + " (" + mode.ToString() + ")";
			}
		}

		public WindParameter(float value = 0f, WindOverrideMode mode = WindOverrideMode.Global, bool overrideState = false)
			: base(default(WindParamaterValue), overrideState)
		{
			this.value = new WindParamaterValue
			{
				mode = mode,
				customValue = ((mode <= WindOverrideMode.Global) ? value : 0f),
				additiveValue = ((mode == WindOverrideMode.Additive) ? value : 0f),
				multiplyValue = ((mode == WindOverrideMode.Multiply) ? value : 1f)
			};
		}

		public override void Interp(WindParamaterValue from, WindParamaterValue to, float t)
		{
			m_Value.mode = ((t > 0f) ? to.mode : from.mode);
			m_Value.customValue = from.customValue + (to.customValue - from.customValue) * t;
			m_Value.additiveValue = from.additiveValue + (to.additiveValue - from.additiveValue) * t;
			m_Value.multiplyValue = from.multiplyValue + (to.multiplyValue - from.multiplyValue) * t;
		}

		public override int GetHashCode()
		{
			return ((((17 * 23 + overrideState.GetHashCode()) * 23 + value.mode.GetHashCode()) * 23 + value.customValue.GetHashCode()) * 23 + value.additiveValue.GetHashCode()) * 23 + value.multiplyValue.GetHashCode();
		}

		public virtual float GetValue(HDCamera camera)
		{
			if (value.mode == WindOverrideMode.Custom)
			{
				return value.customValue;
			}
			float globalValue = GetGlobalValue(camera);
			if (value.mode == WindOverrideMode.Additive)
			{
				return globalValue + value.additiveValue;
			}
			if (value.mode == WindOverrideMode.Multiply)
			{
				return globalValue * value.multiplyValue;
			}
			return globalValue;
		}

		protected abstract float GetGlobalValue(HDCamera camera);
	}
}
