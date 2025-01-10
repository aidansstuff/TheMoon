namespace UnityEngine.Rendering.HighDefinition
{
	internal class HDShadowResolutionRequest
	{
		public Rect dynamicAtlasViewport;

		public Rect cachedAtlasViewport;

		public Vector2 resolution;

		public ShadowMapType shadowMapType;

		public HDShadowResolutionRequest ShallowCopy()
		{
			return (HDShadowResolutionRequest)MemberwiseClone();
		}
	}
}
