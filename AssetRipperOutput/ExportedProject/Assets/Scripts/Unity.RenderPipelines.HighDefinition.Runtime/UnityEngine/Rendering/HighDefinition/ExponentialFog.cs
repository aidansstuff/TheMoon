using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Obsolete]
	internal class ExponentialFog : AtmosphericScattering
	{
		private static readonly int m_ExpFogParam = Shader.PropertyToID("_ExpFogParameters");

		[Tooltip("Sets the distance from the Camera at which the fog reaches its maximum thickness.")]
		public MinFloatParameter fogDistance = new MinFloatParameter(200f, 0f);

		[Tooltip("Sets the height, in world space, at which HDRP begins to decrease the fog density from 1.0.")]
		public FloatParameter fogBaseHeight = new FloatParameter(0f);

		[Tooltip("Controls the falloff of height fog attenuation, larger values result in sharper attenuation.")]
		public ClampedFloatParameter fogHeightAttenuation = new ClampedFloatParameter(0.2f, 0f, 1f);

		internal override void PushShaderParameters(HDCamera hdCamera, CommandBuffer cmd)
		{
		}

		private ExponentialFog()
		{
			base.displayName = "Exponential Fog (Deprecated)";
		}
	}
}
