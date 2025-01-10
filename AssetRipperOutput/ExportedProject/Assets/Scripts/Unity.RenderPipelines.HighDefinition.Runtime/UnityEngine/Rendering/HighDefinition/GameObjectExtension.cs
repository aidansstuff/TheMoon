namespace UnityEngine.Rendering.HighDefinition
{
	public static class GameObjectExtension
	{
		public static HDAdditionalLightData AddHDLight(this GameObject gameObject, HDLightTypeAndShape lightTypeAndShape)
		{
			HDAdditionalLightData hDAdditionalLightData = gameObject.AddComponent<HDAdditionalLightData>();
			HDAdditionalLightData.InitDefaultHDAdditionalLightData(hDAdditionalLightData);
			hDAdditionalLightData.enableSpotReflector = false;
			hDAdditionalLightData.SetLightTypeAndShape(lightTypeAndShape);
			return hDAdditionalLightData;
		}

		public static void RemoveHDLight(this GameObject gameObject)
		{
			Light component = gameObject.GetComponent<Light>();
			CoreUtils.Destroy(gameObject.GetComponent<HDAdditionalLightData>());
			CoreUtils.Destroy(component);
		}
	}
}
