namespace UnityEngine.Rendering.HighDefinition
{
	public static class ProbeRenderStepsExt
	{
		public static bool IsNone(this ProbeRenderSteps steps)
		{
			return steps == ProbeRenderSteps.None;
		}

		public static bool HasCubeFace(this ProbeRenderSteps steps, CubemapFace face)
		{
			ProbeRenderSteps probeRenderSteps = FromCubeFace(face);
			if (probeRenderSteps != 0)
			{
				return (steps & probeRenderSteps) == probeRenderSteps;
			}
			return true;
		}

		public static ProbeRenderSteps FromCubeFace(CubemapFace face)
		{
			return face switch
			{
				CubemapFace.PositiveX => ProbeRenderSteps.CubeFace0, 
				CubemapFace.NegativeX => ProbeRenderSteps.CubeFace1, 
				CubemapFace.PositiveY => ProbeRenderSteps.CubeFace2, 
				CubemapFace.NegativeY => ProbeRenderSteps.CubeFace3, 
				CubemapFace.PositiveZ => ProbeRenderSteps.CubeFace4, 
				CubemapFace.NegativeZ => ProbeRenderSteps.CubeFace5, 
				_ => ProbeRenderSteps.Planar, 
			};
		}

		public static ProbeRenderSteps FromProbeType(ProbeSettings.ProbeType probeType)
		{
			return probeType switch
			{
				ProbeSettings.ProbeType.ReflectionProbe => ProbeRenderSteps.ReflectionProbeMask, 
				ProbeSettings.ProbeType.PlanarProbe => ProbeRenderSteps.PlanarProbeMask, 
				_ => ProbeRenderSteps.None, 
			};
		}

		public static ProbeRenderSteps LowestSetBit(this ProbeRenderSteps steps)
		{
			return (ProbeRenderSteps)((int)steps & (0 - steps));
		}
	}
}
