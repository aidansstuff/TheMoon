namespace Unity.Netcode
{
	internal interface INetworkMessage
	{
		int Version { get; }

		void Serialize(FastBufferWriter writer, int targetVersion);

		bool Deserialize(FastBufferReader reader, ref NetworkContext context, int receivedMessageVersion);

		void Handle(ref NetworkContext context);
	}
}
