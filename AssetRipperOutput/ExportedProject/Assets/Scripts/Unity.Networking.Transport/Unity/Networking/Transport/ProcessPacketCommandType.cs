namespace Unity.Networking.Transport
{
	internal enum ProcessPacketCommandType : byte
	{
		Drop = 0,
		AddressUpdate = 1,
		ConnectionAccept = 2,
		ConnectionReject = 3,
		ConnectionRequest = 4,
		Data = 5,
		Disconnect = 6,
		DataWithImplicitConnectionAccept = 7,
		Ping = 8,
		Pong = 9,
		ProtocolStatusUpdate = 10
	}
}
