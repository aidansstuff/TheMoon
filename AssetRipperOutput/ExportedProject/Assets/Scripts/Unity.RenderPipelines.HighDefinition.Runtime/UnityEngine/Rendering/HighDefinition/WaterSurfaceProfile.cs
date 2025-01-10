namespace UnityEngine.Rendering.HighDefinition
{
	[GenerateHLSL(PackingRules.Exact, false, false, false, 1, false, false, false, -1, ".\\Library\\PackageCache\\com.unity.render-pipelines.high-definition@14.0.8\\Runtime\\Water\\WaterSystemDef.cs")]
	internal struct WaterSurfaceProfile
	{
		public Vector3 waterAmbientProbe;

		public float tipScatteringHeight;

		public float bodyScatteringHeight;

		public float maxRefractionDistance;

		public uint lightLayers;

		public int cameraUnderWater;

		public Vector3 transparencyColor;

		public float outScatteringCoefficient;

		public Vector3 scatteringColor;

		public float envPerceptualRoughness;

		public float smoothnessFadeStart;

		public float smoothnessFadeDistance;

		public float roughnessEndValue;

		public float padding;
	}
}
