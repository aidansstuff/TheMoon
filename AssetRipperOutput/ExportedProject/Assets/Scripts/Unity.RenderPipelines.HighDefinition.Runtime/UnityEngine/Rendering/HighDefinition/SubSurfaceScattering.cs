using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Ray Tracing/SubSurface Scattering (Preview)", new Type[] { typeof(HDRenderPipeline) })]
	public sealed class SubSurfaceScattering : VolumeComponent
	{
		[Tooltip("Enable ray traced sub-surface scattering.")]
		public BoolParameter rayTracing = new BoolParameter(value: false);

		[Tooltip("Number of samples for sub-surface scattering.")]
		public ClampedIntParameter sampleCount = new ClampedIntParameter(1, 1, 32);

		public SubSurfaceScattering()
		{
			base.displayName = "SubSurface Scattering (Preview)";
		}
	}
}
