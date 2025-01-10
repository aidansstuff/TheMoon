namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Lighting\\VolumetricLighting\\HDRenderPipeline.VolumetricLighting.cs")]
	internal struct LocalVolumetricFogEngineData
	{
		public Vector3 scattering;

		public float extinction;

		public Vector3 textureTiling;

		public int invertFade;

		public Vector3 textureScroll;

		public float rcpDistFadeLen;

		public Vector3 rcpPosFaceFade;

		public float endTimesRcpDistFadeLen;

		public Vector3 rcpNegFaceFade;

		public LocalVolumetricFogBlendingMode blendingMode;

		public Vector3 albedo;

		public LocalVolumetricFogFalloffMode falloffMode;

		public static LocalVolumetricFogEngineData GetNeutralValues()
		{
			LocalVolumetricFogEngineData result = default(LocalVolumetricFogEngineData);
			result.scattering = Vector3.zero;
			result.extinction = 0f;
			result.textureTiling = Vector3.one;
			result.textureScroll = Vector3.zero;
			result.rcpPosFaceFade = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			result.rcpNegFaceFade = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
			result.invertFade = 0;
			result.rcpDistFadeLen = 0f;
			result.endTimesRcpDistFadeLen = 1f;
			result.falloffMode = LocalVolumetricFogFalloffMode.Linear;
			result.blendingMode = LocalVolumetricFogBlendingMode.Additive;
			result.albedo = Vector3.zero;
			return result;
		}
	}
}
