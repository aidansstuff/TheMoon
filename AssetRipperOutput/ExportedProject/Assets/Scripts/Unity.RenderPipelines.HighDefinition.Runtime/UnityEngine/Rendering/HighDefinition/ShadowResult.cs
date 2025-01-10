using UnityEngine.Experimental.Rendering.RenderGraphModule;

namespace UnityEngine.Rendering.HighDefinition
{
	internal struct ShadowResult
	{
		public TextureHandle punctualShadowResult;

		public TextureHandle cachedPunctualShadowResult;

		public TextureHandle directionalShadowResult;

		public TextureHandle areaShadowResult;

		public TextureHandle cachedAreaShadowResult;
	}
}
