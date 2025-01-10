using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Obsolete]
	internal class VolumetricFog : AtmosphericScattering
	{
		public ColorParameter albedo = new ColorParameter(Color.white);

		public MinFloatParameter meanFreePath = new MinFloatParameter(1000000f, 1f);

		public FloatParameter baseHeight = new FloatParameter(0f);

		public FloatParameter maximumHeight = new FloatParameter(10f);

		public ClampedFloatParameter anisotropy = new ClampedFloatParameter(0f, -1f, 1f);

		public ClampedFloatParameter globalLightProbeDimmer = new ClampedFloatParameter(1f, 0f, 1f);

		public BoolParameter enableDistantFog = new BoolParameter(value: false);

		internal override void PushShaderParameters(HDCamera hdCamera, CommandBuffer cmd)
		{
		}

		private VolumetricFog()
		{
			base.displayName = "Volumetric Fog (Deprecated)";
		}
	}
}
