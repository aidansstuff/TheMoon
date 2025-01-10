using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition-config@14.0.8\\Runtime\\ShaderConfig.cs")]
	public class InternalLightCullingDefs
	{
		public static int s_MaxNrBigTileLightsPlusOne = Math.Clamp((ShaderConfig.FPTLMaxLightCount + 1) * 8, 512, 1024);

		public static int s_LightListMaxCoarseEntries = Math.Clamp(ShaderConfig.FPTLMaxLightCount + 1, 64, 256);

		public static int s_LightClusterMaxCoarseEntries = Math.Clamp((ShaderConfig.FPTLMaxLightCount + 1) * 2, 128, 256);

		public static int s_LightDwordPerFptlTile = (ShaderConfig.FPTLMaxLightCount + 1) / 2;

		public static int s_LightClusterPackingCountBits = (int)Mathf.Ceil(Mathf.Log(Mathf.NextPowerOfTwo(ShaderConfig.FPTLMaxLightCount), 2f));

		public static int s_LightClusterPackingCountMask = (1 << s_LightClusterPackingCountBits) - 1;

		public static int s_LightClusterPackingOffsetBits = 32 - s_LightClusterPackingCountBits;

		public static int s_LightClusterPackingOffsetMask = (1 << s_LightClusterPackingOffsetBits) - 1;
	}
}
