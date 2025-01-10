namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\RenderPipeline\\Raytracing\\HDRaytracingManager.cs")]
	internal enum RayTracingRendererFlag
	{
		Opaque = 1,
		CastShadowTransparent = 2,
		CastShadowOpaque = 4,
		CastShadow = 6,
		AmbientOcclusion = 8,
		Reflection = 16,
		GlobalIllumination = 32,
		RecursiveRendering = 64,
		PathTracing = 128,
		All = 255
	}
}
