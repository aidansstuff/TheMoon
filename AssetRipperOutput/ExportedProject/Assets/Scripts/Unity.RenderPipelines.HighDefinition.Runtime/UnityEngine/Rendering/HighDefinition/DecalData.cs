namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, false, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Material\\Decal\\Decal.cs")]
	internal struct DecalData
	{
		public Matrix4x4 worldToDecal;

		public Matrix4x4 normalToWorld;

		public Vector4 diffuseScaleBias;

		public Vector4 normalScaleBias;

		public Vector4 maskScaleBias;

		public Vector4 baseColor;

		public Vector4 remappingAOS;

		public Vector4 scalingBAndRemappingM;

		public Vector3 blendParams;

		public uint decalLayerMask;
	}
}
