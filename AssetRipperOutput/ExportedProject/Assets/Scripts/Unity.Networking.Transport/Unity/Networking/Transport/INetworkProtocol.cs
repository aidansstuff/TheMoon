using System;

namespace Unity.Networking.Transport
{
	internal interface INetworkProtocol : IDisposable
	{
		void Initialize(NetworkSettings settings);

		NetworkProtocol CreateProtocolInterface();

		int Bind(INetworkInterface networkInterface, ref NetworkInterfaceEndPoint localEndPoint);

		int CreateConnectionAddress(INetworkInterface networkInterface, NetworkEndPoint endPoint, out NetworkInterfaceEndPoint address);

		NetworkEndPoint GetRemoteEndPoint(INetworkInterface networkInterface, NetworkInterfaceEndPoint address);
	}
}
