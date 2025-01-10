using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools
{
	internal static class MetricObserverFactory
	{
		internal static IMetricObserver Construct()
		{
			return new MetricObserver();
		}
	}
}
