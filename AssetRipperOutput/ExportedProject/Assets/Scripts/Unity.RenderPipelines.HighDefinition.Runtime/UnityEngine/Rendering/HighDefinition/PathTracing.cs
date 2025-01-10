using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Ray Tracing/Path Tracing (Preview)", new Type[] { typeof(HDRenderPipeline) })]
	public sealed class PathTracing : VolumeComponent
	{
		[Tooltip("Enables path tracing (thus disabling most other passes).")]
		public BoolParameter enable = new BoolParameter(value: false, BoolParameter.DisplayType.EnumPopup);

		[Tooltip("Defines the layers that path tracing should include.")]
		public LayerMaskParameter layerMask = new LayerMaskParameter(-1);

		[Tooltip("Defines the maximum number of paths cast within each pixel, over time (one per frame).")]
		public ClampedIntParameter maximumSamples = new ClampedIntParameter(256, 1, 16384);

		[Tooltip("Defines the minimum number of bounces for each path, in [1, 32].")]
		public ClampedIntParameter minimumDepth = new ClampedIntParameter(1, 1, 32);

		[Tooltip("Defines the maximum number of bounces for each path, in [minimumDepth, 32].")]
		public ClampedIntParameter maximumDepth = new ClampedIntParameter(4, 1, 32);

		[Tooltip("Defines the maximum, post-exposed luminance computed for indirect path segments. Lower values help prevent noise and fireflies (very bright pixels), but introduce bias by darkening the overall result. Increase this value if your image looks too dark.")]
		public MinFloatParameter maximumIntensity = new MinFloatParameter(10f, 0f);

		[Tooltip("Defines if and when sky importance sampling is enabled. It should be turned on for sky models with high contrast and bright spots, and turned off for smooth, uniform skies.")]
		public SkyImportanceSamplingParameter skyImportanceSampling = new SkyImportanceSamplingParameter(SkyImportanceSamplingMode.HDRIOnly);

		[Tooltip("Defines the number of tiles (X: width, Y: height) and the indices of the current tile (Z: i in [0, width[, W: j in [0, height[) for interleaved tiled rendering.")]
		public Vector4Parameter tilingParameters = new Vector4Parameter(new Vector4(1f, 1f, 0f, 0f));

		public PathTracing()
		{
			base.displayName = "Path Tracing (Preview)";
		}
	}
}
