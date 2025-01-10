using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class RayTracingModeParameter : VolumeParameter<RayTracingMode>
	{
		public RayTracingModeParameter(RayTracingMode value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
