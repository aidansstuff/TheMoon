using System;
using UnityEngine.Experimental.Rendering;

namespace UnityEngine.Rendering.HighDefinition
{
	[Serializable]
	internal struct GPUCacheSettingSRP
	{
		public GraphicsFormat format;

		public uint sizeInMegaBytes;
	}
}
