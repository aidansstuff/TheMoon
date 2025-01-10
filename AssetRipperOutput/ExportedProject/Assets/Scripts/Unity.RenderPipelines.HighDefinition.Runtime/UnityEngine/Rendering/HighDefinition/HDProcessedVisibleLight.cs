namespace UnityEngine.Rendering.HighDefinition
{
	internal struct HDProcessedVisibleLight
	{
		public int dataIndex;

		public GPULightType gpuLightType;

		public HDLightType lightType;

		public float lightDistanceFade;

		public float lightVolumetricDistanceFade;

		public float distanceToCamera;

		public HDProcessedVisibleLightsBuilder.ShadowMapFlags shadowMapFlags;

		public bool isBakedShadowMask;
	}
}
