using System;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	public struct GlobalDecalSettings
	{
		public int drawDistance;

		public int atlasWidth;

		public int atlasHeight;

		public bool perChannelMask;

		internal static GlobalDecalSettings NewDefault()
		{
			GlobalDecalSettings result = default(GlobalDecalSettings);
			result.drawDistance = 1000;
			result.atlasWidth = 4096;
			result.atlasHeight = 4096;
			return result;
		}
	}
}
