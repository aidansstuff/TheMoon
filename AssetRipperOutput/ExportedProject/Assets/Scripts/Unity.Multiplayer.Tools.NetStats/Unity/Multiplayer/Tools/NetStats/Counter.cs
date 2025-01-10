using System;

namespace Unity.Multiplayer.Tools.NetStats
{
	[Serializable]
	internal class Counter : Metric<long>
	{
		public override MetricContainerType MetricContainerType => MetricContainerType.Counter;

		public Counter(MetricId metricId, long defaultValue = 0L)
			: base(metricId, defaultValue)
		{
		}

		public void Increment(long increment = 1L)
		{
			base.Value += increment;
		}
	}
}
