using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools.MetricTypes
{
	[MetricTypeEnum(DisplayName = "Built-In Metrics")]
	[MetricTypeSortPriority(SortPriority = SortPriority.VeryHigh)]
	public enum DirectedMetricType
	{
		[MetricMetadata(Units = Units.Bytes)]
		TotalBytesSent = 6,
		[MetricMetadata(Units = Units.Bytes)]
		TotalBytesReceived = 5,
		[MetricMetadata(DisplayName = "RPCs Sent")]
		RpcSent = 10,
		[MetricMetadata(DisplayName = "RPCs Received")]
		RpcReceived = 9,
		[MetricMetadata(DisplayName = "Named Messages Sent")]
		NamedMessageSent = 14,
		[MetricMetadata(DisplayName = "Named Messages Received")]
		NamedMessageReceived = 13,
		[MetricMetadata(DisplayName = "Unnamed Messages Sent")]
		UnnamedMessageSent = 18,
		[MetricMetadata(DisplayName = "Unnamed Messages Received")]
		UnnamedMessageReceived = 17,
		[MetricMetadata(DisplayName = "Network Variable Deltas Sent")]
		NetworkVariableDeltaSent = 22,
		[MetricMetadata(DisplayName = "Network Variable Deltas Received")]
		NetworkVariableDeltaReceived = 21,
		[MetricMetadata(DisplayName = "Objects Spawned Sent")]
		ObjectSpawnedSent = 26,
		[MetricMetadata(DisplayName = "Objects Spawned Received")]
		ObjectSpawnedReceived = 25,
		[MetricMetadata(DisplayName = "Objects Destroyed Sent")]
		ObjectDestroyedSent = 30,
		[MetricMetadata(DisplayName = "Objects Destroyed Received")]
		ObjectDestroyedReceived = 29,
		[MetricMetadata(DisplayName = "Ownership Changes Sent")]
		OwnershipChangeSent = 34,
		[MetricMetadata(DisplayName = "Ownership Changes Received")]
		OwnershipChangeReceived = 33,
		[MetricMetadata(DisplayName = "Server Logs Sent")]
		ServerLogSent = 38,
		[MetricMetadata(DisplayName = "Server Logs Received")]
		ServerLogReceived = 37,
		[MetricMetadata(DisplayName = "Scene Events Sent")]
		SceneEventSent = 42,
		[MetricMetadata(DisplayName = "Scene Events Received")]
		SceneEventReceived = 41,
		[MetricMetadata(DisplayName = "Network Messages Sent")]
		NetworkMessageSent = 46,
		[MetricMetadata(DisplayName = "Network Messages Received")]
		NetworkMessageReceived = 45,
		PacketsSent = 50,
		PacketsReceived = 49,
		[MetricMetadata(DisplayName = "RTT To Server", MetricKind = MetricKind.Gauge, Units = Units.Seconds)]
		RttToServer = 55,
		[MetricMetadata(MetricKind = MetricKind.Gauge)]
		NetworkObjects = 59,
		[MetricMetadata(MetricKind = MetricKind.Gauge)]
		Connections = 63,
		[MetricMetadata(MetricKind = MetricKind.Gauge, DisplayAsPercentage = true)]
		PacketLoss = 65
	}
}
