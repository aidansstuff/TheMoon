namespace Unity.Netcode
{
	internal struct NamedMessage : INetworkMessage
	{
		public ulong Hash;

		public FastBufferWriter SendData;

		private FastBufferReader m_ReceiveData;

		public int Version => 0;

		public unsafe void Serialize(FastBufferWriter writer, int targetVersion)
		{
			writer.WriteValueSafe(in Hash, default(FastBufferWriter.ForPrimitives));
			writer.WriteBytesSafe(SendData.GetUnsafePtr(), SendData.Length);
		}

		public bool Deserialize(FastBufferReader reader, ref NetworkContext context, int receivedMessageVersion)
		{
			reader.ReadValueSafe(out Hash, default(FastBufferWriter.ForPrimitives));
			m_ReceiveData = reader;
			return true;
		}

		public void Handle(ref NetworkContext context)
		{
			if (!((NetworkManager)context.SystemOwner).ShutdownInProgress)
			{
				((NetworkManager)context.SystemOwner).CustomMessagingManager.InvokeNamedMessage(Hash, context.SenderId, m_ReceiveData, context.SerializedHeaderSize);
			}
		}
	}
}
