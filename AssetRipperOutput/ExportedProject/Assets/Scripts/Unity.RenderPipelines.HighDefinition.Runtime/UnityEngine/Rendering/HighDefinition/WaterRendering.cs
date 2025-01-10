using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[VolumeComponentMenuForRenderPipeline("Lighting/WaterRendering", new Type[] { typeof(HDRenderPipeline) })]
	public sealed class WaterRendering : VolumeComponent
	{
		public enum WaterGridResolution
		{
			VeryLow128 = 0x80,
			Low256 = 0x100,
			Medium512 = 0x200,
			High1024 = 0x400,
			Ultra2048 = 0x800
		}

		[Serializable]
		public sealed class WaterGridResolutionParameter : VolumeParameter<WaterGridResolution>
		{
			public WaterGridResolutionParameter(WaterGridResolution value, bool overrideState = false)
				: base(value, overrideState)
			{
			}
		}

		[Tooltip("When enabled, the water surfaces are rendered.")]
		public BoolParameter enable = new BoolParameter(value: false, BoolParameter.DisplayType.EnumPopup);

		[Tooltip("Sets the size of the minimum water grids in meters.")]
		public MinFloatParameter minGridSize = new MinFloatParameter(50f, 50f);

		[Tooltip("Sets the size of the maximum water grids in meters.")]
		public MinFloatParameter maxGridSize = new MinFloatParameter(2500f, 250f);

		[Tooltip("Sets the elevation at which the max grid size is reached.")]
		public MinFloatParameter elevationTransition = new MinFloatParameter(1000f, 20f);

		[Tooltip("Controls the number of LOD patches that are rendered.")]
		public ClampedIntParameter numLevelOfDetails = new ClampedIntParameter(3, 1, 4);

		[Tooltip("Sets the maximum tessellation factor for the water surface.")]
		[AdditionalProperty]
		public ClampedFloatParameter maxTessellationFactor = new ClampedFloatParameter(10f, 0f, 15f);

		[Tooltip(" Sets the distance at which the tessellation factor start to lower.")]
		[AdditionalProperty]
		public MinFloatParameter tessellationFactorFadeStart = new MinFloatParameter(150f, 0f);

		[Tooltip("Sets the range at which the tessellation factor reaches zero.")]
		[AdditionalProperty]
		public MinFloatParameter tessellationFactorFadeRange = new MinFloatParameter(1850f, 10f);

		[Tooltip("Controls the influence of the ambient light probe on the water surfaces.")]
		public ClampedFloatParameter ambientProbeDimmer = new ClampedFloatParameter(1f, 0f, 1f);

		private WaterRendering()
		{
			base.displayName = "Water Rendering";
		}
	}
}
