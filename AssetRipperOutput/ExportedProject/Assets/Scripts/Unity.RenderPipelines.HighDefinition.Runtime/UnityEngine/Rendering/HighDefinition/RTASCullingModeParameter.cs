using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class RTASCullingModeParameter : VolumeParameter<RTASCullingMode>
	{
		public RTASCullingModeParameter(RTASCullingMode value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
