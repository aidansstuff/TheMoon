using System.Collections.Generic;

namespace Unity.Netcode
{
	public class NetworkBehaviourUpdater
	{
		private NetworkManager m_NetworkManager;

		private NetworkConnectionManager m_ConnectionManager;

		private HashSet<NetworkObject> m_DirtyNetworkObjects = new HashSet<NetworkObject>();

		internal void AddForUpdate(NetworkObject networkObject)
		{
			m_DirtyNetworkObjects.Add(networkObject);
		}

		internal void NetworkBehaviourUpdate()
		{
			m_DirtyNetworkObjects.RemoveWhere((NetworkObject sobj) => sobj == null);
			if (m_ConnectionManager.LocalClient.IsServer)
			{
				foreach (NetworkObject dirtyNetworkObject in m_DirtyNetworkObjects)
				{
					for (int i = 0; i < dirtyNetworkObject.ChildNetworkBehaviours.Count; i++)
					{
						dirtyNetworkObject.ChildNetworkBehaviours[i].PreVariableUpdate();
					}
					for (int j = 0; j < m_ConnectionManager.ConnectedClientsList.Count; j++)
					{
						NetworkClient networkClient = m_ConnectionManager.ConnectedClientsList[j];
						if (dirtyNetworkObject.IsNetworkVisibleTo(networkClient.ClientId))
						{
							for (int k = 0; k < dirtyNetworkObject.ChildNetworkBehaviours.Count; k++)
							{
								dirtyNetworkObject.ChildNetworkBehaviours[k].VariableUpdate(networkClient.ClientId);
							}
						}
					}
				}
			}
			else
			{
				foreach (NetworkObject dirtyNetworkObject2 in m_DirtyNetworkObjects)
				{
					if (dirtyNetworkObject2.IsOwner)
					{
						for (int l = 0; l < dirtyNetworkObject2.ChildNetworkBehaviours.Count; l++)
						{
							dirtyNetworkObject2.ChildNetworkBehaviours[l].PreVariableUpdate();
						}
						for (int m = 0; m < dirtyNetworkObject2.ChildNetworkBehaviours.Count; m++)
						{
							dirtyNetworkObject2.ChildNetworkBehaviours[m].VariableUpdate(0uL);
						}
					}
				}
			}
			foreach (NetworkObject dirtyNetworkObject3 in m_DirtyNetworkObjects)
			{
				for (int n = 0; n < dirtyNetworkObject3.ChildNetworkBehaviours.Count; n++)
				{
					NetworkBehaviour networkBehaviour = dirtyNetworkObject3.ChildNetworkBehaviours[n];
					for (int num = 0; num < networkBehaviour.NetworkVariableFields.Count; num++)
					{
						if (networkBehaviour.NetworkVariableFields[num].IsDirty() && !networkBehaviour.NetworkVariableIndexesToResetSet.Contains(num))
						{
							networkBehaviour.NetworkVariableIndexesToResetSet.Add(num);
							networkBehaviour.NetworkVariableIndexesToReset.Add(num);
						}
					}
				}
			}
			foreach (NetworkObject dirtyNetworkObject4 in m_DirtyNetworkObjects)
			{
				dirtyNetworkObject4.PostNetworkVariableWrite();
			}
			m_DirtyNetworkObjects.Clear();
		}

		internal void Initialize(NetworkManager networkManager)
		{
			m_NetworkManager = networkManager;
			m_ConnectionManager = networkManager.ConnectionManager;
			m_NetworkManager.NetworkTickSystem.Tick += NetworkBehaviourUpdater_Tick;
		}

		internal void Shutdown()
		{
			m_NetworkManager.NetworkTickSystem.Tick -= NetworkBehaviourUpdater_Tick;
		}

		private void NetworkBehaviourUpdater_Tick()
		{
			NetworkBehaviourUpdate();
			m_NetworkManager.SpawnManager.HandleNetworkObjectShow();
		}
	}
}
