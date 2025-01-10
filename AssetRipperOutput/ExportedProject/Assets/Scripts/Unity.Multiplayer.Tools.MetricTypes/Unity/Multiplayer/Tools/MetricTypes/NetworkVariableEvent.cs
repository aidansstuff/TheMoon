using System;
using Unity.Collections;

namespace Unity.Multiplayer.Tools.MetricTypes
{
	[Serializable]
	internal struct NetworkVariableEvent : INetworkMetricEvent, INetworkObjectEvent
	{
		public ConnectionInfo Connection { get; }

		public NetworkObjectIdentifier NetworkId { get; }

		public FixedString64Bytes Name { get; }

		public FixedString64Bytes NetworkBehaviourName { get; }

		public long BytesCount { get; }

		public NetworkVariableEvent(ConnectionInfo connection, NetworkObjectIdentifier networkId, string name, string networkBehaviourName, long bytesCount)
			: this(connection, networkId, StringConversionUtility.ConvertToFixedString(name), StringConversionUtility.ConvertToFixedString(networkBehaviourName), bytesCount)
		{
		}

		public NetworkVariableEvent(ConnectionInfo connection, NetworkObjectIdentifier networkId, FixedString64Bytes name, FixedString64Bytes networkBehaviourName, long bytesCount)
		{
			Connection = connection;
			NetworkId = networkId;
			Name = name;
			NetworkBehaviourName = networkBehaviourName;
			BytesCount = bytesCount;
		}
	}
}
