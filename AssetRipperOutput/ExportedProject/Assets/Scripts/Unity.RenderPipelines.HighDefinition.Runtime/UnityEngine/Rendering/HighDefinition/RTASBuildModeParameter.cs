using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class RTASBuildModeParameter : VolumeParameter<RTASBuildMode>
	{
		public RTASBuildModeParameter(RTASBuildMode value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
