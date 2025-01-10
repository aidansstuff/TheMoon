namespace UnityEngine.Rendering.HighDefinition
{
	internal struct PunctualShadowProperties
	{
		public bool isSpot;

		public bool softShadow;

		public int lightIndex;

		public float lightRadius;

		public float lightConeAngle;

		public Vector3 lightPosition;

		public int kernelSize;

		public bool distanceBasedDenoiser;
	}
}
