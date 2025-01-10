using Unity.Collections;
using Unity.Mathematics;

namespace UnityEngine.Rendering.HighDefinition
{
	public struct WaterSimSearchData
	{
		[ReadOnly]
		public NativeArray<float4> displacementData;

		public float waterSurfaceElevation;

		public int simulationRes;

		public WaterSpectrumParameters spectrum;

		public WaterRenderingParameters rendering;

		public int activeBandCount;
	}
}
