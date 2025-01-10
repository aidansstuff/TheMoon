namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Sky\\PhysicallyBasedSky\\ShaderVariablesPhysicallyBasedSky.cs", needAccessors = false, generateCBuffer = true, constantRegister = 2)]
	internal struct ShaderVariablesPhysicallyBasedSky
	{
		public float _PlanetaryRadius;

		public float _RcpPlanetaryRadius;

		public float _AtmosphericDepth;

		public float _RcpAtmosphericDepth;

		public float _AtmosphericRadius;

		public float _AerosolAnisotropy;

		public float _AerosolPhasePartConstant;

		public float _Unused;

		public float _AirDensityFalloff;

		public float _AirScaleHeight;

		public float _AerosolDensityFalloff;

		public float _AerosolScaleHeight;

		public Vector4 _AirSeaLevelExtinction;

		public Vector4 _AirSeaLevelScattering;

		public Vector4 _AerosolSeaLevelScattering;

		public Vector4 _GroundAlbedo;

		public Vector4 _PlanetCenterPosition;

		public Vector4 _HorizonTint;

		public Vector4 _ZenithTint;

		public float _AerosolSeaLevelExtinction;

		public float _IntensityMultiplier;

		public float _ColorSaturation;

		public float _AlphaSaturation;

		public float _AlphaMultiplier;

		public float _HorizonZenithShiftPower;

		public float _HorizonZenithShiftScale;

		public float _Unused2;
	}
}
