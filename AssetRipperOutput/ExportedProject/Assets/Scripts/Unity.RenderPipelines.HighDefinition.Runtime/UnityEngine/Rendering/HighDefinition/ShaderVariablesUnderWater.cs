namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Water\\WaterSystemDef.cs", needAccessors = false, generateCBuffer = true)]
	internal struct ShaderVariablesUnderWater
	{
		public Vector4 _WaterRefractionColor;

		public Vector4 _WaterScatteringColor;

		public float _MaxViewDistanceMultiplier;

		public float _OutScatteringCoeff;

		public float _WaterTransitionSize;

		public float _PaddingUW;
	}
}
