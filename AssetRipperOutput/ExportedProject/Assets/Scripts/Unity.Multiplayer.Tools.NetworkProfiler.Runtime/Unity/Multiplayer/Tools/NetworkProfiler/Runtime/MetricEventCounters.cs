using System.Collections.Generic;

namespace Unity.Multiplayer.Tools.NetworkProfiler.Runtime
{
	internal class MetricEventCounters
	{
		private readonly ICounter m_SentCounter;

		private readonly ICounter m_ReceivedCounter;

		public string Sent { get; }

		public string Received { get; }

		public MetricEventCounters(string displayName, ICounterFactory counterFactory)
		{
			Sent = displayName + " Sent";
			Received = displayName + " Received";
			m_SentCounter = counterFactory.Construct(Sent);
			m_ReceivedCounter = counterFactory.Construct(Received);
		}

		public void Sample<TEventData>(IReadOnlyCollection<TEventData> sent, IReadOnlyCollection<TEventData> received)
		{
			m_SentCounter.Sample(sent.Count);
			m_ReceivedCounter.Sample(received.Count);
		}
	}
}
