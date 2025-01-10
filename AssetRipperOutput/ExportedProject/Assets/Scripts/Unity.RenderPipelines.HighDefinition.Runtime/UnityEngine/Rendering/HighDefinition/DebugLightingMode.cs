namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Debug\\LightingDebug.cs")]
	public enum DebugLightingMode
	{
		None = 0,
		DiffuseLighting = 1,
		SpecularLighting = 2,
		DirectDiffuseLighting = 3,
		DirectSpecularLighting = 4,
		IndirectDiffuseLighting = 5,
		ReflectionLighting = 6,
		RefractionLighting = 7,
		EmissiveLighting = 8,
		LuxMeter = 9,
		LuminanceMeter = 10,
		MatcapView = 11,
		VisualizeCascade = 12,
		VisualizeShadowMasks = 13,
		IndirectDiffuseOcclusion = 14,
		IndirectSpecularOcclusion = 15,
		ProbeVolumeSampledSubdivision = 16
	}
}
