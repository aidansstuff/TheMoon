using System.Collections.Generic;
using Unity.Multiplayer.Tools.MetricTypes;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Runtime
{
	internal class MetricByteCounters
	{
		private readonly ICounter m_SentCounter;

		private readonly ICounter m_ReceivedCounter;

		public string Sent { get; }

		public string Received { get; }

		public MetricByteCounters(string displayName, ICounterFactory counterFactory)
		{
			Sent = displayName + " Bytes Sent";
			Received = displayName + " Bytes Received";
			m_SentCounter = counterFactory.Construct(Sent);
			m_ReceivedCounter = counterFactory.Construct(Received);
		}

		public void Sample<TEventData>(IReadOnlyList<TEventData> sentMetrics, IReadOnlyList<TEventData> receivedMetrics) where TEventData : struct, INetworkMetricEvent
		{
			long num = 0L;
			for (int i = 0; i < sentMetrics.Count; i++)
			{
				num += sentMetrics[i].BytesCount;
			}
			long num2 = 0L;
			for (int j = 0; j < receivedMetrics.Count; j++)
			{
				num2 += receivedMetrics[j].BytesCount;
			}
			Sample(num, num2);
		}

		public void Sample(long sent, long received)
		{
			m_SentCounter.Sample(sent);
			m_ReceivedCounter.Sample(received);
		}
	}
}
