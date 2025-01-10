using UnityEditor.Rendering.HighDefinition;

namespace UnityEngine.Rendering.HighDefinition
{
	internal static class UnlitAPI
	{
		internal static void ValidateMaterial(Material material)
		{
			material.SetupBaseUnlitKeywords();
			material.SetupBaseUnlitPass();
			if (material.HasProperty("_EmissiveColorMap"))
			{
				CoreUtils.SetKeyword(material, "_EMISSIVE_COLOR_MAP", material.GetTexture("_EmissiveColorMap"));
			}
			if (material.HasProperty("_UseEmissiveIntensity") && material.GetFloat("_UseEmissiveIntensity") != 0f)
			{
				material.UpdateEmissiveColorFromIntensityAndEmissiveColorLDR();
			}
			bool receivesLighting = material.HasProperty("_ShadowMatteFilter") && material.GetFloat("_ShadowMatteFilter") != 0f;
			BaseLitAPI.SetupStencil(material, receivesLighting, receivesSSR: false, useSplitLighting: false);
		}
	}
}
