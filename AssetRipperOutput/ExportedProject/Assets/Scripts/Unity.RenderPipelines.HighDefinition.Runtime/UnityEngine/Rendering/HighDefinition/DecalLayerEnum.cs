using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Flags]
	public enum DecalLayerEnum
	{
		Nothing = 0,
		DecalLayerDefault = 1,
		DecalLayer1 = 2,
		DecalLayer2 = 4,
		DecalLayer3 = 8,
		DecalLayer4 = 0x10,
		DecalLayer5 = 0x20,
		DecalLayer6 = 0x40,
		DecalLayer7 = 0x80,
		Everything = 0xFF
	}
}
