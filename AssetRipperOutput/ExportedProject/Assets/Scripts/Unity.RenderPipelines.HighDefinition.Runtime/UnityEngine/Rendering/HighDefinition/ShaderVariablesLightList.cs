namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Lighting\\LightLoop\\LightLoop.cs", needAccessors = false, generateCBuffer = true)]
	internal struct ShaderVariablesLightList
	{
		[HLSLArray(2, typeof(Matrix4x4))]
		public unsafe fixed float g_mInvScrProjectionArr[32];

		[HLSLArray(2, typeof(Matrix4x4))]
		public unsafe fixed float g_mScrProjectionArr[32];

		[HLSLArray(2, typeof(Matrix4x4))]
		public unsafe fixed float g_mInvProjectionArr[32];

		[HLSLArray(2, typeof(Matrix4x4))]
		public unsafe fixed float g_mProjectionArr[32];

		public Vector4 g_screenSize;

		public Vector2Int g_viDimensions;

		public int g_iNrVisibLights;

		public uint g_isOrthographic;

		public uint g_BaseFeatureFlags;

		public int g_iNumSamplesMSAA;

		public uint _EnvLightIndexShift;

		public uint _DecalIndexShift;
	}
}
