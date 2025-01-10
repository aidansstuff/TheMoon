using System;

namespace Unity.Networking.Transport.TLS
{
	internal struct SecureUserData
	{
		public IntPtr StreamData;

		public NetworkSendInterface Interface;

		public NetworkInterfaceEndPoint Remote;

		public NetworkSendQueueHandle QueueHandle;

		public int Size;

		public int BytesProcessed;
	}
}
