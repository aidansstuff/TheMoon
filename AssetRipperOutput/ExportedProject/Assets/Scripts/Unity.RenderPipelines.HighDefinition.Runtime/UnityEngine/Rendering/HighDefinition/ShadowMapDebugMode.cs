namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Debug\\LightingDebug.cs")]
	public enum ShadowMapDebugMode
	{
		None = 0,
		VisualizePunctualLightAtlas = 1,
		VisualizeDirectionalLightAtlas = 2,
		VisualizeAreaLightAtlas = 3,
		VisualizeCachedPunctualLightAtlas = 4,
		VisualizeCachedAreaLightAtlas = 5,
		VisualizeShadowMap = 6,
		SingleShadow = 7
	}
}
