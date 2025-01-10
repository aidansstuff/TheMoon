namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, true, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Lighting\\LightLoop\\LightLoop.cs")]
	internal struct LightVolumeData
	{
		public Vector3 lightPos;

		public uint lightVolume;

		public Vector3 lightAxisX;

		public uint lightCategory;

		public Vector3 lightAxisY;

		public float radiusSq;

		public Vector3 lightAxisZ;

		public float cotan;

		public Vector3 boxInnerDist;

		public uint featureFlags;

		public Vector3 boxInvRange;

		public float unused2;
	}
}
