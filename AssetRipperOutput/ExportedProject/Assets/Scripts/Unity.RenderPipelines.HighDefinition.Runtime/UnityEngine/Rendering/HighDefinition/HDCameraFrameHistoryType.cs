namespace UnityEngine.Rendering.HighDefinition
{
	public enum HDCameraFrameHistoryType
	{
		ColorBufferMipChain = 0,
		Exposure = 1,
		TemporalAntialiasing = 2,
		TAAMotionVectorMagnitude = 3,
		DepthOfFieldCoC = 4,
		Normal = 5,
		Depth = 6,
		Depth1 = 7,
		AmbientOcclusion = 8,
		RaytracedAmbientOcclusion = 9,
		RaytracedShadowHistory = 10,
		RaytracedShadowHistoryValidity = 11,
		RaytracedShadowDistanceValidity = 12,
		RaytracedReflection = 13,
		RaytracedIndirectDiffuseHF = 14,
		RaytracedIndirectDiffuseLF = 15,
		RayTracedSubSurface = 16,
		PathTracing = 17,
		TemporalAntialiasingPostDoF = 18,
		VolumetricClouds0 = 19,
		VolumetricClouds1 = 20,
		ScreenSpaceReflectionAccumulation = 21,
		AlbedoAOV = 22,
		NormalAOV = 23,
		MotionVectorAOV = 24,
		DenoiseHistory = 25
	}
}
