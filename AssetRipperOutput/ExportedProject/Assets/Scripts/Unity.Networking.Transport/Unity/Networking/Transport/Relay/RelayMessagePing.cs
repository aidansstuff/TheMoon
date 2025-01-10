namespace Unity.Networking.Transport.Relay
{
	internal struct RelayMessagePing
	{
		public const int Length = 22;

		public RelayMessageHeader Header;

		public RelayAllocationId FromAllocationId;

		public ushort SequenceNumber;

		internal static RelayMessagePing Create(RelayAllocationId fromAllocationId, ushort dataLength)
		{
			RelayMessagePing result = default(RelayMessagePing);
			result.Header = RelayMessageHeader.Create(RelayMessageType.Ping);
			result.FromAllocationId = fromAllocationId;
			result.SequenceNumber = 1;
			return result;
		}
	}
}
