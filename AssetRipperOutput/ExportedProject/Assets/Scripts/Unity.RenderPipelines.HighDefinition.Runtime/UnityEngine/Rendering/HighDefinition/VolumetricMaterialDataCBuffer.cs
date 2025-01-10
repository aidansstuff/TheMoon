namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Lighting\\VolumetricLighting\\HDRenderPipeline.VolumetricLighting.cs", needAccessors = false, generateCBuffer = true)]
	internal struct VolumetricMaterialDataCBuffer
	{
		public Vector4 _VolumetricMaterialObbRight;

		public Vector4 _VolumetricMaterialObbUp;

		public Vector4 _VolumetricMaterialObbExtents;

		public Vector4 _VolumetricMaterialObbCenter;

		public Vector4 _VolumetricMaterialAlbedo;

		public Vector4 _VolumetricMaterialRcpPosFaceFade;

		public Vector4 _VolumetricMaterialRcpNegFaceFade;

		public float _VolumetricMaterialInvertFade;

		public float _VolumetricMaterialExtinction;

		public float _VolumetricMaterialRcpDistFadeLen;

		public float _VolumetricMaterialEndTimesRcpDistFadeLen;

		public float _VolumetricMaterialFalloffMode;

		public float padding0;

		public float padding1;

		public float padding2;
	}
}
