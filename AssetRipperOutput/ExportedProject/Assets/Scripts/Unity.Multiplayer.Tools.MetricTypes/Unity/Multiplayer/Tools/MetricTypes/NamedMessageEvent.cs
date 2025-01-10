using System;
using Unity.Collections;

namespace Unity.Multiplayer.Tools.MetricTypes
{
	[Serializable]
	internal struct NamedMessageEvent : INetworkMetricEvent
	{
		public ConnectionInfo Connection { get; }

		public FixedString64Bytes Name { get; }

		public long BytesCount { get; }

		public NamedMessageEvent(ConnectionInfo connection, string name, long bytesCount)
			: this(connection, StringConversionUtility.ConvertToFixedString(name), bytesCount)
		{
		}

		public NamedMessageEvent(ConnectionInfo connection, FixedString64Bytes name, long bytesCount)
		{
			Connection = connection;
			Name = name;
			BytesCount = bytesCount;
		}
	}
}
