using System.Collections.Generic;

namespace Unity.Services.Qos
{
	internal class QosResult : IQosAnnotatedResult, IQosResult
	{
		public string Region { get; }

		public int AverageLatencyMs { get; }

		public float PacketLossPercent { get; }

		public Dictionary<string, List<string>> Annotations { get; }

		public QosResult(string region, int averageLatencyMs, float packetLossPercent, Dictionary<string, List<string>> annotations = null)
		{
			Region = region;
			AverageLatencyMs = averageLatencyMs;
			PacketLossPercent = packetLossPercent;
			Annotations = annotations ?? new Dictionary<string, List<string>>();
		}
	}
}
