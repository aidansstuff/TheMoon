namespace Unity.Netcode
{
	internal struct ServerRpcMessage : INetworkMessage
	{
		public RpcMetadata Metadata;

		public FastBufferWriter WriteBuffer;

		public FastBufferReader ReadBuffer;

		public int Version => 0;

		public void Serialize(FastBufferWriter writer, int targetVersion)
		{
			RpcMessageHelpers.Serialize(ref writer, ref Metadata, ref WriteBuffer);
		}

		public bool Deserialize(FastBufferReader reader, ref NetworkContext context, int receivedMessageVersion)
		{
			return RpcMessageHelpers.Deserialize(ref reader, ref context, ref Metadata, ref ReadBuffer);
		}

		public void Handle(ref NetworkContext context)
		{
			__RpcParams _RpcParams = default(__RpcParams);
			_RpcParams.Server = new ServerRpcParams
			{
				Receive = new ServerRpcReceiveParams
				{
					SenderClientId = context.SenderId
				}
			};
			__RpcParams rpcParams = _RpcParams;
			RpcMessageHelpers.Handle(ref context, ref Metadata, ref ReadBuffer, ref rpcParams);
		}
	}
}
