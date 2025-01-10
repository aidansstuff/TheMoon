using UnityEngine.Rendering.HighDefinition.Attributes;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class Water : RenderPipelineMaterial
	{
		[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\Water\\Water.cs")]
		public enum MaterialFeatureFlags
		{
			WaterStandard = 1,
			WaterCinematic = 2
		}

		[GenerateHLSL(PackingRules.Exact, false, false, true, 1600, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\Water\\Water.cs")]
		public struct SurfaceData
		{
			[MaterialSharedPropertyMapping(MaterialSharedProperty.Albedo)]
			[SurfaceDataAttributes("Base Color", false, true, FieldPrecision.Default, false, "")]
			public Vector3 baseColor;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.Normal)]
			[SurfaceDataAttributes(new string[] { "Normal WS", "Normal View Space" }, true, false, FieldPrecision.Default, true, "")]
			public Vector3 normalWS;

			[SurfaceDataAttributes(new string[] { "Low Frequency Normal WS", "Low Frequency Normal View Space" }, true, false, FieldPrecision.Default, true, "")]
			public Vector3 lowFrequencyNormalWS;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.Smoothness)]
			[SurfaceDataAttributes("Smoothness", false, false, FieldPrecision.Default, false, "")]
			public float perceptualSmoothness;

			[SurfaceDataAttributes("Foam", false, false, FieldPrecision.Default, false, "")]
			public float foam;

			public float tipThickness;

			[SurfaceDataAttributes("Caustics", false, false, FieldPrecision.Default, false, "")]
			public float caustics;
		}

		[GenerateHLSL(PackingRules.Exact, false, false, true, 1650, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\Water\\Water.cs")]
		public struct BSDFData
		{
			[SurfaceDataAttributes("", false, true, FieldPrecision.Default, false, "")]
			public Vector3 diffuseColor;

			public Vector3 fresnel0;

			[SurfaceDataAttributes(new string[] { "Normal WS", "Normal View Space" }, true, false, FieldPrecision.Default, true, "")]
			public Vector3 normalWS;

			[SurfaceDataAttributes(new string[] { "Low Frequency Normal WS", "Low Frequency Normal View Space" }, true, false, FieldPrecision.Default, false, "")]
			public Vector3 lowFrequencyNormalWS;

			public float perceptualRoughness;

			public float roughness;

			public float caustics;

			public float foam;

			public float tipThickness;

			public uint surfaceIndex;
		}
	}
}
