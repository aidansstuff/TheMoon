namespace UnityEngine.Rendering.HighDefinition
{
	public enum AccelerationStructureStatus
	{
		Clear = 0,
		Added = 1,
		Excluded = 2,
		TransparencyIssue = 4,
		NullMaterial = 8,
		MissingMesh = 0x10
	}
}
