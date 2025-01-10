using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Ray Tracing/Light Cluster (Preview)", new Type[] { typeof(HDRenderPipeline) })]
	public sealed class LightCluster : VolumeComponent
	{
		[Tooltip("Controls the range of the cluster around the camera in meters.")]
		public MinFloatParameter cameraClusterRange = new MinFloatParameter(10f, 0.001f);

		public LightCluster()
		{
			base.displayName = "Light Cluster (Preview)";
		}
	}
}
