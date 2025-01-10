using System;
using System.Collections.Generic;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	internal sealed class VirtualTexturingSettingsSRP
	{
		public int streamingCpuCacheSizeInMegaBytes = 256;

		public int streamingMipPreloadTexturesPerFrame;

		public int streamingPreloadMipCount = 1;

		public List<GPUCacheSettingSRP> streamingGpuCacheSettings = new List<GPUCacheSettingSRP>
		{
			new GPUCacheSettingSRP
			{
				format = GraphicsFormat.None,
				sizeInMegaBytes = 128u
			}
		};
	}
}
