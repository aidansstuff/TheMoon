using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[Obsolete]
	internal class VolumetricLightingController : VolumeComponent
	{
		[Tooltip("Sets the distance (in meters) from the Camera's Near Clipping Plane to the back of the Camera's volumetric lighting buffer.")]
		public MinFloatParameter depthExtent = new MinFloatParameter(64f, 0.1f);

		[Tooltip("Controls the distribution of slices along the Camera's focal axis. 0 is exponential distribution and 1 is linear distribution.")]
		[FormerlySerializedAs("depthDistributionUniformity")]
		public ClampedFloatParameter sliceDistributionUniformity = new ClampedFloatParameter(0.75f, 0f, 1f);

		private VolumetricLightingController()
		{
			base.displayName = "Volumetric Fog Quality (Deprecated)";
		}
	}
}
