using UnityEngine.Rendering.HighDefinition.Attributes;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class Eye : RenderPipelineMaterial
	{
		[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\Eye\\Eye.cs")]
		public enum MaterialFeatureFlags
		{
			EyeCinematic = 1,
			EyeSubsurfaceScattering = 2,
			EyeCausticFromLUT = 4
		}

		[GenerateHLSL(PackingRules.Exact, false, false, true, 1500, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\Eye\\Eye.cs")]
		public struct SurfaceData
		{
			[SurfaceDataAttributes("Material Features", false, false, FieldPrecision.Default, false, "")]
			public uint materialFeatures;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.Albedo)]
			[SurfaceDataAttributes("Base Color", false, true, FieldPrecision.Default, false, "")]
			public Vector3 baseColor;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.Normal)]
			[SurfaceDataAttributes(new string[] { "Normal", "Normal View Space" }, true, false, FieldPrecision.Default, false, "", checkIsNormalized = true)]
			public Vector3 normalWS;

			[SurfaceDataAttributes(new string[] { "Iris Normal", "Iris Normal View Space" }, true, false, FieldPrecision.Default, false, "", checkIsNormalized = true)]
			public Vector3 irisNormalWS;

			[SurfaceDataAttributes(new string[] { "Geometric Normal", "Geometric Normal View Space" }, true, false, FieldPrecision.Default, false, "", checkIsNormalized = true)]
			public Vector3 geomNormalWS;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.Smoothness)]
			[SurfaceDataAttributes("Smoothness", false, false, FieldPrecision.Default, false, "")]
			public float perceptualSmoothness;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.AmbientOcclusion)]
			[SurfaceDataAttributes("Ambient Occlusion", false, false, FieldPrecision.Default, false, "")]
			public float ambientOcclusion;

			[SurfaceDataAttributes("Specular Occlusion", false, false, FieldPrecision.Default, false, "")]
			public float specularOcclusion;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.Specular)]
			[SurfaceDataAttributes("IOR", false, true, FieldPrecision.Default, false, "")]
			public float IOR;

			[SurfaceDataAttributes("Mask", false, true, FieldPrecision.Default, false, "")]
			public Vector2 mask;

			[SurfaceDataAttributes("Diffusion Profile Hash", false, false, FieldPrecision.Default, false, "")]
			public uint diffusionProfileHash;

			[SurfaceDataAttributes("Subsurface Mask", false, false, FieldPrecision.Default, false, "")]
			public float subsurfaceMask;

			[SurfaceDataAttributes("Iris Plane Offset", false, false, FieldPrecision.Default, false, "")]
			public float irisPlaneOffset;

			[SurfaceDataAttributes("Iris Radius", false, false, FieldPrecision.Default, false, "")]
			public float irisRadius;

			[SurfaceDataAttributes("Caustic intensity multiplier", false, false, FieldPrecision.Default, false, "")]
			public float causticIntensity;

			[SurfaceDataAttributes("Blending factor between caustic and cinematic diffuse", false, false, FieldPrecision.Default, false, "")]
			public float causticBlend;
		}

		[GenerateHLSL(PackingRules.Exact, false, false, true, 1550, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\Eye\\Eye.cs")]
		public struct BSDFData
		{
			public uint materialFeatures;

			[SurfaceDataAttributes("", false, true, FieldPrecision.Default, false, "")]
			public Vector3 diffuseColor;

			public Vector3 fresnel0;

			public float IOR;

			public float ambientOcclusion;

			public float specularOcclusion;

			[SurfaceDataAttributes(new string[] { "Normal WS", "Normal View Space" }, true, false, FieldPrecision.Default, false, "", checkIsNormalized = true)]
			public Vector3 normalWS;

			[SurfaceDataAttributes(new string[] { "Diffuse Normal WS", "Diffuse Normal View Space" }, true, false, FieldPrecision.Default, false, "", checkIsNormalized = true)]
			public Vector3 diffuseNormalWS;

			[SurfaceDataAttributes(new string[] { "Geometric Normal", "Geometric Normal View Space" }, true, false, FieldPrecision.Default, false, "", checkIsNormalized = true)]
			public Vector3 geomNormalWS;

			public float perceptualRoughness;

			public Vector2 mask;

			public float irisPlaneOffset;

			public float irisRadius;

			public float causticIntensity;

			public float causticBlend;

			public uint diffusionProfileIndex;

			public float subsurfaceMask;

			public float roughness;
		}

		private Texture3D m_EyeCausticLUT;

		public static readonly int _PreIntegratedEyeCaustic = Shader.PropertyToID("_PreIntegratedEyeCaustic");

		public override void Build(HDRenderPipelineAsset hdAsset, HDRenderPipelineRuntimeResources defaultResources)
		{
			m_EyeCausticLUT = defaultResources.textures.eyeCausticLUT;
		}

		public override void Cleanup()
		{
			m_EyeCausticLUT = null;
		}

		public override void RenderInit(CommandBuffer cmd)
		{
		}

		public override void Bind(CommandBuffer cmd)
		{
			cmd.SetGlobalTexture(_PreIntegratedEyeCaustic, m_EyeCausticLUT);
		}
	}
}
