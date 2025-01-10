namespace Unity.Netcode
{
	internal struct NetworkMessageHeader : INetworkSerializeByMemcpy
	{
		public uint MessageType;

		public uint MessageSize;
	}
}
