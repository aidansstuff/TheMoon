using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class HDRenderPipelineRayTracingResources : HDRenderPipelineResources
	{
		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/Reflections/RaytracingReflections.raytrace", ReloadAttribute.Package.Root)]
		public RayTracingShader reflectionRaytracingRT;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/Reflections/RaytracingReflections.compute", ReloadAttribute.Package.Root)]
		public ComputeShader reflectionRaytracingCS;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/RaytracingReflectionFilter.compute", ReloadAttribute.Package.Root)]
		public ComputeShader reflectionBilateralFilterCS;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/Shadows/RaytracingShadow.raytrace", ReloadAttribute.Package.Root)]
		public RayTracingShader shadowRaytracingRT;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/Shadows/RayTracingContactShadow.raytrace", ReloadAttribute.Package.Root)]
		public RayTracingShader contactShadowRayTracingRT;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/Shadows/RaytracingShadow.compute", ReloadAttribute.Package.Root)]
		public ComputeShader shadowRaytracingCS;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/Shadows/RaytracingShadowFilter.compute", ReloadAttribute.Package.Root)]
		public ComputeShader shadowFilterCS;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/RaytracingRenderer.raytrace", ReloadAttribute.Package.Root)]
		public RayTracingShader forwardRaytracing;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/RaytracingLightCluster.compute", ReloadAttribute.Package.Root)]
		public ComputeShader lightClusterBuildCS;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/DebugLightCluster.shader", ReloadAttribute.Package.Root)]
		public Shader lightClusterDebugS;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/DebugLightCluster.compute", ReloadAttribute.Package.Root)]
		public ComputeShader lightClusterDebugCS;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/IndirectDiffuse/RaytracingIndirectDiffuse_APVOff.raytrace", ReloadAttribute.Package.Root)]
		public RayTracingShader indirectDiffuseRaytracingOffRT;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/IndirectDiffuse/RaytracingIndirectDiffuse_APVL1.raytrace", ReloadAttribute.Package.Root)]
		public RayTracingShader indirectDiffuseRaytracingL1RT;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/IndirectDiffuse/RaytracingIndirectDiffuse_APVL2.raytrace", ReloadAttribute.Package.Root)]
		public RayTracingShader indirectDiffuseRaytracingL2RT;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/IndirectDiffuse/RaytracingIndirectDiffuse.compute", ReloadAttribute.Package.Root)]
		public ComputeShader indirectDiffuseRaytracingCS;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/RaytracingAmbientOcclusion.raytrace", ReloadAttribute.Package.Root)]
		public RayTracingShader aoRaytracingRT;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/RaytracingAmbientOcclusion.compute", ReloadAttribute.Package.Root)]
		public ComputeShader aoRaytracingCS;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/RayTracingSubSurface.raytrace", ReloadAttribute.Package.Root)]
		public RayTracingShader subSurfaceRayTracingRT;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/SubSurface/RayTracingSubSurface.compute", ReloadAttribute.Package.Root)]
		public ComputeShader subSurfaceRayTracingCS;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/Denoising/SimpleDenoiser.compute", ReloadAttribute.Package.Root)]
		public ComputeShader simpleDenoiserCS;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/Denoising/ReflectionDenoiser.compute", ReloadAttribute.Package.Root)]
		public ComputeShader reflectionDenoiserCS;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/Denoising/DiffuseShadowDenoiser.compute", ReloadAttribute.Package.Root)]
		public ComputeShader diffuseShadowDenoiserCS;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/Deferred/RaytracingGBuffer.raytrace", ReloadAttribute.Package.Root)]
		public RayTracingShader gBufferRaytracingRT;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/Deferred/RaytracingDeferred.compute", ReloadAttribute.Package.Root)]
		public ComputeShader deferredRaytracingCS;

		[Reload("Runtime/RenderPipeline/PathTracing/Shaders/PathTracingMain.raytrace", ReloadAttribute.Package.Root)]
		public RayTracingShader pathTracingRT;

		[Reload("Runtime/RenderPipeline/PathTracing/Shaders/PathTracingSkySamplingData.compute", ReloadAttribute.Package.Root)]
		public ComputeShader pathTracingSkySamplingDataCS;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/RayMarching.compute", ReloadAttribute.Package.Root)]
		public ComputeShader rayMarchingCS;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/Common/RayBinning.compute", ReloadAttribute.Package.Root)]
		public ComputeShader rayBinningCS;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/CountTracedRays.compute", ReloadAttribute.Package.Root)]
		public ComputeShader countTracedRays;

		[Reload("Runtime/RenderPipelineResources/Texture/ReflectionKernelMapping.png", ReloadAttribute.Package.Root)]
		public Texture2D reflectionFilterMapping;

		[Reload("Runtime/RenderPipeline/Raytracing/Shaders/RTASDebug.raytrace", ReloadAttribute.Package.Root)]
		public RayTracingShader rtasDebug;
	}
}
