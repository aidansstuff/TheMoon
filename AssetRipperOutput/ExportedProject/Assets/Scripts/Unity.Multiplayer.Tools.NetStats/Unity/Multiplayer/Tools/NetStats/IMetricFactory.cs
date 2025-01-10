namespace Unity.Multiplayer.Tools.NetStats
{
	internal interface IMetricFactory
	{
		bool TryConstruct(MetricHeader header, out IMetric metric);
	}
}
