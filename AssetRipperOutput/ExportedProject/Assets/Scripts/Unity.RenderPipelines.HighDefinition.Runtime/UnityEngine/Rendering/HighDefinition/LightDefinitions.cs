namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Lighting\\LightLoop\\LightLoop.cs")]
	internal class LightDefinitions
	{
		public static float s_ViewportScaleZ = 1f;

		public static int s_UseLeftHandCameraSpace = 1;

		public static int s_TileSizeFptl = 16;

		public static int s_TileSizeClustered = 32;

		public static int s_TileSizeBigTile = 64;

		public static int s_TileIndexMask = 32767;

		public static int s_TileIndexShiftX = 0;

		public static int s_TileIndexShiftY = 15;

		public static int s_TileIndexShiftEye = 30;

		public static int s_NumFeatureVariants = 29;

		public static uint s_LightFeatureMaskFlags = 16773120u;

		public static uint s_LightFeatureMaskFlagsOpaque = 16642048u;

		public static uint s_LightFeatureMaskFlagsTransparent = 16510976u;

		public static uint s_MaterialFeatureMaskFlags = 4095u;

		public static uint s_RayTracedScreenSpaceShadowFlag = 4096u;

		public static uint s_ScreenSpaceColorShadowFlag = 256u;

		public static uint s_InvalidScreenSpaceShadow = 255u;

		public static uint s_ScreenSpaceShadowIndexMask = 255u;

		public static int s_ContactShadowFadeBits = 8;

		public static int s_ContactShadowMaskBits = 32 - s_ContactShadowFadeBits;

		public static int s_ContactShadowFadeMask = (1 << s_ContactShadowFadeBits) - 1;

		public static int s_ContactShadowMaskMask = (1 << s_ContactShadowMaskBits) - 1;
	}
}
