namespace Unity.Networking.Transport.Relay
{
	internal enum RelayMessageType : byte
	{
		Bind = 0,
		BindReceived = 1,
		Ping = 2,
		ConnectRequest = 3,
		Accepted = 6,
		Disconnect = 9,
		Relay = 10,
		Error = 12
	}
}
