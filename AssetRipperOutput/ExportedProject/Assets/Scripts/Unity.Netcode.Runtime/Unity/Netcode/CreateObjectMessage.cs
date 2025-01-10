namespace Unity.Netcode
{
	internal struct CreateObjectMessage : INetworkMessage
	{
		public NetworkObject.SceneObject ObjectInfo;

		private FastBufferReader m_ReceivedNetworkVariableData;

		public int Version => 0;

		public void Serialize(FastBufferWriter writer, int targetVersion)
		{
			ObjectInfo.Serialize(writer);
		}

		public bool Deserialize(FastBufferReader reader, ref NetworkContext context, int receivedMessageVersion)
		{
			NetworkManager networkManager = (NetworkManager)context.SystemOwner;
			if (!networkManager.IsClient)
			{
				return false;
			}
			ObjectInfo.Deserialize(reader);
			if (!networkManager.NetworkConfig.ForceSamePrefabs && !networkManager.SpawnManager.HasPrefab(ObjectInfo))
			{
				networkManager.DeferredMessageManager.DeferMessage(IDeferredNetworkMessageManager.TriggerType.OnAddPrefab, ObjectInfo.Hash, reader, ref context);
				return false;
			}
			m_ReceivedNetworkVariableData = reader;
			return true;
		}

		public void Handle(ref NetworkContext context)
		{
			NetworkManager networkManager = (NetworkManager)context.SystemOwner;
			NetworkObject networkObject = NetworkObject.AddSceneObject(in ObjectInfo, m_ReceivedNetworkVariableData, networkManager);
			networkManager.NetworkMetrics.TrackObjectSpawnReceived(context.SenderId, networkObject, context.MessageSize);
		}
	}
}
