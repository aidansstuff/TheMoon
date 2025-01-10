namespace Unity.Netcode
{
	internal struct SceneEventMessage : INetworkMessage
	{
		public SceneEventData EventData;

		private FastBufferReader m_ReceivedData;

		public int Version => 0;

		public void Serialize(FastBufferWriter writer, int targetVersion)
		{
			EventData.Serialize(writer);
		}

		public bool Deserialize(FastBufferReader reader, ref NetworkContext context, int receivedMessageVersion)
		{
			m_ReceivedData = reader;
			return true;
		}

		public void Handle(ref NetworkContext context)
		{
			((NetworkManager)context.SystemOwner).SceneManager.HandleSceneEvent(context.SenderId, m_ReceivedData);
		}
	}
}
