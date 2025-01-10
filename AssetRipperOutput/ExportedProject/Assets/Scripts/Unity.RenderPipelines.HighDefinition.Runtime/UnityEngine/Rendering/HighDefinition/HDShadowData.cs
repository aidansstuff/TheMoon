namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Lighting\\Shadow\\HDShadowManager.cs", needAccessors = false)]
	internal struct HDShadowData
	{
		public Vector3 rot0;

		public Vector3 rot1;

		public Vector3 rot2;

		public Vector3 pos;

		public Vector4 proj;

		public Vector2 atlasOffset;

		public float worldTexelSize;

		public float normalBias;

		[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
		public Vector4 zBufferParam;

		public Vector4 shadowMapSize;

		public Vector4 shadowFilterParams0;

		public Vector3 cacheTranslationDelta;

		public float isInCachedAtlas;

		public Matrix4x4 shadowToWorld;
	}
}
