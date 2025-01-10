using System;

namespace Unity.Multiplayer.Tools.MetricTypes
{
	[Serializable]
	internal struct UnnamedMessageEvent : INetworkMetricEvent
	{
		public ConnectionInfo Connection { get; }

		public long BytesCount { get; }

		public UnnamedMessageEvent(ConnectionInfo connection, long bytesCount)
		{
			Connection = connection;
			BytesCount = bytesCount;
		}
	}
}
