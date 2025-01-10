using UnityEditor.Rendering.HighDefinition;

namespace UnityEngine.Rendering.HighDefinition
{
	internal static class LitAPI
	{
		internal static void ValidateMaterial(Material material)
		{
			BaseLitAPI.SetupBaseLitKeywords(material);
			BaseLitAPI.SetupBaseLitMaterialPass(material);
			bool receivesSSR = ((material.GetSurfaceType() != 0) ? (material.HasProperty("_ReceivesSSRTransparent") && material.GetInt("_ReceivesSSRTransparent") != 0) : (material.HasProperty("_ReceivesSSR") && material.GetInt("_ReceivesSSR") != 0));
			BaseLitAPI.SetupStencil(material, receivesLighting: true, receivesSSR, material.GetMaterialId() == MaterialId.LitSSS);
			BaseLitAPI.SetupDisplacement(material);
			if (material.HasProperty("_NormalMapSpace"))
			{
				NormalMapSpace normalMapSpace = (NormalMapSpace)material.GetFloat("_NormalMapSpace");
				CoreUtils.SetKeyword(material, "_MAPPING_PLANAR", (int)material.GetFloat("_UVBase") == 4);
				CoreUtils.SetKeyword(material, "_MAPPING_TRIPLANAR", (int)material.GetFloat("_UVBase") == 5);
				CoreUtils.SetKeyword(material, "_NORMALMAP_TANGENT_SPACE", normalMapSpace == NormalMapSpace.TangentSpace);
				if (normalMapSpace == NormalMapSpace.TangentSpace)
				{
					CoreUtils.SetKeyword(material, "_NORMALMAP", (bool)material.GetTexture("_NormalMap") || (bool)material.GetTexture("_DetailMap"));
					CoreUtils.SetKeyword(material, "_TANGENTMAP", material.GetTexture("_TangentMap"));
					CoreUtils.SetKeyword(material, "_BENTNORMALMAP", material.GetTexture("_BentNormalMap"));
				}
				else
				{
					CoreUtils.SetKeyword(material, "_NORMALMAP", material.GetTexture("_NormalMapOS"));
					CoreUtils.SetKeyword(material, "_TANGENTMAP", material.GetTexture("_TangentMapOS"));
					CoreUtils.SetKeyword(material, "_BENTNORMALMAP", material.GetTexture("_BentNormalMapOS"));
				}
			}
			if (material.HasProperty("_MaskMap"))
			{
				CoreUtils.SetKeyword(material, "_MASKMAP", material.GetTexture("_MaskMap"));
			}
			if (material.HasProperty("_UVEmissive") && material.HasProperty("_EmissiveColorMap"))
			{
				CoreUtils.SetKeyword(material, "_EMISSIVE_MAPPING_PLANAR", (int)material.GetFloat("_UVEmissive") == 4 && (bool)material.GetTexture("_EmissiveColorMap"));
				CoreUtils.SetKeyword(material, "_EMISSIVE_MAPPING_TRIPLANAR", (int)material.GetFloat("_UVEmissive") == 5 && (bool)material.GetTexture("_EmissiveColorMap"));
				CoreUtils.SetKeyword(material, "_EMISSIVE_MAPPING_BASE", (int)material.GetFloat("_UVEmissive") == 6 && (bool)material.GetTexture("_EmissiveColorMap"));
				CoreUtils.SetKeyword(material, "_EMISSIVE_COLOR_MAP", material.GetTexture("_EmissiveColorMap"));
			}
			if (material.HasProperty("_UseEmissiveIntensity") && material.GetFloat("_UseEmissiveIntensity") != 0f)
			{
				material.UpdateEmissiveColorFromIntensityAndEmissiveColorLDR();
			}
			if (material.HasProperty("_SpecularOcclusionMode"))
			{
				CoreUtils.SetKeyword(material, "_ENABLESPECULAROCCLUSION", state: false);
				int @int = material.GetInt("_SpecularOcclusionMode");
				CoreUtils.SetKeyword(material, "_SPECULAR_OCCLUSION_NONE", @int == 0);
				CoreUtils.SetKeyword(material, "_SPECULAR_OCCLUSION_FROM_BENT_NORMAL_MAP", @int == 2);
			}
			if (material.HasProperty("_HeightMap"))
			{
				CoreUtils.SetKeyword(material, "_HEIGHTMAP", material.GetTexture("_HeightMap"));
			}
			if (material.HasProperty("_AnisotropyMap"))
			{
				CoreUtils.SetKeyword(material, "_ANISOTROPYMAP", material.GetTexture("_AnisotropyMap"));
			}
			if (material.HasProperty("_DetailMap"))
			{
				CoreUtils.SetKeyword(material, "_DETAIL_MAP", material.GetTexture("_DetailMap"));
			}
			if (material.HasProperty("_SubsurfaceMaskMap"))
			{
				CoreUtils.SetKeyword(material, "_SUBSURFACE_MASK_MAP", material.GetTexture("_SubsurfaceMaskMap"));
			}
			if (material.HasProperty("_TransmissionMaskMap"))
			{
				CoreUtils.SetKeyword(material, "_TRANSMISSION_MASK_MAP", material.GetTexture("_TransmissionMaskMap"));
			}
			if (material.HasProperty("_ThicknessMap"))
			{
				CoreUtils.SetKeyword(material, "_THICKNESSMAP", material.GetTexture("_ThicknessMap"));
			}
			if (material.HasProperty("_IridescenceThicknessMap"))
			{
				CoreUtils.SetKeyword(material, "_IRIDESCENCE_THICKNESSMAP", material.GetTexture("_IridescenceThicknessMap"));
			}
			if (material.HasProperty("_SpecularColorMap"))
			{
				CoreUtils.SetKeyword(material, "_SPECULARCOLORMAP", material.GetTexture("_SpecularColorMap"));
			}
			if (material.HasProperty("_UVDetail") || material.HasProperty("_UVBase"))
			{
				bool flag = (int)material.GetFloat("_UVDetail") == 2 || (int)material.GetFloat("_UVBase") == 2;
				if ((int)material.GetFloat("_UVDetail") == 3 || (int)material.GetFloat("_UVBase") == 3)
				{
					material.DisableKeyword("_REQUIRE_UV2");
					material.EnableKeyword("_REQUIRE_UV3");
				}
				else if (flag)
				{
					material.EnableKeyword("_REQUIRE_UV2");
					material.DisableKeyword("_REQUIRE_UV3");
				}
				else
				{
					material.DisableKeyword("_REQUIRE_UV2");
					material.DisableKeyword("_REQUIRE_UV3");
				}
			}
			if (material.HasProperty("_MaterialID"))
			{
				MaterialId materialId = material.GetMaterialId();
				CoreUtils.SetKeyword(material, "_MATERIAL_FEATURE_SUBSURFACE_SCATTERING", materialId == MaterialId.LitSSS);
				CoreUtils.SetKeyword(material, "_MATERIAL_FEATURE_TRANSMISSION", materialId switch
				{
					MaterialId.LitSSS => material.GetFloat("_TransmissionEnable") > 0f, 
					MaterialId.LitTranslucent => true, 
					_ => false, 
				});
				CoreUtils.SetKeyword(material, "_MATERIAL_FEATURE_ANISOTROPY", materialId == MaterialId.LitAniso);
				CoreUtils.SetKeyword(material, "_MATERIAL_FEATURE_CLEAR_COAT", (double)material.GetFloat("_CoatMask") > 0.0 || (bool)material.GetTexture("_CoatMaskMap"));
				CoreUtils.SetKeyword(material, "_MATERIAL_FEATURE_IRIDESCENCE", materialId == MaterialId.LitIridescence);
				CoreUtils.SetKeyword(material, "_MATERIAL_FEATURE_SPECULAR_COLOR", materialId == MaterialId.LitSpecular);
			}
			if (material.HasProperty("_RefractionModel"))
			{
				bool flag2 = material.GetSurfaceType() == SurfaceType.Transparent && !HDRenderQueue.k_RenderQueue_PreRefraction.Contains(material.renderQueue);
				CoreUtils.SetKeyword(material, "_TRANSMITTANCECOLORMAP", (bool)material.GetTexture("_TransmittanceColorMap") && flag2);
			}
		}
	}
}
