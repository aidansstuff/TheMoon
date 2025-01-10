using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class WaterCPUSimulation
	{
		[BurstCompile]
		internal struct PhillipsSpectrumInitialization : IJobParallelFor
		{
			public int simulationResolution;

			public int waterSampleOffset;

			public int sliceIndex;

			public int bufferOffset;

			public float windSpeed;

			public float windOrientation;

			public float directionDampner;

			public float patchSize;

			[WriteOnly]
			[NativeDisableParallelForRestriction]
			public NativeArray<float2> H0Buffer;

			private float Phillips(float2 k, float2 w, float V, float directionDampener, float patchSize)
			{
				float num = k.x * k.x + k.y * k.y;
				float num2 = 0f;
				if ((double)num != 0.0)
				{
					float num3 = V * V / 9.81f;
					float num4 = Mathf.Lerp(Vector2.Dot(k / Mathf.Sqrt(num), w), 0.5f, directionDampner);
					num2 = Mathf.Exp(-1f / (num * num3 * num3)) / (num * num) * (num4 * num4) * ((num4 < 0f) ? directionDampner : 1f);
				}
				return 0.2f * num2 / (patchSize * patchSize);
			}

			public void Execute(int index)
			{
				int num = index % simulationResolution;
				int num2 = index / simulationResolution;
				uint3 @uint = new uint3((uint)num, (uint)num2, (uint)sliceIndex);
				float4 @float = WaterHashFunctionFloat4(new uint3((uint)(num + waterSampleOffset), (uint)(num2 + waterSampleOffset), (uint)sliceIndex) + 64u);
				float2 float2 = 0.70710677f * new float2(GaussianDis(@float.x, @float.y), GaussianDis(@float.z, @float.w));
				float2 k = MathF.PI * 2f * ((float2)@uint.xy - (float)simulationResolution * 0.5f) / patchSize;
				float2 w = -HDRenderPipeline.OrientationToDirection(windOrientation);
				float f = Phillips(k, w, windSpeed, directionDampner, patchSize);
				H0Buffer[index + bufferOffset] = float2 * Mathf.Sqrt(f);
			}
		}

		[BurstCompile]
		internal struct EvaluateDispersion : IJobParallelFor
		{
			public int simulationResolution;

			public int bufferOffset;

			public float patchSize;

			public float simulationTime;

			[ReadOnly]
			public NativeArray<float2> H0Buffer;

			[WriteOnly]
			public NativeArray<float4> HtRealBuffer;

			[WriteOnly]
			public NativeArray<float4> HtImaginaryBuffer;

			public void Execute(int index)
			{
				int num = index % simulationResolution;
				int num2 = index / simulationResolution;
				float2 @float = MathF.PI * 2f * (new float2(num, num2) - (float)simulationResolution * 0.5f) / patchSize;
				float num3 = Mathf.Sqrt(@float.x * @float.x + @float.y * @float.y);
				float num4 = Mathf.Sqrt(9.81f * num3);
				float2 b = new float2(@float.x / num3, 0f);
				float2 b2 = new float2(@float.y / num3, 0f);
				float2 b3 = ComplexMult(H0Buffer[index + bufferOffset], ComplexExp(num4 * simulationTime));
				float2 float2 = ComplexMult(ComplexMult(new float2(0f, -1f), b), b3);
				float2 float3 = ComplexMult(ComplexMult(new float2(0f, -1f), b2), b3);
				if (float.IsNaN(float2.x))
				{
					float2.x = 0f;
				}
				if (float.IsNaN(float2.y))
				{
					float2.y = 0f;
				}
				if (float.IsNaN(float3.x))
				{
					float3.x = 0f;
				}
				if (float.IsNaN(float3.y))
				{
					float3.y = 0f;
				}
				int num5 = simulationResolution / 2;
				if (num == num5 && num2 == num5)
				{
					float2 = new float2(0f, 0f);
					float3 = new float2(0f, 0f);
				}
				HtRealBuffer[index] = new float4(b3.x, float2.x, float3.x, 0f);
				HtImaginaryBuffer[index] = new float4(b3.y, float2.y, float3.y, 0f);
			}
		}

		[BurstCompile]
		internal struct InverseFFT : IJobParallelFor
		{
			public int simulationResolution;

			public int butterflyCount;

			public int bufferOffset;

			public bool columnPass;

			[ReadOnly]
			public NativeArray<float4> HtRealBufferInput;

			[ReadOnly]
			public NativeArray<float4> HtImaginaryBufferInput;

			[NativeDisableParallelForRestriction]
			public NativeArray<float3> pingPongArray;

			[NativeDisableParallelForRestriction]
			public NativeArray<uint4> textureIndicesArray;

			[WriteOnly]
			[NativeDisableParallelForRestriction]
			public NativeArray<float4> HtRealBufferOutput;

			[WriteOnly]
			[NativeDisableParallelForRestriction]
			public NativeArray<float4> HtImaginaryBufferOutput;

			private uint2 reversebits_uint2(uint2 input)
			{
				uint2 @uint = input;
				@uint = ((@uint & 2863311530u) >> 1) | ((@uint & 1431655765u) << 1);
				@uint = ((@uint & 3435973836u) >> 2) | ((@uint & 858993459u) << 2);
				@uint = ((@uint & 4042322160u) >> 4) | ((@uint & 252645135u) << 4);
				@uint = ((@uint & 4278255360u) >> 8) | ((@uint & 16711935u) << 8);
				return (@uint >> 16) | (@uint << 16);
			}

			private void GetButterflyValues(uint passIndex, uint x, out uint2 indices, out float2 weights)
			{
				uint num = (uint)(2 << (int)passIndex);
				uint num2 = num / 2;
				uint num3 = x & ~(num - 1);
				uint num4 = x & (num2 - 1);
				uint num5 = x & (num - 1);
				float f = MathF.PI * 2f * (float)num5 / (float)num;
				weights.y = 0f - Mathf.Sin(f);
				weights.x = Mathf.Cos(f);
				indices.x = num3 + num4;
				indices.y = num3 + num4 + num2;
				if (passIndex == 0)
				{
					uint2 @uint = reversebits_uint2(indices.xy);
					indices = new uint2((@uint.x >> 32 - butterflyCount) & (uint)(simulationResolution - 1), (@uint.y >> 32 - butterflyCount) & (uint)(simulationResolution - 1));
				}
			}

			private void ButterflyPass(uint passIndex, uint x, uint t0, uint t1, int ppOffset, out float3 resultR, out float3 resultI)
			{
				GetButterflyValues(passIndex, x, out var indices, out var weights);
				float3 @float = pingPongArray[ppOffset + (int)t0 * simulationResolution + (int)indices.x];
				float3 float2 = pingPongArray[ppOffset + (int)t1 * simulationResolution + (int)indices.x];
				float3 float3 = pingPongArray[ppOffset + (int)t0 * simulationResolution + (int)indices.y];
				float3 float4 = pingPongArray[ppOffset + (int)t1 * simulationResolution + (int)indices.y];
				resultR = @float + weights.x * float3 + weights.y * float4;
				resultI = float2 - weights.y * float3 + weights.x * float4;
			}

			public void Execute(int index)
			{
				int num = 4 * simulationResolution * index;
				for (int i = 0; i < simulationResolution; i++)
				{
					uint2 @uint = ((!columnPass) ? new uint2((uint)i, (uint)index) : new uint2((uint)index, (uint)i));
					uint index2 = @uint.x + (uint)((int)@uint.y * simulationResolution);
					pingPongArray[num + 0 + i] = HtRealBufferInput[(int)index2].xyz;
					pingPongArray[num + simulationResolution + i] = HtImaginaryBufferInput[(int)index2].xyz;
				}
				for (int j = 0; j < simulationResolution; j++)
				{
					textureIndicesArray[index * simulationResolution + j] = new uint4(0u, 1u, 2u, 3u);
				}
				for (int k = 0; k < butterflyCount - 1; k++)
				{
					for (int l = 0; l < simulationResolution; l++)
					{
						int2 @int = new int2(l, index);
						uint4 value = textureIndicesArray[index * simulationResolution + l];
						ButterflyPass((uint)k, (uint)l, value.x, value.y, num, out var resultR, out var resultI);
						pingPongArray[num + (int)value.z * simulationResolution + @int.x] = resultR;
						pingPongArray[num + (int)value.w * simulationResolution + @int.x] = resultI;
						value.xyzw = value.zwxy;
						textureIndicesArray[index * simulationResolution + @int.x] = value;
					}
				}
				for (int m = 0; m < simulationResolution; m++)
				{
					uint2 uint2 = ((!columnPass) ? new uint2((uint)m, (uint)index) : new uint2((uint)index, (uint)m));
					uint num2 = uint2.x + (uint)((int)uint2.y * simulationResolution);
					uint4 uint3 = textureIndicesArray[index * simulationResolution + m];
					ButterflyPass((uint)(butterflyCount - 1), (uint)m, uint3.x, uint3.y, num, out var resultR2, out var resultI2);
					if (columnPass)
					{
						float num3 = ((((uint)(m + index) & (true ? 1u : 0u)) != 0) ? (-1f) : 1f);
						HtRealBufferOutput[(int)num2 + bufferOffset] = new float4(resultR2 * num3, 0f);
					}
					else
					{
						HtRealBufferOutput[(int)num2] = new float4(resultR2, 0f);
						HtImaginaryBufferOutput[(int)num2] = new float4(resultI2, 0f);
					}
				}
			}
		}

		internal const float k_EarthGravity = 9.81f;

		internal const float k_OneOverSqrt2 = 0.70710677f;

		internal const float k_PhillipsAmplitudeScalar = 0.2f;

		internal const int k_NoiseFunctionOffset = 64;

		internal static uint4 WaterHashFunctionUInt4(uint3 coord)
		{
			uint4 xyzz = coord.xyzz;
			xyzz = ((xyzz >> 16) ^ xyzz.yzxy) * 73244475u;
			xyzz = ((xyzz >> 16) ^ xyzz.yzxz) * 73244475u;
			return ((xyzz >> 16) ^ xyzz.yzxx) * 73244475u;
		}

		internal static float4 WaterHashFunctionFloat4(uint3 p)
		{
			uint4 @uint = WaterHashFunctionUInt4(p);
			return new float4(@uint.x, @uint.y, @uint.z, @uint.w) / 4.2949673E+09f;
		}

		internal static float GaussianDis(float u, float v)
		{
			return Mathf.Sqrt(-2f * Mathf.Log(Mathf.Max(u, 1E-06f))) * Mathf.Cos(MathF.PI * v);
		}

		internal static float2 ComplexExp(float arg)
		{
			return new float2(Mathf.Cos(arg), Mathf.Sin(arg));
		}

		internal static float2 ComplexMult(float2 a, float2 b)
		{
			return new float2(a.x * b.x - a.y * b.y, a.x * b.y + a.y * b.x);
		}
	}
}
