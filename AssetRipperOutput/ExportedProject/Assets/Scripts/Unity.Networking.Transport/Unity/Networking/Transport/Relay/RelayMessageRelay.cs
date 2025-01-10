namespace Unity.Networking.Transport.Relay
{
	internal struct RelayMessageRelay
	{
		public const int Length = 38;

		public RelayMessageHeader Header;

		public RelayAllocationId FromAllocationId;

		public RelayAllocationId ToAllocationId;

		public ushort DataLength;

		internal static RelayMessageRelay Create(RelayAllocationId fromAllocationId, RelayAllocationId toAllocationId, ushort dataLength)
		{
			RelayMessageRelay result = default(RelayMessageRelay);
			result.Header = RelayMessageHeader.Create(RelayMessageType.Relay);
			result.FromAllocationId = fromAllocationId;
			result.ToAllocationId = toAllocationId;
			result.DataLength = RelayNetworkProtocol.SwitchEndianness(dataLength);
			return result;
		}
	}
}
