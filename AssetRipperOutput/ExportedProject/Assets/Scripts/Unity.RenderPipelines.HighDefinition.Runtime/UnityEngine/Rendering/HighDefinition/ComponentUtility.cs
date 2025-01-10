namespace UnityEngine.Rendering.HighDefinition
{
	public static class ComponentUtility
	{
		public static bool IsHDCamera(Camera camera)
		{
			return camera.GetComponent<HDAdditionalCameraData>() != null;
		}

		public static bool IsHDLight(Light light)
		{
			return light.GetComponent<HDAdditionalLightData>() != null;
		}

		public static bool IsHDReflectionProbe(ReflectionProbe reflectionProbe)
		{
			return reflectionProbe.GetComponent<HDAdditionalReflectionData>() != null;
		}
	}
}
