namespace Unity.Netcode
{
	internal struct DestroyObjectMessage : INetworkMessage, INetworkSerializeByMemcpy
	{
		public ulong NetworkObjectId;

		public bool DestroyGameObject;

		public int Version => 0;

		public void Serialize(FastBufferWriter writer, int targetVersion)
		{
			BytePacker.WriteValueBitPacked(writer, NetworkObjectId);
			writer.WriteValueSafe(in DestroyGameObject, default(FastBufferWriter.ForPrimitives));
		}

		public bool Deserialize(FastBufferReader reader, ref NetworkContext context, int receivedMessageVersion)
		{
			NetworkManager networkManager = (NetworkManager)context.SystemOwner;
			if (!networkManager.IsClient)
			{
				return false;
			}
			ByteUnpacker.ReadValueBitPacked(reader, out NetworkObjectId);
			reader.ReadValueSafe(out DestroyGameObject, default(FastBufferWriter.ForPrimitives));
			if (!networkManager.SpawnManager.SpawnedObjects.TryGetValue(NetworkObjectId, out var _))
			{
				networkManager.DeferredMessageManager.DeferMessage(IDeferredNetworkMessageManager.TriggerType.OnSpawn, NetworkObjectId, reader, ref context);
				return false;
			}
			return true;
		}

		public void Handle(ref NetworkContext context)
		{
			NetworkManager networkManager = (NetworkManager)context.SystemOwner;
			if (networkManager.SpawnManager.SpawnedObjects.TryGetValue(NetworkObjectId, out var value))
			{
				networkManager.NetworkMetrics.TrackObjectDestroyReceived(context.SenderId, value, context.MessageSize);
				networkManager.SpawnManager.OnDespawnObject(value, DestroyGameObject);
			}
		}
	}
}
