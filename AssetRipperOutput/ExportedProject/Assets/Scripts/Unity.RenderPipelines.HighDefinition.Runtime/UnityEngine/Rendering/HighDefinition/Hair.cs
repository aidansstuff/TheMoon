using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.HighDefinition.Attributes;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class Hair : RenderPipelineMaterial
	{
		[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\Hair\\Hair.cs")]
		public enum MaterialFeatureFlags
		{
			HairKajiyaKay = 1,
			HairMarschner = 2
		}

		[GenerateHLSL(PackingRules.Exact, false, false, true, 1400, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\Hair\\Hair.cs")]
		public struct SurfaceData
		{
			[SurfaceDataAttributes("Material Features", false, false, FieldPrecision.Default, false, "")]
			public uint materialFeatures;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.AmbientOcclusion)]
			[SurfaceDataAttributes("Ambient Occlusion", false, false, FieldPrecision.Default, false, "")]
			public float ambientOcclusion;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.Albedo)]
			[SurfaceDataAttributes("Diffuse", false, true, FieldPrecision.Default, false, "")]
			public Vector3 diffuseColor;

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

			[SurfaceDataAttributes("Transmittance", false, false, FieldPrecision.Default, false, "")]
			public Vector3 transmittance;

			[SurfaceDataAttributes("Rim Transmission Intensity", false, false, FieldPrecision.Default, false, "")]
			public float rimTransmissionIntensity;

			[SurfaceDataAttributes("Hair Strand Direction", true, false, FieldPrecision.Default, false, "")]
			public Vector3 hairStrandDirectionWS;

			[SurfaceDataAttributes("Secondary Smoothness", false, false, FieldPrecision.Default, false, "")]
			public float secondaryPerceptualSmoothness;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.Specular)]
			[SurfaceDataAttributes("Specular Tint", false, true, FieldPrecision.Default, false, "")]
			public Vector3 specularTint;

			[SurfaceDataAttributes("Secondary Specular Tint", false, true, FieldPrecision.Default, false, "")]
			public Vector3 secondarySpecularTint;

			[SurfaceDataAttributes("Specular Shift", false, false, FieldPrecision.Default, false, "")]
			public float specularShift;

			[SurfaceDataAttributes("Secondary Specular Shift", false, false, FieldPrecision.Default, false, "")]
			public float secondarySpecularShift;

			[SurfaceDataAttributes("Absorption Coefficient", false, false, FieldPrecision.Default, false, "")]
			public Vector3 absorption;

			[SurfaceDataAttributes("Eumelanin", false, false, FieldPrecision.Default, false, "")]
			public float eumelanin;

			[SurfaceDataAttributes("Pheomelanin", false, false, FieldPrecision.Default, false, "")]
			public float pheomelanin;

			[SurfaceDataAttributes("Azimuthal Roughness", false, false, FieldPrecision.Default, false, "")]
			public float perceptualRadialSmoothness;

			[SurfaceDataAttributes("Cuticle Angle", false, false, FieldPrecision.Default, false, "")]
			public float cuticleAngle;

			[SurfaceDataAttributes("Strand Count Probe", false, false, FieldPrecision.Default, false, "")]
			public Vector4 strandCountProbe;

			[SurfaceDataAttributes("Strand Shadow Bias", false, false, FieldPrecision.Default, false, "")]
			public float strandShadowBias;
		}

		[GenerateHLSL(PackingRules.Exact, false, false, true, 1450, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\Hair\\Hair.cs")]
		public struct BSDFData
		{
			public uint materialFeatures;

			public float ambientOcclusion;

			public float specularOcclusion;

			[SurfaceDataAttributes("", false, true, FieldPrecision.Default, false, "")]
			public Vector3 diffuseColor;

			public Vector3 fresnel0;

			public Vector3 specularTint;

			[SurfaceDataAttributes(new string[] { "Normal WS", "Normal View Space" }, true, false, FieldPrecision.Default, false, "", checkIsNormalized = true)]
			public Vector3 normalWS;

			[SurfaceDataAttributes(new string[] { "Geometric Normal", "Geometric Normal View Space" }, true, false, FieldPrecision.Default, false, "", checkIsNormalized = true)]
			public Vector3 geomNormalWS;

			public float perceptualRoughness;

			public Vector3 transmittance;

			public float rimTransmissionIntensity;

			[SurfaceDataAttributes("", true, false, FieldPrecision.Default, false, "")]
			public Vector3 hairStrandDirectionWS;

			public float anisotropy;

			public Vector3 tangentWS;

			public Vector3 bitangentWS;

			public float roughnessT;

			public float roughnessB;

			public float h;

			public float secondaryPerceptualRoughness;

			public Vector3 secondarySpecularTint;

			public float specularExponent;

			public float secondarySpecularExponent;

			public float specularShift;

			public float secondarySpecularShift;

			public Vector3 absorption;

			public float lightPathLength;

			public float cuticleAngle;

			public float cuticleAngleR;

			public float cuticleAngleTT;

			public float cuticleAngleTRT;

			public float roughnessR;

			public float roughnessTT;

			public float roughnessTRT;

			public float perceptualRoughnessRadial;

			public Vector3 distributionNormalizationFactor;

			public Vector4 strandCountProbe;

			public float strandShadowBias;

			public float splineVisibility;
		}

		private const int m_Dim = 64;

		private ComputeShader m_PreIntegratedFiberScatteringCS;

		private RenderTexture m_PreIntegratedFiberScatteringLUT;

		private bool m_PreIntegratedFiberScatteringIsInit;

		private RenderTexture m_PreIntegratedFiberAverageScatteringLUT;

		private bool m_PreIntegratedFiberAverageScatteringIsInit;

		public static readonly int _PreIntegratedHairFiberScatteringUAV = Shader.PropertyToID("_PreIntegratedHairFiberScatteringUAV");

		public static readonly int _PreIntegratedHairFiberScattering = Shader.PropertyToID("_PreIntegratedHairFiberScattering");

		public static readonly int _PreIntegratedAverageHairFiberScatteringUAV = Shader.PropertyToID("_PreIntegratedAverageHairFiberScatteringUAV");

		public static readonly int _PreIntegratedAverageHairFiberScattering = Shader.PropertyToID("_PreIntegratedAverageHairFiberScattering");

		public override void Build(HDRenderPipelineAsset hdAsset, HDRenderPipelineRuntimeResources defaultResources)
		{
			PreIntegratedFGD.instance.Build(PreIntegratedFGD.FGDIndex.FGD_GGXAndDisneyDiffuse);
			LTCAreaLight.instance.Build();
			m_PreIntegratedFiberScatteringLUT = new RenderTexture(64, 64, 0, GraphicsFormat.R16G16_SFloat)
			{
				dimension = TextureDimension.Tex3D,
				volumeDepth = 64,
				enableRandomWrite = true,
				hideFlags = HideFlags.HideAndDontSave,
				filterMode = FilterMode.Point,
				wrapMode = TextureWrapMode.Clamp,
				name = CoreUtils.GetRenderTargetAutoName(64, 64, 0, GraphicsFormat.R16G16_SFloat, "PreIntegratedFiberScattering")
			};
			m_PreIntegratedFiberScatteringLUT.Create();
			m_PreIntegratedFiberAverageScatteringLUT = new RenderTexture(64, 64, 0, GraphicsFormat.R16G16B16A16_SFloat)
			{
				dimension = TextureDimension.Tex3D,
				volumeDepth = 64,
				enableRandomWrite = true,
				hideFlags = HideFlags.HideAndDontSave,
				filterMode = FilterMode.Point,
				wrapMode = TextureWrapMode.Clamp,
				name = CoreUtils.GetRenderTargetAutoName(64, 64, 0, GraphicsFormat.R16G16B16A16_SFloat, "PreIntegratedAverageFiberScattering")
			};
			m_PreIntegratedFiberAverageScatteringLUT.Create();
			m_PreIntegratedFiberScatteringCS = defaultResources.shaders.preIntegratedFiberScatteringCS;
		}

		public override void Cleanup()
		{
			PreIntegratedFGD.instance.Cleanup(PreIntegratedFGD.FGDIndex.FGD_GGXAndDisneyDiffuse);
			LTCAreaLight.instance.Cleanup();
			CoreUtils.Destroy(m_PreIntegratedFiberScatteringLUT);
			m_PreIntegratedFiberScatteringLUT = null;
			CoreUtils.Destroy(m_PreIntegratedFiberAverageScatteringLUT);
			m_PreIntegratedFiberAverageScatteringLUT = null;
		}

		public override void RenderInit(CommandBuffer cmd)
		{
			PreIntegratedFGD.instance.RenderInit(PreIntegratedFGD.FGDIndex.FGD_GGXAndDisneyDiffuse, cmd);
			if (!(m_PreIntegratedFiberScatteringCS == null))
			{
				if (!m_PreIntegratedFiberAverageScatteringIsInit)
				{
					cmd.SetComputeTextureParam(m_PreIntegratedFiberScatteringCS, 1, _PreIntegratedAverageHairFiberScatteringUAV, m_PreIntegratedFiberAverageScatteringLUT);
					cmd.DispatchCompute(m_PreIntegratedFiberScatteringCS, 1, HDUtils.DivRoundUp(64, 8), HDUtils.DivRoundUp(64, 8), HDUtils.DivRoundUp(64, 8));
					m_PreIntegratedFiberAverageScatteringIsInit = true;
				}
				cmd.SetGlobalTexture(_PreIntegratedAverageHairFiberScattering, m_PreIntegratedFiberAverageScatteringLUT);
				if (!m_PreIntegratedFiberScatteringIsInit)
				{
					cmd.SetComputeTextureParam(m_PreIntegratedFiberScatteringCS, 0, _PreIntegratedHairFiberScatteringUAV, m_PreIntegratedFiberScatteringLUT);
					cmd.DispatchCompute(m_PreIntegratedFiberScatteringCS, 0, HDUtils.DivRoundUp(64, 8), HDUtils.DivRoundUp(64, 8), HDUtils.DivRoundUp(64, 8));
					m_PreIntegratedFiberScatteringIsInit = true;
				}
			}
		}

		public override void Bind(CommandBuffer cmd)
		{
			PreIntegratedFGD.instance.Bind(cmd, PreIntegratedFGD.FGDIndex.FGD_GGXAndDisneyDiffuse);
			LTCAreaLight.instance.Bind(cmd);
			if (m_PreIntegratedFiberScatteringLUT == null)
			{
				throw new Exception("Pre-Integrated Hair Fiber LUT not available!");
			}
			cmd.SetGlobalTexture(_PreIntegratedHairFiberScattering, m_PreIntegratedFiberScatteringLUT);
			if (m_PreIntegratedFiberAverageScatteringLUT == null)
			{
				throw new Exception("Pre-Integrated Hair Fiber LUT not available!");
			}
			cmd.SetGlobalTexture(_PreIntegratedAverageHairFiberScattering, m_PreIntegratedFiberAverageScatteringLUT);
		}
	}
}
