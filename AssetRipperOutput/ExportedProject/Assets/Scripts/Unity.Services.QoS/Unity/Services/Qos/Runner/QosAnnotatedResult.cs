using System.Collections.Generic;

namespace Unity.Services.Qos.Runner
{
	public struct QosAnnotatedResult
	{
		public string Region;

		public int AverageLatencyMs;

		public float PacketLossPercent;

		public Dictionary<string, List<string>> Annotations;
	}
}
