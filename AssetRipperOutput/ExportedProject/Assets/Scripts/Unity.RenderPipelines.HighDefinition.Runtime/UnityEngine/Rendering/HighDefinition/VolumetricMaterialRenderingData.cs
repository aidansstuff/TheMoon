namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Lighting\\VolumetricLighting\\HDRenderPipeline.VolumetricLighting.cs", needAccessors = false)]
	internal struct VolumetricMaterialRenderingData
	{
		public Vector4 viewSpaceBounds;

		public uint startSliceIndex;

		public uint sliceCount;

		public uint padding0;

		public uint padding1;

		[HLSLArray(8, typeof(Vector4))]
		public unsafe fixed float obbVertexPositionWS[32];
	}
}
