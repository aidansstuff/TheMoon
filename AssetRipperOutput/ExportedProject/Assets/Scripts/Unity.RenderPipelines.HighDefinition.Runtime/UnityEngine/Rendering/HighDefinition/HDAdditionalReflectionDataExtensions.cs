namespace UnityEngine.Rendering.HighDefinition
{
	public static class HDAdditionalReflectionDataExtensions
	{
		public static void RequestRenderNextUpdate(this ReflectionProbe probe)
		{
			HDAdditionalReflectionData component = probe.GetComponent<HDAdditionalReflectionData>();
			if (component != null && !component.Equals(null))
			{
				component.RequestRenderNextUpdate();
			}
		}
	}
}
