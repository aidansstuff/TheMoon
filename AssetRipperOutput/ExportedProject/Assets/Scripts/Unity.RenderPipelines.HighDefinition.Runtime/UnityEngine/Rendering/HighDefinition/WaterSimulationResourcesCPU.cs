using Unity.Collections;
using Unity.Mathematics;

namespace UnityEngine.Rendering.HighDefinition
{
	internal class WaterSimulationResourcesCPU
	{
		public NativeArray<float2> h0BufferCPU;

		public NativeArray<float4> displacementBufferCPU;
	}
}
