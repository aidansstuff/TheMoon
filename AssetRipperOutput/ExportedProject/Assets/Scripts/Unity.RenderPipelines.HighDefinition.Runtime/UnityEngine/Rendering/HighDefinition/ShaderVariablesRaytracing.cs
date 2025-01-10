namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\RenderPipeline\\Raytracing\\Shaders\\ShaderVariablesRaytracing.cs", needAccessors = false, generateCBuffer = true, constantRegister = 3)]
	internal struct ShaderVariablesRaytracing
	{
		public float _RayTracingPadding0;

		public float _RaytracingRayMaxLength;

		public int _RaytracingNumSamples;

		public int _RaytracingSampleIndex;

		public float _RaytracingIntensityClamp;

		public int _RayCountEnabled;

		public int _RaytracingPreExposition;

		public float _RaytracingCameraNearPlane;

		public float _RaytracingPixelSpreadAngle;

		public float _RaytracingReflectionMinSmoothness;

		public float _RaytracingReflectionSmoothnessFadeStart;

		public int _RaytracingMinRecursion;

		public int _RaytracingMaxRecursion;

		public int _RayTracingDiffuseLightingOnly;

		public float _DirectionalShadowFallbackIntensity;

		public float _RayTracingLodBias;

		public int _RayTracingRayMissFallbackHierarchy;

		public int _RayTracingLastBounceFallbackHierarchy;

		public int _RayTracingClampingFlag;

		public float _RayTracingAmbientProbeDimmer;

		public int _RayTracingAPVRayMiss;

		public float _RayTracingRayBias;

		public float _RayTracingDistantRayBias;

		public int _PaddingRT0;
	}
}
