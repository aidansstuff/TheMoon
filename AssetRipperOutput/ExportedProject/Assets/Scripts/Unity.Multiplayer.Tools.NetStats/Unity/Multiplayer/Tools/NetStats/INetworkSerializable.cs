namespace Unity.Multiplayer.Tools.NetStats
{
	internal interface INetworkSerializable
	{
		void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter;
	}
}
