namespace Unity.Netcode
{
	internal struct ChangeOwnershipMessage : INetworkMessage, INetworkSerializeByMemcpy
	{
		public ulong NetworkObjectId;

		public ulong OwnerClientId;

		public int Version => 0;

		public void Serialize(FastBufferWriter writer, int targetVersion)
		{
			BytePacker.WriteValueBitPacked(writer, NetworkObjectId);
			BytePacker.WriteValueBitPacked(writer, OwnerClientId);
		}

		public bool Deserialize(FastBufferReader reader, ref NetworkContext context, int receivedMessageVersion)
		{
			NetworkManager networkManager = (NetworkManager)context.SystemOwner;
			if (!networkManager.IsClient)
			{
				return false;
			}
			ByteUnpacker.ReadValueBitPacked(reader, out NetworkObjectId);
			ByteUnpacker.ReadValueBitPacked(reader, out OwnerClientId);
			if (!networkManager.SpawnManager.SpawnedObjects.ContainsKey(NetworkObjectId))
			{
				networkManager.DeferredMessageManager.DeferMessage(IDeferredNetworkMessageManager.TriggerType.OnSpawn, NetworkObjectId, reader, ref context);
				return false;
			}
			return true;
		}

		public void Handle(ref NetworkContext context)
		{
			NetworkManager networkManager = (NetworkManager)context.SystemOwner;
			NetworkObject networkObject = networkManager.SpawnManager.SpawnedObjects[NetworkObjectId];
			ulong ownerClientId = networkObject.OwnerClientId;
			networkObject.OwnerClientId = OwnerClientId;
			if (ownerClientId == networkManager.LocalClientId)
			{
				networkObject.InvokeBehaviourOnLostOwnership();
			}
			if (OwnerClientId == networkManager.LocalClientId)
			{
				networkObject.InvokeBehaviourOnGainedOwnership();
			}
			if (OwnerClientId != networkManager.LocalClientId && ownerClientId != networkManager.LocalClientId)
			{
				for (int i = 0; i < networkObject.ChildNetworkBehaviours.Count; i++)
				{
					networkObject.ChildNetworkBehaviours[i].UpdateNetworkProperties();
				}
			}
			networkManager.NetworkMetrics.TrackOwnershipChangeReceived(context.SenderId, networkObject, context.MessageSize);
		}
	}
}
