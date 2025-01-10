using Unity.Multiplayer.Tools;

namespace Unity.Netcode
{
	internal class NetworkMetricsManager
	{
		private NetworkManager m_NetworkManager;

		internal INetworkMetrics NetworkMetrics { get; private set; }

		public void UpdateMetrics()
		{
			NetworkMetrics.UpdateNetworkObjectsCount(m_NetworkManager.SpawnManager.SpawnedObjects.Count);
			NetworkMetrics.UpdateConnectionsCount((!m_NetworkManager.IsServer) ? 1 : m_NetworkManager.ConnectionManager.ConnectedClients.Count);
			NetworkMetrics.DispatchFrame();
		}

		public void Initialize(NetworkManager networkManager)
		{
			m_NetworkManager = networkManager;
			if (NetworkMetrics == null)
			{
				NetworkMetrics = new NetworkMetrics();
			}
			NetworkSolutionInterfaceParameters @interface = default(NetworkSolutionInterfaceParameters);
			@interface.NetworkObjectProvider = new NetworkObjectProvider(networkManager);
			NetworkSolutionInterface.SetInterface(@interface);
		}
	}
}
