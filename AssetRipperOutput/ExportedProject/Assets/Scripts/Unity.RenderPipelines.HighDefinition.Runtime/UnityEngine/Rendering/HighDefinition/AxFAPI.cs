using UnityEditor.Rendering.HighDefinition;

namespace UnityEngine.Rendering.HighDefinition
{
	internal static class AxFAPI
	{
		private const string kIntPropAsFloatSuffix = "F";

		private const string kFlags = "_Flags";

		private const string kFlagsB = "_FlagsB";

		private const string kSVBRDF_BRDFType = "_SVBRDF_BRDFType";

		private const string kSVBRDF_BRDFVariants = "_SVBRDF_BRDFVariants";

		private const string kSVBRDF_BRDFType_DiffuseType = "_SVBRDF_BRDFType_DiffuseType";

		private const string kSVBRDF_BRDFType_SpecularType = "_SVBRDF_BRDFType_SpecularType";

		private const string kSVBRDF_BRDFVariants_FresnelType = "_SVBRDF_BRDFVariants_FresnelType";

		private const string kSVBRDF_BRDFVariants_WardType = "_SVBRDF_BRDFVariants_WardType";

		private const string kSVBRDF_BRDFVariants_BlinnType = "_SVBRDF_BRDFVariants_BlinnType";

		private const string kCarPaint2_FlakeMaxThetaI = "_CarPaint2_FlakeMaxThetaI";

		private const string kCarPaint2_FlakeNumThetaF = "_CarPaint2_FlakeNumThetaF";

		private const string kCarPaint2_FlakeNumThetaI = "_CarPaint2_FlakeNumThetaI";

		private const string kAxF_BRDFType = "_AxF_BRDFType";

		private const string kMappingMode = "_MappingMode";

		private const string kMappingMask = "_MappingMask";

		private const string kPlanarSpace = "_PlanarSpace";

		public static Vector4 AxFMappingModeToMask(AxFMappingMode mappingMode)
		{
			Vector4 result = Vector4.zero;
			if (mappingMode <= AxFMappingMode.UV3)
			{
				float x = ((mappingMode == AxFMappingMode.UV0) ? 1f : 0f);
				float y = ((mappingMode == AxFMappingMode.UV1) ? 1f : 0f);
				float z = ((mappingMode == AxFMappingMode.UV2) ? 1f : 0f);
				float w = ((mappingMode == AxFMappingMode.UV3) ? 1f : 0f);
				result = new Vector4(x, y, z, w);
			}
			else if (mappingMode < AxFMappingMode.Triplanar)
			{
				float x2 = ((mappingMode == AxFMappingMode.PlanarYZ) ? 1f : 0f);
				float y2 = ((mappingMode == AxFMappingMode.PlanarZX) ? 1f : 0f);
				float z2 = ((mappingMode == AxFMappingMode.PlanarXY) ? 1f : 0f);
				float w2 = 0f;
				result = new Vector4(x2, y2, z2, w2);
			}
			return result;
		}

		public static void ValidateMaterial(Material material)
		{
			material.SetupBaseUnlitKeywords();
			material.SetupBaseUnlitPass();
			AxfBrdfType axfBrdfType = (AxfBrdfType)material.GetFloat("_AxF_BRDFType");
			CoreUtils.SetKeyword(material, "_AXF_BRDF_TYPE_SVBRDF", axfBrdfType == AxfBrdfType.SVBRDF);
			CoreUtils.SetKeyword(material, "_AXF_BRDF_TYPE_CAR_PAINT", axfBrdfType == AxfBrdfType.CAR_PAINT);
			AxFMappingMode axFMappingMode = (AxFMappingMode)material.GetFloat("_MappingMode");
			material.SetVector("_MappingMask", AxFMappingModeToMask(axFMappingMode));
			bool flag = axFMappingMode >= AxFMappingMode.PlanarXY && axFMappingMode < AxFMappingMode.Triplanar;
			bool state = material.GetFloat("_PlanarSpace") > 0f;
			CoreUtils.SetKeyword(material, "_MAPPING_PLANAR", flag);
			CoreUtils.SetKeyword(material, "_MAPPING_TRIPLANAR", axFMappingMode == AxFMappingMode.Triplanar);
			if (flag || axFMappingMode == AxFMappingMode.Triplanar)
			{
				CoreUtils.SetKeyword(material, "_PLANAR_LOCAL", state);
			}
			CoreUtils.SetKeyword(material, "_REQUIRE_UV1", axFMappingMode == AxFMappingMode.UV1);
			CoreUtils.SetKeyword(material, "_REQUIRE_UV2", axFMappingMode == AxFMappingMode.UV2);
			CoreUtils.SetKeyword(material, "_REQUIRE_UV3", axFMappingMode == AxFMappingMode.UV3);
			bool flag2 = material.HasProperty("_SupportDecals") && material.GetFloat("_SupportDecals") > 0f;
			CoreUtils.SetKeyword(material, "_DISABLE_DECALS", !flag2);
			bool flag3 = false;
			flag3 = ((material.GetSurfaceType() != SurfaceType.Transparent) ? (material.HasProperty("_ReceivesSSR") && material.GetFloat("_ReceivesSSR") != 0f) : (material.HasProperty("_ReceivesSSRTransparent") && material.GetFloat("_ReceivesSSRTransparent") != 0f));
			CoreUtils.SetKeyword(material, "_DISABLE_SSR", material.HasProperty("_ReceivesSSR") && material.GetFloat("_ReceivesSSR") == 0f);
			CoreUtils.SetKeyword(material, "_DISABLE_SSR_TRANSPARENT", material.HasProperty("_ReceivesSSRTransparent") && (double)material.GetFloat("_ReceivesSSRTransparent") == 0.0);
			CoreUtils.SetKeyword(material, "_ENABLE_GEOMETRIC_SPECULAR_AA", material.HasProperty("_EnableGeometricSpecularAA") && material.GetFloat("_EnableGeometricSpecularAA") > 0f);
			CoreUtils.SetKeyword(material, "_SPECULAR_OCCLUSION_NONE", material.HasProperty("_SpecularOcclusionMode") && material.GetFloat("_SpecularOcclusionMode") == 0f);
			BaseLitAPI.SetupStencil(material, receivesLighting: true, flag3, useSplitLighting: false);
			uint num = (uint)material.GetFloat("_Flags");
			num |= 0x800000u;
			material.SetFloat("_FlagsB", num);
			uint num2 = (uint)material.GetFloat("_SVBRDF_BRDFType");
			uint num3 = (uint)material.GetFloat("_SVBRDF_BRDFVariants");
			SvbrdfDiffuseType svbrdfDiffuseType = (SvbrdfDiffuseType)((int)num2 & 1);
			SvbrdfSpecularType svbrdfSpecularType = (SvbrdfSpecularType)((int)(num2 >> 1) & 7);
			SvbrdfFresnelVariant svbrdfFresnelVariant = (SvbrdfFresnelVariant)((int)num3 & 3);
			SvbrdfSpecularVariantWard svbrdfSpecularVariantWard = (SvbrdfSpecularVariantWard)((int)(num3 >> 2) & 3);
			SvbrdfSpecularVariantBlinn svbrdfSpecularVariantBlinn = (SvbrdfSpecularVariantBlinn)((int)(num3 >> 4) & 3);
			material.SetFloat("_SVBRDF_BRDFType_DiffuseType", (float)svbrdfDiffuseType);
			material.SetFloat("_SVBRDF_BRDFType_SpecularType", (float)svbrdfSpecularType);
			material.SetFloat("_SVBRDF_BRDFVariants_FresnelType", (float)svbrdfFresnelVariant);
			material.SetFloat("_SVBRDF_BRDFVariants_WardType", (float)svbrdfSpecularVariantWard);
			material.SetFloat("_SVBRDF_BRDFVariants_BlinnType", (float)svbrdfSpecularVariantBlinn);
			material.SetFloat("_CarPaint2_FlakeMaxThetaIF", material.GetFloat("_CarPaint2_FlakeMaxThetaI"));
			material.SetFloat("_CarPaint2_FlakeNumThetaFF", material.GetFloat("_CarPaint2_FlakeNumThetaF"));
			material.SetFloat("_CarPaint2_FlakeNumThetaIF", material.GetFloat("_CarPaint2_FlakeNumThetaI"));
		}
	}
}
