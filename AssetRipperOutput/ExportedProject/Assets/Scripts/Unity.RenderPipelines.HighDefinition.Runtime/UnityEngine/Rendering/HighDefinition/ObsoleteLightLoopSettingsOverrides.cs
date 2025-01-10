using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Flags]
	[Obsolete("For data migration")]
	internal enum ObsoleteLightLoopSettingsOverrides
	{
		FptlForForwardOpaque = 1,
		BigTilePrepass = 2,
		ComputeLightEvaluation = 4,
		ComputeLightVariants = 8,
		ComputeMaterialVariants = 0x10,
		TileAndCluster = 0x20
	}
}
