using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public struct GlobalLowResolutionTransparencySettings
	{
		public bool enabled;

		public bool checkerboardDepthBuffer;

		public LowResTransparentUpsample upsampleType;

		internal static GlobalLowResolutionTransparencySettings NewDefault()
		{
			GlobalLowResolutionTransparencySettings result = default(GlobalLowResolutionTransparencySettings);
			result.enabled = true;
			result.checkerboardDepthBuffer = true;
			result.upsampleType = LowResTransparentUpsample.NearestDepth;
			return result;
		}
	}
}
