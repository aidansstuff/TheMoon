using Unity.Profiling;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Runtime
{
	internal class ByteCounterFactory : ICounterFactory
	{
		public ICounter Construct(string name)
		{
			return new CounterWrapper(new ProfilerCounter<long>(ProfilerCategory.Network, name, ProfilerMarkerDataUnit.Bytes));
		}
	}
}
