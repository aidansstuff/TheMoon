using System;

namespace Unity.Multiplayer.Tools.MetricTypes
{
	[Serializable]
	internal struct ServerLogEvent : INetworkMetricEvent
	{
		public ConnectionInfo Connection { get; }

		public LogLevel LogLevel { get; }

		public long BytesCount { get; }

		public ServerLogEvent(ConnectionInfo connection, LogLevel logLevel, long bytesCount)
		{
			Connection = connection;
			LogLevel = logLevel;
			BytesCount = bytesCount;
		}
	}
}
