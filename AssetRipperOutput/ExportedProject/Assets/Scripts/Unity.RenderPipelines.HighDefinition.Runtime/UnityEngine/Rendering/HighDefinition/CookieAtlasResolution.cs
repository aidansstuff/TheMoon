using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public enum CookieAtlasResolution
	{
		CookieResolution64 = 0x40,
		CookieResolution128 = 0x80,
		CookieResolution256 = 0x100,
		CookieResolution512 = 0x200,
		CookieResolution1024 = 0x400,
		CookieResolution2048 = 0x800,
		CookieResolution4096 = 0x1000,
		CookieResolution8192 = 0x2000,
		CookieResolution16384 = 0x4000
	}
}
