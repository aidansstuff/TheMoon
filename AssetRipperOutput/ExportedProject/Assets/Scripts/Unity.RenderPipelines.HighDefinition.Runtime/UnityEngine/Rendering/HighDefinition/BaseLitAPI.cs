using UnityEditor.Rendering.HighDefinition;

namespace UnityEngine.Rendering.HighDefinition
{
	internal abstract class BaseLitAPI
	{
		protected const string kWindEnabled = "_EnableWind";

		public static DisplacementMode GetFilteredDisplacementMode(Material material)
		{
			return material.GetFilteredDisplacementMode((DisplacementMode)material.GetFloat("_DisplacementMode"));
		}

		public static void SetupBaseLitKeywords(Material material)
		{
			material.SetupBaseUnlitKeywords();
			if (material.HasProperty("_DoubleSidedEnable") && material.GetFloat("_DoubleSidedEnable") > 0f)
			{
				switch ((DoubleSidedNormalMode)(int)material.GetFloat("_DoubleSidedNormalMode"))
				{
				case DoubleSidedNormalMode.Mirror:
					material.SetVector("_DoubleSidedConstants", new Vector4(1f, 1f, -1f, 0f));
					break;
				case DoubleSidedNormalMode.Flip:
					material.SetVector("_DoubleSidedConstants", new Vector4(-1f, -1f, -1f, 0f));
					break;
				case DoubleSidedNormalMode.None:
					material.SetVector("_DoubleSidedConstants", new Vector4(1f, 1f, 1f, 0f));
					break;
				}
			}
			if (material.HasProperty("_DisplacementMode"))
			{
				DisplacementMode filteredDisplacementMode = GetFilteredDisplacementMode(material);
				bool flag = filteredDisplacementMode != DisplacementMode.None;
				bool flag2 = filteredDisplacementMode == DisplacementMode.Vertex;
				bool flag3 = filteredDisplacementMode == DisplacementMode.Pixel;
				bool flag4 = filteredDisplacementMode == DisplacementMode.Tessellation;
				CoreUtils.SetKeyword(material, "_VERTEX_DISPLACEMENT", flag2);
				CoreUtils.SetKeyword(material, "_PIXEL_DISPLACEMENT", flag3);
				CoreUtils.SetKeyword(material, "_TESSELLATION_DISPLACEMENT", flag4);
				bool flag5 = material.GetFloat("_DisplacementLockObjectScale") > 0f;
				bool flag6 = material.GetFloat("_DisplacementLockTilingScale") > 0f;
				CoreUtils.SetKeyword(material, "_VERTEX_DISPLACEMENT_LOCK_OBJECT_SCALE", flag5 && (flag2 || flag4));
				CoreUtils.SetKeyword(material, "_PIXEL_DISPLACEMENT_LOCK_OBJECT_SCALE", flag5 && flag3);
				CoreUtils.SetKeyword(material, "_DISPLACEMENT_LOCK_TILING_SCALE", flag6 && flag);
				bool state = material.GetFloat("_DepthOffsetEnable") > 0f && flag3;
				CoreUtils.SetKeyword(material, "_DEPTHOFFSET_ON", state);
			}
			CoreUtils.SetKeyword(material, "_VERTEX_WIND", state: false);
			material.SetupMainTexForAlphaTestGI("_BaseColorMap", "_BaseColor");
			CoreUtils.SetKeyword(material, "_DISABLE_DECALS", material.HasProperty("_SupportDecals") && material.GetFloat("_SupportDecals") == 0f);
			CoreUtils.SetKeyword(material, "_DISABLE_SSR", material.HasProperty("_ReceivesSSR") && material.GetFloat("_ReceivesSSR") == 0f);
			CoreUtils.SetKeyword(material, "_DISABLE_SSR_TRANSPARENT", material.HasProperty("_ReceivesSSRTransparent") && material.GetFloat("_ReceivesSSRTransparent") == 0f);
			CoreUtils.SetKeyword(material, "_ENABLE_GEOMETRIC_SPECULAR_AA", material.HasProperty("_EnableGeometricSpecularAA") && material.GetFloat("_EnableGeometricSpecularAA") == 1f);
			if (material.HasProperty("_RefractionModel"))
			{
				ScreenSpaceRefraction.RefractionModel refractionModel = (ScreenSpaceRefraction.RefractionModel)material.GetFloat("_RefractionModel");
				bool flag7 = material.GetSurfaceType() == SurfaceType.Transparent && !HDRenderQueue.k_RenderQueue_PreRefraction.Contains(material.renderQueue);
				CoreUtils.SetKeyword(material, "_REFRACTION_PLANE", refractionModel == ScreenSpaceRefraction.RefractionModel.Planar && flag7);
				CoreUtils.SetKeyword(material, "_REFRACTION_SPHERE", refractionModel == ScreenSpaceRefraction.RefractionModel.Sphere && flag7);
				CoreUtils.SetKeyword(material, "_REFRACTION_THIN", refractionModel == ScreenSpaceRefraction.RefractionModel.Thin && flag7);
			}
		}

		public static void SetupStencil(Material material, bool receivesLighting, bool receivesSSR, bool useSplitLighting)
		{
			bool forwardOnly = material.shader.FindPropertyIndex("_ZTestGBuffer") == -1;
			ComputeStencilProperties(receivesLighting, forwardOnly, receivesSSR, useSplitLighting, out var stencilRef, out var stencilWriteMask, out var stencilRefDepth, out var stencilWriteMaskDepth, out var stencilRefGBuffer, out var stencilWriteMaskGBuffer, out var stencilRefMV, out var stencilWriteMaskMV);
			if (material.HasProperty("_StencilRef"))
			{
				material.SetInt("_StencilRef", stencilRef);
				material.SetInt("_StencilWriteMask", stencilWriteMask);
			}
			if (material.HasProperty("_StencilRefDepth"))
			{
				material.SetInt("_StencilRefDepth", stencilRefDepth);
				material.SetInt("_StencilWriteMaskDepth", stencilWriteMaskDepth);
			}
			if (material.HasProperty("_StencilRefGBuffer"))
			{
				material.SetInt("_StencilRefGBuffer", stencilRefGBuffer);
				material.SetInt("_StencilWriteMaskGBuffer", stencilWriteMaskGBuffer);
			}
			if (material.HasProperty("_StencilRefDistortionVec"))
			{
				material.SetInt("_StencilRefDistortionVec", 4);
				material.SetInt("_StencilWriteMaskDistortionVec", 4);
			}
			if (material.HasProperty("_StencilRefMV"))
			{
				material.SetInt("_StencilRefMV", stencilRefMV);
				material.SetInt("_StencilWriteMaskMV", stencilWriteMaskMV);
			}
		}

		public static void ComputeStencilProperties(bool receivesLighting, bool forwardOnly, bool receivesSSR, bool useSplitLighting, out int stencilRef, out int stencilWriteMask, out int stencilRefDepth, out int stencilWriteMaskDepth, out int stencilRefGBuffer, out int stencilWriteMaskGBuffer, out int stencilRefMV, out int stencilWriteMaskMV)
		{
			stencilRef = 0;
			stencilWriteMask = 6;
			stencilRefDepth = 0;
			stencilWriteMaskDepth = 0;
			stencilRefGBuffer = 2;
			stencilWriteMaskGBuffer = 6;
			stencilRefMV = 32;
			stencilWriteMaskMV = 32;
			if (forwardOnly)
			{
				stencilWriteMaskMV |= 2;
			}
			if (useSplitLighting)
			{
				stencilRefGBuffer |= 4;
				stencilRef |= 4;
			}
			if (receivesSSR)
			{
				stencilRefDepth |= 8;
				stencilRefGBuffer |= 8;
				stencilRefMV |= 8;
			}
			stencilWriteMaskDepth |= 8;
			stencilWriteMaskGBuffer |= 8;
			stencilWriteMaskMV |= 8;
			if (!receivesLighting)
			{
				stencilRefDepth |= 1;
				stencilWriteMaskDepth |= 1;
				stencilRefMV |= 1;
			}
			stencilWriteMaskDepth |= 1;
			stencilWriteMaskGBuffer |= 1;
			stencilWriteMaskMV |= 1;
		}

		public static void SetupBaseLitMaterialPass(Material material)
		{
			material.SetupBaseUnlitPass();
		}

		public static void SetupDisplacement(Material material, int layerCount = 1)
		{
			DisplacementMode filteredDisplacementMode = GetFilteredDisplacementMode(material);
			for (int i = 0; i < layerCount; i++)
			{
				string name = ((layerCount > 1) ? ("_HeightAmplitude" + i) : "_HeightAmplitude");
				string name2 = ((layerCount > 1) ? ("_HeightCenter" + i) : "_HeightCenter");
				if (material.HasProperty(name) && material.HasProperty(name2))
				{
					string name3 = ((layerCount > 1) ? ("_HeightPoMAmplitude" + i) : "_HeightPoMAmplitude");
					string name4 = ((layerCount > 1) ? ("_HeightMapParametrization" + i) : "_HeightMapParametrization");
					string name5 = ((layerCount > 1) ? ("_HeightTessAmplitude" + i) : "_HeightTessAmplitude");
					string name6 = ((layerCount > 1) ? ("_HeightTessCenter" + i) : "_HeightTessCenter");
					string name7 = ((layerCount > 1) ? ("_HeightOffset" + i) : "_HeightOffset");
					string name8 = ((layerCount > 1) ? ("_HeightMin" + i) : "_HeightMin");
					string name9 = ((layerCount > 1) ? ("_HeightMax" + i) : "_HeightMax");
					if (filteredDisplacementMode == DisplacementMode.Pixel)
					{
						material.SetFloat(name, material.GetFloat(name3) * 0.01f);
						material.SetFloat(name2, 1f);
					}
					else if ((int)material.GetFloat(name4) == 0)
					{
						float @float = material.GetFloat(name7);
						float float2 = material.GetFloat(name8);
						float num = material.GetFloat(name9) - float2;
						material.SetFloat(name, num * 0.01f);
						material.SetFloat(name2, (0f - (float2 + @float)) / Mathf.Max(1E-06f, num));
					}
					else
					{
						float float3 = material.GetFloat(name7);
						float float4 = material.GetFloat(name6);
						float float5 = material.GetFloat(name5);
						material.SetFloat(name, float5 * 0.01f);
						material.SetFloat(name2, (0f - float3) / Mathf.Max(1E-06f, float5) + float4);
					}
				}
			}
		}
	}
}
