using Unity.Multiplayer.Tools;
using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Netcode
{
	internal class NetcodeObserver
	{
		public static IMetricObserver Observer { get; } = MetricObserverFactory.Construct();

	}
}
