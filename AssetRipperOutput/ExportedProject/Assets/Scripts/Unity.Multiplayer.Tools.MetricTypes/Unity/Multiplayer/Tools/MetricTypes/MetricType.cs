namespace Unity.Multiplayer.Tools.MetricTypes
{
	internal enum MetricType
	{
		None = 0,
		TotalBytes = 1,
		Rpc = 2,
		NamedMessage = 3,
		UnnamedMessage = 4,
		NetworkVariableDelta = 5,
		ObjectSpawned = 6,
		ObjectDestroyed = 7,
		OwnershipChange = 8,
		ServerLog = 9,
		SceneEvent = 10,
		NetworkMessage = 11,
		Packets = 12,
		RttToServer = 13,
		NetworkObjects = 14,
		Connections = 15,
		PacketLoss = 16
	}
}
