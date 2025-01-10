using Unity.Mathematics;

namespace UnityEngine.Rendering.HighDefinition
{
	public struct WaterSearchParameters
	{
		public float3 targetPosition;

		public float3 startPosition;

		public float error;

		public int maxIterations;
	}
}
