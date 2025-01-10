namespace Unity.Networking.Transport.Relay
{
	internal struct RelayMessageHeader
	{
		public const int Length = 4;

		public ushort Signature;

		public byte Version;

		public RelayMessageType Type;

		public bool IsValid()
		{
			if (Signature == 29402)
			{
				return Version == 0;
			}
			return false;
		}

		internal static RelayMessageHeader Create(RelayMessageType type)
		{
			RelayMessageHeader result = default(RelayMessageHeader);
			result.Signature = 29402;
			result.Version = 0;
			result.Type = type;
			return result;
		}
	}
}
