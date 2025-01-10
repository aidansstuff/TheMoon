using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Runtime
{
	internal static class ProfilerMetricObserverFactory
	{
		public static IMetricObserver Construct()
		{
			return new ExtensibilityProfilerMetricObserver();
		}
	}
}
