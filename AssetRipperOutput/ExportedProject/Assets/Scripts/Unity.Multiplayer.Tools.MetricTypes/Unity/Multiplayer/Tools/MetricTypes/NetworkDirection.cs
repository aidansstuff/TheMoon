using System;

namespace Unity.Multiplayer.Tools.MetricTypes
{
	[Flags]
	internal enum NetworkDirection
	{
		None = 0,
		Received = 1,
		Sent = 2,
		SentAndReceived = 3
	}
}
