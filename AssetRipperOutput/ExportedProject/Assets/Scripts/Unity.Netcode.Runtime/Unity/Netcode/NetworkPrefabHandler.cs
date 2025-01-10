using System;
using System.Collections.Generic;
using UnityEngine;

namespace Unity.Netcode
{
	public class NetworkPrefabHandler
	{
		private NetworkManager m_NetworkManager;

		private readonly Dictionary<uint, INetworkPrefabInstanceHandler> m_PrefabAssetToPrefabHandler = new Dictionary<uint, INetworkPrefabInstanceHandler>();

		private readonly Dictionary<uint, uint> m_PrefabInstanceToPrefabAsset = new Dictionary<uint, uint>();

		internal static string PrefabDebugHelper(NetworkPrefab networkPrefab)
		{
			return "NetworkPrefab \"" + networkPrefab.Prefab.name + "\"";
		}

		public bool AddHandler(GameObject networkPrefabAsset, INetworkPrefabInstanceHandler instanceHandler)
		{
			return AddHandler(networkPrefabAsset.GetComponent<NetworkObject>().GlobalObjectIdHash, instanceHandler);
		}

		public bool AddHandler(NetworkObject prefabAssetNetworkObject, INetworkPrefabInstanceHandler instanceHandler)
		{
			return AddHandler(prefabAssetNetworkObject.GlobalObjectIdHash, instanceHandler);
		}

		public bool AddHandler(uint globalObjectIdHash, INetworkPrefabInstanceHandler instanceHandler)
		{
			if (!m_PrefabAssetToPrefabHandler.ContainsKey(globalObjectIdHash))
			{
				m_PrefabAssetToPrefabHandler.Add(globalObjectIdHash, instanceHandler);
				return true;
			}
			return false;
		}

		public void RegisterHostGlobalObjectIdHashValues(GameObject sourceNetworkPrefab, List<GameObject> networkPrefabOverrides)
		{
			if (NetworkManager.Singleton.IsListening)
			{
				if (NetworkManager.Singleton.IsHost)
				{
					NetworkObject component = sourceNetworkPrefab.GetComponent<NetworkObject>();
					if (sourceNetworkPrefab != null)
					{
						uint globalObjectIdHash = component.GlobalObjectIdHash;
						{
							foreach (GameObject networkPrefabOverride in networkPrefabOverrides)
							{
								if (networkPrefabOverride.TryGetComponent<NetworkObject>(out var component2))
								{
									if (!m_PrefabInstanceToPrefabAsset.ContainsKey(component2.GlobalObjectIdHash))
									{
										m_PrefabInstanceToPrefabAsset.Add(component2.GlobalObjectIdHash, globalObjectIdHash);
									}
									else
									{
										Debug.LogWarning(component2.name + " appears to be a duplicate entry!");
									}
									continue;
								}
								throw new Exception(component2.name + " does not have a NetworkObject component!");
							}
							return;
						}
					}
					throw new Exception(sourceNetworkPrefab.name + " does not have a NetworkObject component!");
				}
				throw new Exception("You should only call RegisterHostGlobalObjectIdHashValues as a Host!");
			}
			throw new Exception("You can only call RegisterHostGlobalObjectIdHashValues once NetworkManager is listening!");
		}

		public bool RemoveHandler(GameObject networkPrefabAsset)
		{
			return RemoveHandler(networkPrefabAsset.GetComponent<NetworkObject>().GlobalObjectIdHash);
		}

		public bool RemoveHandler(NetworkObject networkObject)
		{
			return RemoveHandler(networkObject.GlobalObjectIdHash);
		}

		public bool RemoveHandler(uint globalObjectIdHash)
		{
			if (m_PrefabInstanceToPrefabAsset.ContainsValue(globalObjectIdHash))
			{
				uint key = 0u;
				foreach (KeyValuePair<uint, uint> item in m_PrefabInstanceToPrefabAsset)
				{
					if (item.Value == globalObjectIdHash)
					{
						key = item.Key;
						break;
					}
				}
				m_PrefabInstanceToPrefabAsset.Remove(key);
			}
			return m_PrefabAssetToPrefabHandler.Remove(globalObjectIdHash);
		}

		internal bool ContainsHandler(GameObject networkPrefab)
		{
			return ContainsHandler(networkPrefab.GetComponent<NetworkObject>().GlobalObjectIdHash);
		}

		internal bool ContainsHandler(NetworkObject networkObject)
		{
			return ContainsHandler(networkObject.GlobalObjectIdHash);
		}

		internal bool ContainsHandler(uint networkPrefabHash)
		{
			if (!m_PrefabAssetToPrefabHandler.ContainsKey(networkPrefabHash))
			{
				return m_PrefabInstanceToPrefabAsset.ContainsKey(networkPrefabHash);
			}
			return true;
		}

		internal uint GetSourceGlobalObjectIdHash(uint networkPrefabHash)
		{
			if (m_PrefabAssetToPrefabHandler.ContainsKey(networkPrefabHash))
			{
				return networkPrefabHash;
			}
			if (m_PrefabInstanceToPrefabAsset.TryGetValue(networkPrefabHash, out var value))
			{
				return value;
			}
			return 0u;
		}

		internal NetworkObject HandleNetworkPrefabSpawn(uint networkPrefabAssetHash, ulong ownerClientId, Vector3 position, Quaternion rotation)
		{
			if (m_PrefabAssetToPrefabHandler.TryGetValue(networkPrefabAssetHash, out var value))
			{
				NetworkObject networkObject = value.Instantiate(ownerClientId, position, rotation);
				if (networkObject != null && !m_PrefabInstanceToPrefabAsset.ContainsKey(networkObject.GlobalObjectIdHash))
				{
					m_PrefabInstanceToPrefabAsset.Add(networkObject.GlobalObjectIdHash, networkPrefabAssetHash);
				}
				return networkObject;
			}
			return null;
		}

		internal void HandleNetworkPrefabDestroy(NetworkObject networkObjectInstance)
		{
			uint globalObjectIdHash = networkObjectInstance.GlobalObjectIdHash;
			INetworkPrefabInstanceHandler value3;
			if (m_PrefabInstanceToPrefabAsset.TryGetValue(globalObjectIdHash, out var value))
			{
				if (m_PrefabAssetToPrefabHandler.TryGetValue(value, out var value2))
				{
					value2.Destroy(networkObjectInstance);
				}
			}
			else if (m_PrefabAssetToPrefabHandler.TryGetValue(globalObjectIdHash, out value3))
			{
				value3.Destroy(networkObjectInstance);
			}
		}

		public GameObject GetNetworkPrefabOverride(GameObject gameObject)
		{
			if (gameObject.TryGetComponent<NetworkObject>(out var component) && m_NetworkManager.NetworkConfig.Prefabs.NetworkPrefabOverrideLinks.ContainsKey(component.GlobalObjectIdHash))
			{
				NetworkPrefabOverride @override = m_NetworkManager.NetworkConfig.Prefabs.NetworkPrefabOverrideLinks[component.GlobalObjectIdHash].Override;
				if ((uint)(@override - 1) <= 1u)
				{
					return m_NetworkManager.NetworkConfig.Prefabs.NetworkPrefabOverrideLinks[component.GlobalObjectIdHash].OverridingTargetPrefab;
				}
			}
			return gameObject;
		}

		public void AddNetworkPrefab(GameObject prefab)
		{
			if (m_NetworkManager.IsListening && m_NetworkManager.NetworkConfig.ForceSamePrefabs)
			{
				throw new Exception("All prefabs must be registered before starting NetworkManager when ForceSamePrefabs is enabled.");
			}
			NetworkObject component = prefab.GetComponent<NetworkObject>();
			if (!component)
			{
				throw new Exception("All NetworkPrefabs must contain a NetworkObject component.");
			}
			NetworkPrefab networkPrefab = new NetworkPrefab
			{
				Prefab = prefab
			};
			bool flag = m_NetworkManager.NetworkConfig.Prefabs.Add(networkPrefab);
			if (m_NetworkManager.IsListening && flag)
			{
				m_NetworkManager.DeferredMessageManager.ProcessTriggers(IDeferredNetworkMessageManager.TriggerType.OnAddPrefab, component.GlobalObjectIdHash);
			}
		}

		public void RemoveNetworkPrefab(GameObject prefab)
		{
			if (m_NetworkManager.IsListening && m_NetworkManager.NetworkConfig.ForceSamePrefabs)
			{
				throw new Exception("Prefabs cannot be removed after starting NetworkManager when ForceSamePrefabs is enabled.");
			}
			uint globalObjectIdHash = prefab.GetComponent<NetworkObject>().GlobalObjectIdHash;
			m_NetworkManager.NetworkConfig.Prefabs.Remove(prefab);
			if (ContainsHandler(globalObjectIdHash))
			{
				RemoveHandler(globalObjectIdHash);
			}
		}

		internal void RegisterPlayerPrefab()
		{
			NetworkConfig networkConfig = m_NetworkManager.NetworkConfig;
			if (!(networkConfig.PlayerPrefab != null))
			{
				return;
			}
			if (networkConfig.PlayerPrefab.TryGetComponent<NetworkObject>(out var component))
			{
				if (!networkConfig.Prefabs.NetworkPrefabOverrideLinks.ContainsKey(component.GlobalObjectIdHash))
				{
					AddNetworkPrefab(networkConfig.PlayerPrefab);
				}
			}
			else
			{
				Debug.LogError("PlayerPrefab (\"" + networkConfig.PlayerPrefab.name + "\") has no NetworkObject assigned to it!.");
			}
		}

		internal void Initialize(NetworkManager networkManager)
		{
			m_NetworkManager = networkManager;
		}
	}
}
