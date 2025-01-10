using System;

namespace Unity.Multiplayer.Tools.MetricTypes
{
	[Serializable]
	internal struct OwnershipChangeEvent : INetworkMetricEvent, INetworkObjectEvent
	{
		public ConnectionInfo Connection { get; }

		public NetworkObjectIdentifier NetworkId { get; }

		public long BytesCount { get; }

		public OwnershipChangeEvent(ConnectionInfo connection, NetworkObjectIdentifier networkId, long bytesCount)
		{
			Connection = connection;
			NetworkId = networkId;
			BytesCount = bytesCount;
		}
	}
}
