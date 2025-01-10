using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Flags]
	[Obsolete("For data migration")]
	internal enum ObsoleteFrameSettingsOverrides
	{
		Shadow = 1,
		ContactShadow = 2,
		ShadowMask = 4,
		SSR = 8,
		SSAO = 0x10,
		SubsurfaceScattering = 0x20,
		Transmission = 0x40,
		AtmosphericScaterring = 0x80,
		Volumetrics = 0x100,
		ReprojectionForVolumetrics = 0x200,
		LightLayers = 0x400,
		MSAA = 0x800,
		ExposureControl = 0x1000,
		TransparentPrepass = 0x2000,
		TransparentPostpass = 0x4000,
		MotionVectors = 0x8000,
		ObjectMotionVectors = 0x10000,
		Decals = 0x20000,
		RoughRefraction = 0x40000,
		Distortion = 0x80000,
		Postprocess = 0x100000,
		ShaderLitMode = 0x200000,
		DepthPrepassWithDeferredRendering = 0x400000,
		OpaqueObjects = 0x1000000,
		TransparentObjects = 0x2000000,
		AsyncCompute = 0x800000,
		LightListAsync = 0x8000000,
		SSRAsync = 0x10000000,
		SSAOAsync = 0x20000000,
		ContactShadowsAsync = 0x40000000,
		VolumeVoxelizationsAsync = int.MinValue
	}
}
