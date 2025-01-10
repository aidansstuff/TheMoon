namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Debug\\DebugDisplay.cs", needAccessors = false, generateCBuffer = true)]
	internal struct ShaderVariablesDebugDisplay
	{
		[HLSLArray(32, typeof(Vector4))]
		public unsafe fixed float _DebugRenderingLayersColors[128];

		[HLSLArray(11, typeof(ShaderGenUInt4))]
		public unsafe fixed uint _DebugViewMaterialArray[44];

		[HLSLArray(7, typeof(Vector4))]
		public unsafe fixed float _DebugAPVSubdivColors[28];

		public int _DebugLightingMode;

		public int _DebugLightLayersMask;

		public int _DebugShadowMapMode;

		public int _DebugMipMapMode;

		public int _DebugFullScreenMode;

		public float _DebugTransparencyOverdrawWeight;

		public int _DebugMipMapModeTerrainTexture;

		public int _ColorPickerMode;

		public Vector4 _DebugViewportSize;

		public Vector4 _DebugLightingAlbedo;

		public Vector4 _DebugLightingSmoothness;

		public Vector4 _DebugLightingNormal;

		public Vector4 _DebugLightingAmbientOcclusion;

		public Vector4 _DebugLightingSpecularColor;

		public Vector4 _DebugLightingEmissiveColor;

		public Vector4 _DebugLightingMaterialValidateHighColor;

		public Vector4 _DebugLightingMaterialValidateLowColor;

		public Vector4 _DebugLightingMaterialValidatePureMetalColor;

		public Vector4 _MousePixelCoord;

		public Vector4 _MouseClickPixelCoord;

		public int _MatcapMixAlbedo;

		public float _MatcapViewScale;

		public int _DebugSingleShadowIndex;

		public int _DebugIsLitShaderModeDeferred;

		public int _DebugAOVOutput;

		public float _ShaderVariablesDebugDisplayPad0;

		public float _ShaderVariablesDebugDisplayPad1;

		public float _ShaderVariablesDebugDisplayPad2;
	}
}
