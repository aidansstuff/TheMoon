using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public enum PlanarReflectionAtlasResolution
	{
		Resolution64 = 0x40,
		Resolution128 = 0x80,
		Resolution256 = 0x100,
		Resolution512 = 0x200,
		Resolution1024 = 0x400,
		Resolution2048 = 0x800,
		Resolution4096 = 0x1000,
		Resolution8192 = 0x2000,
		Resolution16384 = 0x4000
	}
}
