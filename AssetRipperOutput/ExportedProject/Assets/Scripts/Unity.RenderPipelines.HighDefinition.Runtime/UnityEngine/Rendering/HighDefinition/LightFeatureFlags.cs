namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Lighting\\LightLoop\\LightLoop.cs")]
	internal enum LightFeatureFlags
	{
		Punctual = 0x1000,
		Area = 0x2000,
		Directional = 0x4000,
		Env = 0x8000,
		Sky = 0x10000,
		SSRefraction = 0x20000,
		SSReflection = 0x40000
	}
}
