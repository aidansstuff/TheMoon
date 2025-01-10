using UnityEngine;

namespace Unity.Networking.Transport.Relay
{
	public static class NetworkDriverRelayExtensions
	{
		public static RelayConnectionStatus GetRelayConnectionStatus(this NetworkDriver driver)
		{
			if (driver.NetworkProtocol is RelayNetworkProtocol)
			{
				return (RelayConnectionStatus)driver.ProtocolStatus;
			}
			return RelayConnectionStatus.NotUsingRelay;
		}

		public static NetworkConnection Connect(this NetworkDriver driver)
		{
			if (driver.NetworkProtocol is RelayNetworkProtocol)
			{
				return driver.Connect(default(NetworkEndPoint));
			}
			Debug.LogError("Can't call Connect without an endpoint when not using the Relay.");
			return default(NetworkConnection);
		}
	}
}
