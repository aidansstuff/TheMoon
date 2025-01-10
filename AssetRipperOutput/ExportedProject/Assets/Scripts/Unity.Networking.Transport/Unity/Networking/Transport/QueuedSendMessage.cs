namespace Unity.Networking.Transport
{
	public struct QueuedSendMessage
	{
		public unsafe fixed byte Data[1472];

		public NetworkInterfaceEndPoint Dest;

		public int DataLength;
	}
}
