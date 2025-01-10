using System.Diagnostics;
using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Runtime
{
	internal class ExtensibilityProfilerMetricObserver : IMetricObserver
	{
		private readonly INetStatSerializer m_NetStatSerializer;

		public ExtensibilityProfilerMetricObserver()
		{
			m_NetStatSerializer = new NetStatSerializer();
		}

		public void Observe(MetricCollection collection)
		{
		}

		[Conditional("ENABLE_PROFILER")]
		private void PopulateProfilerIfEnabled(MetricCollection collection)
		{
			ProfilerCounters.Instance.UpdateFromMetrics(collection);
			using (m_NetStatSerializer.Serialize(collection))
			{
			}
		}
	}
}
