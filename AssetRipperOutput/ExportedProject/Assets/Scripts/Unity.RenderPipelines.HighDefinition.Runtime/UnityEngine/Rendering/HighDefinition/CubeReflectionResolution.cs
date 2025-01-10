using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public enum CubeReflectionResolution
	{
		CubeReflectionResolution128 = 0x80,
		CubeReflectionResolution256 = 0x100,
		CubeReflectionResolution512 = 0x200,
		CubeReflectionResolution1024 = 0x400,
		CubeReflectionResolution2048 = 0x800,
		CubeReflectionResolution4096 = 0x1000
	}
}
