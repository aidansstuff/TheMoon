using Unity.Collections;

namespace Unity.Netcode
{
	internal struct ConnectionRequestMessage : INetworkMessage
	{
		public ulong ConfigHash;

		public byte[] ConnectionData;

		public bool ShouldSendConnectionData;

		public NativeArray<MessageVersionData> MessageVersions;

		public int Version => 0;

		public void Serialize(FastBufferWriter writer, int targetVersion)
		{
			BytePacker.WriteValueBitPacked(writer, MessageVersions.Length);
			foreach (MessageVersionData messageVersion in MessageVersions)
			{
				messageVersion.Serialize(writer);
			}
			if (ShouldSendConnectionData)
			{
				writer.WriteValueSafe(in ConfigHash, default(FastBufferWriter.ForPrimitives));
				writer.WriteValueSafe(ConnectionData, default(FastBufferWriter.ForPrimitives));
			}
			else
			{
				writer.WriteValueSafe(in ConfigHash, default(FastBufferWriter.ForPrimitives));
			}
		}

		public bool Deserialize(FastBufferReader reader, ref NetworkContext context, int receivedMessageVersion)
		{
			NetworkManager networkManager = (NetworkManager)context.SystemOwner;
			if (!networkManager.IsServer)
			{
				return false;
			}
			ByteUnpacker.ReadValueBitPacked(reader, out int value);
			for (int i = 0; i < value; i++)
			{
				MessageVersionData messageVersionData = default(MessageVersionData);
				messageVersionData.Deserialize(reader);
				networkManager.ConnectionManager.MessageManager.SetVersion(context.SenderId, messageVersionData.Hash, messageVersionData.Version);
				if (networkManager.ConnectionManager.MessageManager.GetMessageForHash(messageVersionData.Hash) == typeof(ConnectionRequestMessage))
				{
					receivedMessageVersion = messageVersionData.Version;
				}
			}
			if (networkManager.NetworkConfig.ConnectionApproval)
			{
				if (!reader.TryBeginRead(FastBufferWriter.GetWriteSize(in ConfigHash, default(FastBufferWriter.ForStructs)) + FastBufferWriter.GetWriteSize<int>()))
				{
					if (NetworkLog.CurrentLogLevel <= LogLevel.Normal)
					{
						NetworkLog.LogWarning("Incomplete connection request message given config - possible NetworkConfig mismatch.");
					}
					networkManager.DisconnectClient(context.SenderId);
					return false;
				}
				reader.ReadValue(out ConfigHash, default(FastBufferWriter.ForPrimitives));
				if (!networkManager.NetworkConfig.CompareConfig(ConfigHash))
				{
					if (NetworkLog.CurrentLogLevel <= LogLevel.Normal)
					{
						NetworkLog.LogWarning("NetworkConfig mismatch. The configuration between the server and client does not match");
					}
					networkManager.DisconnectClient(context.SenderId);
					return false;
				}
				reader.ReadValueSafe(out ConnectionData, default(FastBufferWriter.ForPrimitives));
			}
			else
			{
				if (!reader.TryBeginRead(FastBufferWriter.GetWriteSize(in ConfigHash, default(FastBufferWriter.ForStructs))))
				{
					if (NetworkLog.CurrentLogLevel <= LogLevel.Normal)
					{
						NetworkLog.LogWarning("Incomplete connection request message.");
					}
					networkManager.DisconnectClient(context.SenderId);
					return false;
				}
				reader.ReadValue(out ConfigHash, default(FastBufferWriter.ForPrimitives));
				if (!networkManager.NetworkConfig.CompareConfig(ConfigHash))
				{
					if (NetworkLog.CurrentLogLevel <= LogLevel.Normal)
					{
						NetworkLog.LogWarning("NetworkConfig mismatch. The configuration between the server and client does not match");
					}
					networkManager.DisconnectClient(context.SenderId);
					return false;
				}
			}
			return true;
		}

		public void Handle(ref NetworkContext context)
		{
			NetworkManager networkManager = (NetworkManager)context.SystemOwner;
			ulong senderId = context.SenderId;
			if (networkManager.ConnectionManager.PendingClients.TryGetValue(senderId, out var value))
			{
				value.ConnectionState = PendingClient.State.PendingApproval;
			}
			if (networkManager.NetworkConfig.ConnectionApproval)
			{
				ConnectionRequestMessage connectionRequestMessage = this;
				networkManager.ConnectionManager.ApproveConnection(ref connectionRequestMessage, ref context);
				return;
			}
			NetworkManager.ConnectionApprovalResponse response = new NetworkManager.ConnectionApprovalResponse
			{
				Approved = true,
				CreatePlayerObject = (networkManager.NetworkConfig.PlayerPrefab != null)
			};
			networkManager.ConnectionManager.HandleConnectionApproval(senderId, response);
		}
	}
}
