namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Lighting\\ScreenSpaceLighting\\ShaderVariablesScreenSpaceReflection.cs", needAccessors = false, generateCBuffer = true)]
	internal struct ShaderVariablesScreenSpaceReflection
	{
		public float _SsrThicknessScale;

		public float _SsrThicknessBias;

		public int _SsrStencilBit;

		public int _SsrIterLimit;

		public float _SsrRoughnessFadeEnd;

		public float _SsrRoughnessFadeRcpLength;

		public float _SsrRoughnessFadeEndTimesRcpLength;

		public float _SsrEdgeFadeRcpLength;

		public Vector4 _ColorPyramidUvScaleAndLimitPrevFrame;

		public int _SsrDepthPyramidMaxMip;

		public int _SsrColorPyramidMaxMip;

		public int _SsrReflectsSky;

		public float _SsrAccumulationAmount;

		public float _SsrPBRSpeedRejection;

		public float _SsrPBRBias;

		public float _SsrPRBSpeedRejectionScalerFactor;

		public float _SsrPBRPad0;
	}
}
