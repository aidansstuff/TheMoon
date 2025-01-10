namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Water\\WaterSystemDef.cs", needAccessors = false, generateCBuffer = true)]
	internal struct ShaderVariablesWaterRendering
	{
		public Vector2 _GridSize;

		public Vector2 _WaterRotation;

		public Vector4 _PatchOffset;

		public uint _WaterLODCount;

		public uint _NumWaterPatches;

		public float _FoamIntensity;

		public float _CausticsIntensity;

		public Vector2 _WaterMaskScale;

		public Vector2 _WaterMaskOffset;

		public Vector2 _FoamMaskScale;

		public Vector2 _FoamMaskOffset;

		public float _CausticsPlaneBlendDistance;

		public int _WaterCausticsEnabled;

		public uint _WaterDecalLayer;

		public int _InfiniteSurface;

		public float _WaterMaxTessellationFactor;

		public float _WaterTessellationFadeStart;

		public float _WaterTessellationFadeRange;

		public int _CameraInUnderwaterRegion;
	}
}
