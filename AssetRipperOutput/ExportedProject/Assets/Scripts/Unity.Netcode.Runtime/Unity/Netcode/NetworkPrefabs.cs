using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Unity.Netcode
{
	[Serializable]
	public class NetworkPrefabs
	{
		[SerializeField]
		public List<NetworkPrefabsList> NetworkPrefabsLists = new List<NetworkPrefabsList>();

		[NonSerialized]
		public Dictionary<uint, NetworkPrefab> NetworkPrefabOverrideLinks = new Dictionary<uint, NetworkPrefab>();

		[NonSerialized]
		public Dictionary<uint, uint> OverrideToNetworkPrefab = new Dictionary<uint, uint>();

		[NonSerialized]
		private List<NetworkPrefab> m_Prefabs = new List<NetworkPrefab>();

		[NonSerialized]
		private List<NetworkPrefab> m_RuntimeAddedPrefabs = new List<NetworkPrefab>();

		public IReadOnlyList<NetworkPrefab> Prefabs => m_Prefabs;

		private void AddTriggeredByNetworkPrefabList(NetworkPrefab networkPrefab)
		{
			if (AddPrefabRegistration(networkPrefab))
			{
				m_Prefabs.Add(networkPrefab);
			}
		}

		private void RemoveTriggeredByNetworkPrefabList(NetworkPrefab networkPrefab)
		{
			m_Prefabs.Remove(networkPrefab);
		}

		~NetworkPrefabs()
		{
			Shutdown();
		}

		internal void Shutdown()
		{
			foreach (NetworkPrefabsList networkPrefabsList in NetworkPrefabsLists)
			{
				networkPrefabsList.OnAdd = (NetworkPrefabsList.OnAddDelegate)Delegate.Remove(networkPrefabsList.OnAdd, new NetworkPrefabsList.OnAddDelegate(AddTriggeredByNetworkPrefabList));
				networkPrefabsList.OnRemove = (NetworkPrefabsList.OnRemoveDelegate)Delegate.Remove(networkPrefabsList.OnRemove, new NetworkPrefabsList.OnRemoveDelegate(RemoveTriggeredByNetworkPrefabList));
			}
		}

		public void Initialize(bool warnInvalid = true)
		{
			m_Prefabs.Clear();
			foreach (NetworkPrefabsList networkPrefabsList in NetworkPrefabsLists)
			{
				networkPrefabsList.OnAdd = (NetworkPrefabsList.OnAddDelegate)Delegate.Combine(networkPrefabsList.OnAdd, new NetworkPrefabsList.OnAddDelegate(AddTriggeredByNetworkPrefabList));
				networkPrefabsList.OnRemove = (NetworkPrefabsList.OnRemoveDelegate)Delegate.Combine(networkPrefabsList.OnRemove, new NetworkPrefabsList.OnRemoveDelegate(RemoveTriggeredByNetworkPrefabList));
			}
			NetworkPrefabOverrideLinks.Clear();
			OverrideToNetworkPrefab.Clear();
			List<NetworkPrefab> list = new List<NetworkPrefab>();
			if (NetworkPrefabsLists.Count != 0)
			{
				foreach (NetworkPrefabsList networkPrefabsList2 in NetworkPrefabsLists)
				{
					foreach (NetworkPrefab prefab in networkPrefabsList2.PrefabList)
					{
						list.Add(prefab);
					}
				}
			}
			m_Prefabs = new List<NetworkPrefab>();
			List<NetworkPrefab> list2 = null;
			if (warnInvalid)
			{
				list2 = new List<NetworkPrefab>();
			}
			foreach (NetworkPrefab item in list)
			{
				if (AddPrefabRegistration(item))
				{
					m_Prefabs.Add(item);
				}
				else
				{
					list2?.Add(item);
				}
			}
			foreach (NetworkPrefab runtimeAddedPrefab in m_RuntimeAddedPrefabs)
			{
				if (AddPrefabRegistration(runtimeAddedPrefab))
				{
					m_Prefabs.Add(runtimeAddedPrefab);
				}
				else
				{
					list2?.Add(runtimeAddedPrefab);
				}
			}
			if (list2 != null && list2.Count > 0 && NetworkLog.CurrentLogLevel <= LogLevel.Error)
			{
				StringBuilder stringBuilder = new StringBuilder("Removing invalid prefabs from Network Prefab registration: ");
				stringBuilder.Append(string.Join(", ", list2));
				NetworkLog.LogWarning(stringBuilder.ToString());
			}
		}

		public bool Add(NetworkPrefab networkPrefab)
		{
			if (AddPrefabRegistration(networkPrefab))
			{
				m_Prefabs.Add(networkPrefab);
				m_RuntimeAddedPrefabs.Add(networkPrefab);
				return true;
			}
			return false;
		}

		public void Remove(NetworkPrefab prefab)
		{
			if (prefab == null)
			{
				throw new ArgumentNullException("prefab");
			}
			m_Prefabs.Remove(prefab);
			m_RuntimeAddedPrefabs.Remove(prefab);
			OverrideToNetworkPrefab.Remove(prefab.TargetPrefabGlobalObjectIdHash);
			NetworkPrefabOverrideLinks.Remove(prefab.SourcePrefabGlobalObjectIdHash);
		}

		public void Remove(GameObject prefab)
		{
			if (prefab == null)
			{
				throw new ArgumentNullException("prefab");
			}
			for (int i = 0; i < m_Prefabs.Count; i++)
			{
				if (m_Prefabs[i].Prefab == prefab)
				{
					Remove(m_Prefabs[i]);
					return;
				}
			}
			for (int j = 0; j < m_RuntimeAddedPrefabs.Count; j++)
			{
				if (m_RuntimeAddedPrefabs[j].Prefab == prefab)
				{
					Remove(m_RuntimeAddedPrefabs[j]);
					break;
				}
			}
		}

		public bool Contains(GameObject prefab)
		{
			for (int i = 0; i < m_Prefabs.Count; i++)
			{
				if (m_Prefabs[i].Prefab == prefab)
				{
					return true;
				}
			}
			return false;
		}

		public bool Contains(NetworkPrefab prefab)
		{
			for (int i = 0; i < m_Prefabs.Count; i++)
			{
				if (m_Prefabs[i].Equals(prefab))
				{
					return true;
				}
			}
			return false;
		}

		private bool AddPrefabRegistration(NetworkPrefab networkPrefab)
		{
			if (networkPrefab == null)
			{
				return false;
			}
			if (!networkPrefab.Validate())
			{
				return false;
			}
			uint sourcePrefabGlobalObjectIdHash = networkPrefab.SourcePrefabGlobalObjectIdHash;
			uint targetPrefabGlobalObjectIdHash = networkPrefab.TargetPrefabGlobalObjectIdHash;
			if (NetworkPrefabOverrideLinks.ContainsKey(sourcePrefabGlobalObjectIdHash))
			{
				NetworkObject component = networkPrefab.Prefab.GetComponent<NetworkObject>();
				Debug.LogError(string.Format("{0} ({1}) has a duplicate {2} source entry value of: {3}!", "NetworkPrefab", component.name, "GlobalObjectIdHash", sourcePrefabGlobalObjectIdHash));
				return false;
			}
			if (networkPrefab.Override == NetworkPrefabOverride.None)
			{
				NetworkPrefabOverrideLinks.Add(sourcePrefabGlobalObjectIdHash, networkPrefab);
				return true;
			}
			if (OverrideToNetworkPrefab.ContainsKey(targetPrefabGlobalObjectIdHash))
			{
				NetworkObject component2 = networkPrefab.Prefab.GetComponent<NetworkObject>();
				Debug.LogError(string.Format("{0} (\"{1}\") has a duplicate {2} target entry value of: {3}!", "NetworkPrefab", component2.name, "GlobalObjectIdHash", targetPrefabGlobalObjectIdHash));
				return false;
			}
			switch (networkPrefab.Override)
			{
			case NetworkPrefabOverride.Prefab:
				NetworkPrefabOverrideLinks.Add(sourcePrefabGlobalObjectIdHash, networkPrefab);
				OverrideToNetworkPrefab.Add(targetPrefabGlobalObjectIdHash, sourcePrefabGlobalObjectIdHash);
				break;
			case NetworkPrefabOverride.Hash:
				NetworkPrefabOverrideLinks.Add(sourcePrefabGlobalObjectIdHash, networkPrefab);
				OverrideToNetworkPrefab.Add(targetPrefabGlobalObjectIdHash, sourcePrefabGlobalObjectIdHash);
				break;
			}
			return true;
		}
	}
}
