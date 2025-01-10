using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Debug\\LightingDebug.cs")]
	[Flags]
	public enum DebugLightFilterMode
	{
		None = 0,
		DirectDirectional = 1,
		DirectPunctual = 2,
		DirectRectangle = 4,
		DirectTube = 8,
		DirectSpotCone = 0x10,
		DirectSpotPyramid = 0x20,
		DirectSpotBox = 0x40,
		IndirectReflectionProbe = 0x80,
		IndirectPlanarProbe = 0x100
	}
}
