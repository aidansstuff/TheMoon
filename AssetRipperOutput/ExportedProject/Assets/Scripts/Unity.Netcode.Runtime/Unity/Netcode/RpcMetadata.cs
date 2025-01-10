namespace Unity.Netcode
{
	internal struct RpcMetadata : INetworkSerializeByMemcpy
	{
		public ulong NetworkObjectId;

		public ushort NetworkBehaviourId;

		public uint NetworkRpcMethodId;
	}
}
