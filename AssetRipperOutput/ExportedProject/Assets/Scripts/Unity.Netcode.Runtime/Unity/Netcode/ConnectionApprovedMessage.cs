using System.Collections.Generic;
using Unity.Collections;

namespace Unity.Netcode
{
	internal struct ConnectionApprovedMessage : INetworkMessage
	{
		public ulong OwnerClientId;

		public int NetworkTick;

		public HashSet<NetworkObject> SpawnedObjectsList;

		private FastBufferReader m_ReceivedSceneObjectData;

		public NativeArray<MessageVersionData> MessageVersions;

		public int Version => 0;

		public void Serialize(FastBufferWriter writer, int targetVersion)
		{
			BytePacker.WriteValueBitPacked(writer, MessageVersions.Length);
			foreach (MessageVersionData messageVersion in MessageVersions)
			{
				messageVersion.Serialize(writer);
			}
			BytePacker.WriteValueBitPacked(writer, OwnerClientId);
			BytePacker.WriteValueBitPacked(writer, NetworkTick);
			uint value = 0u;
			if (SpawnedObjectsList != null)
			{
				int position = writer.Position;
				writer.Seek(writer.Position + FastBufferWriter.GetWriteSize(in value, default(FastBufferWriter.ForStructs)));
				foreach (NetworkObject spawnedObjects in SpawnedObjectsList)
				{
					if (spawnedObjects.CheckObjectVisibility == null || spawnedObjects.CheckObjectVisibility(OwnerClientId))
					{
						spawnedObjects.Observers.Add(OwnerClientId);
						spawnedObjects.GetMessageSceneObject(OwnerClientId).Serialize(writer);
						value++;
					}
				}
				writer.Seek(position);
				writer.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
				writer.Seek(writer.Length);
			}
			else
			{
				writer.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
			}
		}

		public bool Deserialize(FastBufferReader reader, ref NetworkContext context, int receivedMessageVersion)
		{
			NetworkManager networkManager = (NetworkManager)context.SystemOwner;
			if (!networkManager.IsClient)
			{
				return false;
			}
			ByteUnpacker.ReadValueBitPacked(reader, out int value);
			NativeArray<uint> serverMessageOrder = new NativeArray<uint>(value, Allocator.Temp);
			for (int i = 0; i < value; i++)
			{
				MessageVersionData messageVersionData = default(MessageVersionData);
				messageVersionData.Deserialize(reader);
				networkManager.ConnectionManager.MessageManager.SetVersion(context.SenderId, messageVersionData.Hash, messageVersionData.Version);
				serverMessageOrder[i] = messageVersionData.Hash;
				if (networkManager.ConnectionManager.MessageManager.GetMessageForHash(messageVersionData.Hash) == typeof(ConnectionApprovedMessage))
				{
					receivedMessageVersion = messageVersionData.Version;
				}
			}
			networkManager.ConnectionManager.MessageManager.SetServerMessageOrder(serverMessageOrder);
			serverMessageOrder.Dispose();
			ByteUnpacker.ReadValueBitPacked(reader, out OwnerClientId);
			ByteUnpacker.ReadValueBitPacked(reader, out NetworkTick);
			m_ReceivedSceneObjectData = reader;
			return true;
		}

		public void Handle(ref NetworkContext context)
		{
			NetworkManager networkManager = (NetworkManager)context.SystemOwner;
			networkManager.LocalClientId = OwnerClientId;
			networkManager.NetworkMetrics.SetConnectionId(networkManager.LocalClientId);
			NetworkTime networkTime = new NetworkTime(networkManager.NetworkTickSystem.TickRate, NetworkTick);
			networkManager.NetworkTimeSystem.Reset(networkTime.Time, 0.15000000596046448);
			networkManager.NetworkTickSystem.Reset(networkManager.NetworkTimeSystem.LocalTime, networkManager.NetworkTimeSystem.ServerTime);
			networkManager.ConnectionManager.LocalClient.SetRole(isServer: false, isClient: true, networkManager);
			networkManager.ConnectionManager.LocalClient.IsApproved = true;
			networkManager.ConnectionManager.LocalClient.ClientId = OwnerClientId;
			networkManager.ConnectionManager.StopClientApprovalCoroutine();
			if (!networkManager.NetworkConfig.EnableSceneManagement)
			{
				networkManager.SpawnManager.DestroySceneObjects();
				m_ReceivedSceneObjectData.ReadValueSafe(out uint value, default(FastBufferWriter.ForPrimitives));
				for (ushort num = 0; num < value; num++)
				{
					NetworkObject.SceneObject sceneObject = default(NetworkObject.SceneObject);
					sceneObject.Deserialize(m_ReceivedSceneObjectData);
					NetworkObject.AddSceneObject(in sceneObject, m_ReceivedSceneObjectData, networkManager);
				}
				networkManager.IsConnectedClient = true;
				networkManager.ConnectionManager.InvokeOnClientConnectedCallback(context.SenderId);
			}
		}
	}
}
