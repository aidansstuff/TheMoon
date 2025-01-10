using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.Netcode
{
	[AddComponentMenu("Netcode/Network Object", -99)]
	[DisallowMultipleComponent]
	public sealed class NetworkObject : MonoBehaviour
	{
		public delegate bool VisibilityDelegate(ulong clientId);

		public delegate bool SpawnDelegate(ulong clientId);

		internal struct SceneObject
		{
			public struct TransformData : INetworkSerializeByMemcpy
			{
				public Vector3 Position;

				public Quaternion Rotation;

				public Vector3 Scale;
			}

			private byte m_BitField;

			public uint Hash;

			public ulong NetworkObjectId;

			public ulong OwnerClientId;

			public ulong ParentObjectId;

			public TransformData Transform;

			public ulong? LatestParent;

			public NetworkObject OwnerObject;

			public ulong TargetClientId;

			public int NetworkSceneHandle;

			public bool IsPlayerObject
			{
				get
				{
					return ByteUtility.GetBit(m_BitField, 0);
				}
				set
				{
					ByteUtility.SetBit(ref m_BitField, 0, value);
				}
			}

			public bool HasParent
			{
				get
				{
					return ByteUtility.GetBit(m_BitField, 1);
				}
				set
				{
					ByteUtility.SetBit(ref m_BitField, 1, value);
				}
			}

			public bool IsSceneObject
			{
				get
				{
					return ByteUtility.GetBit(m_BitField, 2);
				}
				set
				{
					ByteUtility.SetBit(ref m_BitField, 2, value);
				}
			}

			public bool HasTransform
			{
				get
				{
					return ByteUtility.GetBit(m_BitField, 3);
				}
				set
				{
					ByteUtility.SetBit(ref m_BitField, 3, value);
				}
			}

			public bool IsLatestParentSet
			{
				get
				{
					return ByteUtility.GetBit(m_BitField, 4);
				}
				set
				{
					ByteUtility.SetBit(ref m_BitField, 4, value);
				}
			}

			public bool WorldPositionStays
			{
				get
				{
					return ByteUtility.GetBit(m_BitField, 5);
				}
				set
				{
					ByteUtility.SetBit(ref m_BitField, 5, value);
				}
			}

			public bool DestroyWithScene
			{
				get
				{
					return ByteUtility.GetBit(m_BitField, 6);
				}
				set
				{
					ByteUtility.SetBit(ref m_BitField, 6, value);
				}
			}

			public void Serialize(FastBufferWriter writer)
			{
				writer.WriteValueSafe(in m_BitField, default(FastBufferWriter.ForPrimitives));
				writer.WriteValueSafe(in Hash, default(FastBufferWriter.ForPrimitives));
				BytePacker.WriteValueBitPacked(writer, NetworkObjectId);
				BytePacker.WriteValueBitPacked(writer, OwnerClientId);
				if (HasParent)
				{
					BytePacker.WriteValueBitPacked(writer, ParentObjectId);
					if (IsLatestParentSet)
					{
						BytePacker.WriteValueBitPacked(writer, LatestParent.Value);
					}
				}
				int num = 0;
				num += (HasTransform ? FastBufferWriter.GetWriteSize<TransformData>() : 0);
				num += FastBufferWriter.GetWriteSize<int>();
				if (!writer.TryBeginWrite(num))
				{
					throw new OverflowException("Could not serialize SceneObject: Out of buffer space.");
				}
				if (HasTransform)
				{
					writer.WriteValue(in Transform, default(FastBufferWriter.ForStructs));
				}
				int value = OwnerObject.GetSceneOriginHandle();
				writer.WriteValue(in value, default(FastBufferWriter.ForPrimitives));
				BufferSerializer<BufferSerializerWriter> serializer = new BufferSerializer<BufferSerializerWriter>(new BufferSerializerWriter(writer));
				OwnerObject.SynchronizeNetworkBehaviours(ref serializer, TargetClientId);
			}

			public void Deserialize(FastBufferReader reader)
			{
				reader.ReadValueSafe(out m_BitField, default(FastBufferWriter.ForPrimitives));
				reader.ReadValueSafe(out Hash, default(FastBufferWriter.ForPrimitives));
				ByteUnpacker.ReadValueBitPacked(reader, out NetworkObjectId);
				ByteUnpacker.ReadValueBitPacked(reader, out OwnerClientId);
				if (HasParent)
				{
					ByteUnpacker.ReadValueBitPacked(reader, out ParentObjectId);
					if (IsLatestParentSet)
					{
						ByteUnpacker.ReadValueBitPacked(reader, out ulong value);
						LatestParent = value;
					}
				}
				int num = 0;
				num += (HasTransform ? FastBufferWriter.GetWriteSize<TransformData>() : 0);
				num += FastBufferWriter.GetWriteSize<int>();
				if (!reader.TryBeginRead(num))
				{
					throw new OverflowException("Could not deserialize SceneObject: Reading past the end of the buffer");
				}
				if (HasTransform)
				{
					reader.ReadValue(out Transform, default(FastBufferWriter.ForStructs));
				}
				reader.ReadValue(out NetworkSceneHandle, default(FastBufferWriter.ForPrimitives));
			}
		}

		[HideInInspector]
		[SerializeField]
		internal uint GlobalObjectIdHash;

		private bool m_IsPrefab;

		internal NetworkManager NetworkManagerOwner;

		public bool AlwaysReplicateAsRoot;

		public bool SynchronizeTransform = true;

		public bool ActiveSceneSynchronization;

		public bool SceneMigrationSynchronization = true;

		public Action OnMigratedToNewScene;

		[Tooltip("When false, the NetworkObject will spawn with no observers initially. (default is true)")]
		public bool SpawnWithObservers = true;

		public VisibilityDelegate CheckObjectVisibility;

		public SpawnDelegate IncludeTransformWhenSpawning;

		public bool DontDestroyWithOwner;

		public bool AutoObjectParentSync = true;

		internal readonly HashSet<ulong> Observers = new HashSet<ulong>();

		private string m_CachedNameForMetrics;

		private readonly HashSet<ulong> m_EmptyULongHashSet = new HashSet<ulong>();

		internal int SceneOriginHandle;

		internal int NetworkSceneHandle;

		private Scene m_SceneOrigin;

		private ulong? m_LatestParent;

		private Transform m_CachedParent;

		private bool m_CachedWorldPositionStays = true;

		internal static HashSet<NetworkObject> OrphanChildren = new HashSet<NetworkObject>();

		private List<NetworkBehaviour> m_ChildNetworkBehaviours;

		[HideInInspector]
		public uint PrefabIdHash
		{
			get
			{
				foreach (NetworkPrefab prefab in NetworkManager.NetworkConfig.Prefabs.Prefabs)
				{
					if (prefab.Prefab == base.gameObject)
					{
						return GlobalObjectIdHash;
					}
				}
				return 0u;
			}
		}

		public NetworkManager NetworkManager => NetworkManagerOwner ?? NetworkManager.Singleton;

		public ulong NetworkObjectId { get; internal set; }

		public ulong OwnerClientId { get; internal set; }

		public bool IsPlayerObject { get; internal set; }

		public bool IsLocalPlayer
		{
			get
			{
				if (NetworkManager != null && IsPlayerObject)
				{
					return OwnerClientId == NetworkManager.LocalClientId;
				}
				return false;
			}
		}

		public bool IsOwner
		{
			get
			{
				if (NetworkManager != null)
				{
					return OwnerClientId == NetworkManager.LocalClientId;
				}
				return false;
			}
		}

		public bool IsOwnedByServer
		{
			get
			{
				if (NetworkManager != null)
				{
					return OwnerClientId == 0;
				}
				return false;
			}
		}

		public bool IsSpawned { get; internal set; }

		public bool? IsSceneObject { get; internal set; }

		public bool DestroyWithScene { get; set; }

		internal Scene SceneOrigin
		{
			get
			{
				return m_SceneOrigin;
			}
			set
			{
				if (SceneOriginHandle == 0 && value.IsValid() && value.isLoaded)
				{
					m_SceneOrigin = value;
					SceneOriginHandle = value.handle;
				}
			}
		}

		internal List<NetworkBehaviour> ChildNetworkBehaviours
		{
			get
			{
				if (m_ChildNetworkBehaviours != null)
				{
					return m_ChildNetworkBehaviours;
				}
				m_ChildNetworkBehaviours = new List<NetworkBehaviour>();
				NetworkBehaviour[] componentsInChildren = GetComponentsInChildren<NetworkBehaviour>(includeInactive: true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i].NetworkObject == this)
					{
						m_ChildNetworkBehaviours.Add(componentsInChildren[i]);
					}
				}
				return m_ChildNetworkBehaviours;
			}
		}

		internal string GetNameForMetrics()
		{
			return m_CachedNameForMetrics ?? (m_CachedNameForMetrics = base.name);
		}

		public HashSet<ulong>.Enumerator GetObservers()
		{
			if (!IsSpawned)
			{
				return m_EmptyULongHashSet.GetEnumerator();
			}
			return Observers.GetEnumerator();
		}

		public bool IsNetworkVisibleTo(ulong clientId)
		{
			if (!IsSpawned)
			{
				return false;
			}
			return Observers.Contains(clientId);
		}

		internal int GetSceneOriginHandle()
		{
			if (SceneOriginHandle == 0 && IsSpawned && IsSceneObject != false)
			{
				throw new Exception("GetSceneOriginHandle called when SceneOriginHandle is still zero but the NetworkObject is already spawned!");
			}
			if (SceneOriginHandle == 0)
			{
				return base.gameObject.scene.handle;
			}
			return SceneOriginHandle;
		}

		private void Awake()
		{
			SetCachedParent(base.transform.parent);
			SceneOrigin = base.gameObject.scene;
		}

		public void NetworkShow(ulong clientId)
		{
			if (!IsSpawned)
			{
				throw new SpawnStateException("Object is not spawned");
			}
			if (!NetworkManager.IsServer)
			{
				throw new NotServerException("Only server can change visibility");
			}
			if (Observers.Contains(clientId))
			{
				throw new VisibilityChangeException("The object is already visible");
			}
			if (CheckObjectVisibility != null && !CheckObjectVisibility(clientId))
			{
				if (NetworkManager.LogLevel <= LogLevel.Normal)
				{
					NetworkLog.LogWarning(string.Format("[NetworkShow] Trying to make {0} {1} visible to client ({2}) but {3} returned false!", "NetworkObject", base.gameObject.name, clientId, "CheckObjectVisibility"));
				}
			}
			else
			{
				NetworkManager.SpawnManager.MarkObjectForShowingTo(this, clientId);
				Observers.Add(clientId);
			}
		}

		public static void NetworkShow(List<NetworkObject> networkObjects, ulong clientId)
		{
			if (networkObjects == null || networkObjects.Count == 0)
			{
				throw new ArgumentNullException("At least one NetworkObject has to be provided");
			}
			NetworkManager networkManager = networkObjects[0].NetworkManager;
			if (!networkManager.IsServer)
			{
				throw new NotServerException("Only server can change visibility");
			}
			for (int i = 0; i < networkObjects.Count; i++)
			{
				if (!networkObjects[i].IsSpawned)
				{
					throw new SpawnStateException("Object is not spawned");
				}
				if (networkObjects[i].Observers.Contains(clientId))
				{
					throw new VisibilityChangeException(string.Format("{0} with NetworkId: {1} is already visible", "NetworkObject", networkObjects[i].NetworkObjectId));
				}
				if (networkObjects[i].NetworkManager != networkManager)
				{
					throw new ArgumentNullException("All NetworkObjects must belong to the same NetworkManager");
				}
			}
			foreach (NetworkObject networkObject in networkObjects)
			{
				networkObject.NetworkShow(clientId);
			}
		}

		public void NetworkHide(ulong clientId)
		{
			if (!IsSpawned)
			{
				throw new SpawnStateException("Object is not spawned");
			}
			if (!NetworkManager.IsServer)
			{
				throw new NotServerException("Only server can change visibility");
			}
			if (clientId == 0L)
			{
				throw new VisibilityChangeException("Cannot hide an object from the server");
			}
			if (!NetworkManager.SpawnManager.RemoveObjectFromShowingTo(this, clientId))
			{
				if (!Observers.Contains(clientId))
				{
					throw new VisibilityChangeException("The object is already hidden");
				}
				Observers.Remove(clientId);
				DestroyObjectMessage destroyObjectMessage = default(DestroyObjectMessage);
				destroyObjectMessage.NetworkObjectId = NetworkObjectId;
				destroyObjectMessage.DestroyGameObject = !IsSceneObject.Value;
				DestroyObjectMessage message = destroyObjectMessage;
				int num = NetworkManager.ConnectionManager.SendMessage(ref message, NetworkDelivery.ReliableSequenced, clientId);
				NetworkManager.NetworkMetrics.TrackObjectDestroySent(clientId, this, num);
			}
		}

		public static void NetworkHide(List<NetworkObject> networkObjects, ulong clientId)
		{
			if (networkObjects == null || networkObjects.Count == 0)
			{
				throw new ArgumentNullException("At least one NetworkObject has to be provided");
			}
			NetworkManager networkManager = networkObjects[0].NetworkManager;
			if (!networkManager.IsServer)
			{
				throw new NotServerException("Only server can change visibility");
			}
			if (clientId == 0L)
			{
				throw new VisibilityChangeException("Cannot hide an object from the server");
			}
			for (int i = 0; i < networkObjects.Count; i++)
			{
				if (!networkObjects[i].IsSpawned)
				{
					throw new SpawnStateException("Object is not spawned");
				}
				if (!networkObjects[i].Observers.Contains(clientId))
				{
					throw new VisibilityChangeException(string.Format("{0} with {1}: {2} is already hidden", "NetworkObject", "NetworkObjectId", networkObjects[i].NetworkObjectId));
				}
				if (networkObjects[i].NetworkManager != networkManager)
				{
					throw new ArgumentNullException("All NetworkObjects must belong to the same NetworkManager");
				}
			}
			foreach (NetworkObject networkObject in networkObjects)
			{
				networkObject.NetworkHide(clientId);
			}
		}

		private void OnDestroy()
		{
			if (!NetworkManager)
			{
				return;
			}
			NetworkObject value;
			if (NetworkManager.IsListening && !NetworkManager.IsServer && IsSpawned && (!IsSceneObject.HasValue || !IsSceneObject.Value) && !NetworkManager.ShutdownInProgress)
			{
				if (NetworkManager.LogLevel <= LogLevel.Error)
				{
					NetworkLog.LogErrorServer("Destroy a spawned NetworkObject on a non-host client is not valid. Call Destroy or Despawn on the server/host instead.");
				}
			}
			else if (NetworkManager.SpawnManager != null && NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(NetworkObjectId, out value) && this == value)
			{
				NetworkManager.SpawnManager.OnDespawnObject(value, destroyGameObject: false);
			}
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SpawnInternal(bool destroyWithScene, ulong ownerClientId, bool playerObject)
		{
			if (!NetworkManager.IsListening)
			{
				throw new NotListeningException("NetworkManager is not listening, start a server or host before spawning objects");
			}
			if (!NetworkManager.IsServer)
			{
				throw new NotServerException("Only server can spawn NetworkObjects");
			}
			NetworkManager.SpawnManager.SpawnNetworkObjectLocally(this, NetworkManager.SpawnManager.GetNetworkObjectId(), IsSceneObject.HasValue && IsSceneObject.Value, playerObject, ownerClientId, destroyWithScene);
			for (int i = 0; i < NetworkManager.ConnectedClientsList.Count; i++)
			{
				if (Observers.Contains(NetworkManager.ConnectedClientsList[i].ClientId))
				{
					NetworkManager.SpawnManager.SendSpawnCallForObject(NetworkManager.ConnectedClientsList[i].ClientId, this);
				}
			}
		}

		public void Spawn(bool destroyWithScene = false)
		{
			SpawnInternal(destroyWithScene, 0uL, playerObject: false);
		}

		public void SpawnWithOwnership(ulong clientId, bool destroyWithScene = false)
		{
			SpawnInternal(destroyWithScene, clientId, playerObject: false);
		}

		public void SpawnAsPlayerObject(ulong clientId, bool destroyWithScene = false)
		{
			SpawnInternal(destroyWithScene, clientId, playerObject: true);
		}

		public void Despawn(bool destroy = true)
		{
			MarkVariablesDirty(dirty: false);
			NetworkManager.SpawnManager.DespawnObject(this, destroy);
		}

		public void RemoveOwnership()
		{
			NetworkManager.SpawnManager.RemoveOwnership(this);
		}

		public void ChangeOwnership(ulong newOwnerClientId)
		{
			NetworkManager.SpawnManager.ChangeOwnership(this, newOwnerClientId);
		}

		internal void InvokeBehaviourOnLostOwnership()
		{
			if (!NetworkManager.IsServer)
			{
				NetworkManager.SpawnManager.UpdateOwnershipTable(this, OwnerClientId, isRemoving: true);
			}
			for (int i = 0; i < ChildNetworkBehaviours.Count; i++)
			{
				ChildNetworkBehaviours[i].InternalOnLostOwnership();
			}
		}

		internal void InvokeBehaviourOnGainedOwnership()
		{
			if (!NetworkManager.IsServer && NetworkManager.LocalClientId == OwnerClientId)
			{
				NetworkManager.SpawnManager.UpdateOwnershipTable(this, OwnerClientId);
			}
			for (int i = 0; i < ChildNetworkBehaviours.Count; i++)
			{
				if (ChildNetworkBehaviours[i].gameObject.activeInHierarchy)
				{
					ChildNetworkBehaviours[i].InternalOnGainedOwnership();
				}
				else
				{
					Debug.LogWarning(ChildNetworkBehaviours[i].gameObject.name + " is disabled! Netcode for GameObjects does not support disabled NetworkBehaviours! The " + ChildNetworkBehaviours[i].GetType().Name + " component was skipped during ownership assignment!");
				}
			}
		}

		internal void InvokeBehaviourOnNetworkObjectParentChanged(NetworkObject parentNetworkObject)
		{
			for (int i = 0; i < ChildNetworkBehaviours.Count; i++)
			{
				ChildNetworkBehaviours[i].OnNetworkObjectParentChanged(parentNetworkObject);
			}
		}

		public bool WorldPositionStays()
		{
			return m_CachedWorldPositionStays;
		}

		internal void SetCachedParent(Transform parentTransform)
		{
			m_CachedParent = parentTransform;
		}

		internal ulong? GetNetworkParenting()
		{
			return m_LatestParent;
		}

		internal void SetNetworkParenting(ulong? latestParent, bool worldPositionStays)
		{
			m_LatestParent = latestParent;
			m_CachedWorldPositionStays = worldPositionStays;
		}

		public bool TrySetParent(Transform parent, bool worldPositionStays = true)
		{
			if (parent == null)
			{
				return TrySetParent((NetworkObject)null, worldPositionStays);
			}
			NetworkObject component = parent.GetComponent<NetworkObject>();
			if (!(component == null))
			{
				return TrySetParent(component, worldPositionStays);
			}
			return false;
		}

		public bool TrySetParent(GameObject parent, bool worldPositionStays = true)
		{
			if (parent == null)
			{
				return TrySetParent((NetworkObject)null, worldPositionStays);
			}
			NetworkObject component = parent.GetComponent<NetworkObject>();
			if (!(component == null))
			{
				return TrySetParent(component, worldPositionStays);
			}
			return false;
		}

		internal bool TryRemoveParentCachedWorldPositionStays()
		{
			return TrySetParent((NetworkObject)null, m_CachedWorldPositionStays);
		}

		public bool TryRemoveParent(bool worldPositionStays = true)
		{
			return TrySetParent((NetworkObject)null, worldPositionStays);
		}

		public bool TrySetParent(NetworkObject parent, bool worldPositionStays = true)
		{
			if (!AutoObjectParentSync)
			{
				return false;
			}
			if (NetworkManager == null || !NetworkManager.IsListening)
			{
				return false;
			}
			if (!NetworkManager.IsServer)
			{
				return false;
			}
			if (!IsSpawned)
			{
				return false;
			}
			if (parent != null && !parent.IsSpawned)
			{
				return false;
			}
			m_CachedWorldPositionStays = worldPositionStays;
			if (parent == null)
			{
				base.transform.SetParent(null, worldPositionStays);
			}
			else
			{
				base.transform.SetParent(parent.transform, worldPositionStays);
			}
			return true;
		}

		private unsafe void OnTransformParentChanged()
		{
			if (!AutoObjectParentSync || base.transform.parent == m_CachedParent)
			{
				return;
			}
			if (NetworkManager == null || !NetworkManager.IsListening)
			{
				base.transform.parent = m_CachedParent;
				Debug.LogException(new NotListeningException("NetworkManager is not listening, start a server or host before reparenting"));
				return;
			}
			if (!NetworkManager.IsServer)
			{
				base.transform.parent = m_CachedParent;
				Debug.LogException(new NotServerException("Only the server can reparent NetworkObjects"));
				return;
			}
			if (!IsSpawned)
			{
				base.transform.parent = m_CachedParent;
				Debug.LogException(new SpawnStateException("NetworkObject can only be reparented after being spawned"));
				return;
			}
			bool removeParent = false;
			Transform parent = base.transform.parent;
			if (parent != null)
			{
				if (!base.transform.parent.TryGetComponent<NetworkObject>(out var component))
				{
					base.transform.parent = m_CachedParent;
					Debug.LogException(new InvalidParentException("Invalid parenting, NetworkObject moved under a non-NetworkObject parent"));
					return;
				}
				if (!component.IsSpawned)
				{
					base.transform.parent = m_CachedParent;
					Debug.LogException(new SpawnStateException("NetworkObject can only be reparented under another spawned NetworkObject"));
					return;
				}
				m_LatestParent = component.NetworkObjectId;
			}
			else
			{
				m_LatestParent = null;
				removeParent = m_CachedParent != null;
			}
			ApplyNetworkParenting(removeParent);
			ParentSyncMessage parentSyncMessage = default(ParentSyncMessage);
			parentSyncMessage.NetworkObjectId = NetworkObjectId;
			parentSyncMessage.IsLatestParentSet = m_LatestParent.HasValue && m_LatestParent.HasValue;
			parentSyncMessage.LatestParent = m_LatestParent;
			parentSyncMessage.RemoveParent = removeParent;
			parentSyncMessage.WorldPositionStays = m_CachedWorldPositionStays;
			parentSyncMessage.Position = (m_CachedWorldPositionStays ? base.transform.position : base.transform.localPosition);
			parentSyncMessage.Rotation = (m_CachedWorldPositionStays ? base.transform.rotation : base.transform.localRotation);
			parentSyncMessage.Scale = base.transform.localScale;
			ParentSyncMessage message = parentSyncMessage;
			if (parent == null)
			{
				m_CachedWorldPositionStays = true;
			}
			int count = NetworkManager.ConnectedClientsIds.Count;
			ulong* ptr = stackalloc ulong[count];
			int numClientIds = 0;
			foreach (ulong connectedClientsId in NetworkManager.ConnectedClientsIds)
			{
				if (Observers.Contains(connectedClientsId))
				{
					ptr[numClientIds++] = connectedClientsId;
				}
			}
			NetworkManager.ConnectionManager.SendMessage(ref message, NetworkDelivery.ReliableSequenced, ptr, numClientIds);
		}

		internal bool ApplyNetworkParenting(bool removeParent = false, bool ignoreNotSpawned = false)
		{
			if (!AutoObjectParentSync)
			{
				return false;
			}
			if (!IsSpawned && !ignoreNotSpawned)
			{
				return false;
			}
			bool flag = IsSceneObject.HasValue && IsSceneObject.Value;
			if (base.transform.parent != null && !removeParent && !m_LatestParent.HasValue && flag)
			{
				NetworkObject component = base.transform.parent.GetComponent<NetworkObject>();
				if (component == null)
				{
					m_CachedWorldPositionStays = false;
					return true;
				}
				if (!component.IsSpawned)
				{
					OrphanChildren.Add(this);
					return false;
				}
				SetNetworkParenting(component.NetworkObjectId, worldPositionStays: false);
				m_CachedParent = component.transform;
				return true;
			}
			if (removeParent || !m_LatestParent.HasValue)
			{
				m_CachedParent = null;
				base.transform.SetParent(null, m_CachedWorldPositionStays);
				InvokeBehaviourOnNetworkObjectParentChanged(null);
				return true;
			}
			if (m_LatestParent.HasValue && !NetworkManager.SpawnManager.SpawnedObjects.ContainsKey(m_LatestParent.Value))
			{
				OrphanChildren.Add(this);
				return false;
			}
			NetworkObject networkObject = NetworkManager.SpawnManager.SpawnedObjects[m_LatestParent.Value];
			m_CachedParent = networkObject.transform;
			base.transform.SetParent(networkObject.transform, m_CachedWorldPositionStays);
			InvokeBehaviourOnNetworkObjectParentChanged(networkObject);
			return true;
		}

		internal static void CheckOrphanChildren()
		{
			List<NetworkObject> list = new List<NetworkObject>();
			foreach (NetworkObject orphanChild in OrphanChildren)
			{
				if (orphanChild.ApplyNetworkParenting())
				{
					list.Add(orphanChild);
				}
			}
			foreach (NetworkObject item in list)
			{
				OrphanChildren.Remove(item);
			}
		}

		internal void InvokeBehaviourNetworkSpawn()
		{
			NetworkManager.SpawnManager.UpdateOwnershipTable(this, OwnerClientId);
			for (int i = 0; i < ChildNetworkBehaviours.Count; i++)
			{
				if (ChildNetworkBehaviours[i].gameObject.activeInHierarchy)
				{
					ChildNetworkBehaviours[i].InternalOnNetworkSpawn();
				}
				else
				{
					Debug.LogWarning(ChildNetworkBehaviours[i].gameObject.name + " is disabled! Netcode for GameObjects does not support spawning disabled NetworkBehaviours! The " + ChildNetworkBehaviours[i].GetType().Name + " component was skipped during spawn!");
				}
			}
			for (int j = 0; j < ChildNetworkBehaviours.Count; j++)
			{
				if (ChildNetworkBehaviours[j].gameObject.activeInHierarchy)
				{
					ChildNetworkBehaviours[j].VisibleOnNetworkSpawn();
				}
			}
		}

		internal void InvokeBehaviourNetworkDespawn()
		{
			NetworkManager.SpawnManager.UpdateOwnershipTable(this, OwnerClientId, isRemoving: true);
			for (int i = 0; i < ChildNetworkBehaviours.Count; i++)
			{
				ChildNetworkBehaviours[i].InternalOnNetworkDespawn();
			}
		}

		internal void WriteNetworkVariableData(FastBufferWriter writer, ulong targetClientId)
		{
			for (int i = 0; i < ChildNetworkBehaviours.Count; i++)
			{
				NetworkBehaviour networkBehaviour = ChildNetworkBehaviours[i];
				networkBehaviour.InitializeVariables();
				networkBehaviour.WriteNetworkVariableData(writer, targetClientId);
			}
		}

		internal void MarkVariablesDirty(bool dirty)
		{
			for (int i = 0; i < ChildNetworkBehaviours.Count; i++)
			{
				ChildNetworkBehaviours[i].MarkVariablesDirty(dirty);
			}
		}

		internal static void VerifyParentingStatus()
		{
			if (NetworkLog.CurrentLogLevel <= LogLevel.Normal && OrphanChildren.Count > 0)
			{
				NetworkLog.LogWarning(string.Format("{0} ({1}) children not resolved to parents by the end of frame", "NetworkObject", OrphanChildren.Count));
			}
		}

		internal void SetNetworkVariableData(FastBufferReader reader, ulong clientId)
		{
			for (int i = 0; i < ChildNetworkBehaviours.Count; i++)
			{
				NetworkBehaviour networkBehaviour = ChildNetworkBehaviours[i];
				networkBehaviour.InitializeVariables();
				networkBehaviour.SetNetworkVariableData(reader, clientId);
			}
		}

		internal ushort GetNetworkBehaviourOrderIndex(NetworkBehaviour instance)
		{
			if (instance.NetworkBehaviourIdCache < ChildNetworkBehaviours.Count)
			{
				if (ChildNetworkBehaviours[instance.NetworkBehaviourIdCache] == instance)
				{
					return instance.NetworkBehaviourIdCache;
				}
				instance.NetworkBehaviourIdCache = 0;
			}
			for (ushort num = 0; num < ChildNetworkBehaviours.Count; num++)
			{
				if (ChildNetworkBehaviours[num] == instance)
				{
					instance.NetworkBehaviourIdCache = num;
					return num;
				}
			}
			return 0;
		}

		internal NetworkBehaviour GetNetworkBehaviourAtOrderIndex(ushort index)
		{
			if (index >= ChildNetworkBehaviours.Count)
			{
				if (NetworkLog.CurrentLogLevel <= LogLevel.Error)
				{
					NetworkLog.LogError(string.Format("{0} index {1} was out of bounds for {2}. NetworkBehaviours must be the same, and in the same order, between server and client.", "NetworkBehaviour", index, base.name));
				}
				if (NetworkLog.CurrentLogLevel <= LogLevel.Developer)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append("Known child NetworkBehaviours:");
					for (int i = 0; i < ChildNetworkBehaviours.Count; i++)
					{
						NetworkBehaviour networkBehaviour = ChildNetworkBehaviours[i];
						stringBuilder.Append($" [{i}] {networkBehaviour.__getTypeName()}");
						stringBuilder.Append((i < ChildNetworkBehaviours.Count - 1) ? "," : ".");
					}
					NetworkLog.LogInfo(stringBuilder.ToString());
				}
				return null;
			}
			return ChildNetworkBehaviours[index];
		}

		internal void PostNetworkVariableWrite()
		{
			for (int i = 0; i < ChildNetworkBehaviours.Count; i++)
			{
				ChildNetworkBehaviours[i].PostNetworkVariableWrite();
			}
		}

		internal void SynchronizeNetworkBehaviours<T>(ref BufferSerializer<T> serializer, ulong targetClientId = 0uL) where T : IReaderWriter
		{
			if (serializer.IsWriter)
			{
				FastBufferWriter fastBufferWriter = serializer.GetFastBufferWriter();
				int position = fastBufferWriter.Position;
				ushort value = 0;
				fastBufferWriter.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
				int position2 = fastBufferWriter.Position;
				WriteNetworkVariableData(fastBufferWriter, targetClientId);
				int position3 = fastBufferWriter.Position;
				byte value2 = 0;
				fastBufferWriter.WriteValueSafe(in value2, default(FastBufferWriter.ForPrimitives));
				byte value3 = 0;
				foreach (NetworkBehaviour childNetworkBehaviour in ChildNetworkBehaviours)
				{
					if (childNetworkBehaviour.Synchronize(ref serializer, targetClientId))
					{
						value3++;
					}
				}
				int position4 = fastBufferWriter.Position;
				fastBufferWriter.Seek(position);
				ushort value4 = (ushort)(position4 - position2);
				fastBufferWriter.WriteValueSafe(in value4, default(FastBufferWriter.ForPrimitives));
				fastBufferWriter.Seek(position3);
				fastBufferWriter.WriteValueSafe(in value3, default(FastBufferWriter.ForPrimitives));
				fastBufferWriter.Seek(position4);
			}
			else
			{
				FastBufferReader fastBufferReader = serializer.GetFastBufferReader();
				fastBufferReader.ReadValueSafe(out ushort _, default(FastBufferWriter.ForPrimitives));
				_ = fastBufferReader.Position;
				SetNetworkVariableData(fastBufferReader, targetClientId);
				fastBufferReader.ReadValueSafe(out byte value6, default(FastBufferWriter.ForPrimitives));
				ushort value7 = 0;
				for (int i = 0; i < value6; i++)
				{
					serializer.SerializeValue(ref value7, default(FastBufferWriter.ForPrimitives));
					GetNetworkBehaviourAtOrderIndex(value7).Synchronize(ref serializer, targetClientId);
				}
			}
		}

		internal SceneObject GetMessageSceneObject(ulong targetClientId)
		{
			SceneObject sceneObject = default(SceneObject);
			sceneObject.NetworkObjectId = NetworkObjectId;
			sceneObject.OwnerClientId = OwnerClientId;
			sceneObject.IsPlayerObject = IsPlayerObject;
			sceneObject.IsSceneObject = IsSceneObject ?? true;
			sceneObject.DestroyWithScene = DestroyWithScene;
			sceneObject.Hash = HostCheckForGlobalObjectIdHashOverride();
			sceneObject.OwnerObject = this;
			sceneObject.TargetClientId = targetClientId;
			SceneObject result = sceneObject;
			NetworkObject networkObject = null;
			if (!AlwaysReplicateAsRoot && base.transform.parent != null)
			{
				networkObject = base.transform.parent.GetComponent<NetworkObject>();
				if (networkObject == null && result.IsSceneObject)
				{
					result.HasParent = true;
					result.WorldPositionStays = m_CachedWorldPositionStays;
				}
			}
			if (networkObject != null)
			{
				result.HasParent = true;
				result.ParentObjectId = networkObject.NetworkObjectId;
				result.WorldPositionStays = m_CachedWorldPositionStays;
				ulong? networkParenting = GetNetworkParenting();
				if (result.IsLatestParentSet = networkParenting.HasValue && networkParenting.HasValue)
				{
					result.LatestParent = networkParenting.Value;
				}
			}
			if (IncludeTransformWhenSpawning == null || IncludeTransformWhenSpawning(OwnerClientId))
			{
				result.HasTransform = SynchronizeTransform;
				bool flag2 = result.HasParent && !m_CachedWorldPositionStays;
				bool flag3 = result.HasParent && !m_CachedWorldPositionStays;
				if (!AutoObjectParentSync)
				{
					flag2 = false;
					flag3 = result.HasParent;
				}
				result.Transform = new SceneObject.TransformData
				{
					Position = (flag2 ? base.transform.localPosition : base.transform.position),
					Rotation = (flag2 ? base.transform.localRotation : base.transform.rotation),
					Scale = (flag3 ? base.transform.localScale : base.transform.lossyScale)
				};
			}
			return result;
		}

		internal static NetworkObject AddSceneObject(in SceneObject sceneObject, FastBufferReader reader, NetworkManager networkManager)
		{
			NetworkObject networkObject = networkManager.SpawnManager.CreateLocalNetworkObject(sceneObject);
			if (networkObject == null)
			{
				if (networkManager.LogLevel <= LogLevel.Normal)
				{
					NetworkLog.LogError(string.Format("Failed to spawn {0} for Hash {1}.", "NetworkObject", sceneObject.Hash));
				}
				try
				{
					reader.ReadValueSafe(out ushort value, default(FastBufferWriter.ForPrimitives));
					reader.Seek(reader.Position + value);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
				return null;
			}
			networkObject.OwnerClientId = sceneObject.OwnerClientId;
			BufferSerializer<BufferSerializerReader> serializer = new BufferSerializer<BufferSerializerReader>(new BufferSerializerReader(reader));
			networkObject.SynchronizeNetworkBehaviours(ref serializer, networkManager.LocalClientId);
			networkManager.SpawnManager.SpawnNetworkObjectLocally(networkObject, in sceneObject, sceneObject.DestroyWithScene);
			return networkObject;
		}

		internal void SubscribeToActiveSceneForSynch()
		{
			if (ActiveSceneSynchronization && IsSceneObject.HasValue && !IsSceneObject.Value)
			{
				SceneManager.activeSceneChanged -= CurrentlyActiveSceneChanged;
				SceneManager.activeSceneChanged += CurrentlyActiveSceneChanged;
			}
		}

		private void CurrentlyActiveSceneChanged(Scene current, Scene next)
		{
			if (!(NetworkManager == null) && !NetworkManager.ShutdownInProgress && IsSpawned && IsSceneObject == false && ActiveSceneSynchronization && IsSceneObject.HasValue && !IsSceneObject.Value && base.gameObject.scene != next && base.gameObject.transform.parent == null)
			{
				SceneManager.MoveGameObjectToScene(base.gameObject, next);
				SceneChangedUpdate(next);
			}
		}

		internal void SceneChangedUpdate(Scene scene, bool notify = false)
		{
			if (NetworkManager.SceneManager != null)
			{
				SceneOriginHandle = scene.handle;
				if (!NetworkManager.IsServer && NetworkManager.SceneManager.ClientSceneHandleToServerSceneHandle.ContainsKey(SceneOriginHandle))
				{
					NetworkSceneHandle = NetworkManager.SceneManager.ClientSceneHandleToServerSceneHandle[SceneOriginHandle];
				}
				else if (NetworkManager.IsServer)
				{
					NetworkSceneHandle = SceneOriginHandle;
				}
				else if (NetworkManager.LogLevel == LogLevel.Developer)
				{
					NetworkLog.LogWarningServer($"[Client-{NetworkManager.LocalClientId}][{base.gameObject.name}] Server - " + $"client scene mismatch detected! Client-side scene handle ({SceneOriginHandle}) for scene ({base.gameObject.scene.name})" + "has no associated server side (network) scene handle!");
				}
				OnMigratedToNewScene?.Invoke();
				if (NetworkManager.IsServer && notify && base.transform.parent == null)
				{
					NetworkManager.SceneManager.NotifyNetworkObjectSceneChanged(this);
				}
			}
		}

		private void Update()
		{
			if (SceneMigrationSynchronization && !(NetworkManager == null) && !NetworkManager.ShutdownInProgress && IsSpawned && IsSceneObject == false && base.gameObject.scene.handle != SceneOriginHandle)
			{
				SceneChangedUpdate(base.gameObject.scene, notify: true);
			}
		}

		internal uint HostCheckForGlobalObjectIdHashOverride()
		{
			if (NetworkManager.IsHost)
			{
				if (NetworkManager.PrefabHandler.ContainsHandler(this))
				{
					uint sourceGlobalObjectIdHash = NetworkManager.PrefabHandler.GetSourceGlobalObjectIdHash(GlobalObjectIdHash);
					if (sourceGlobalObjectIdHash != 0)
					{
						return sourceGlobalObjectIdHash;
					}
					return GlobalObjectIdHash;
				}
				if (NetworkManager.NetworkConfig.Prefabs.OverrideToNetworkPrefab.TryGetValue(GlobalObjectIdHash, out var value))
				{
					return value;
				}
			}
			return GlobalObjectIdHash;
		}

		internal void OnNetworkBehaviourDestroyed(NetworkBehaviour networkBehaviour)
		{
			if (networkBehaviour.IsSpawned && IsSpawned)
			{
				if (NetworkManager.LogLevel == LogLevel.Developer)
				{
					NetworkLog.LogWarning("NetworkBehaviour-" + networkBehaviour.name + " is being destroyed while NetworkObject-" + base.name + " is still spawned! (could break state synchronization)");
				}
				ChildNetworkBehaviours.Remove(networkBehaviour);
			}
		}
	}
}
