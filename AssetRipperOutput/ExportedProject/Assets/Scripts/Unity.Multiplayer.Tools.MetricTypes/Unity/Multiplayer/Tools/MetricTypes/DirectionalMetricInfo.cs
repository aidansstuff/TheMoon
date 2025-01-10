using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools.MetricTypes
{
	internal struct DirectionalMetricInfo
	{
		internal DirectedMetricType DirectedMetricType { get; }

		internal MetricType Type => DirectedMetricType.GetMetric();

		internal NetworkDirection Direction => DirectedMetricType.GetDirection();

		internal MetricId Id => DirectedMetricType.GetId();

		internal string DisplayName => DirectedMetricType.GetDisplayName();

		public DirectionalMetricInfo(DirectedMetricType directedMetricType)
		{
			DirectedMetricType = directedMetricType;
		}

		public DirectionalMetricInfo(MetricType metricType, NetworkDirection networkDirection)
		{
			DirectedMetricType = metricType.GetDirectedMetric(networkDirection);
		}
	}
}
