namespace UnityEngine.Rendering.HighDefinition
{
	public struct HDEffectsParameters
	{
		public bool shadows;

		public bool ambientOcclusion;

		public int aoLayerMask;

		public bool reflections;

		public int reflLayerMask;

		public bool globalIllumination;

		public int giLayerMask;

		public bool recursiveRendering;

		public int recursiveLayerMask;

		public bool subSurface;

		public bool pathTracing;

		public int ptLayerMask;

		public bool rayTracingRequired;
	}
}
