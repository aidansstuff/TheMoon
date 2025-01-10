namespace UnityEngine.Rendering.HighDefinition
{
	internal static class GPULightTypeExtension
	{
		public static bool IsAreaLight(this GPULightType lightType)
		{
			if (lightType != GPULightType.Rectangle)
			{
				return lightType == GPULightType.Tube;
			}
			return true;
		}

		public static bool IsSpot(this GPULightType lightType)
		{
			if (lightType != GPULightType.Spot && lightType != GPULightType.ProjectorBox)
			{
				return lightType == GPULightType.ProjectorPyramid;
			}
			return true;
		}
	}
}
