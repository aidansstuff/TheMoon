namespace Unity.Services.Qos
{
	public interface IQosResult
	{
		string Region { get; }

		int AverageLatencyMs { get; }

		float PacketLossPercent { get; }
	}
}
