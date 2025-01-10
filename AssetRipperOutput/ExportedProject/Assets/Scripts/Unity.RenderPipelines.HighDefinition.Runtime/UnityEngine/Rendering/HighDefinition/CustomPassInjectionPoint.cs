namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\RenderPipeline\\RenderPass\\CustomPass\\CustomPassInjectionPoint.cs")]
	public enum CustomPassInjectionPoint
	{
		BeforeRendering = 0,
		AfterOpaqueDepthAndNormal = 5,
		AfterOpaqueAndSky = 6,
		BeforePreRefraction = 4,
		BeforeTransparent = 1,
		BeforePostProcess = 2,
		AfterPostProcess = 3
	}
}
