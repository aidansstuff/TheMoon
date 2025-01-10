namespace Unity.Netcode
{
	internal struct ServerLogMessage : INetworkMessage
	{
		public NetworkLog.LogType LogType;

		public string Message;

		public int Version => 0;

		public void Serialize(FastBufferWriter writer, int targetVersion)
		{
			writer.WriteValueSafe(in LogType, default(FastBufferWriter.ForEnums));
			BytePacker.WriteValuePacked(writer, Message);
		}

		public bool Deserialize(FastBufferReader reader, ref NetworkContext context, int receivedMessageVersion)
		{
			NetworkManager networkManager = (NetworkManager)context.SystemOwner;
			if (networkManager.IsServer && networkManager.NetworkConfig.EnableNetworkLogs)
			{
				reader.ReadValueSafe(out LogType, default(FastBufferWriter.ForEnums));
				ByteUnpacker.ReadValuePacked(reader, out Message);
				return true;
			}
			return false;
		}

		public void Handle(ref NetworkContext context)
		{
			NetworkManager obj = (NetworkManager)context.SystemOwner;
			ulong senderId = context.SenderId;
			obj.NetworkMetrics.TrackServerLogReceived(senderId, (uint)LogType, context.MessageSize);
			switch (LogType)
			{
			case NetworkLog.LogType.Info:
				NetworkLog.LogInfoServerLocal(Message, senderId);
				break;
			case NetworkLog.LogType.Warning:
				NetworkLog.LogWarningServerLocal(Message, senderId);
				break;
			case NetworkLog.LogType.Error:
				NetworkLog.LogErrorServerLocal(Message, senderId);
				break;
			}
		}
	}
}
