using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Visual Environment", new Type[] { typeof(HDRenderPipeline) })]
	public sealed class VisualEnvironment : VolumeComponent
	{
		[Header("Sky")]
		public NoInterpIntParameter skyType = new NoInterpIntParameter(0);

		public NoInterpIntParameter cloudType = new NoInterpIntParameter(0);

		public SkyAmbientModeParameter skyAmbientMode = new SkyAmbientModeParameter(SkyAmbientMode.Dynamic);

		[Header("Wind")]
		public ClampedFloatParameter windOrientation = new ClampedFloatParameter(0f, 0f, 360f);

		public FloatParameter windSpeed = new FloatParameter(0f);

		[SerializeField]
		internal FogTypeParameter fogType = new FogTypeParameter(FogType.None);
	}
}
