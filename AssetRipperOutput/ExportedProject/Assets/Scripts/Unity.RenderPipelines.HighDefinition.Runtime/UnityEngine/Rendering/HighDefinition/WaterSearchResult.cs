using Unity.Mathematics;

namespace UnityEngine.Rendering.HighDefinition
{
	public struct WaterSearchResult
	{
		public float height;

		public float error;

		public float3 candidateLocation;

		public int numIterations;
	}
}
