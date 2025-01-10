namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Lighting\\VolumetricLighting\\VolumetricCloudsDef.cs", needAccessors = false, generateCBuffer = true)]
	internal struct ShaderVariablesClouds
	{
		public float _MaxRayMarchingDistance;

		public float _HighestCloudAltitude;

		public float _LowestCloudAltitude;

		public float _EarthRadius;

		public Vector2 _CloudRangeSquared;

		public int _NumPrimarySteps;

		public int _NumLightSteps;

		public Vector4 _CloudMapTiling;

		public Vector2 _WindDirection;

		public Vector2 _WindVector;

		public Vector2 _ShapeNoiseOffset;

		public float _VerticalShapeWindDisplacement;

		public float _VerticalErosionWindDisplacement;

		public float _VerticalShapeNoiseOffset;

		public float _LargeWindSpeed;

		public float _MediumWindSpeed;

		public float _SmallWindSpeed;

		public Vector4 _SunLightColor;

		public Vector4 _SunDirection;

		public int _PhysicallyBasedSun;

		public float _MultiScattering;

		public float _ErosionOcclusion;

		public float _PowderEffectIntensity;

		public float _NormalizationFactor;

		public float _MaxCloudDistance;

		public float _DensityMultiplier;

		public float _ShapeFactor;

		public float _ErosionFactor;

		public float _ShapeScale;

		public float _ErosionScale;

		public float _TemporalAccumulationFactor;

		public Vector4 _ScatteringTint;

		public Vector4 _FinalScreenSize;

		public Vector4 _IntermediateScreenSize;

		public Vector4 _TraceScreenSize;

		public Vector2 _HistoryViewportSize;

		public Vector2 _HistoryBufferSize;

		public int _ExposureSunColor;

		public int _AccumulationFrameIndex;

		public int _SubPixelIndex;

		public int _RenderForSky;

		public float _FadeInStart;

		public float _FadeInDistance;

		public int _LowResolutionEvaluation;

		public int _EnableIntegration;

		public Vector4 _SunRight;

		public Vector4 _SunUp;

		public float _ShadowIntensity;

		public float _ShadowFallbackValue;

		public int _ShadowCookieResolution;

		public float _ShadowPlaneOffset;

		public Vector2 _ShadowRegionSize;

		public float _CloudHistoryInvalidation;

		public float _PaddingVC0;

		public Vector4 _WorldSpaceShadowCenter;

		public Matrix4x4 _CameraViewProjection_NO;

		public Matrix4x4 _CameraInverseViewProjection_NO;

		public Matrix4x4 _CameraPrevViewProjection_NO;

		public Matrix4x4 _CloudsPixelCoordToViewDirWS;

		public float _AltitudeDistortion;

		public float _ErosionFactorCompensation;

		public int _EnableFastToneMapping;

		public int _IsPlanarReflection;

		public int _ValidMaxZMask;

		public float _ImprovedTransmittanceBlend;

		public float _CubicTransmittance;

		public int _Padding1;

		[HLSLArray(12, typeof(Vector4))]
		public unsafe fixed float _DistanceBasedWeights[48];
	}
}
