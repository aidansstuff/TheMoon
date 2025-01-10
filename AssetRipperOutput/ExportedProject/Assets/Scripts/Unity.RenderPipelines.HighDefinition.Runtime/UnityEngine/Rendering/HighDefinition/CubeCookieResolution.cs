using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public enum CubeCookieResolution
	{
		CubeCookieResolution64 = 0x40,
		CubeCookieResolution128 = 0x80,
		CubeCookieResolution256 = 0x100,
		CubeCookieResolution512 = 0x200,
		CubeCookieResolution1024 = 0x400,
		CubeCookieResolution2048 = 0x800,
		CubeCookieResolution4096 = 0x1000
	}
}
