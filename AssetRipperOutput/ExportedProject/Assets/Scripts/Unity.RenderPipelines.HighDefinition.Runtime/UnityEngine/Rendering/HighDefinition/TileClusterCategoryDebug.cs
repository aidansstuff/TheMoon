using System;

namespace UnityEngine.Rendering.HighDefinition
{
	public enum TileClusterCategoryDebug
	{
		Punctual = 1,
		Area = 2,
		[InspectorName("Area and Punctual")]
		AreaAndPunctual = 3,
		[InspectorName("Reflection Probes")]
		Environment = 4,
		[InspectorName("Reflection Probes and Punctual")]
		EnvironmentAndPunctual = 5,
		[InspectorName("Reflection Probes and Area")]
		EnvironmentAndArea = 6,
		[InspectorName("Reflection Probes, Area and Punctual")]
		EnvironmentAndAreaAndPunctual = 7,
		Decal = 8,
		[Obsolete("Unused")]
		LocalVolumetricFog = 0,
		[Obsolete("Unused", true)]
		[InspectorName("Local Volumetric Fog")]
		DensityVolumes = 0
	}
}
