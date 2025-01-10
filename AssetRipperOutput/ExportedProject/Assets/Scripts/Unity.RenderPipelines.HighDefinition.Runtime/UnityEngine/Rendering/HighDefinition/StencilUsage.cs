namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\RenderPipeline\\HDStencilUsage.cs")]
	internal enum StencilUsage
	{
		Clear = 0,
		IsUnlit = 1,
		RequiresDeferredLighting = 2,
		SubsurfaceScattering = 4,
		TraceReflectionRay = 8,
		Decals = 16,
		ObjectMotionVector = 32,
		ExcludeFromTAA = 2,
		DistortionVectors = 4,
		SMAA = 4,
		WaterSurface = 16,
		AfterOpaqueReservedBits = 56,
		UserBit0 = 64,
		UserBit1 = 128,
		HDRPReservedBits = 63
	}
}
