using Unity.Multiplayer.Tools.MetricTypes;
using Unity.Multiplayer.Tools.NetStats;

public class _003CNetStats_TypeRegistration_003E
{
	static void Run()
	{
		EventMetricFactory.RegisterType<NetworkMessageEvent>();
		EventMetricFactory.RegisterType<NamedMessageEvent>();
		EventMetricFactory.RegisterType<UnnamedMessageEvent>();
		EventMetricFactory.RegisterType<NetworkVariableEvent>();
		EventMetricFactory.RegisterType<OwnershipChangeEvent>();
		EventMetricFactory.RegisterType<ObjectSpawnedEvent>();
		EventMetricFactory.RegisterType<ObjectDestroyedEvent>();
		EventMetricFactory.RegisterType<RpcEvent>();
		EventMetricFactory.RegisterType<ServerLogEvent>();
		EventMetricFactory.RegisterType<SceneEventMetric>();
	}
}
