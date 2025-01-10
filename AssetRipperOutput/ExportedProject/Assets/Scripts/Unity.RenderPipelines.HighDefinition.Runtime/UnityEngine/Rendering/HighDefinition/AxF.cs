using System;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering.HighDefinition.Attributes;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class AxF : RenderPipelineMaterial
	{
		[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\AxF\\AxF.cs")]
		public enum FeatureFlags
		{
			AxfAnisotropy = 1,
			AxfClearCoat = 2,
			AxfClearCoatRefraction = 4,
			AxfUseHeightMap = 8,
			AxfBRDFColorDiagonalClamp = 0x10,
			AxfHonorMinRoughness = 0x100,
			AxfHonorMinRoughnessCoat = 0x200,
			AxfDebugTest = 0x800000
		}

		[GenerateHLSL(PackingRules.Exact, false, false, true, 1200, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\AxF\\AxF.cs")]
		public struct SurfaceData
		{
			[MaterialSharedPropertyMapping(MaterialSharedProperty.Smoothness)]
			[SurfaceDataAttributes("Smoothness", false, false, FieldPrecision.Default, false, "", precision = FieldPrecision.Real)]
			public float perceptualSmoothness;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.AmbientOcclusion)]
			[SurfaceDataAttributes("Ambient Occlusion", false, false, FieldPrecision.Default, false, "")]
			public float ambientOcclusion;

			[SurfaceDataAttributes("Specular Occlusion", false, false, FieldPrecision.Default, false, "")]
			public float specularOcclusion;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.Normal)]
			[SurfaceDataAttributes(new string[] { "Normal", "Normal View Space" }, true, false, FieldPrecision.Default, false, "", checkIsNormalized = true)]
			public Vector3 normalWS;

			[SurfaceDataAttributes("Tangent", true, false, FieldPrecision.Default, false, "")]
			public Vector3 tangentWS;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.Albedo)]
			[SurfaceDataAttributes("Diffuse Color", false, true, FieldPrecision.Default, false, "")]
			public Vector3 diffuseColor;

			[MaterialSharedPropertyMapping(MaterialSharedProperty.Specular)]
			[SurfaceDataAttributes("Specular Color", false, true, FieldPrecision.Default, false, "")]
			public Vector3 specularColor;

			[SurfaceDataAttributes("Fresnel F0", false, false, FieldPrecision.Default, false, "")]
			public Vector3 fresnel0;

			[SurfaceDataAttributes("Specular Lobe", false, false, FieldPrecision.Default, false, "")]
			public Vector3 specularLobe;

			[SurfaceDataAttributes("Height", false, false, FieldPrecision.Default, false, "")]
			public float height_mm;

			[SurfaceDataAttributes("Anisotropic Angle", false, false, FieldPrecision.Default, false, "")]
			public float anisotropyAngle;

			[SurfaceDataAttributes("Flakes UV (or PlanarZY)", false, false, FieldPrecision.Default, false, "")]
			public Vector2 flakesUVZY;

			[SurfaceDataAttributes("Flakes PlanarXZ", false, false, FieldPrecision.Default, false, "")]
			public Vector2 flakesUVXZ;

			[SurfaceDataAttributes("Flakes PlanarXY", false, false, FieldPrecision.Default, false, "")]
			public Vector2 flakesUVXY;

			[SurfaceDataAttributes("Flakes Mip (and for PlanarZY)", false, false, FieldPrecision.Default, false, "")]
			public float flakesMipLevelZY;

			[SurfaceDataAttributes("Flakes Mip for PlanarXZ", false, false, FieldPrecision.Default, false, "")]
			public float flakesMipLevelXZ;

			[SurfaceDataAttributes("Flakes Mip for PlanarXY", false, false, FieldPrecision.Default, false, "")]
			public float flakesMipLevelXY;

			[SurfaceDataAttributes("Flakes Triplanar Weights", false, false, FieldPrecision.Default, false, "")]
			public Vector3 flakesTriplanarWeights;

			[SurfaceDataAttributes("Flakes ddx (and for PlanarZY)", false, false, FieldPrecision.Default, false, "")]
			public Vector2 flakesDdxZY;

			[SurfaceDataAttributes("Flakes ddy (and for PlanarZY)", false, false, FieldPrecision.Default, false, "")]
			public Vector2 flakesDdyZY;

			[SurfaceDataAttributes("Flakes ddx for PlanarXZ", false, false, FieldPrecision.Default, false, "")]
			public Vector2 flakesDdxXZ;

			[SurfaceDataAttributes("Flakes ddy for PlanarXZ", false, false, FieldPrecision.Default, false, "")]
			public Vector2 flakesDdyXZ;

			[SurfaceDataAttributes("Flakes ddx for PlanarXY", false, false, FieldPrecision.Default, false, "")]
			public Vector2 flakesDdxXY;

			[SurfaceDataAttributes("Flakes ddy for PlanarXY", false, false, FieldPrecision.Default, false, "")]
			public Vector2 flakesDdyXY;

			[SurfaceDataAttributes("Clearcoat Color", false, false, FieldPrecision.Default, false, "")]
			public Vector3 clearcoatColor;

			[SurfaceDataAttributes("Clearcoat Normal", true, false, FieldPrecision.Default, false, "")]
			public Vector3 clearcoatNormalWS;

			[SurfaceDataAttributes("Clearcoat IOR", false, false, FieldPrecision.Default, false, "")]
			public float clearcoatIOR;

			[SurfaceDataAttributes(new string[] { "Geometric Normal", "Geometric Normal View Space" }, true, false, FieldPrecision.Default, false, "", checkIsNormalized = true)]
			public Vector3 geomNormalWS;

			[SurfaceDataAttributes("View Direction", true, false, FieldPrecision.Default, false, "")]
			public Vector3 viewWS;
		}

		[GenerateHLSL(PackingRules.Exact, false, false, true, 1250, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\AxF\\AxF.cs")]
		public struct BSDFData
		{
			public float ambientOcclusion;

			public float specularOcclusion;

			[SurfaceDataAttributes(new string[] { "Normal WS", "Normal View Space" }, true, false, FieldPrecision.Default, false, "", checkIsNormalized = true)]
			public Vector3 normalWS;

			[SurfaceDataAttributes("", true, false, FieldPrecision.Default, false, "")]
			public Vector3 tangentWS;

			[SurfaceDataAttributes("", true, false, FieldPrecision.Default, false, "")]
			public Vector3 bitangentWS;

			public Vector3 diffuseColor;

			public Vector3 specularColor;

			public Vector3 fresnel0;

			public float perceptualRoughness;

			public Vector3 roughness;

			public float height_mm;

			public Vector2 flakesUVZY;

			public Vector2 flakesUVXZ;

			public Vector2 flakesUVXY;

			public float flakesMipLevelZY;

			public float flakesMipLevelXZ;

			public float flakesMipLevelXY;

			public Vector3 flakesTriplanarWeights;

			public Vector2 flakesDdxZY;

			public Vector2 flakesDdyZY;

			public Vector2 flakesDdxXZ;

			public Vector2 flakesDdyXZ;

			public Vector2 flakesDdxXY;

			public Vector2 flakesDdyXY;

			public Vector3 clearcoatColor;

			[SurfaceDataAttributes("", true, false, FieldPrecision.Default, false, "")]
			public Vector3 clearcoatNormalWS;

			public float clearcoatIOR;

			[SurfaceDataAttributes(new string[] { "Geometric Normal", "Geometric Normal View Space" }, true, false, FieldPrecision.Default, false, "", checkIsNormalized = true)]
			public Vector3 geomNormalWS;

			[SurfaceDataAttributes("View Direction", true, false, FieldPrecision.Default, false, "")]
			public Vector3 viewWS;
		}

		private Texture2DArray m_LtcData;

		private Material m_preIntegratedFGDMaterial_Ward;

		private Material m_preIntegratedFGDMaterial_CookTorrance;

		private RenderTexture m_preIntegratedFGD_Ward;

		private RenderTexture m_preIntegratedFGD_CookTorrance;

		private bool m_precomputedFGDTablesAreInit;

		public static readonly int _PreIntegratedFGD_Ward = Shader.PropertyToID("_PreIntegratedFGD_Ward");

		public static readonly int _PreIntegratedFGD_CookTorrance = Shader.PropertyToID("_PreIntegratedFGD_CookTorrance");

		public static readonly int _AxFLtcData = Shader.PropertyToID("_AxFLtcData");

		public override void Build(HDRenderPipelineAsset hdAsset, HDRenderPipelineRuntimeResources defaultResources)
		{
			m_preIntegratedFGDMaterial_Ward = CoreUtils.CreateEngineMaterial(defaultResources.shaders.preIntegratedFGD_WardPS);
			if (m_preIntegratedFGDMaterial_Ward == null)
			{
				throw new Exception("Failed to create material for Ward BRDF pre-integration!");
			}
			m_preIntegratedFGDMaterial_CookTorrance = CoreUtils.CreateEngineMaterial(defaultResources.shaders.preIntegratedFGD_CookTorrancePS);
			if (m_preIntegratedFGDMaterial_CookTorrance == null)
			{
				throw new Exception("Failed to create material for Cook-Torrance BRDF pre-integration!");
			}
			m_preIntegratedFGD_Ward = new RenderTexture(128, 128, 0, GraphicsFormat.A2B10G10R10_UNormPack32);
			m_preIntegratedFGD_Ward.hideFlags = HideFlags.HideAndDontSave;
			m_preIntegratedFGD_Ward.filterMode = FilterMode.Bilinear;
			m_preIntegratedFGD_Ward.wrapMode = TextureWrapMode.Clamp;
			m_preIntegratedFGD_Ward.hideFlags = HideFlags.DontSave;
			m_preIntegratedFGD_Ward.name = CoreUtils.GetRenderTargetAutoName(128, 128, 1, GraphicsFormat.A2B10G10R10_UNormPack32, "PreIntegratedFGD_Ward");
			m_preIntegratedFGD_Ward.Create();
			m_preIntegratedFGD_CookTorrance = new RenderTexture(128, 128, 0, GraphicsFormat.A2B10G10R10_UNormPack32);
			m_preIntegratedFGD_CookTorrance.hideFlags = HideFlags.HideAndDontSave;
			m_preIntegratedFGD_CookTorrance.filterMode = FilterMode.Bilinear;
			m_preIntegratedFGD_CookTorrance.wrapMode = TextureWrapMode.Clamp;
			m_preIntegratedFGD_CookTorrance.hideFlags = HideFlags.DontSave;
			m_preIntegratedFGD_CookTorrance.name = CoreUtils.GetRenderTargetAutoName(128, 128, 1, GraphicsFormat.A2B10G10R10_UNormPack32, "PreIntegratedFGD_CookTorrance");
			m_preIntegratedFGD_CookTorrance.Create();
			m_LtcData = new Texture2DArray(64, 64, 3, GraphicsFormat.R16G16B16A16_SFloat, TextureCreationFlags.None)
			{
				hideFlags = HideFlags.HideAndDontSave,
				wrapMode = TextureWrapMode.Clamp,
				filterMode = FilterMode.Bilinear,
				name = CoreUtils.GetTextureAutoName(64, 64, GraphicsFormat.R16G16B16A16_SFloat, TextureDimension.Tex2DArray, "LTC_LUT", mips: false, 2)
			};
			LTCAreaLight.LoadLUT(m_LtcData, 0, GraphicsFormat.R16G16B16A16_SFloat, LTCAreaLight.s_LtcMatrixData_GGX);
			m_LtcData.Apply();
			LTCAreaLight.instance.Build();
		}

		public override void Cleanup()
		{
			CoreUtils.Destroy(m_preIntegratedFGD_CookTorrance);
			CoreUtils.Destroy(m_preIntegratedFGD_Ward);
			CoreUtils.Destroy(m_preIntegratedFGDMaterial_CookTorrance);
			CoreUtils.Destroy(m_preIntegratedFGDMaterial_Ward);
			m_preIntegratedFGD_CookTorrance = null;
			m_preIntegratedFGD_Ward = null;
			m_preIntegratedFGDMaterial_Ward = null;
			m_preIntegratedFGDMaterial_CookTorrance = null;
			m_precomputedFGDTablesAreInit = false;
			CoreUtils.Destroy(m_LtcData);
			LTCAreaLight.instance.Cleanup();
		}

		public override void RenderInit(CommandBuffer cmd)
		{
			if (m_precomputedFGDTablesAreInit || m_preIntegratedFGDMaterial_Ward == null || m_preIntegratedFGDMaterial_CookTorrance == null)
			{
				return;
			}
			if (GL.wireframe)
			{
				m_preIntegratedFGD_Ward.Create();
				m_preIntegratedFGD_CookTorrance.Create();
				return;
			}
			using (new ProfilingScope(cmd, ProfilingSampler.Get(HDProfileId.PreIntegradeWardCookTorrance)))
			{
				CoreUtils.DrawFullScreen(cmd, m_preIntegratedFGDMaterial_Ward, new RenderTargetIdentifier(m_preIntegratedFGD_Ward));
				CoreUtils.DrawFullScreen(cmd, m_preIntegratedFGDMaterial_CookTorrance, new RenderTargetIdentifier(m_preIntegratedFGD_CookTorrance));
			}
			m_precomputedFGDTablesAreInit = true;
		}

		public override void Bind(CommandBuffer cmd)
		{
			if (m_preIntegratedFGD_Ward == null || m_preIntegratedFGD_CookTorrance == null)
			{
				throw new Exception("Ward & Cook-Torrance BRDF pre-integration table not available!");
			}
			cmd.SetGlobalTexture(_PreIntegratedFGD_Ward, m_preIntegratedFGD_Ward);
			cmd.SetGlobalTexture(_PreIntegratedFGD_CookTorrance, m_preIntegratedFGD_CookTorrance);
			cmd.SetGlobalTexture(_AxFLtcData, m_LtcData);
			LTCAreaLight.instance.Bind(cmd);
		}
	}
}
