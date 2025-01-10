using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class RayTracingFallbackHierachyParameter : VolumeParameter<RayTracingFallbackHierachy>
	{
		public RayTracingFallbackHierachyParameter(RayTracingFallbackHierachy value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
