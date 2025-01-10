namespace UnityEngine.Rendering.HighDefinition
{
	public enum DepthOfFieldMode
	{
		Off = 0,
		[InspectorName("Physical Camera")]
		UsePhysicalCamera = 1,
		[InspectorName("Manual Ranges")]
		Manual = 2
	}
}
