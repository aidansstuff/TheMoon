using System;
using UnityEngine.Serialization;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	[Obsolete("For data migration")]
	internal class ObsoleteLightLoopSettings
	{
		public ObsoleteLightLoopSettingsOverrides overrides;

		[FormerlySerializedAs("enableTileAndCluster")]
		public bool enableDeferredTileAndCluster;

		public bool enableComputeLightEvaluation;

		public bool enableComputeLightVariants;

		public bool enableComputeMaterialVariants;

		public bool enableFptlForForwardOpaque;

		public bool enableBigTilePrepass;

		public bool isFptlEnabled;
	}
}
