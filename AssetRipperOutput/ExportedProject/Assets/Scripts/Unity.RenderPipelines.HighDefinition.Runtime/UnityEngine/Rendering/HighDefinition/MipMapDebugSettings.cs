using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public class MipMapDebugSettings
	{
		public DebugMipMapMode debugMipMapMode;

		public DebugMipMapModeTerrainTexture terrainTexture;

		public bool IsDebugDisplayEnabled()
		{
			return debugMipMapMode != DebugMipMapMode.None;
		}
	}
}
