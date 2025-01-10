using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Flags]
	public enum CameraSettingsFields
	{
		none = 0,
		bufferClearColorMode = 2,
		bufferClearBackgroundColorHDR = 4,
		bufferClearClearDepth = 8,
		volumesLayerMask = 0x10,
		volumesAnchorOverride = 0x20,
		frustumMode = 0x40,
		frustumAspect = 0x80,
		frustumFarClipPlane = 0x100,
		frustumNearClipPlane = 0x200,
		frustumFieldOfView = 0x400,
		frustumProjectionMatrix = 0x800,
		cullingUseOcclusionCulling = 0x1000,
		cullingCullingMask = 0x2000,
		cullingInvertFaceCulling = 0x4000,
		customRenderingSettings = 0x8000,
		flipYMode = 0x10000,
		frameSettings = 0x20000,
		probeLayerMask = 0x40000
	}
}
