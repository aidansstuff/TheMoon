namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, false, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Lighting\\LightDefinition.cs")]
	internal struct LightData
	{
		public Vector3 positionRWS;

		public uint lightLayers;

		public float lightDimmer;

		public float volumetricLightDimmer;

		[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
		public float angleScale;

		[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
		public float angleOffset;

		public Vector3 forward;

		public float iesCut;

		public GPULightType lightType;

		public Vector3 right;

		public float penumbraTint;

		[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
		public float range;

		public CookieMode cookieMode;

		public int shadowIndex;

		public Vector3 up;

		public float rangeAttenuationScale;

		public Vector3 color;

		public float rangeAttenuationBias;

		public Vector4 cookieScaleOffset;

		public Vector3 shadowTint;

		public float shadowDimmer;

		public float volumetricShadowDimmer;

		public int nonLightMappedOnly;

		[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
		public float minRoughness;

		public int screenSpaceShadowIndex;

		[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
		public Vector4 shadowMaskSelector;

		[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
		public Vector4 size;

		public int contactShadowMask;

		public float diffuseDimmer;

		public float specularDimmer;

		public float __unused__;

		public Vector2 padding;

		public float isRayTracedContactShadow;

		public float boxLightSafeExtent;
	}
}
