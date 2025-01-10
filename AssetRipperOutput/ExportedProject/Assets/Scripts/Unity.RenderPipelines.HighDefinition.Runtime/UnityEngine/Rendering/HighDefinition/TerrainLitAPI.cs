using UnityEditor.Rendering.HighDefinition;

namespace UnityEngine.Rendering.HighDefinition
{
	internal static class TerrainLitAPI
	{
		public static void ValidateMaterial(Material material)
		{
			BaseLitAPI.SetupBaseLitKeywords(material);
			BaseLitAPI.SetupBaseLitMaterialPass(material);
			bool flag = false;
			flag = ((!material.HasProperty("_SurfaceType") || (int)material.GetFloat("_SurfaceType") != 1) ? (material.HasProperty("_ReceivesSSR") && material.GetFloat("_ReceivesSSR") != 0f) : (material.HasProperty("_ReceivesSSRTransparent") && material.GetFloat("_ReceivesSSRTransparent") != 0f));
			BaseLitAPI.SetupStencil(material, receivesLighting: true, flag, material.GetMaterialId() == MaterialId.LitSSS);
			bool state = material.HasProperty("_EnableHeightBlend") && material.GetFloat("_EnableHeightBlend") > 0f;
			CoreUtils.SetKeyword(material, "_TERRAIN_BLEND_HEIGHT", state);
			bool state2 = material.HasProperty("_EnableInstancedPerPixelNormal") && material.GetFloat("_EnableInstancedPerPixelNormal") > 0f;
			CoreUtils.SetKeyword(material, "_TERRAIN_INSTANCED_PERPIXEL_NORMAL", state2);
			int @int = material.GetInt("_SpecularOcclusionMode");
			CoreUtils.SetKeyword(material, "_SPECULAR_OCCLUSION_NONE", @int == 0);
		}
	}
}
