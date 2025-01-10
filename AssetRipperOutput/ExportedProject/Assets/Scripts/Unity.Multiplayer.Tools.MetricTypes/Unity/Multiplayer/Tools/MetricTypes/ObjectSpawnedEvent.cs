using System;

namespace Unity.Multiplayer.Tools.MetricTypes
{
	[Serializable]
	internal struct ObjectSpawnedEvent : INetworkMetricEvent, INetworkObjectEvent
	{
		public ConnectionInfo Connection { get; }

		public NetworkObjectIdentifier NetworkId { get; }

		public long BytesCount { get; }

		public ObjectSpawnedEvent(ConnectionInfo connection, NetworkObjectIdentifier networkId, long bytesCount)
		{
			Connection = connection;
			NetworkId = networkId;
			BytesCount = bytesCount;
		}
	}
}
