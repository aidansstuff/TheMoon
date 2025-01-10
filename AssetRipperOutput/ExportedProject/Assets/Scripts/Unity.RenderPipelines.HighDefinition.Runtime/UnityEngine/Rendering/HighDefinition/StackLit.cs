using UnityEngine.Rendering.HighDefinition.Attributes;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class StackLit : RenderPipelineMaterial
	{
		[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\StackLit\\StackLit.cs")]
		public enum MaterialFeatureFlags
		{
			StackLitStandard = 1,
			StackLitDualSpecularLobe = 2,
			StackLitAnisotropy = 4,
			StackLitCoat = 8,
			StackLitIridescence = 0x10,
			StackLitSubsurfaceScattering = 0x20,
			StackLitTransmission = 0x40,
			StackLitCoatNormalMap = 0x80,
			StackLitSpecularColor = 0x100,
			StackLitHazyGloss = 0x200
		}

		public enum BaseParametrization
		{
			BaseMetallic = 0,
			SpecularColor = 1
		}

		public enum DualSpecularLobeParametrization
		{
			Direct = 0,
			HazyGloss = 1
		}

		[GenerateHLSL(PackingRules.Exact, false, false, true, 1100, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\StackLit\\StackLit.cs")]
		public struct SurfaceData
		{
			[SurfaceDataAttributes("Material Features", false, false, FieldPrecision.Default, false, "")]
			public uint materialFeatures;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.Albedo)]
			[SurfaceDataAttributes("Base Color", false, true, FieldPrecision.Default, false, "")]
			public Vector3 baseColor;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.AmbientOcclusion)]
			[SurfaceDataAttributes("Ambient Occlusion", false, false, FieldPrecision.Default, false, "")]
			public float ambientOcclusion;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.Metal)]
			[SurfaceDataAttributes("Metallic", false, false, FieldPrecision.Default, false, "")]
			public float metallic;

			[SurfaceDataAttributes("Dielectric IOR", false, false, FieldPrecision.Default, false, "")]
			public float dielectricIor;

			[SurfaceDataAttributes("Use Profile IOR", false, false, FieldPrecision.Default, false, "")]
			public bool useProfileIor;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.Specular)]
			[SurfaceDataAttributes("Specular Color", false, true, FieldPrecision.Default, false, "")]
			public Vector3 specularColor;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.Normal)]
			[SurfaceDataAttributes(new string[] { "Normal", "Normal View Space" }, true, false, FieldPrecision.Default, false, "", checkIsNormalized = true)]
			public Vector3 normalWS;

			[SurfaceDataAttributes(new string[] { "Geometric Normal", "Geometric Normal View Space" }, true, false, FieldPrecision.Default, false, "", checkIsNormalized = true)]
			public Vector3 geomNormalWS;

			[SurfaceDataAttributes(new string[] { "Coat Normal", "Coat Normal View Space" }, true, false, FieldPrecision.Default, false, "", checkIsNormalized = true)]
			public Vector3 coatNormalWS;

			[SurfaceDataAttributes(new string[] { "Bent Normal", "Bent Normal View Space" }, true, false, FieldPrecision.Default, false, "", checkIsNormalized = true)]
			public Vector3 bentNormalWS;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.Smoothness)]
			[SurfaceDataAttributes("Smoothness A", false, false, FieldPrecision.Default, false, "")]
			public float perceptualSmoothnessA;

			[SurfaceDataAttributes("Smoothness B", false, false, FieldPrecision.Default, false, "")]
			public float perceptualSmoothnessB;

			[SurfaceDataAttributes("Lobe Mixing", false, false, FieldPrecision.Default, false, "")]
			public float lobeMix;

			[SurfaceDataAttributes("Haziness", false, false, FieldPrecision.Default, false, "")]
			public float haziness;

			[SurfaceDataAttributes("Haze Extent", false, false, FieldPrecision.Default, false, "")]
			public float hazeExtent;

			[SurfaceDataAttributes("Hazy Gloss Max Dielectric f0 When Using Metallic Input", false, false, FieldPrecision.Default, false, "")]
			public float hazyGlossMaxDielectricF0;

			[SurfaceDataAttributes("Tangent", true, false, FieldPrecision.Default, false, "")]
			public Vector3 tangentWS;

			[SurfaceDataAttributes("AnisotropyA", false, false, FieldPrecision.Default, false, "")]
			public float anisotropyA;

			[SurfaceDataAttributes("AnisotropyB", false, false, FieldPrecision.Default, false, "")]
			public float anisotropyB;

			[SurfaceDataAttributes("Iridescence Ior", false, false, FieldPrecision.Default, false, "")]
			public float iridescenceIor;

			[SurfaceDataAttributes("Iridescence Layer Thickness", false, false, FieldPrecision.Default, false, "")]
			public float iridescenceThickness;

			[SurfaceDataAttributes("Iridescence Mask", false, false, FieldPrecision.Default, false, "")]
			public float iridescenceMask;

			[SurfaceDataAttributes("Iridescence Coat Fixup TIR", false, false, FieldPrecision.Default, false, "")]
			public float iridescenceCoatFixupTIR;

			[SurfaceDataAttributes("Iridescence Coat Fixup TIR Clamp", false, false, FieldPrecision.Default, false, "")]
			public float iridescenceCoatFixupTIRClamp;

			[SurfaceDataAttributes("Coat Smoothness", false, false, FieldPrecision.Default, false, "")]
			public float coatPerceptualSmoothness;

			[SurfaceDataAttributes("Coat mask", false, false, FieldPrecision.Default, false, "")]
			public float coatMask;

			[SurfaceDataAttributes("Coat IOR", false, false, FieldPrecision.Default, false, "")]
			public float coatIor;

			[SurfaceDataAttributes("Coat Thickness", false, false, FieldPrecision.Default, false, "")]
			public float coatThickness;

			[SurfaceDataAttributes("Coat Extinction Coefficient", false, false, FieldPrecision.Default, false, "")]
			public Vector3 coatExtinction;

			[SurfaceDataAttributes("Diffusion Profile Hash", false, false, FieldPrecision.Default, false, "")]
			public uint diffusionProfileHash;

			[SurfaceDataAttributes("Subsurface Mask", false, false, FieldPrecision.Default, false, "")]
			public float subsurfaceMask;

			[SurfaceDataAttributes("Transmission Mask", false, false, FieldPrecision.Default, false, "")]
			public float transmissionMask;

			[SurfaceDataAttributes("Thickness", false, false, FieldPrecision.Default, false, "")]
			public float thickness;

			[SurfaceDataAttributes("Specular Occlusion From Custom Input", false, false, FieldPrecision.Default, false, "")]
			public float specularOcclusionCustomInput;

			[SurfaceDataAttributes("Specular Occlusion Fixup Visibility Ratio Threshold", false, false, FieldPrecision.Default, false, "")]
			public float soFixupVisibilityRatioThreshold;

			[SurfaceDataAttributes("Specular Occlusion Fixup Strength", false, false, FieldPrecision.Default, false, "")]
			public float soFixupStrengthFactor;

			[SurfaceDataAttributes("Specular Occlusion Fixup Max Added Roughness", false, false, FieldPrecision.Default, false, "")]
			public float soFixupMaxAddedRoughness;
		}

		[GenerateHLSL(PackingRules.Exact, false, false, true, 1150, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\StackLit\\StackLit.cs")]
		public struct BSDFData
		{
			public uint materialFeatures;

			[SurfaceDataAttributes("", false, true, FieldPrecision.Default, false, "")]
			public Vector3 diffuseColor;

			public Vector3 fresnel0;

			public float ambientOcclusion;

			[SurfaceDataAttributes(new string[] { "Normal WS", "Normal View Space" }, true, false, FieldPrecision.Default, false, "", checkIsNormalized = true)]
			public Vector3 normalWS;

			[SurfaceDataAttributes(new string[] { "Geometric Normal", "Geometric Normal View Space" }, true, false, FieldPrecision.Default, false, "", checkIsNormalized = true)]
			public Vector3 geomNormalWS;

			[SurfaceDataAttributes(new string[] { "Coat Normal", "Coat Normal View Space" }, true, false, FieldPrecision.Default, false, "", checkIsNormalized = true)]
			public Vector3 coatNormalWS;

			[SurfaceDataAttributes(new string[] { "Bent Normal", "Bent Normal View Space" }, true, false, FieldPrecision.Default, false, "", checkIsNormalized = true)]
			public Vector3 bentNormalWS;

			public float perceptualRoughnessA;

			public float perceptualRoughnessB;

			public float lobeMix;

			[SurfaceDataAttributes("", true, false, FieldPrecision.Default, false, "")]
			public Vector3 tangentWS;

			[SurfaceDataAttributes("", true, false, FieldPrecision.Default, false, "")]
			public Vector3 bitangentWS;

			public float roughnessAT;

			public float roughnessAB;

			public float roughnessBT;

			public float roughnessBB;

			public float anisotropyA;

			public float anisotropyB;

			public float coatRoughness;

			public float coatPerceptualRoughness;

			public float coatMask;

			public float coatIor;

			public float coatThickness;

			public Vector3 coatExtinction;

			public float iridescenceIor;

			public float iridescenceThickness;

			public float iridescenceMask;

			public float iridescenceCoatFixupTIR;

			public float iridescenceCoatFixupTIRClamp;

			public uint diffusionProfileIndex;

			public float subsurfaceMask;

			public float thickness;

			public bool useThickObjectMode;

			public Vector3 transmittance;

			public float specularOcclusionCustomInput;

			public float soFixupVisibilityRatioThreshold;

			public float soFixupStrengthFactor;

			public float soFixupMaxAddedRoughness;
		}

		public override void Build(HDRenderPipelineAsset hdAsset, HDRenderPipelineRuntimeResources defaultResources)
		{
			PreIntegratedFGD.instance.Build(PreIntegratedFGD.FGDIndex.FGD_GGXAndDisneyDiffuse);
			LTCAreaLight.instance.Build();
			SPTDistribution.instance.Build();
		}

		public override void Cleanup()
		{
			PreIntegratedFGD.instance.Cleanup(PreIntegratedFGD.FGDIndex.FGD_GGXAndDisneyDiffuse);
			LTCAreaLight.instance.Cleanup();
			SPTDistribution.instance.Cleanup();
		}

		public override void RenderInit(CommandBuffer cmd)
		{
			PreIntegratedFGD.instance.RenderInit(PreIntegratedFGD.FGDIndex.FGD_GGXAndDisneyDiffuse, cmd);
		}

		public override void Bind(CommandBuffer cmd)
		{
			PreIntegratedFGD.instance.Bind(cmd, PreIntegratedFGD.FGDIndex.FGD_GGXAndDisneyDiffuse);
			LTCAreaLight.instance.Bind(cmd);
			SPTDistribution.instance.Bind(cmd);
		}
	}
}
