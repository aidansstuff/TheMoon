using UnityEditor.Rendering.HighDefinition;

namespace UnityEngine.Rendering.HighDefinition
{
	internal static class LayeredLitAPI
	{
		private const string kLayerInfluenceMaskMap = "_LayerInfluenceMaskMap";

		private const string kVertexColorMode = "_VertexColorMode";

		private const string kUVBlendMask = "_UVBlendMask";

		private const string kkUseMainLayerInfluence = "_UseMainLayerInfluence";

		private const string kUseHeightBasedBlend = "_UseHeightBasedBlend";

		private const string kObjectScaleAffectTile = "_ObjectScaleAffectTile";

		private const string kOpacityAsDensity = "_OpacityAsDensity";

		public static void SetupLayersMappingKeywords(Material material)
		{
			CoreUtils.SetKeyword(material, "_LAYER_TILING_COUPLED_WITH_UNIFORM_OBJECT_SCALE", material.GetFloat("_ObjectScaleAffectTile") > 0f);
			UVBaseMapping uVBaseMapping = (UVBaseMapping)material.GetFloat("_UVBlendMask");
			CoreUtils.SetKeyword(material, "_LAYER_MAPPING_PLANAR_BLENDMASK", uVBaseMapping == UVBaseMapping.Planar);
			CoreUtils.SetKeyword(material, "_LAYER_MAPPING_TRIPLANAR_BLENDMASK", uVBaseMapping == UVBaseMapping.Triplanar);
			int num = (int)material.GetFloat("_LayerCount");
			switch (num)
			{
			case 4:
				CoreUtils.SetKeyword(material, "_LAYEREDLIT_4_LAYERS", state: true);
				CoreUtils.SetKeyword(material, "_LAYEREDLIT_3_LAYERS", state: false);
				break;
			case 3:
				CoreUtils.SetKeyword(material, "_LAYEREDLIT_4_LAYERS", state: false);
				CoreUtils.SetKeyword(material, "_LAYEREDLIT_3_LAYERS", state: true);
				break;
			default:
				CoreUtils.SetKeyword(material, "_LAYEREDLIT_4_LAYERS", state: false);
				CoreUtils.SetKeyword(material, "_LAYEREDLIT_3_LAYERS", state: false);
				break;
			}
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < num; i++)
			{
				string name = string.Format("{0}{1}", "_UVBase", i);
				UVBaseMapping uVBaseMapping2 = (UVBaseMapping)material.GetFloat(name);
				string keyword = string.Format("{0}{1}", "_LAYER_MAPPING_PLANAR", i);
				CoreUtils.SetKeyword(material, keyword, uVBaseMapping2 == UVBaseMapping.Planar);
				string keyword2 = string.Format("{0}{1}", "_LAYER_MAPPING_TRIPLANAR", i);
				CoreUtils.SetKeyword(material, keyword2, uVBaseMapping2 == UVBaseMapping.Triplanar);
				string name2 = string.Format("{0}{1}", "_UVBase", i);
				string name3 = string.Format("{0}{1}", "_UVDetail", i);
				if ((int)material.GetFloat(name3) == 2 || (int)material.GetFloat(name2) == 2)
				{
					flag2 = true;
				}
				if ((int)material.GetFloat(name3) == 3 || (int)material.GetFloat(name2) == 3)
				{
					flag = true;
					break;
				}
			}
			if (flag)
			{
				material.DisableKeyword("_REQUIRE_UV2");
				material.EnableKeyword("_REQUIRE_UV3");
			}
			else if (flag2)
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

		internal static void ValidateMaterial(Material material)
		{
			MaterialId materialId = material.GetMaterialId();
			if (material.HasProperty("_MaterialID") && materialId != MaterialId.LitStandard && materialId != 0 && materialId != MaterialId.LitTranslucent)
			{
				materialId = MaterialId.LitStandard;
				material.SetFloat("_MaterialID", (float)materialId);
			}
			BaseLitAPI.SetupBaseLitKeywords(material);
			BaseLitAPI.SetupBaseLitMaterialPass(material);
			SetupLayersMappingKeywords(material);
			bool receivesSSR = ((material.GetSurfaceType() != 0) ? (material.HasProperty("_ReceivesSSRTransparent") && material.GetInt("_ReceivesSSRTransparent") != 0) : (material.HasProperty("_ReceivesSSR") && material.GetInt("_ReceivesSSR") != 0));
			BaseLitAPI.SetupStencil(material, receivesLighting: true, receivesSSR, materialId == MaterialId.LitSSS);
			for (int i = 0; i < 4; i++)
			{
				NormalMapSpace normalMapSpace = (NormalMapSpace)material.GetFloat("_NormalMapSpace" + i);
				CoreUtils.SetKeyword(material, "_NORMALMAP_TANGENT_SPACE" + i, normalMapSpace == NormalMapSpace.TangentSpace);
				if (normalMapSpace == NormalMapSpace.TangentSpace)
				{
					CoreUtils.SetKeyword(material, "_NORMALMAP" + i, (bool)material.GetTexture("_NormalMap" + i) || (bool)material.GetTexture("_DetailMap" + i));
					CoreUtils.SetKeyword(material, "_BENTNORMALMAP" + i, material.GetTexture("_BentNormalMap" + i));
				}
				else
				{
					CoreUtils.SetKeyword(material, "_NORMALMAP" + i, (bool)material.GetTexture("_NormalMapOS" + i) || (bool)material.GetTexture("_DetailMap" + i));
					CoreUtils.SetKeyword(material, "_BENTNORMALMAP" + i, material.GetTexture("_BentNormalMapOS" + i));
				}
				CoreUtils.SetKeyword(material, "_MASKMAP" + i, material.GetTexture("_MaskMap" + i));
				CoreUtils.SetKeyword(material, "_DETAIL_MAP" + i, material.GetTexture("_DetailMap" + i));
				CoreUtils.SetKeyword(material, "_HEIGHTMAP" + i, material.GetTexture("_HeightMap" + i));
				CoreUtils.SetKeyword(material, "_SUBSURFACE_MASK_MAP" + i, material.GetTexture("_SubsurfaceMaskMap" + i));
				CoreUtils.SetKeyword(material, "_TRANSMISSION_MASK_MAP" + i, material.GetTexture("_TransmissionMaskMap" + i));
				CoreUtils.SetKeyword(material, "_THICKNESSMAP" + i, material.GetTexture("_ThicknessMap" + i));
			}
			CoreUtils.SetKeyword(material, "_INFLUENCEMASK_MAP", (bool)material.GetTexture("_LayerInfluenceMaskMap") && material.GetFloat("_UseMainLayerInfluence") != 0f);
			CoreUtils.SetKeyword(material, "_EMISSIVE_MAPPING_PLANAR", (int)material.GetFloat("_UVEmissive") == 4 && (bool)material.GetTexture("_EmissiveColorMap"));
			CoreUtils.SetKeyword(material, "_EMISSIVE_MAPPING_TRIPLANAR", (int)material.GetFloat("_UVEmissive") == 5 && (bool)material.GetTexture("_EmissiveColorMap"));
			CoreUtils.SetKeyword(material, "_EMISSIVE_MAPPING_BASE", (int)material.GetFloat("_UVEmissive") == 6 && (bool)material.GetTexture("_EmissiveColorMap"));
			CoreUtils.SetKeyword(material, "_EMISSIVE_COLOR_MAP", material.GetTexture("_EmissiveColorMap"));
			if (material.HasProperty("_UseEmissiveIntensity") && material.GetFloat("_UseEmissiveIntensity") != 0f)
			{
				material.UpdateEmissiveColorFromIntensityAndEmissiveColorLDR();
			}
			CoreUtils.SetKeyword(material, "_ENABLESPECULAROCCLUSION", state: false);
			int @int = material.GetInt("_SpecularOcclusionMode");
			CoreUtils.SetKeyword(material, "_SPECULAR_OCCLUSION_NONE", @int == 0);
			CoreUtils.SetKeyword(material, "_SPECULAR_OCCLUSION_FROM_BENT_NORMAL_MAP", @int == 2);
			CoreUtils.SetKeyword(material, "_MAIN_LAYER_INFLUENCE_MODE", material.GetFloat("_UseMainLayerInfluence") != 0f);
			switch ((VertexColorMode)(int)material.GetFloat("_VertexColorMode"))
			{
			case VertexColorMode.Multiply:
				CoreUtils.SetKeyword(material, "_LAYER_MASK_VERTEX_COLOR_MUL", state: true);
				CoreUtils.SetKeyword(material, "_LAYER_MASK_VERTEX_COLOR_ADD", state: false);
				break;
			case VertexColorMode.Add:
				CoreUtils.SetKeyword(material, "_LAYER_MASK_VERTEX_COLOR_MUL", state: false);
				CoreUtils.SetKeyword(material, "_LAYER_MASK_VERTEX_COLOR_ADD", state: true);
				break;
			default:
				CoreUtils.SetKeyword(material, "_LAYER_MASK_VERTEX_COLOR_MUL", state: false);
				CoreUtils.SetKeyword(material, "_LAYER_MASK_VERTEX_COLOR_ADD", state: false);
				break;
			}
			bool state = material.GetFloat("_UseHeightBasedBlend") != 0f;
			CoreUtils.SetKeyword(material, "_HEIGHT_BASED_BLEND", state);
			bool flag = false;
			for (int j = 0; j < material.GetInt("_LayerCount"); j++)
			{
				flag |= material.GetFloat("_OpacityAsDensity" + j) != 0f;
			}
			CoreUtils.SetKeyword(material, "_DENSITY_MODE", flag);
			CoreUtils.SetKeyword(material, "_MATERIAL_FEATURE_SUBSURFACE_SCATTERING", materialId == MaterialId.LitSSS);
			CoreUtils.SetKeyword(material, "_MATERIAL_FEATURE_TRANSMISSION", materialId switch
			{
				MaterialId.LitSSS => material.GetFloat("_TransmissionEnable") > 0f, 
				MaterialId.LitTranslucent => true, 
				_ => false, 
			});
			BaseLitAPI.SetupDisplacement(material, material.GetInt("_LayerCount"));
		}
	}
}
