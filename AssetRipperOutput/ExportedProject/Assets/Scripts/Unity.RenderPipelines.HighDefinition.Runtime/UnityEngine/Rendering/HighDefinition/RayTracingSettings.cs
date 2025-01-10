using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Ray Tracing/Ray Tracing Settings (Preview)", new Type[] { typeof(HDRenderPipeline) })]
	public sealed class RayTracingSettings : VolumeComponent
	{
		[Tooltip("Controls the bias for all real-time ray tracing effects.")]
		public ClampedFloatParameter rayBias = new ClampedFloatParameter(0.001f, 0f, 0.1f);

		[Tooltip("Controls the Ray Bias value used when the distance between the pixel and the camera is close to the far plane. Between the near and far plane the Ray Bias and Distant Ray Bias are interpolated linearly. This does not affect Path Tracing or Recursive Rendering. This value can be increased to mitigate Ray Tracing z-fighting issues at a distance.")]
		public ClampedFloatParameter distantRayBias = new ClampedFloatParameter(0.001f, 0f, 0.1f);

		[Tooltip("When enabled, the culling region for punctual and area lights shadow maps is increased from frustum culling to extended culling. For Directional lights, cascades are not extended, but additional objects may appear in the cascades.")]
		[FormerlySerializedAs("extendCulling")]
		public BoolParameter extendShadowCulling = new BoolParameter(value: true);

		[Tooltip("Enables the override of the camera culling. This increases the validity area of animated skinned mesh that are outside of the frustum.")]
		public BoolParameter extendCameraCulling = new BoolParameter(value: false);

		[Tooltip("Controls the maximal ray length for ray traced directional shadows.")]
		public MinFloatParameter directionalShadowRayLength = new MinFloatParameter(1000f, 0.01f);

		[Tooltip("Controls the fallback directional shadow value that is used when the point to shade is outside of the cascade.")]
		public ClampedFloatParameter directionalShadowFallbackIntensity = new ClampedFloatParameter(1f, 0f, 1f);

		[Tooltip("Controls how the ray tracing acceleration structure is build.")]
		[AdditionalProperty]
		public RTASBuildModeParameter buildMode = new RTASBuildModeParameter(RTASBuildMode.Automatic);

		[Tooltip("Controls how the maximum distance for the ray tracing culling is defined.")]
		[AdditionalProperty]
		public RTASCullingModeParameter cullingMode = new RTASCullingModeParameter(RTASCullingMode.ExtendedFrustum);

		[Tooltip("Specifies the radius of the sphere used to cull objects out of the ray tracing acceleration structure when the culling mode is set to Sphere.")]
		public MinFloatParameter cullingDistance = new MinFloatParameter(1000f, 0.01f);

		public RayTracingSettings()
		{
			base.displayName = "Ray Tracing Settings (Preview)";
		}
	}
}
