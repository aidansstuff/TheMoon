namespace Unity.Networking.Transport.Error
{
	public enum DisconnectReason : byte
	{
		Default = 0,
		Timeout = 1,
		MaxConnectionAttempts = 2,
		ClosedByRemote = 3,
		Count = 4
	}
}
