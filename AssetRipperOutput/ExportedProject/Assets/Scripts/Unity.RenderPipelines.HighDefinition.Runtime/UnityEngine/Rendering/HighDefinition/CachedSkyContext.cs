using System;

namespace UnityEngine.Rendering.HighDefinition
{
	internal struct CachedSkyContext
	{
		public Type type;

		public SkyRenderingContext renderingContext;

		public int hash;

		public int refCount;

		public void Reset()
		{
			hash = 0;
			refCount = 0;
			if (renderingContext != null)
			{
				renderingContext.Reset();
			}
		}

		public void Cleanup()
		{
			Reset();
			if (renderingContext != null)
			{
				renderingContext.Cleanup();
				renderingContext = null;
			}
		}
	}
}
