using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools
{
	internal static class RnsmMetricObserverFactory
	{
		public static IMetricObserver Construct()
		{
			return NoOpMetricObserver.Instance;
		}
	}
}
