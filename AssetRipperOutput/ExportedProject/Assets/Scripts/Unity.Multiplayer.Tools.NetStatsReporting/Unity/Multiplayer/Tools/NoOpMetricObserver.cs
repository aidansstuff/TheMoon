using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools
{
	internal class NoOpMetricObserver : IMetricObserver
	{
		public static NoOpMetricObserver Instance { get; } = new NoOpMetricObserver();


		private NoOpMetricObserver()
		{
		}

		public void Observe(MetricCollection collection)
		{
		}
	}
}
