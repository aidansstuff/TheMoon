using Unity.Profiling;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Runtime
{
	internal class CounterWrapper : ICounter
	{
		private ProfilerCounter<long> m_Counter;

		public CounterWrapper(ProfilerCounter<long> counter)
		{
			m_Counter = counter;
		}

		public void Sample(long inValue)
		{
		}
	}
}
