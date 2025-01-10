namespace Unity.Networking.Transport.Relay
{
	internal struct RelayMessageError
	{
		public const int Length = 21;

		public RelayMessageHeader Header;

		public RelayAllocationId AllocationId;

		public byte ErrorCode;

		internal static RelayMessageError Create(RelayAllocationId allocationId, byte errorCode)
		{
			RelayMessageError result = default(RelayMessageError);
			result.Header = RelayMessageHeader.Create(RelayMessageType.Error);
			result.AllocationId = allocationId;
			result.ErrorCode = errorCode;
			return result;
		}
	}
}
