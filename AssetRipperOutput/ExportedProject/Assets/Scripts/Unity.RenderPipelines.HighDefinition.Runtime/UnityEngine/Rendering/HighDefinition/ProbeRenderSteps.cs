using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Flags]
	public enum ProbeRenderSteps
	{
		None = 0,
		CubeFace0 = 1,
		CubeFace1 = 2,
		CubeFace2 = 4,
		CubeFace3 = 8,
		CubeFace4 = 0x10,
		CubeFace5 = 0x20,
		Planar = 0x40,
		IncrementRenderCount = 0x80,
		ReflectionProbeMask = 0xBF,
		PlanarProbeMask = 0xC0
	}
}
