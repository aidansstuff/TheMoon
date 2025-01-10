namespace UnityEngine.Rendering.HighDefinition
{
	internal static class ShaderGraphAPI
	{
		private static readonly string[] floatPropertiesToSynchronize = new string[1] { "_RequireSplitLighting" };

		public static void SynchronizeShaderGraphProperties(Material material)
		{
			Material material2 = new Material(material.shader);
			string[] array = floatPropertiesToSynchronize;
			foreach (string name in array)
			{
				if (material.HasProperty(name) && material2.HasProperty(name))
				{
					material.SetFloat(name, material2.GetFloat(name));
				}
			}
			CoreUtils.Destroy(material2);
			material2 = null;
		}

		public static void ValidateUnlitMaterial(Material material)
		{
			SynchronizeShaderGraphProperties(material);
			UnlitAPI.ValidateMaterial(material);
		}

		public static void ValidateLightingMaterial(Material material)
		{
			SynchronizeShaderGraphProperties(material);
			BaseLitAPI.SetupBaseLitKeywords(material);
			BaseLitAPI.SetupBaseLitMaterialPass(material);
			bool flag = false;
			flag = ((!material.HasProperty("_SurfaceType") || (int)material.GetFloat("_SurfaceType") != 1) ? (material.HasProperty("_ReceivesSSR") && material.GetFloat("_ReceivesSSR") != 0f) : (material.HasProperty("_ReceivesSSRTransparent") && material.GetFloat("_ReceivesSSRTransparent") != 0f));
			bool useSplitLighting = false;
			int num = material.shader.FindPropertyIndex("_RequireSplitLighting");
			if (num != -1)
			{
				useSplitLighting = material.shader.GetPropertyDefaultFloatValue(num) != 0f;
			}
			BaseLitAPI.SetupStencil(material, receivesLighting: true, flag, useSplitLighting);
		}

		public static void ValidateDecalMaterial(Material material)
		{
			DecalAPI.SetupCommonDecalMaterialKeywordsAndPass(material);
		}

		public static void ValidateFogVolumeMaterial(Material material)
		{
			FogVolumeAPI.SetupFogVolumeKeywordsAndProperties(material);
		}
	}
}
