using System;
using System.Diagnostics;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[DebuggerDisplay("FrameSettings overriding {overrides.ToString(\"X\")}")]
	[Obsolete("For data migration")]
	internal class ObsoleteFrameSettings
	{
		public ObsoleteFrameSettingsOverrides overrides;

		public bool enableShadow;

		public bool enableContactShadows;

		public bool enableShadowMask;

		public bool enableSSR;

		public bool enableSSAO;

		public bool enableSubsurfaceScattering;

		public bool enableTransmission;

		public bool enableAtmosphericScattering;

		public bool enableVolumetrics;

		public bool enableReprojectionForVolumetrics;

		public bool enableLightLayers;

		public bool enableExposureControl = true;

		public float diffuseGlobalDimmer;

		public float specularGlobalDimmer;

		public ObsoleteLitShaderMode shaderLitMode;

		public bool enableDepthPrepassWithDeferredRendering;

		public bool enableTransparentPrepass;

		public bool enableMotionVectors;

		public bool enableObjectMotionVectors;

		[FormerlySerializedAs("enableDBuffer")]
		public bool enableDecals;

		public bool enableRoughRefraction;

		public bool enableTransparentPostpass;

		public bool enableDistortion;

		public bool enablePostprocess;

		public bool enableOpaqueObjects;

		public bool enableTransparentObjects;

		public bool enableRealtimePlanarReflection;

		public bool enableMSAA;

		public bool enableAsyncCompute;

		public bool runLightListAsync;

		public bool runSSRAsync;

		public bool runSSAOAsync;

		public bool runContactShadowsAsync;

		public bool runVolumeVoxelizationAsync;

		public ObsoleteLightLoopSettings lightLoopSettings;
	}
}
