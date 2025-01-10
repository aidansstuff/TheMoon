namespace Unity.Multiplayer.Tools.NetStats
{
	internal class CounterFactory : IMetricFactory
	{
		public bool TryConstruct(MetricHeader header, out IMetric metric)
		{
			metric = new Counter(header.MetricId, 0L);
			return true;
		}
	}
}
