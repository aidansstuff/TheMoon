namespace UnityEngine.Rendering.HighDefinition
{
	internal struct HDLightRenderData
	{
		public HDAdditionalLightData.PointLightHDType pointLightType;

		public SpotLightShape spotLightShape;

		public AreaLightShape areaLightShape;

		public LightLayerEnum lightLayer;

		public float fadeDistance;

		public float distance;

		public float angularDiameter;

		public float volumetricFadeDistance;

		public bool includeForRayTracing;

		public bool useScreenSpaceShadows;

		public bool useRayTracedShadows;

		public bool colorShadow;

		public float lightDimmer;

		public float volumetricDimmer;

		public float shadowDimmer;

		public float shadowFadeDistance;

		public float volumetricShadowDimmer;

		public float shapeWidth;

		public float shapeHeight;

		public float aspectRatio;

		public float innerSpotPercent;

		public float spotIESCutoffPercent;

		public float shapeRadius;

		public float barnDoorLength;

		public float barnDoorAngle;

		public float flareSize;

		public float flareFalloff;

		public bool affectVolumetric;

		public bool affectDiffuse;

		public bool affectSpecular;

		public bool applyRangeAttenuation;

		public bool penumbraTint;

		public bool interactsWithSky;

		public Color surfaceTint;

		public Color shadowTint;

		public Color flareTint;
	}
}
