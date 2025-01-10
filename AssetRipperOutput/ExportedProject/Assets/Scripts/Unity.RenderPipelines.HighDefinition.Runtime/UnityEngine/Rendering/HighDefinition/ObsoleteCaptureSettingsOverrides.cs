using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Flags]
	[Obsolete]
	internal enum ObsoleteCaptureSettingsOverrides
	{
		ClearColorMode = 4,
		BackgroundColorHDR = 8,
		ClearDepth = 0x10,
		CullingMask = 0x20,
		UseOcclusionCulling = 0x40,
		VolumeLayerMask = 0x80,
		VolumeAnchorOverride = 0x100,
		Projection = 0x200,
		NearClip = 0x400,
		FarClip = 0x800,
		FieldOfview = 0x1000,
		OrphographicSize = 0x2000,
		RenderingPath = 0x4000,
		ShadowDistance = 0x40000
	}
}
