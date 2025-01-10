using System.Runtime.InteropServices;

namespace Unity.Networking.Transport
{
	[StructLayout(LayoutKind.Sequential, Size = 1)]
	public struct NetworkParameterConstants
	{
		public const int InitialEventQueueSize = 100;

		public const int InvalidConnectionId = -1;

		public const int DriverDataStreamSize = 65536;

		public const int ConnectTimeoutMS = 1000;

		public const int MaxConnectAttempts = 60;

		public const int DisconnectTimeoutMS = 30000;

		public const int HeartbeatTimeoutMS = 500;

		public const int MaxMessageSize = 1400;

		public const int MTU = 1400;

		internal const int MaxPacketBufferSize = 1472;
	}
}
