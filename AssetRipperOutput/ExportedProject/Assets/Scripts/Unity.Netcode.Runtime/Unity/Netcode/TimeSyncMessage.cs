namespace Unity.Netcode
{
	internal struct TimeSyncMessage : INetworkMessage, INetworkSerializeByMemcpy
	{
		public int Tick;

		public int Version => 0;

		public void Serialize(FastBufferWriter writer, int targetVersion)
		{
			BytePacker.WriteValueBitPacked(writer, Tick);
		}

		public bool Deserialize(FastBufferReader reader, ref NetworkContext context, int receivedMessageVersion)
		{
			if (!((NetworkManager)context.SystemOwner).IsClient)
			{
				return false;
			}
			ByteUnpacker.ReadValueBitPacked(reader, out Tick);
			return true;
		}

		public void Handle(ref NetworkContext context)
		{
			NetworkManager networkManager = (NetworkManager)context.SystemOwner;
			NetworkTime networkTime = new NetworkTime(networkManager.NetworkTickSystem.TickRate, Tick);
			networkManager.NetworkTimeSystem.Sync(networkTime.Time, (double)networkManager.NetworkConfig.NetworkTransport.GetCurrentRtt(context.SenderId) / 1000.0);
		}
	}
}
