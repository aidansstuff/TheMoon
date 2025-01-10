namespace Unity.Networking.Transport.Relay
{
	internal struct RelayMessageAccepted
	{
		public const int Length = 36;

		public RelayMessageHeader Header;

		public RelayAllocationId FromAllocationId;

		public RelayAllocationId ToAllocationId;

		internal static RelayMessageAccepted Create(RelayAllocationId fromAllocationId, RelayAllocationId toAllocationId)
		{
			RelayMessageAccepted result = default(RelayMessageAccepted);
			result.Header = RelayMessageHeader.Create(RelayMessageType.Accepted);
			result.FromAllocationId = fromAllocationId;
			result.ToAllocationId = toAllocationId;
			return result;
		}
	}
}
