using System;

namespace Unity.Multiplayer.Tools.MetricTypes
{
	[Serializable]
	internal struct ObjectDestroyedEvent : INetworkMetricEvent, INetworkObjectEvent
	{
		public ConnectionInfo Connection { get; }

		public NetworkObjectIdentifier NetworkId { get; }

		public long BytesCount { get; }

		public ObjectDestroyedEvent(ConnectionInfo connection, NetworkObjectIdentifier networkId, long bytesCount)
		{
			Connection = connection;
			NetworkId = networkId;
			BytesCount = bytesCount;
		}
	}
}
