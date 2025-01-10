using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.Rendering.HighDefinition
{
	[BurstCompile]
	public struct WaterSimulationSearchJob : IJobParallelFor
	{
		public WaterSimSearchData simSearchData;

		[ReadOnly]
		[NativeDisableParallelForRestriction]
		public NativeArray<float3> targetPositionBuffer;

		[ReadOnly]
		[NativeDisableParallelForRestriction]
		public NativeArray<float3> startPositionBuffer;

		public float error;

		public int maxIterations;

		[WriteOnly]
		[NativeDisableParallelForRestriction]
		public NativeArray<float> heightBuffer;

		[WriteOnly]
		[NativeDisableParallelForRestriction]
		public NativeArray<float> errorBuffer;

		[WriteOnly]
		[NativeDisableParallelForRestriction]
		public NativeArray<float3> candidateLocationBuffer;

		[WriteOnly]
		[NativeDisableParallelForRestriction]
		public NativeArray<int> stepCountBuffer;

		public void Execute(int index)
		{
			WaterSearchParameters wsp = default(WaterSearchParameters);
			wsp.targetPosition = targetPositionBuffer[index];
			wsp.startPosition = startPositionBuffer[index];
			wsp.error = error;
			wsp.maxIterations = maxIterations;
			WaterSearchResult sr = default(WaterSearchResult);
			HDRenderPipeline.FindWaterSurfaceHeight(simSearchData, wsp, out sr);
			heightBuffer[index] = sr.height;
			errorBuffer[index] = sr.error;
			candidateLocationBuffer[index] = sr.candidateLocation;
			stepCountBuffer[index] = sr.numIterations;
		}
	}
}
