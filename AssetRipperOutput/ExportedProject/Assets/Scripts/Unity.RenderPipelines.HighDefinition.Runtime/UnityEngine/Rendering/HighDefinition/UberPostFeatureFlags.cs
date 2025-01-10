using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Flags]
	internal enum UberPostFeatureFlags
	{
		None = 0,
		ChromaticAberration = 1,
		Vignette = 2,
		LensDistortion = 4,
		EnableAlpha = 8
	}
}
