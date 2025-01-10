namespace Unity.Multiplayer.Tools.MetricTypes
{
	internal interface INetworkMetricEvent
	{
		ConnectionInfo Connection { get; }

		long BytesCount { get; }
	}
}
