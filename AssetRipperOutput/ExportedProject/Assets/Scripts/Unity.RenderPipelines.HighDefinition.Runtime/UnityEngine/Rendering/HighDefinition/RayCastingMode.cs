namespace UnityEngine.Rendering.HighDefinition
{
	public enum RayCastingMode
	{
		[InspectorName("Ray Marching")]
		RayMarching = 1,
		[InspectorName("Ray Tracing (Preview)")]
		RayTracing = 2,
		[InspectorName("Mixed (Preview)")]
		Mixed = 4
	}
}
