namespace Unity.Netcode
{
	internal struct DisconnectReasonMessage : INetworkMessage
	{
		public string Reason;

		public int Version => 0;

		public void Serialize(FastBufferWriter writer, int targetVersion)
		{
			string s = Reason ?? string.Empty;
			BytePacker.WriteValueBitPacked(writer, Version);
			if (writer.TryBeginWrite(FastBufferWriter.GetWriteSize(s)))
			{
				writer.WriteValue(s);
				return;
			}
			writer.WriteValueSafe(string.Empty);
			NetworkLog.LogWarning("Disconnect reason didn't fit. Disconnected without sending a reason. Consider shortening the reason string.");
		}

		public bool Deserialize(FastBufferReader reader, ref NetworkContext context, int receivedMessageVersion)
		{
			ByteUnpacker.ReadValueBitPacked(reader, out receivedMessageVersion);
			reader.ReadValueSafe(out Reason, oneByteChars: false);
			return true;
		}

		public void Handle(ref NetworkContext context)
		{
			((NetworkManager)context.SystemOwner).ConnectionManager.DisconnectReason = Reason;
		}
	}
}
