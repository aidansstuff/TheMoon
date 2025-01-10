using System;
using System.Diagnostics;

namespace UnityEngine.Rendering
{
	[Serializable]
	[DebuggerDisplay("{m_Value} ({m_OverrideState})")]
	public class BoolParameter : VolumeParameter<bool>
	{
		public enum DisplayType
		{
			Checkbox = 0,
			EnumPopup = 1
		}

		[NonSerialized]
		public DisplayType displayType;

		public BoolParameter(bool value, bool overrideState = false)
			: base(value, overrideState)
		{
		}

		public BoolParameter(bool value, DisplayType displayType, bool overrideState = false)
			: base(value, overrideState)
		{
			this.displayType = displayType;
		}
	}
}
