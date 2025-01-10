namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\RenderPipeline\\ShaderPass\\ShaderPass.cs")]
	internal enum ShaderPass
	{
		GBuffer = 0,
		Forward = 1,
		ForwardUnlit = 2,
		DeferredLighting = 3,
		DepthOnly = 4,
		TransparentDepthPrepass = 5,
		TransparentDepthPostpass = 6,
		MotionVectors = 7,
		Distortion = 8,
		LightTransport = 9,
		Shadows = 10,
		SubsurfaceScattering = 11,
		VolumetricLighting = 12,
		DbufferProjector = 13,
		DbufferMesh = 14,
		ForwardEmissiveProjector = 15,
		ForwardEmissiveMesh = 16,
		Raytracing = 17,
		RaytracingIndirect = 18,
		RaytracingVisibility = 19,
		RaytracingForward = 20,
		RaytracingGBuffer = 21,
		RaytracingSubSurface = 22,
		PathTracing = 23,
		RayTracingDebug = 24,
		Constant = 25,
		FullScreenDebug = 26
	}
}
