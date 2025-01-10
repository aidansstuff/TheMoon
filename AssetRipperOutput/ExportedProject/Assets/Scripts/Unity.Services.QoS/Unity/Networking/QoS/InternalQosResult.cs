namespace Unity.Networking.QoS
{
	internal struct InternalQosResult
	{
		internal const uint InvalidLatencyValue = uint.MaxValue;

		internal const float InvalidPacketLossValue = float.MaxValue;

		internal uint RequestsSent;

		internal uint ResponsesReceived;

		internal uint InvalidRequests;

		internal uint InvalidResponses;

		internal uint DuplicateResponses;

		internal FcType FcType;

		internal byte FcUnits;

		internal uint AggregateLatencyMs;

		internal uint AverageLatencyMs
		{
			get
			{
				if (ResponsesReceived == 0)
				{
					return uint.MaxValue;
				}
				return AggregateLatencyMs / ResponsesReceived;
			}
		}

		internal float PacketLoss
		{
			get
			{
				if (RequestsSent != 0 && ResponsesReceived <= RequestsSent)
				{
					return 1f - (float)ResponsesReceived / (float)RequestsSent;
				}
				return float.MaxValue;
			}
		}

		internal void AddAggregateLatency(uint amountMs)
		{
			AggregateLatencyMs += amountMs;
		}
	}
}
