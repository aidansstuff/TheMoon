using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Flags]
	public enum ProbeSettingsFields
	{
		none = 0,
		type = 1,
		mode = 2,
		lightingMultiplier = 4,
		lightingWeight = 8,
		lightingLightLayer = 0x10,
		lightingRangeCompression = 0x20,
		proxyUseInfluenceVolumeAsProxyVolume = 0x40,
		proxyCapturePositionProxySpace = 0x80,
		proxyCaptureRotationProxySpace = 0x100,
		proxyMirrorPositionProxySpace = 0x200,
		proxyMirrorRotationProxySpace = 0x400,
		frustumFieldOfViewMode = 0x800,
		frustumFixedValue = 0x1000,
		frustumAutomaticScale = 0x2000,
		frustumViewerScale = 0x4000,
		lightingFadeDistance = 0x8000,
		resolution = 0x10000,
		roughReflections = 0x20000,
		cubeResolution = 0x40000
	}
}
