namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, false, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Lighting\\LightDefinition.cs")]
	internal struct DirectionalLightData
	{
		public Vector3 positionRWS;

		public uint lightLayers;

		public float lightDimmer;

		public float volumetricLightDimmer;

		public Vector3 forward;

		public CookieMode cookieMode;

		public Vector4 cookieScaleOffset;

		public Vector3 right;

		public int shadowIndex;

		public Vector3 up;

		public int contactShadowIndex;

		public Vector3 color;

		public int contactShadowMask;

		public Vector3 shadowTint;

		public float shadowDimmer;

		public float volumetricShadowDimmer;

		public int nonLightMappedOnly;

		[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
		public float minRoughness;

		public int screenSpaceShadowIndex;

		[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
		public Vector4 shadowMaskSelector;

		public Vector2 cascadesBorderFadeScaleBias;

		public float diffuseDimmer;

		public float specularDimmer;

		public float penumbraTint;

		public float isRayTracedContactShadow;

		public float distanceFromCamera;

		public float angularDiameter;

		public float flareFalloff;

		public float flareCosInner;

		public float flareCosOuter;

		public float __unused__;

		public Vector3 flareTint;

		public float flareSize;

		public Vector3 surfaceTint;

		public Vector4 surfaceTextureScaleOffset;
	}
}
