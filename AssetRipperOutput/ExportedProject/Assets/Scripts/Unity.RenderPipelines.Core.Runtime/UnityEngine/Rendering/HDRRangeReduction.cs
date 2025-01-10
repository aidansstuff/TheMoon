namespace UnityEngine.Rendering
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.core@14.0.8\\Runtime\\PostProcessing\\HDROutputDefines.cs")]
	public enum HDRRangeReduction
	{
		None = 0,
		Reinhard = 1,
		BT2390 = 2,
		ACES1000Nits = 3,
		ACES2000Nits = 4,
		ACES4000Nits = 5
	}
}
