namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, false, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Lighting\\LightDefinition.cs")]
	internal struct EnvLightData
	{
		public uint lightLayers;

		public Vector3 capturePositionRWS;

		public EnvShapeType influenceShapeType;

		public Vector3 proxyExtents;

		[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
		public float minProjectionDistance;

		public Vector3 proxyPositionRWS;

		public Vector3 proxyForward;

		public Vector3 proxyUp;

		public Vector3 proxyRight;

		public Vector3 influencePositionRWS;

		public Vector3 influenceForward;

		public Vector3 influenceUp;

		public Vector3 influenceRight;

		public Vector3 influenceExtents;

		public Vector3 blendDistancePositive;

		public Vector3 blendDistanceNegative;

		public Vector3 blendNormalDistancePositive;

		public Vector3 blendNormalDistanceNegative;

		[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
		public Vector3 boxSideFadePositive;

		[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
		public Vector3 boxSideFadeNegative;

		public float weight;

		public float multiplier;

		public float rangeCompressionFactorCompensation;

		public float roughReflections;

		public float distanceBasedRoughness;

		public int envIndex;

		public Vector4 L0L1;

		public Vector4 L2_1;

		public float L2_2;

		public int normalizeWithAPV;

		public Vector2 padding;
	}
}
