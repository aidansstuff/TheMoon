using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Flags]
	public enum UserStencilUsage
	{
		None = 0,
		UserBit0 = 0x40,
		UserBit1 = 0x80,
		AllUserBits = 0xC0
	}
}
