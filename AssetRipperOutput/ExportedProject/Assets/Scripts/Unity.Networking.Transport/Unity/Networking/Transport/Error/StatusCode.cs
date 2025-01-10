namespace Unity.Networking.Transport.Error
{
	public enum StatusCode
	{
		Success = 0,
		NetworkIdMismatch = -1,
		NetworkVersionMismatch = -2,
		NetworkStateMismatch = -3,
		NetworkPacketOverflow = -4,
		NetworkSendQueueFull = -5,
		NetworkHeaderInvalid = -6,
		NetworkDriverParallelForErr = -7,
		NetworkSendHandleInvalid = -8,
		NetworkArgumentMismatch = -9,
		NetworkSocketError = -10
	}
}
