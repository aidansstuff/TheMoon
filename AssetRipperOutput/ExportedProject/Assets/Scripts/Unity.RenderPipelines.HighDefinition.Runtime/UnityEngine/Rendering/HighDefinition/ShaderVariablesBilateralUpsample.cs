namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Lighting\\ScreenSpaceLighting\\BilateralUpsampleDef.cs", needAccessors = false, generateCBuffer = true)]
	internal struct ShaderVariablesBilateralUpsample
	{
		public Vector4 _HalfScreenSize;

		[HLSLArray(12, typeof(Vector4))]
		public unsafe fixed float _DistanceBasedWeights[48];

		[HLSLArray(8, typeof(Vector4))]
		public unsafe fixed float _TapOffsets[32];
	}
}
