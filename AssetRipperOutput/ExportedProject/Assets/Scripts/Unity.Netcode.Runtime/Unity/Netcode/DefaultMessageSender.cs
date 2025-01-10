using System;

namespace Unity.Netcode
{
	internal class DefaultMessageSender : INetworkMessageSender
	{
		private NetworkTransport m_NetworkTransport;

		private NetworkConnectionManager m_ConnectionManager;

		public DefaultMessageSender(NetworkManager manager)
		{
			m_NetworkTransport = manager.NetworkConfig.NetworkTransport;
			m_ConnectionManager = manager.ConnectionManager;
		}

		public void Send(ulong clientId, NetworkDelivery delivery, FastBufferWriter batchData)
		{
			ArraySegment<byte> payload = batchData.ToTempByteArray();
			m_NetworkTransport.Send(m_ConnectionManager.ClientIdToTransportId(clientId), payload, delivery);
		}
	}
}
