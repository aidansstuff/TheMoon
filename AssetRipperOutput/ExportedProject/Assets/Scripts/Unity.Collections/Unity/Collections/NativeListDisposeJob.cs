using Unity.Burst;
using Unity.Jobs;

namespace Unity.Collections
{
	[BurstCompile]
	[BurstCompatible]
	internal struct NativeListDisposeJob : IJob
	{
		internal NativeListDispose Data;

		public void Execute()
		{
			Data.Dispose();
		}
	}
}
