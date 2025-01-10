using UnityEngine.Rendering.HighDefinition.Attributes;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class Lit : RenderPipelineMaterial
	{
		[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\Lit\\Lit.cs")]
		public enum MaterialFeatureFlags
		{
			LitStandard = 1,
			LitSpecularColor = 2,
			LitSubsurfaceScattering = 4,
			LitTransmission = 8,
			LitAnisotropy = 0x10,
			LitIridescence = 0x20,
			LitClearCoat = 0x40
		}

		[GenerateHLSL(PackingRules.Exact, false, false, true, 1000, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\Lit\\Lit.cs")]
		public struct SurfaceData
		{
			[SurfaceDataAttributes("Material Features", false, false, FieldPrecision.Default, false, "")]
			public uint materialFeatures;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.Albedo)]
			[SurfaceDataAttributes("Base Color", false, true, FieldPrecision.Real, false, "")]
			public Vector3 baseColor;

			[SurfaceDataAttributes("Specular Occlusion", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float specularOcclusion;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.Normal)]
			[SurfaceDataAttributes(new string[] { "Normal", "Normal View Space" }, true, false, FieldPrecision.Default, false, "", checkIsNormalized = true)]
			public Vector3 normalWS;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.Smoothness)]
			[SurfaceDataAttributes("Smoothness", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float perceptualSmoothness;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.AmbientOcclusion)]
			[SurfaceDataAttributes("Ambient Occlusion", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float ambientOcclusion;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.Metal)]
			[SurfaceDataAttributes("Metallic", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float metallic;

			[SurfaceDataAttributes("Coat mask", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float coatMask;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.Specular)]
			[SurfaceDataAttributes("Specular Color", false, true, FieldPrecision.Real, false, "")]
			public Vector3 specularColor;

			[SurfaceDataAttributes("Diffusion Profile Hash", false, false, FieldPrecision.Default, false, "")]
			public uint diffusionProfileHash;

			[SurfaceDataAttributes("Subsurface Mask", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float subsurfaceMask;

			[SurfaceDataAttributes("Transmission Mask", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float transmissionMask;

			[SurfaceDataAttributes("Thickness", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float thickness;

			[SurfaceDataAttributes("Tangent", true, false, FieldPrecision.Default, false, "")]
			public Vector3 tangentWS;

			[SurfaceDataAttributes("Anisotropy", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float anisotropy;

			[SurfaceDataAttributes("Iridescence Layer Thickness", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float iridescenceThickness;

			[SurfaceDataAttributes("Iridescence Mask", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float iridescenceMask;

			[SurfaceDataAttributes(new string[] { "Geometric Normal", "Geometric Normal View Space" }, true, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real, checkIsNormalized = true)]
			public Vector3 geomNormalWS;

			[SurfaceDataAttributes("Index of refraction", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float ior;

			[SurfaceDataAttributes("Transmittance Color", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public Vector3 transmittanceColor;

			[SurfaceDataAttributes("Transmittance Absorption Distance", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float atDistance;

			[SurfaceDataAttributes("Transmittance Mask", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float transmittanceMask;
		}

		[GenerateHLSL(PackingRules.Exact, false, false, true, 1050, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\Lit\\Lit.cs")]
		public struct BSDFData
		{
			public uint materialFeatures;

			[SurfaceDataAttributes("", false, true, FieldPrecision.Real, false, "")]
			public Vector3 diffuseColor;

			[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public Vector3 fresnel0;

			[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float ambientOcclusion;

			[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float specularOcclusion;

			[SurfaceDataAttributes(new string[] { "Normal WS", "Normal View Space" }, true, false, FieldPrecision.Default, true, "")]
			public Vector3 normalWS;

			[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float perceptualRoughness;

			[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float coatMask;

			public uint diffusionProfileIndex;

			[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float subsurfaceMask;

			[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float thickness;

			public bool useThickObjectMode;

			[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public Vector3 transmittance;

			[SurfaceDataAttributes("", true, false, FieldPrecision.Default, false, "")]
			public Vector3 tangentWS;

			[SurfaceDataAttributes("", true, false, FieldPrecision.Default, false, "")]
			public Vector3 bitangentWS;

			[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float roughnessT;

			[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float roughnessB;

			[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float anisotropy;

			[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float iridescenceThickness;

			[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float iridescenceMask;

			[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float coatRoughness;

			[SurfaceDataAttributes(new string[] { "Geometric Normal", "Geometric Normal View Space" }, true, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real, checkIsNormalized = true)]
			public Vector3 geomNormalWS;

			[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float ior;

			[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public Vector3 absorptionCoefficient;

			[SurfaceDataAttributes("", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float transmittanceMask;
		}

		public override bool IsDefferedMaterial()
		{
			return true;
		}

		public override void Build(HDRenderPipelineAsset hdAsset, HDRenderPipelineRuntimeResources defaultResources)
		{
			PreIntegratedFGD.instance.Build(PreIntegratedFGD.FGDIndex.FGD_GGXAndDisneyDiffuse);
			LTCAreaLight.instance.Build();
		}

		public override void Cleanup()
		{
			PreIntegratedFGD.instance.Cleanup(PreIntegratedFGD.FGDIndex.FGD_GGXAndDisneyDiffuse);
			LTCAreaLight.instance.Cleanup();
		}

		public override void RenderInit(CommandBuffer cmd)
		{
			PreIntegratedFGD.instance.RenderInit(PreIntegratedFGD.FGDIndex.FGD_GGXAndDisneyDiffuse, cmd);
		}

		public override void Bind(CommandBuffer cmd)
		{
			PreIntegratedFGD.instance.Bind(cmd, PreIntegratedFGD.FGDIndex.FGD_GGXAndDisneyDiffuse);
			LTCAreaLight.instance.Bind(cmd);
		}
	}
}
