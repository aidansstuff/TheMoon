using System;
using Unity.Collections;

namespace Unity.Multiplayer.Tools.MetricTypes
{
	[Serializable]
	internal struct NetworkMessageEvent : INetworkMetricEvent
	{
		public ConnectionInfo Connection { get; }

		public FixedString64Bytes Name { get; }

		public long BytesCount { get; }

		public NetworkMessageEvent(ConnectionInfo connection, string name, long bytesCount)
			: this(connection, StringConversionUtility.ConvertToFixedString(name), bytesCount)
		{
		}

		public NetworkMessageEvent(ConnectionInfo connection, FixedString64Bytes name, long bytesCount)
		{
			Connection = connection;
			Name = name;
			BytesCount = bytesCount;
		}
	}
}
