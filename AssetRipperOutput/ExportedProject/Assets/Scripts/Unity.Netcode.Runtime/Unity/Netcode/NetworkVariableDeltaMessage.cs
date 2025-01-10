using System;
using System.Collections.Generic;
using Unity.Collections;

namespace Unity.Netcode
{
	internal struct NetworkVariableDeltaMessage : INetworkMessage
	{
		public ulong NetworkObjectId;

		public ushort NetworkBehaviourIndex;

		public HashSet<int> DeliveryMappedNetworkVariableIndex;

		public ulong TargetClientId;

		public NetworkBehaviour NetworkBehaviour;

		private FastBufferReader m_ReceivedNetworkVariableData;

		public int Version => 0;

		public void Serialize(FastBufferWriter writer, int targetVersion)
		{
			if (!writer.TryBeginWrite(FastBufferWriter.GetWriteSize(in NetworkObjectId, default(FastBufferWriter.ForStructs)) + FastBufferWriter.GetWriteSize(in NetworkBehaviourIndex, default(FastBufferWriter.ForStructs))))
			{
				throw new OverflowException("Not enough space in the buffer to write NetworkVariableDeltaMessage");
			}
			BytePacker.WriteValueBitPacked(writer, NetworkObjectId);
			BytePacker.WriteValueBitPacked(writer, NetworkBehaviourIndex);
			for (int i = 0; i < NetworkBehaviour.NetworkVariableFields.Count; i++)
			{
				if (!DeliveryMappedNetworkVariableIndex.Contains(i))
				{
					if (NetworkBehaviour.NetworkManager.NetworkConfig.EnsureNetworkVariableLengthSafety)
					{
						BytePacker.WriteValueBitPacked(writer, (ushort)0);
						continue;
					}
					bool value = false;
					writer.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
					continue;
				}
				int length = writer.Length;
				NetworkVariableBase networkVariableBase = NetworkBehaviour.NetworkVariableFields[i];
				bool value2 = networkVariableBase.IsDirty() && networkVariableBase.CanClientRead(TargetClientId) && (NetworkBehaviour.NetworkManager.IsServer || networkVariableBase.CanClientWrite(NetworkBehaviour.NetworkManager.LocalClientId));
				if (networkVariableBase.WritePerm == NetworkVariableWritePermission.Owner && networkVariableBase.OwnerClientId() == TargetClientId)
				{
					value2 = false;
				}
				if (NetworkBehaviour.NetworkManager.SpawnManager.ObjectsToShowToClient.ContainsKey(TargetClientId) && NetworkBehaviour.NetworkManager.SpawnManager.ObjectsToShowToClient[TargetClientId].Contains(NetworkBehaviour.NetworkObject))
				{
					value2 = false;
				}
				if (NetworkBehaviour.NetworkManager.NetworkConfig.EnsureNetworkVariableLengthSafety)
				{
					if (!value2)
					{
						BytePacker.WriteValueBitPacked(writer, (ushort)0);
					}
				}
				else
				{
					writer.WriteValueSafe(in value2, default(FastBufferWriter.ForPrimitives));
				}
				if (!value2)
				{
					continue;
				}
				if (NetworkBehaviour.NetworkManager.NetworkConfig.EnsureNetworkVariableLengthSafety)
				{
					FastBufferWriter writer2 = new FastBufferWriter(NetworkBehaviour.NetworkManager.MessageManager.NonFragmentedMessageMaxSize, Allocator.Temp, NetworkBehaviour.NetworkManager.MessageManager.FragmentedMessageMaxSize);
					NetworkBehaviour.NetworkVariableFields[i].WriteDelta(writer2);
					BytePacker.WriteValueBitPacked(writer, writer2.Length);
					if (!writer.TryBeginWrite(writer2.Length))
					{
						throw new OverflowException("Not enough space in the buffer to write NetworkVariableDeltaMessage");
					}
					writer2.CopyTo(writer);
				}
				else
				{
					networkVariableBase.WriteDelta(writer);
				}
				NetworkBehaviour.NetworkManager.NetworkMetrics.TrackNetworkVariableDeltaSent(TargetClientId, NetworkBehaviour.NetworkObject, networkVariableBase.Name, NetworkBehaviour.__getTypeName(), writer.Length - length);
			}
		}

		public bool Deserialize(FastBufferReader reader, ref NetworkContext context, int receivedMessageVersion)
		{
			ByteUnpacker.ReadValueBitPacked(reader, out NetworkObjectId);
			ByteUnpacker.ReadValueBitPacked(reader, out NetworkBehaviourIndex);
			m_ReceivedNetworkVariableData = reader;
			return true;
		}

		public void Handle(ref NetworkContext context)
		{
			NetworkManager networkManager = (NetworkManager)context.SystemOwner;
			if (networkManager.SpawnManager.SpawnedObjects.TryGetValue(NetworkObjectId, out var value))
			{
				NetworkBehaviour networkBehaviourAtOrderIndex = value.GetNetworkBehaviourAtOrderIndex(NetworkBehaviourIndex);
				if (networkBehaviourAtOrderIndex == null)
				{
					if (NetworkLog.CurrentLogLevel <= LogLevel.Normal)
					{
						NetworkLog.LogWarning(string.Format("Network variable delta message received for a non-existent behaviour. {0}: {1}, {2}: {3}", "NetworkObjectId", NetworkObjectId, "NetworkBehaviourIndex", NetworkBehaviourIndex));
					}
					return;
				}
				for (int i = 0; i < networkBehaviourAtOrderIndex.NetworkVariableFields.Count; i++)
				{
					int value2 = 0;
					if (networkManager.NetworkConfig.EnsureNetworkVariableLengthSafety)
					{
						ByteUnpacker.ReadValueBitPacked(m_ReceivedNetworkVariableData, out value2);
						if (value2 == 0)
						{
							continue;
						}
					}
					else
					{
						m_ReceivedNetworkVariableData.ReadValueSafe(out bool value3, default(FastBufferWriter.ForPrimitives));
						if (!value3)
						{
							continue;
						}
					}
					NetworkVariableBase networkVariableBase = networkBehaviourAtOrderIndex.NetworkVariableFields[i];
					if (networkManager.IsServer && !networkVariableBase.CanClientWrite(context.SenderId))
					{
						if (networkManager.NetworkConfig.EnsureNetworkVariableLengthSafety)
						{
							if (NetworkLog.CurrentLogLevel <= LogLevel.Developer)
							{
								NetworkLog.LogWarning(string.Format("Client wrote to {0} without permission. => {1}: {2} - {3}(): {4} - VariableIndex: {5}", typeof(NetworkVariable<>).Name, "NetworkObjectId", NetworkObjectId, "GetNetworkBehaviourOrderIndex", value.GetNetworkBehaviourOrderIndex(networkBehaviourAtOrderIndex), i));
								NetworkLog.LogError("[" + networkVariableBase.GetType().Name + "]");
							}
							m_ReceivedNetworkVariableData.Seek(m_ReceivedNetworkVariableData.Position + value2);
							continue;
						}
						if (NetworkLog.CurrentLogLevel <= LogLevel.Developer)
						{
							NetworkLog.LogError(string.Format("Client wrote to {0} without permission. No more variables can be read. This is critical. => {1}: {2} - {3}(): {4} - VariableIndex: {5}", typeof(NetworkVariable<>).Name, "NetworkObjectId", NetworkObjectId, "GetNetworkBehaviourOrderIndex", value.GetNetworkBehaviourOrderIndex(networkBehaviourAtOrderIndex), i));
							NetworkLog.LogError("[" + networkVariableBase.GetType().Name + "]");
						}
						break;
					}
					int position = m_ReceivedNetworkVariableData.Position;
					networkVariableBase.ReadDelta(m_ReceivedNetworkVariableData, networkManager.IsServer);
					networkManager.NetworkMetrics.TrackNetworkVariableDeltaReceived(context.SenderId, value, networkVariableBase.Name, networkBehaviourAtOrderIndex.__getTypeName(), context.MessageSize);
					if (!networkManager.NetworkConfig.EnsureNetworkVariableLengthSafety)
					{
						continue;
					}
					if (m_ReceivedNetworkVariableData.Position > position + value2)
					{
						if (NetworkLog.CurrentLogLevel <= LogLevel.Normal)
						{
							NetworkLog.LogWarning(string.Format("Var delta read too far. {0} bytes. => {1}: {2} - {3}(): {4} - VariableIndex: {5}", m_ReceivedNetworkVariableData.Position - (position + value2), "NetworkObjectId", NetworkObjectId, "GetNetworkBehaviourOrderIndex", value.GetNetworkBehaviourOrderIndex(networkBehaviourAtOrderIndex), i));
						}
						m_ReceivedNetworkVariableData.Seek(position + value2);
					}
					else if (m_ReceivedNetworkVariableData.Position < position + value2)
					{
						if (NetworkLog.CurrentLogLevel <= LogLevel.Normal)
						{
							NetworkLog.LogWarning(string.Format("Var delta read too little. {0} bytes. => {1}: {2} - {3}(): {4} - VariableIndex: {5}", position + value2 - m_ReceivedNetworkVariableData.Position, "NetworkObjectId", NetworkObjectId, "GetNetworkBehaviourOrderIndex", value.GetNetworkBehaviourOrderIndex(networkBehaviourAtOrderIndex), i));
						}
						m_ReceivedNetworkVariableData.Seek(position + value2);
					}
				}
			}
			else
			{
				networkManager.DeferredMessageManager.DeferMessage(IDeferredNetworkMessageManager.TriggerType.OnSpawn, NetworkObjectId, m_ReceivedNetworkVariableData, ref context);
			}
		}
	}
}
