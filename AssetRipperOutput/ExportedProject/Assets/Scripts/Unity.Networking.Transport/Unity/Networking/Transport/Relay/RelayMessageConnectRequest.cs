namespace Unity.Networking.Transport.Relay
{
	internal struct RelayMessageConnectRequest
	{
		public const int Length = 276;

		public RelayMessageHeader Header;

		public RelayAllocationId AllocationId;

		public byte ToConnectionDataLength;

		public RelayConnectionData ToConnectionData;

		internal static RelayMessageConnectRequest Create(RelayAllocationId allocationId, RelayConnectionData toConnectionData)
		{
			RelayMessageConnectRequest result = default(RelayMessageConnectRequest);
			result.Header = RelayMessageHeader.Create(RelayMessageType.ConnectRequest);
			result.AllocationId = allocationId;
			result.ToConnectionDataLength = byte.MaxValue;
			result.ToConnectionData = toConnectionData;
			return result;
		}
	}
}
