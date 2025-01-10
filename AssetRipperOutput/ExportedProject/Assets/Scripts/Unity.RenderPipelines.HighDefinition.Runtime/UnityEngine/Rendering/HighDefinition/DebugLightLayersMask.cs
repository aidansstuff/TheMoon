using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Debug\\LightingDebug.cs")]
	[Flags]
	public enum DebugLightLayersMask
	{
		None = 0,
		LightLayer1 = 1,
		LightLayer2 = 2,
		LightLayer3 = 4,
		LightLayer4 = 8,
		LightLayer5 = 0x10,
		LightLayer6 = 0x20,
		LightLayer7 = 0x40,
		LightLayer8 = 0x80
	}
}
