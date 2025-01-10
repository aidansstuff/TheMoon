namespace Unity.Multiplayer.Tools.NetStats
{
	internal class GaugeFactory : IMetricFactory
	{
		public bool TryConstruct(MetricHeader header, out IMetric metric)
		{
			metric = new Gauge(header.MetricId);
			return true;
		}
	}
}
