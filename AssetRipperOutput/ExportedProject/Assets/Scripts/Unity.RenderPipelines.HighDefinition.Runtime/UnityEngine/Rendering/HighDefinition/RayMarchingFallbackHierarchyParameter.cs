using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public sealed class RayMarchingFallbackHierarchyParameter : VolumeParameter<RayMarchingFallbackHierarchy>
	{
		public RayMarchingFallbackHierarchyParameter(RayMarchingFallbackHierarchy value, bool overrideState = false)
			: base(value, overrideState)
		{
		}
	}
}
