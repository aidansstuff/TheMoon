namespace UnityEngine.Rendering.HighDefinition
{
	internal class HDShadowRequest
	{
		public Matrix4x4 view;

		public Matrix4x4 deviceProjectionYFlip;

		public Matrix4x4 deviceProjection;

		public Matrix4x4 projection;

		public BatchCullingProjectionType projectionType;

		public Matrix4x4 shadowToWorld;

		public Vector3 position;

		public Vector4 zBufferParam;

		public Rect dynamicAtlasViewport;

		public Rect cachedAtlasViewport;

		public bool zClip;

		public Vector4[] frustumPlanes;

		public int shadowIndex;

		public ShadowMapType shadowMapType = ShadowMapType.PunctualAtlas;

		public int lightIndex;

		public ShadowSplitData splitData;

		public float normalBias;

		public float worldTexelSize;

		public float slopeBias;

		public float shadowSoftness;

		public int blockerSampleCount;

		public int filterSampleCount;

		public float minFilterSize;

		public float kernelSize;

		public float lightAngle;

		public float maxDepthBias;

		public Vector4 evsmParams;

		public bool shouldUseCachedShadowData;

		public bool shouldRenderCachedComponent;

		public HDShadowData cachedShadowData;

		public bool isInCachedAtlas;

		public bool isMixedCached;
	}
}
