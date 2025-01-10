namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Lighting\\LightDefinition.cs", needAccessors = false, generateCBuffer = true)]
	internal struct EnvLightReflectionData
	{
		public const int s_MaxPlanarReflections = 16;

		public const int s_MaxCubeReflections = 64;

		[HLSLArray(16, typeof(Matrix4x4))]
		public unsafe fixed float _PlanarCaptureVP[256];

		[HLSLArray(16, typeof(Vector4))]
		public unsafe fixed float _PlanarScaleOffset[64];

		[HLSLArray(64, typeof(Vector4))]
		public unsafe fixed float _CubeScaleOffset[256];
	}
}
