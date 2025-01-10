namespace UnityEngine.Rendering.HighDefinition
{
	internal struct ReflectionSystemParameters
	{
		public static ReflectionSystemParameters Default = new ReflectionSystemParameters
		{
			maxPlanarReflectionProbePerCamera = 128,
			maxActivePlanarReflectionProbe = 512,
			planarReflectionProbeSize = 128,
			maxActiveEnvReflectionProbe = 512
		};

		public int maxPlanarReflectionProbePerCamera;

		public int maxActivePlanarReflectionProbe;

		public int planarReflectionProbeSize;

		public int maxActiveEnvReflectionProbe;
	}
}
