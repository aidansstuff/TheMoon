using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine.Rendering.HighDefinition;

namespace UnityEditor.Rendering.HighDefinition
{
	internal class WaterAmplitudeEvaluator
	{
		[BurstCompile]
		internal struct ReductionStep : IJobParallelFor
		{
			[ReadOnly]
			public NativeArray<float4> InputBuffer;

			[WriteOnly]
			[NativeDisableParallelForRestriction]
			public NativeArray<float4> OutputBuffer;

			public void Execute(int index)
			{
				float4 @float = 0f;
				for (int i = 0; i < 64; i++)
				{
					@float = math.max(@float, InputBuffer[i + index * 64]);
				}
				OutputBuffer[index] = @float;
			}
		}

		private const int k_NumIterations = 32;

		private const int k_NumTimeSteps = 512;

		private const WaterSimulationResolution resolutionEnum = WaterSimulationResolution.Low64;

		private const int resolution = 64;

		private const int numPixels = 4096;

		private static void EvaluateMaxAmplitude(NativeArray<float4> startBuffer, NativeArray<float4> intBuffer, NativeArray<float4> finBuffer)
		{
			ReductionStep jobData = default(ReductionStep);
			jobData.InputBuffer = startBuffer;
			jobData.OutputBuffer = intBuffer;
			IJobParallelForExtensions.Schedule(jobData, 64, 1).Complete();
			jobData = default(ReductionStep);
			jobData.InputBuffer = intBuffer;
			jobData.OutputBuffer = finBuffer;
			IJobParallelForExtensions.Schedule(jobData, 1, 1).Complete();
		}
	}
}
