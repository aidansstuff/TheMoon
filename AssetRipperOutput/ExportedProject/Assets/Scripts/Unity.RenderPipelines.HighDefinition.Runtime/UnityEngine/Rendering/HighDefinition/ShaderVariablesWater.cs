namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Water\\WaterSystemDef.cs", needAccessors = false, generateCBuffer = true)]
	internal struct ShaderVariablesWater
	{
		public uint _BandResolution;

		public float _MaxWaveHeight;

		public float _SimulationTime;

		public float _ScatteringWaveHeight;

		public Vector4 _PatchSize;

		public Vector4 _PatchAmplitudeMultiplier;

		public Vector4 _PatchDirectionDampener;

		public Vector4 _PatchWindSpeed;

		public Vector4 _PatchWindOrientation;

		public Vector4 _PatchCurrentSpeed;

		public Vector4 _PatchCurrentOrientation;

		public Vector4 _PatchFadeStart;

		public Vector4 _PatchFadeDistance;

		public Vector4 _PatchFadeValue;

		public float _SimulationFoamSmoothness;

		public float _JacobianDrag;

		public float _SimulationFoamAmount;

		public float _SSSMaskCoefficient;

		public float _Choppiness;

		public float _DeltaTime;

		public float _MaxWaveDisplacement;

		public float _MaxRefractionDistance;

		public Vector2 _FoamOffsets;

		public float _FoamTilling;

		public float _WindFoamAttenuation;

		public Vector4 _TransparencyColor;

		public Vector4 _ScatteringColorTips;

		public float _DisplacementScattering;

		public int _WaterInitialFrame;

		public int _SurfaceIndex;

		public float _CausticsRegionSize;

		public Vector4 _ScatteringLambertLighting;

		public Vector4 _DeepFoamColor;

		public float _OutScatteringCoefficient;

		public float _FoamSmoothness;

		public float _HeightBasedScattering;

		public float _WaterSmoothness;

		public Vector4 _FoamJacobianLambda;

		public int _WaterRefSimRes;

		public float _WaterSpectrumOffset;

		public int _WaterSampleOffset;

		public int _WaterBandCount;

		public Vector2 _PaddingW0;

		public float _AmbientScattering;

		public int _CausticsBandIndex;
	}
}
