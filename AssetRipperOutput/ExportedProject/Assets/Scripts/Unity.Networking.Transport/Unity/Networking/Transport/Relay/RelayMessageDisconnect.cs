namespace Unity.Networking.Transport.Relay
{
	internal struct RelayMessageDisconnect
	{
		public const int Length = 36;

		public RelayMessageHeader Header;

		public RelayAllocationId FromAllocationId;

		public RelayAllocationId ToAllocationId;

		internal static RelayMessageDisconnect Create(RelayAllocationId fromAllocationId, RelayAllocationId toAllocationId)
		{
			RelayMessageDisconnect result = default(RelayMessageDisconnect);
			result.Header = RelayMessageHeader.Create(RelayMessageType.Disconnect);
			result.FromAllocationId = fromAllocationId;
			result.ToAllocationId = toAllocationId;
			return result;
		}
	}
}
