using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Ray Tracing/Recursive Rendering (Preview)", new Type[] { typeof(HDRenderPipeline) })]
	public sealed class RecursiveRendering : VolumeComponent
	{
		[Tooltip("Enable. Enables recursive rendering.")]
		public BoolParameter enable = new BoolParameter(value: false, BoolParameter.DisplayType.EnumPopup);

		[Tooltip("Layer Mask. Layer mask used to include the objects for recursive rendering.")]
		public LayerMaskParameter layerMask = new LayerMaskParameter(-1);

		[Tooltip("Max Depth. Defines the maximal recursion for rays.")]
		public ClampedIntParameter maxDepth = new ClampedIntParameter(4, 1, 10);

		public MinFloatParameter rayLength = new MinFloatParameter(10f, 0f);

		[Tooltip("Minmal Smoothness for Reflection. If the surface has a smoothness value below this threshold, a reflection ray will not be case and it will fallback on other techniques.")]
		public ClampedFloatParameter minSmoothness = new ClampedFloatParameter(0.5f, 0f, 1f);

		[AdditionalProperty]
		[Tooltip("Controls which sources are used to fallback on when the traced ray misses.")]
		public RayTracingFallbackHierachyParameter rayMiss = new RayTracingFallbackHierachyParameter(RayTracingFallbackHierachy.ReflectionProbesAndSky);

		[AdditionalProperty]
		[Tooltip("Controls the fallback hierarchy for lighting the last bounce.")]
		public RayTracingFallbackHierachyParameter lastBounce = new RayTracingFallbackHierachyParameter(RayTracingFallbackHierachy.ReflectionProbesAndSky);

		[Tooltip("Controls the dimmer applied to the ambient and legacy light probes.")]
		[AdditionalProperty]
		public ClampedFloatParameter ambientProbeDimmer = new ClampedFloatParameter(1f, 0f, 1f);

		public RecursiveRendering()
		{
			base.displayName = "Recursive Rendering (Preview)";
		}
	}
}
