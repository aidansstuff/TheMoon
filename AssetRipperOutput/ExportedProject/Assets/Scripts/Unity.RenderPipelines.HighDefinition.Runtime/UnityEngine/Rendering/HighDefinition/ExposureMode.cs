namespace UnityEngine.Rendering.HighDefinition
{
	public enum ExposureMode
	{
		Fixed = 0,
		Automatic = 1,
		AutomaticHistogram = 4,
		CurveMapping = 2,
		[InspectorName("Physical Camera")]
		UsePhysicalCamera = 3
	}
}
