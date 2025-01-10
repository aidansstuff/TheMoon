using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.Netcode
{
	public class NetworkSpawnManager
	{
		internal Dictionary<ulong, List<NetworkObject>> ObjectsToShowToClient = new Dictionary<ulong, List<NetworkObject>>();

		public readonly Dictionary<ulong, NetworkObject> SpawnedObjects = new Dictionary<ulong, NetworkObject>();

		public readonly HashSet<NetworkObject> SpawnedObjectsList = new HashSet<NetworkObject>();

		public readonly Dictionary<ulong, Dictionary<ulong, NetworkObject>> OwnershipToObjectsTable = new Dictionary<ulong, Dictionary<ulong, NetworkObject>>();

		private Dictionary<ulong, ulong> m_ObjectToOwnershipTable = new Dictionary<ulong, ulong>();

		internal readonly Queue<ReleasedNetworkId> ReleasedNetworkObjectIds = new Queue<ReleasedNetworkId>();

		private ulong m_NetworkObjectIdCounter;

		private List<ulong> m_TargetClientIds = new List<ulong>();

		public NetworkManager NetworkManager { get; }

		internal void MarkObjectForShowingTo(NetworkObject networkObject, ulong clientId)
		{
			if (!ObjectsToShowToClient.ContainsKey(clientId))
			{
				ObjectsToShowToClient.Add(clientId, new List<NetworkObject>());
			}
			ObjectsToShowToClient[clientId].Add(networkObject);
		}

		internal bool RemoveObjectFromShowingTo(NetworkObject networkObject, ulong clientId)
		{
			bool flag = false;
			if (!ObjectsToShowToClient.ContainsKey(clientId))
			{
				return false;
			}
			while (ObjectsToShowToClient[clientId].Contains(networkObject))
			{
				Debug.LogWarning("Object was shown and hidden from the same client in the same Network frame. As a result, the client will _not_ receive a NetworkSpawn");
				ObjectsToShowToClient[clientId].Remove(networkObject);
				flag = true;
			}
			if (flag)
			{
				networkObject.Observers.Remove(clientId);
			}
			return flag;
		}

		internal void UpdateOwnershipTable(NetworkObject networkObject, ulong newOwner, bool isRemoving = false)
		{
			ulong num = newOwner;
			if (m_ObjectToOwnershipTable.ContainsKey(networkObject.NetworkObjectId))
			{
				num = m_ObjectToOwnershipTable[networkObject.NetworkObjectId];
				if (isRemoving)
				{
					m_ObjectToOwnershipTable.Remove(networkObject.NetworkObjectId);
				}
				else
				{
					m_ObjectToOwnershipTable[networkObject.NetworkObjectId] = newOwner;
				}
			}
			else
			{
				m_ObjectToOwnershipTable.Add(networkObject.NetworkObjectId, newOwner);
			}
			if (num != newOwner && OwnershipToObjectsTable.ContainsKey(num))
			{
				if (!OwnershipToObjectsTable[num].ContainsKey(networkObject.NetworkObjectId))
				{
					throw new Exception(string.Format("Client-ID {0} had a partial {1} entry! Potentially corrupted {2}?", num, "m_ObjectToOwnershipTable", "OwnershipToObjectsTable"));
				}
				OwnershipToObjectsTable[num].Remove(networkObject.NetworkObjectId);
				if (isRemoving)
				{
					return;
				}
			}
			if (!OwnershipToObjectsTable.ContainsKey(newOwner))
			{
				OwnershipToObjectsTable.Add(newOwner, new Dictionary<ulong, NetworkObject>());
			}
			if (!OwnershipToObjectsTable[newOwner].ContainsKey(networkObject.NetworkObjectId))
			{
				OwnershipToObjectsTable[newOwner].Add(networkObject.NetworkObjectId, networkObject);
			}
			else if (isRemoving)
			{
				OwnershipToObjectsTable[num].Remove(networkObject.NetworkObjectId);
			}
			else if (NetworkManager.LogLevel == LogLevel.Developer)
			{
				NetworkLog.LogWarning($"Setting ownership twice? Client-ID {num} already owns NetworkObject ID {networkObject.NetworkObjectId}!");
			}
		}

		public List<NetworkObject> GetClientOwnedObjects(ulong clientId)
		{
			if (!OwnershipToObjectsTable.ContainsKey(clientId))
			{
				OwnershipToObjectsTable.Add(clientId, new Dictionary<ulong, NetworkObject>());
			}
			return OwnershipToObjectsTable[clientId].Values.ToList();
		}

		internal ulong GetNetworkObjectId()
		{
			if (ReleasedNetworkObjectIds.Count > 0 && NetworkManager.NetworkConfig.RecycleNetworkIds && NetworkManager.RealTimeProvider.UnscaledTime - ReleasedNetworkObjectIds.Peek().ReleaseTime >= NetworkManager.NetworkConfig.NetworkIdRecycleDelay)
			{
				return ReleasedNetworkObjectIds.Dequeue().NetworkId;
			}
			m_NetworkObjectIdCounter++;
			return m_NetworkObjectIdCounter;
		}

		public NetworkObject GetLocalPlayerObject()
		{
			return GetPlayerNetworkObject(NetworkManager.LocalClientId);
		}

		public NetworkObject GetPlayerNetworkObject(ulong clientId)
		{
			if (!NetworkManager.IsServer && NetworkManager.LocalClientId != clientId)
			{
				throw new NotServerException("Only the server can find player objects from other clients.");
			}
			if (TryGetNetworkClient(clientId, out var networkClient))
			{
				return networkClient.PlayerObject;
			}
			return null;
		}

		private bool TryGetNetworkClient(ulong clientId, out NetworkClient networkClient)
		{
			if (NetworkManager.IsServer)
			{
				return NetworkManager.ConnectedClients.TryGetValue(clientId, out networkClient);
			}
			if (NetworkManager.LocalClient != null && clientId == NetworkManager.LocalClient.ClientId)
			{
				networkClient = NetworkManager.LocalClient;
				return true;
			}
			networkClient = null;
			return false;
		}

		internal void RemoveOwnership(NetworkObject networkObject)
		{
			ChangeOwnership(networkObject, 0uL);
		}

		internal void ChangeOwnership(NetworkObject networkObject, ulong clientId)
		{
			if (!NetworkManager.IsServer)
			{
				throw new NotServerException("Only the server can change ownership");
			}
			if (!networkObject.IsSpawned)
			{
				throw new SpawnStateException("Object is not spawned");
			}
			networkObject.OwnerClientId = clientId;
			networkObject.InvokeBehaviourOnLostOwnership();
			networkObject.MarkVariablesDirty(dirty: true);
			NetworkManager.BehaviourUpdater.AddForUpdate(networkObject);
			UpdateOwnershipTable(networkObject, networkObject.OwnerClientId);
			networkObject.InvokeBehaviourOnGainedOwnership();
			ChangeOwnershipMessage changeOwnershipMessage = default(ChangeOwnershipMessage);
			changeOwnershipMessage.NetworkObjectId = networkObject.NetworkObjectId;
			changeOwnershipMessage.OwnerClientId = networkObject.OwnerClientId;
			ChangeOwnershipMessage message = changeOwnershipMessage;
			foreach (KeyValuePair<ulong, NetworkClient> connectedClient in NetworkManager.ConnectedClients)
			{
				if (networkObject.IsNetworkVisibleTo(connectedClient.Value.ClientId))
				{
					int num = NetworkManager.ConnectionManager.SendMessage(ref message, NetworkDelivery.ReliableSequenced, connectedClient.Value.ClientId);
					NetworkManager.NetworkMetrics.TrackOwnershipChangeSent(connectedClient.Key, networkObject, num);
				}
			}
		}

		internal bool HasPrefab(NetworkObject.SceneObject sceneObject)
		{
			if (!NetworkManager.NetworkConfig.EnableSceneManagement || !sceneObject.IsSceneObject)
			{
				if (NetworkManager.PrefabHandler.ContainsHandler(sceneObject.Hash))
				{
					return true;
				}
				if (NetworkManager.NetworkConfig.Prefabs.NetworkPrefabOverrideLinks.TryGetValue(sceneObject.Hash, out var value))
				{
					NetworkPrefabOverride @override = value.Override;
					if (@override == NetworkPrefabOverride.None || (uint)(@override - 1) > 1u)
					{
						return value.Prefab != null;
					}
					return value.OverridingTargetPrefab != null;
				}
				return false;
			}
			return NetworkManager.SceneManager.GetSceneRelativeInSceneNetworkObject(sceneObject.Hash, sceneObject.NetworkSceneHandle) != null;
		}

		internal NetworkObject CreateLocalNetworkObject(NetworkObject.SceneObject sceneObject)
		{
			NetworkObject networkObject = null;
			uint hash = sceneObject.Hash;
			Vector3 vector = (sceneObject.HasTransform ? sceneObject.Transform.Position : default(Vector3));
			Quaternion quaternion = (sceneObject.HasTransform ? sceneObject.Transform.Rotation : default(Quaternion));
			Vector3 localScale = (sceneObject.HasTransform ? sceneObject.Transform.Scale : default(Vector3));
			ulong value = (sceneObject.HasParent ? sceneObject.ParentObjectId : 0);
			bool flag = !sceneObject.HasParent || sceneObject.WorldPositionStays;
			bool flag2 = false;
			if (!NetworkManager.NetworkConfig.EnableSceneManagement || !sceneObject.IsSceneObject)
			{
				if (NetworkManager.PrefabHandler.ContainsHandler(hash))
				{
					networkObject = NetworkManager.PrefabHandler.HandleNetworkPrefabSpawn(hash, sceneObject.OwnerClientId, vector, quaternion);
					networkObject.NetworkManagerOwner = NetworkManager;
					flag2 = true;
				}
				else
				{
					GameObject gameObject = null;
					if (NetworkManager.NetworkConfig.Prefabs.NetworkPrefabOverrideLinks.ContainsKey(hash))
					{
						NetworkPrefabOverride @override = NetworkManager.NetworkConfig.Prefabs.NetworkPrefabOverrideLinks[hash].Override;
						gameObject = ((@override != 0 && (uint)(@override - 1) <= 1u) ? NetworkManager.NetworkConfig.Prefabs.NetworkPrefabOverrideLinks[hash].OverridingTargetPrefab : NetworkManager.NetworkConfig.Prefabs.NetworkPrefabOverrideLinks[hash].Prefab);
					}
					if (gameObject == null)
					{
						if (NetworkLog.CurrentLogLevel <= LogLevel.Error)
						{
							NetworkLog.LogError(string.Format("Failed to create object locally. [{0}={1}]. {2} could not be found. Is the prefab registered with {3}?", "globalObjectIdHash", hash, "NetworkPrefab", "NetworkManager"));
						}
					}
					else
					{
						networkObject = UnityEngine.Object.Instantiate(gameObject).GetComponent<NetworkObject>();
						networkObject.NetworkManagerOwner = NetworkManager;
					}
				}
			}
			else
			{
				networkObject = NetworkManager.SceneManager.GetSceneRelativeInSceneNetworkObject(hash, sceneObject.NetworkSceneHandle);
				if (networkObject == null && NetworkLog.CurrentLogLevel <= LogLevel.Error)
				{
					NetworkLog.LogError(string.Format("{0} hash was not found! In-Scene placed {1} soft synchronization failure for Hash: {2}!", "NetworkPrefab", "NetworkObject", hash));
				}
				if (networkObject != null && !networkObject.gameObject.activeInHierarchy)
				{
					networkObject.gameObject.SetActive(value: true);
				}
			}
			if (networkObject != null)
			{
				networkObject.DestroyWithScene = sceneObject.DestroyWithScene;
				networkObject.NetworkSceneHandle = sceneObject.NetworkSceneHandle;
				if (sceneObject.IsSceneObject && !sceneObject.HasParent && networkObject.transform.parent != null && networkObject.transform.parent.GetComponent<NetworkObject>() != null)
				{
					networkObject.ApplyNetworkParenting(removeParent: true, ignoreNotSpawned: true);
				}
				if (sceneObject.HasTransform && !flag2)
				{
					if (flag || !networkObject.AutoObjectParentSync)
					{
						networkObject.transform.position = vector;
						networkObject.transform.rotation = quaternion;
					}
					else
					{
						networkObject.transform.localPosition = vector;
						networkObject.transform.localRotation = quaternion;
					}
					if (!sceneObject.IsPlayerObject)
					{
						networkObject.transform.localScale = localScale;
					}
				}
				if (sceneObject.HasParent)
				{
					ulong? latestParent = null;
					if (sceneObject.IsLatestParentSet)
					{
						latestParent = value;
					}
					networkObject.SetNetworkParenting(latestParent, flag);
				}
				if (!sceneObject.IsSceneObject && NetworkSceneManager.IsSpawnedObjectsPendingInDontDestroyOnLoad)
				{
					UnityEngine.Object.DontDestroyOnLoad(networkObject.gameObject);
				}
			}
			return networkObject;
		}

		internal void SpawnNetworkObjectLocally(NetworkObject networkObject, ulong networkId, bool sceneObject, bool playerObject, ulong ownerClientId, bool destroyWithScene)
		{
			if (networkObject == null)
			{
				throw new ArgumentNullException("networkObject", "Cannot spawn null object");
			}
			if (networkObject.IsSpawned)
			{
				throw new SpawnStateException("Object is already spawned");
			}
			if (!sceneObject && networkObject.GetComponentsInChildren<NetworkObject>().Length > 1)
			{
				Debug.LogError("Spawning NetworkObjects with nested NetworkObjects is only supported for scene objects. Child NetworkObjects will not be spawned over the network!");
			}
			SpawnNetworkObjectLocallyCommon(networkObject, networkId, sceneObject, playerObject, ownerClientId, destroyWithScene);
		}

		internal void SpawnNetworkObjectLocally(NetworkObject networkObject, in NetworkObject.SceneObject sceneObject, bool destroyWithScene)
		{
			if (networkObject == null)
			{
				throw new ArgumentNullException("networkObject", "Cannot spawn null object");
			}
			if (networkObject.IsSpawned)
			{
				throw new SpawnStateException("Object is already spawned");
			}
			SpawnNetworkObjectLocallyCommon(networkObject, sceneObject.NetworkObjectId, sceneObject.IsSceneObject, sceneObject.IsPlayerObject, sceneObject.OwnerClientId, destroyWithScene);
		}

		private void SpawnNetworkObjectLocallyCommon(NetworkObject networkObject, ulong networkId, bool sceneObject, bool playerObject, ulong ownerClientId, bool destroyWithScene)
		{
			if (SpawnedObjects.ContainsKey(networkId))
			{
				Debug.LogWarning(string.Format("Trying to spawn {0} {1} that already exists!", "NetworkObjectId", networkId));
				return;
			}
			networkObject.IsSpawned = true;
			networkObject.IsSceneObject = sceneObject;
			if (networkObject.IsSceneObject != false && networkObject.SceneOriginHandle == 0)
			{
				networkObject.SceneOrigin = networkObject.gameObject.scene;
			}
			if (networkObject.NetworkManagerOwner != NetworkManager)
			{
				networkObject.NetworkManagerOwner = NetworkManager;
			}
			networkObject.NetworkObjectId = networkId;
			networkObject.DestroyWithScene = sceneObject || destroyWithScene;
			networkObject.OwnerClientId = ownerClientId;
			networkObject.IsPlayerObject = playerObject;
			SpawnedObjects.Add(networkObject.NetworkObjectId, networkObject);
			SpawnedObjectsList.Add(networkObject);
			if (NetworkManager.IsServer)
			{
				if (playerObject)
				{
					if (NetworkManager.ConnectedClients[ownerClientId].PlayerObject != null)
					{
						NetworkManager.ConnectedClients[ownerClientId].PlayerObject.IsPlayerObject = false;
					}
					NetworkManager.ConnectedClients[ownerClientId].PlayerObject = networkObject;
				}
			}
			else if (ownerClientId == NetworkManager.LocalClientId && playerObject)
			{
				if (NetworkManager.LocalClient.PlayerObject != null)
				{
					NetworkManager.LocalClient.PlayerObject.IsPlayerObject = false;
				}
				NetworkManager.LocalClient.PlayerObject = networkObject;
			}
			if (NetworkManager.IsServer && networkObject.SpawnWithObservers)
			{
				for (int i = 0; i < NetworkManager.ConnectedClientsList.Count; i++)
				{
					if (networkObject.CheckObjectVisibility == null || networkObject.CheckObjectVisibility(NetworkManager.ConnectedClientsList[i].ClientId))
					{
						networkObject.Observers.Add(NetworkManager.ConnectedClientsList[i].ClientId);
					}
				}
			}
			networkObject.ApplyNetworkParenting();
			NetworkObject.CheckOrphanChildren();
			networkObject.InvokeBehaviourNetworkSpawn();
			NetworkManager.DeferredMessageManager.ProcessTriggers(IDeferredNetworkMessageManager.TriggerType.OnSpawn, networkId);
			NetworkObject[] componentsInChildren = networkObject.GetComponentsInChildren<NetworkObject>();
			foreach (NetworkObject networkObject2 in componentsInChildren)
			{
				if (!networkObject2.IsSceneObject.HasValue || networkObject2.IsSceneObject.Value)
				{
					networkObject2.IsSceneObject = sceneObject;
				}
			}
			if (!sceneObject)
			{
				networkObject.SubscribeToActiveSceneForSynch();
			}
		}

		internal void SendSpawnCallForObject(ulong clientId, NetworkObject networkObject)
		{
			if (clientId != 0L)
			{
				CreateObjectMessage createObjectMessage = default(CreateObjectMessage);
				createObjectMessage.ObjectInfo = networkObject.GetMessageSceneObject(clientId);
				CreateObjectMessage message = createObjectMessage;
				int num = NetworkManager.ConnectionManager.SendMessage(ref message, NetworkDelivery.ReliableFragmentedSequenced, clientId);
				NetworkManager.NetworkMetrics.TrackObjectSpawnSent(clientId, networkObject, num);
			}
		}

		internal ulong? GetSpawnParentId(NetworkObject networkObject)
		{
			NetworkObject networkObject2 = null;
			if (!networkObject.AlwaysReplicateAsRoot && networkObject.transform.parent != null)
			{
				networkObject2 = networkObject.transform.parent.GetComponent<NetworkObject>();
			}
			if (networkObject2 == null)
			{
				return null;
			}
			return networkObject2.NetworkObjectId;
		}

		internal void DespawnObject(NetworkObject networkObject, bool destroyObject = false)
		{
			if (!networkObject.IsSpawned)
			{
				throw new SpawnStateException("Object is not spawned");
			}
			if (!NetworkManager.IsServer)
			{
				throw new NotServerException("Only server can despawn objects");
			}
			OnDespawnObject(networkObject, destroyObject);
		}

		internal void ServerResetShudownStateForSceneObjects()
		{
			foreach (NetworkObject item in from c in UnityEngine.Object.FindObjectsOfType<NetworkObject>()
				where c.IsSceneObject.HasValue && c.IsSceneObject == true
				select c)
			{
				item.IsSpawned = false;
				item.DestroyWithScene = false;
				item.IsSceneObject = null;
			}
		}

		internal void ServerDestroySpawnedSceneObjects()
		{
			foreach (NetworkObject item in SpawnedObjectsList.ToList())
			{
				if (item.IsSceneObject.HasValue && item.IsSceneObject.Value && item.DestroyWithScene && item.gameObject.scene != NetworkManager.SceneManager.DontDestroyOnLoadScene)
				{
					SpawnedObjectsList.Remove(item);
					UnityEngine.Object.Destroy(item.gameObject);
				}
			}
		}

		internal void DespawnAndDestroyNetworkObjects()
		{
			NetworkObject[] array = UnityEngine.Object.FindObjectsOfType<NetworkObject>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].NetworkManager == NetworkManager)
				{
					if (NetworkManager.PrefabHandler.ContainsHandler(array[i]))
					{
						OnDespawnObject(array[i], destroyGameObject: false);
						NetworkManager.PrefabHandler.HandleNetworkPrefabDestroy(array[i]);
					}
					else if (array[i].IsSpawned)
					{
						bool destroyGameObject = !array[i].IsSceneObject.HasValue || !array[i].IsSceneObject.Value;
						OnDespawnObject(array[i], destroyGameObject);
					}
					else if (array[i].IsSceneObject.HasValue && !array[i].IsSceneObject.Value)
					{
						UnityEngine.Object.Destroy(array[i].gameObject);
					}
				}
			}
		}

		internal void DestroySceneObjects()
		{
			NetworkObject[] array = UnityEngine.Object.FindObjectsOfType<NetworkObject>();
			for (int i = 0; i < array.Length; i++)
			{
				if (!(array[i].NetworkManager == NetworkManager) || (array[i].IsSceneObject.HasValue && !array[i].IsSceneObject.Value))
				{
					continue;
				}
				if (NetworkManager.PrefabHandler.ContainsHandler(array[i]))
				{
					NetworkManager.PrefabHandler.HandleNetworkPrefabDestroy(array[i]);
					if (SpawnedObjects.ContainsKey(array[i].NetworkObjectId))
					{
						OnDespawnObject(array[i], destroyGameObject: false);
					}
				}
				else
				{
					UnityEngine.Object.Destroy(array[i].gameObject);
				}
			}
		}

		internal void ServerSpawnSceneObjectsOnStartSweep()
		{
			NetworkObject[] array = UnityEngine.Object.FindObjectsOfType<NetworkObject>();
			List<NetworkObject> list = new List<NetworkObject>();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].NetworkManager == NetworkManager && !array[i].IsSceneObject.HasValue)
				{
					list.Add(array[i]);
				}
			}
			foreach (NetworkObject item in list)
			{
				SpawnNetworkObjectLocally(item, GetNetworkObjectId(), sceneObject: true, playerObject: false, item.OwnerClientId, destroyWithScene: true);
			}
		}

		internal void OnDespawnObject(NetworkObject networkObject, bool destroyGameObject)
		{
			if (NetworkManager == null)
			{
				return;
			}
			if (networkObject == null)
			{
				Debug.LogWarning("Trying to destroy network object but it is null");
				return;
			}
			if (!SpawnedObjects.ContainsKey(networkObject.NetworkObjectId))
			{
				Debug.LogWarning($"Trying to destroy object {networkObject.NetworkObjectId} but it doesn't seem to exist anymore!");
				return;
			}
			if (!NetworkManager.ShutdownInProgress && NetworkManager.IsServer)
			{
				foreach (NetworkObject spawnedObjects in SpawnedObjectsList)
				{
					ulong? networkParenting = spawnedObjects.GetNetworkParenting();
					if (networkParenting.HasValue && networkParenting.Value == networkObject.NetworkObjectId)
					{
						if (!spawnedObjects.TryRemoveParentCachedWorldPositionStays() && NetworkLog.CurrentLogLevel <= LogLevel.Normal)
						{
							NetworkLog.LogError(string.Format("{0} #{1} could not be moved to the root when its parent {2} #{3} was being destroyed", "NetworkObject", spawnedObjects.NetworkObjectId, "NetworkObject", networkObject.NetworkObjectId));
						}
						if (NetworkLog.CurrentLogLevel <= LogLevel.Normal)
						{
							NetworkLog.LogWarning(string.Format("{0} #{1} moved to the root because its parent {2} #{3} is destroyed", "NetworkObject", spawnedObjects.NetworkObjectId, "NetworkObject", networkObject.NetworkObjectId));
						}
					}
				}
			}
			networkObject.InvokeBehaviourNetworkDespawn();
			if (NetworkManager != null && NetworkManager.IsServer)
			{
				if (NetworkManager.NetworkConfig.RecycleNetworkIds)
				{
					ReleasedNetworkObjectIds.Enqueue(new ReleasedNetworkId
					{
						NetworkId = networkObject.NetworkObjectId,
						ReleaseTime = NetworkManager.RealTimeProvider.UnscaledTime
					});
				}
				if (networkObject != null && NetworkManager.ConnectedClientsList.Count > 0)
				{
					m_TargetClientIds.Clear();
					foreach (ulong connectedClientsId in NetworkManager.ConnectedClientsIds)
					{
						if (networkObject.IsNetworkVisibleTo(connectedClientsId))
						{
							m_TargetClientIds.Add(connectedClientsId);
						}
					}
					DestroyObjectMessage destroyObjectMessage = default(DestroyObjectMessage);
					destroyObjectMessage.NetworkObjectId = networkObject.NetworkObjectId;
					destroyObjectMessage.DestroyGameObject = networkObject.IsSceneObject == false || destroyGameObject;
					DestroyObjectMessage message = destroyObjectMessage;
					int num = NetworkManager.ConnectionManager.SendMessage(ref message, NetworkDelivery.ReliableSequenced, in m_TargetClientIds);
					foreach (ulong targetClientId in m_TargetClientIds)
					{
						NetworkManager.NetworkMetrics.TrackObjectDestroySent(targetClientId, networkObject, num);
					}
				}
			}
			networkObject.IsSpawned = false;
			if (SpawnedObjects.Remove(networkObject.NetworkObjectId))
			{
				SpawnedObjectsList.Remove(networkObject);
			}
			networkObject.Observers.Clear();
			GameObject gameObject = networkObject.gameObject;
			if (destroyGameObject && gameObject != null)
			{
				if (NetworkManager.PrefabHandler.ContainsHandler(networkObject))
				{
					NetworkManager.PrefabHandler.HandleNetworkPrefabDestroy(networkObject);
				}
				else
				{
					UnityEngine.Object.Destroy(gameObject);
				}
			}
		}

		internal void UpdateObservedNetworkObjects(ulong clientId)
		{
			foreach (NetworkObject spawnedObjects in SpawnedObjectsList)
			{
				if (spawnedObjects.CheckObjectVisibility == null)
				{
					if (!spawnedObjects.Observers.Contains(clientId) && (spawnedObjects.SpawnWithObservers || clientId == 0L))
					{
						spawnedObjects.Observers.Add(clientId);
					}
				}
				else if (spawnedObjects.CheckObjectVisibility(clientId))
				{
					spawnedObjects.Observers.Add(clientId);
				}
				else if (spawnedObjects.Observers.Contains(clientId))
				{
					spawnedObjects.Observers.Remove(clientId);
				}
			}
		}

		internal void HandleNetworkObjectShow()
		{
			foreach (KeyValuePair<ulong, List<NetworkObject>> item in ObjectsToShowToClient)
			{
				ulong key = item.Key;
				foreach (NetworkObject item2 in item.Value)
				{
					SendSpawnCallForObject(key, item2);
				}
			}
			ObjectsToShowToClient.Clear();
		}

		internal NetworkSpawnManager(NetworkManager networkManager)
		{
			NetworkManager = networkManager;
		}
	}
}
