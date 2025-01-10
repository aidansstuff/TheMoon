namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\RenderPipeline\\Raytracing\\Shaders\\ShaderVariablesRaytracingLightLoop.cs", needAccessors = false, generateCBuffer = true, constantRegister = 4)]
	internal struct ShaderVariablesRaytracingLightLoop
	{
		public Vector3 _MinClusterPos;

		public uint _LightPerCellCount;

		public Vector3 _MaxClusterPos;

		public uint _PunctualLightCountRT;

		public uint _AreaLightCountRT;

		public uint _EnvLightCountRT;
	}
}
