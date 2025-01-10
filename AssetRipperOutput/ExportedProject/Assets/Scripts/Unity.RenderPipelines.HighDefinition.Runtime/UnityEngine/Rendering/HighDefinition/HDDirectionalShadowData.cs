namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Lighting\\Shadow\\HDShadowManager.cs", needAccessors = false)]
	internal struct HDDirectionalShadowData
	{
		[HLSLArray(4, typeof(Vector4))]
		public unsafe fixed float sphereCascades[16];

		[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
		public Vector4 cascadeDirection;

		[HLSLArray(4, typeof(float))]
		[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
		public unsafe fixed float cascadeBorders[4];
	}
}
