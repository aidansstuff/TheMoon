using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class HDRACESPresetParameter : VolumeParameter<HDRACESPreset>
	{
		public HDRACESPresetParameter(HDRACESPreset value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
