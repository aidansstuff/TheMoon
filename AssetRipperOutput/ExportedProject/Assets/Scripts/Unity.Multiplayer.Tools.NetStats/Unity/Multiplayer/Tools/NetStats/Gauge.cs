using System;

namespace Unity.Multiplayer.Tools.NetStats
{
	[Serializable]
	internal class Gauge : Metric<double>
	{
		public override MetricContainerType MetricContainerType => MetricContainerType.Gauge;

		public Gauge(MetricId metricId, double defaultValue = 0.0)
			: base(metricId, defaultValue)
		{
		}

		public void Set(double value)
		{
			base.Value = value;
		}
	}
}
