using System.Collections.Generic;

namespace Unity.Netcode
{
	public class NetworkClient
	{
		public ulong ClientId;

		public NetworkObject PlayerObject;

		internal bool IsServer { get; set; }

		internal bool IsClient { get; set; }

		internal bool IsHost
		{
			get
			{
				if (IsClient)
				{
					return IsServer;
				}
				return false;
			}
		}

		internal bool IsConnected { get; set; }

		internal bool IsApproved { get; set; }

		public List<NetworkObject> OwnedObjects
		{
			get
			{
				if (!IsConnected)
				{
					return new List<NetworkObject>();
				}
				return SpawnManager.GetClientOwnedObjects(ClientId);
			}
		}

		internal NetworkSpawnManager SpawnManager { get; private set; }

		internal void SetRole(bool isServer, bool isClient, NetworkManager networkManager = null)
		{
			IsServer = isServer;
			IsClient = isClient;
			if (!IsServer && !isClient)
			{
				PlayerObject = null;
				ClientId = 0uL;
				IsConnected = false;
				IsApproved = false;
			}
			if (networkManager != null)
			{
				SpawnManager = networkManager.SpawnManager;
			}
		}

		internal void AssignPlayerObject(ref NetworkObject networkObject)
		{
			PlayerObject = networkObject;
		}
	}
}
