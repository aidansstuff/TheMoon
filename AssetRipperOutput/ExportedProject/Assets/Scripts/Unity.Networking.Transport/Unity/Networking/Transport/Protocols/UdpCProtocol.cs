namespace Unity.Networking.Transport.Protocols
{
	internal enum UdpCProtocol
	{
		ConnectionRequest = 0,
		ConnectionReject = 1,
		ConnectionAccept = 2,
		Disconnect = 3,
		Data = 4,
		Ping = 5,
		Pong = 6
	}
}
