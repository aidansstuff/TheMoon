using UnityEngine.Rendering.HighDefinition.Attributes;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class Fabric : RenderPipelineMaterial
	{
		[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\Fabric\\Fabric.cs")]
		public enum MaterialFeatureFlags
		{
			FabricCottonWool = 1,
			FabricSubsurfaceScattering = 2,
			FabricTransmission = 4
		}

		[GenerateHLSL(PackingRules.Exact, false, false, true, 1300, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\Fabric\\Fabric.cs")]
		public struct SurfaceData
		{
			[SurfaceDataAttributes("Material Features", false, false, FieldPrecision.Default, false, "")]
			public uint materialFeatures;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.Albedo)]
			[SurfaceDataAttributes("Base Color", false, true, FieldPrecision.Default, false, "")]
			public Vector3 baseColor;

			[SurfaceDataAttributes("Specular Occlusion", false, false, FieldPrecision.Default, false, "")]
			public float specularOcclusion;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.Normal)]
			[SurfaceDataAttributes(new string[] { "Normal", "Normal View Space" }, true, false, FieldPrecision.Default, false, "", checkIsNormalized = true)]
			public Vector3 normalWS;

			[SurfaceDataAttributes(new string[] { "Geometric Normal", "Geometric Normal View Space" }, true, false, FieldPrecision.Default, false, "", checkIsNormalized = true)]
			public Vector3 geomNormalWS;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.Smoothness)]
			[SurfaceDataAttributes("Smoothness", false, false, FieldPrecision.Default, false, "")]
			public float perceptualSmoothness;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.AmbientOcclusion)]
			[SurfaceDataAttributes("Ambient Occlusion", false, false, FieldPrecision.Default, false, "")]
			public float ambientOcclusion;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.Specular)]
			[SurfaceDataAttributes("Specular Tint", false, true, FieldPrecision.Default, false, "")]
			public Vector3 specularColor;

			[SurfaceDataAttributes("Diffusion Profile Hash", false, false, FieldPrecision.Default, false, "")]
			public uint diffusionProfileHash;

			[SurfaceDataAttributes("Subsurface Mask", false, false, FieldPrecision.Default, false, "")]
			public float subsurfaceMask;

			[SurfaceDataAttributes("Transmission Mask", false, false, FieldPrecision.Default, false, "")]
			public float transmissionMask;

			[SurfaceDataAttributes("Thickness", false, false, FieldPrecision.Default, false, "")]
			public float thickness;

			[SurfaceDataAttributes("Tangent", true, false, FieldPrecision.Default, false, "")]
			public Vector3 tangentWS;

			[SurfaceDataAttributes("Anisotropy", false, false, FieldPrecision.Default, false, "")]
			public float anisotropy;
		}

		[GenerateHLSL(PackingRules.Exact, false, false, true, 1350, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\Fabric\\Fabric.cs")]
		public struct BSDFData
		{
			public uint materialFeatures;

			[SurfaceDataAttributes("", false, true, FieldPrecision.Default, false, "")]
			public Vector3 diffuseColor;

			public Vector3 fresnel0;

			public float ambientOcclusion;

			public float specularOcclusion;

			[SurfaceDataAttributes(new string[] { "Normal WS", "Normal View Space" }, true, false, FieldPrecision.Default, false, "", checkIsNormalized = true)]
			public Vector3 normalWS;

			[SurfaceDataAttributes(new string[] { "Geometric Normal", "Geometric Normal View Space" }, true, false, FieldPrecision.Default, false, "", checkIsNormalized = true)]
			public Vector3 geomNormalWS;

			public float perceptualRoughness;

			public uint diffusionProfileIndex;

			public float subsurfaceMask;

			public float thickness;

			public bool useThickObjectMode;

			public Vector3 transmittance;

			[SurfaceDataAttributes("", true, false, FieldPrecision.Default, false, "")]
			public Vector3 tangentWS;

			[SurfaceDataAttributes("", true, false, FieldPrecision.Default, false, "")]
			public Vector3 bitangentWS;

			public float roughnessT;

			public float roughnessB;

			public float anisotropy;
		}

		public override void Build(HDRenderPipelineAsset hdAsset, HDRenderPipelineRuntimeResources defaultResources)
		{
			PreIntegratedFGD.instance.Build(PreIntegratedFGD.FGDIndex.FGD_CharlieAndFabricLambert);
		}

		public override void Cleanup()
		{
			PreIntegratedFGD.instance.Cleanup(PreIntegratedFGD.FGDIndex.FGD_CharlieAndFabricLambert);
		}

		public override void RenderInit(CommandBuffer cmd)
		{
			PreIntegratedFGD.instance.RenderInit(PreIntegratedFGD.FGDIndex.FGD_CharlieAndFabricLambert, cmd);
		}

		public override void Bind(CommandBuffer cmd)
		{
			PreIntegratedFGD.instance.Bind(cmd, PreIntegratedFGD.FGDIndex.FGD_CharlieAndFabricLambert);
		}
	}
}
