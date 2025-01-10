using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Unity.Netcode
{
	public sealed class NetworkConnectionManager
	{
		internal NetworkManager NetworkManager;

		internal NetworkMessageManager MessageManager;

		internal NetworkClient LocalClient = new NetworkClient();

		internal Dictionary<ulong, NetworkManager.ConnectionApprovalResponse> ClientsToApprove = new Dictionary<ulong, NetworkManager.ConnectionApprovalResponse>();

		internal Dictionary<ulong, NetworkClient> ConnectedClients = new Dictionary<ulong, NetworkClient>();

		internal Dictionary<ulong, ulong> ClientIdToTransportIdMap = new Dictionary<ulong, ulong>();

		internal Dictionary<ulong, ulong> TransportIdToClientIdMap = new Dictionary<ulong, ulong>();

		internal List<NetworkClient> ConnectedClientsList = new List<NetworkClient>();

		internal List<ulong> ConnectedClientIds = new List<ulong>();

		internal Action<NetworkManager.ConnectionApprovalRequest, NetworkManager.ConnectionApprovalResponse> ConnectionApprovalCallback;

		private Dictionary<ulong, PendingClient> m_PendingClients = new Dictionary<ulong, PendingClient>();

		internal Coroutine LocalClientApprovalCoroutine;

		private ulong m_NextClientId = 1uL;

		public string DisconnectReason { get; internal set; }

		public bool IsListening { get; internal set; }

		internal IReadOnlyDictionary<ulong, PendingClient> PendingClients => m_PendingClients;

		internal ulong ServerTransportId => GetServerTransportId();

		public event Action<ulong> OnClientConnectedCallback;

		public event Action<ulong> OnClientDisconnectCallback;

		public event Action OnTransportFailure;

		internal void InvokeOnClientConnectedCallback(ulong clientId)
		{
			this.OnClientConnectedCallback?.Invoke(clientId);
		}

		internal void StartClientApprovalCoroutine(ulong clientId)
		{
			LocalClientApprovalCoroutine = NetworkManager.StartCoroutine(ApprovalTimeout(clientId));
		}

		internal void StopClientApprovalCoroutine()
		{
			if (LocalClientApprovalCoroutine != null)
			{
				NetworkManager.StopCoroutine(LocalClientApprovalCoroutine);
				LocalClientApprovalCoroutine = null;
			}
		}

		internal void AddPendingClient(ulong clientId)
		{
			m_PendingClients.Add(clientId, new PendingClient
			{
				ClientId = clientId,
				ConnectionState = PendingClient.State.PendingConnection,
				ApprovalCoroutine = NetworkManager.StartCoroutine(ApprovalTimeout(clientId))
			});
			NetworkManager.PendingClients.Add(clientId, PendingClients[clientId]);
		}

		internal void RemovePendingClient(ulong clientId)
		{
			if (m_PendingClients.ContainsKey(clientId) && m_PendingClients[clientId].ApprovalCoroutine != null)
			{
				NetworkManager.StopCoroutine(m_PendingClients[clientId].ApprovalCoroutine);
			}
			m_PendingClients.Remove(clientId);
			NetworkManager.PendingClients.Remove(clientId);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ulong TransportIdToClientId(ulong transportId)
		{
			if (transportId == GetServerTransportId())
			{
				return 0uL;
			}
			if (TransportIdToClientIdMap.TryGetValue(transportId, out var value))
			{
				return value;
			}
			if (NetworkLog.CurrentLogLevel == LogLevel.Developer)
			{
				NetworkLog.LogWarning($"Trying to get the NGO client ID map for the transport ID ({transportId}) but did not find the map entry! Returning default transport ID value.");
			}
			return 0uL;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ulong ClientIdToTransportId(ulong clientId)
		{
			if (clientId == 0L)
			{
				return GetServerTransportId();
			}
			if (ClientIdToTransportIdMap.TryGetValue(clientId, out var value))
			{
				return value;
			}
			if (NetworkLog.CurrentLogLevel == LogLevel.Developer)
			{
				NetworkLog.LogWarning($"Trying to get the transport client ID map for the NGO client ID ({clientId}) but did not find the map entry! Returning default transport ID value.");
			}
			return 0uL;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private ulong GetServerTransportId()
		{
			if (NetworkManager != null)
			{
				NetworkTransport networkTransport = NetworkManager.NetworkConfig.NetworkTransport;
				if (networkTransport != null)
				{
					return networkTransport.ServerClientId;
				}
				throw new NullReferenceException("The transport in the active NetworkConfig is null");
			}
			throw new Exception("There is no NetworkManager assigned to this instance!");
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal ulong TransportIdCleanUp(ulong transportId)
		{
			if (!LocalClient.IsServer && !TransportIdToClientIdMap.ContainsKey(transportId))
			{
				return 0uL;
			}
			ulong num = TransportIdToClientId(transportId);
			TransportIdToClientIdMap.Remove(transportId);
			ClientIdToTransportIdMap.Remove(num);
			return num;
		}

		internal void PollAndHandleNetworkEvents()
		{
			NetworkEvent networkEvent;
			do
			{
				networkEvent = NetworkManager.NetworkConfig.NetworkTransport.PollEvent(out var clientId, out var payload, out var receiveTime);
				HandleNetworkEvent(networkEvent, clientId, payload, receiveTime);
			}
			while (NetworkManager.IsListening && networkEvent != NetworkEvent.Nothing);
		}

		internal void HandleNetworkEvent(NetworkEvent networkEvent, ulong transportClientId, ArraySegment<byte> payload, float receiveTime)
		{
			switch (networkEvent)
			{
			case NetworkEvent.Connect:
				ConnectEventHandler(transportClientId);
				break;
			case NetworkEvent.Data:
				DataEventHandler(transportClientId, ref payload, receiveTime);
				break;
			case NetworkEvent.Disconnect:
				DisconnectEventHandler(transportClientId);
				break;
			case NetworkEvent.TransportFailure:
				TransportFailureEventHandler();
				break;
			}
		}

		internal void ConnectEventHandler(ulong transportClientId)
		{
			ulong num = transportClientId;
			num = ((!LocalClient.IsServer) ? 0 : m_NextClientId++);
			ClientIdToTransportIdMap[num] = transportClientId;
			TransportIdToClientIdMap[transportClientId] = num;
			MessageManager.ClientConnected(num);
			if (LocalClient.IsServer)
			{
				if (NetworkLog.CurrentLogLevel <= LogLevel.Developer)
				{
					NetworkLog.LogInfo("Client Connected");
				}
				AddPendingClient(num);
				return;
			}
			if (NetworkLog.CurrentLogLevel <= LogLevel.Developer)
			{
				NetworkLog.LogInfo("Connected");
			}
			SendConnectionRequest();
			StartClientApprovalCoroutine(num);
		}

		internal void DataEventHandler(ulong transportClientId, ref ArraySegment<byte> payload, float receiveTime)
		{
			ulong clientId = TransportIdToClientId(transportClientId);
			MessageManager.HandleIncomingData(clientId, payload, receiveTime);
		}

		internal void DisconnectEventHandler(ulong transportClientId)
		{
			ulong num = TransportIdCleanUp(transportClientId);
			if (NetworkLog.CurrentLogLevel <= LogLevel.Developer)
			{
				NetworkLog.LogInfo($"Disconnect Event From {num}");
			}
			MessageManager.ProcessIncomingMessageQueue();
			try
			{
				this.OnClientDisconnectCallback?.Invoke(num);
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			if (LocalClient.IsServer)
			{
				OnClientDisconnectFromServer(num);
			}
			else
			{
				NetworkManager.Shutdown(discardMessageQueue: true);
			}
		}

		internal void TransportFailureEventHandler(bool duringStart = false)
		{
			string text = ((!LocalClient.IsServer) ? "Client" : (LocalClient.IsHost ? "Host" : "Server"));
			string text2 = (duringStart ? "start failure" : "failure");
			NetworkLog.LogError(text + " is shutting down due to network transport " + text2 + " of " + NetworkManager.NetworkConfig.NetworkTransport.GetType().Name + "!");
			this.OnTransportFailure?.Invoke();
			if (duringStart)
			{
				LocalClient.SetRole(isServer: false, isClient: false);
				NetworkManager.ShutdownInternal();
			}
			else
			{
				NetworkManager.Shutdown(discardMessageQueue: true);
			}
		}

		private void SendConnectionRequest()
		{
			ConnectionRequestMessage connectionRequestMessage = default(ConnectionRequestMessage);
			connectionRequestMessage.ConfigHash = NetworkManager.NetworkConfig.GetConfig(cache: false);
			connectionRequestMessage.ShouldSendConnectionData = NetworkManager.NetworkConfig.ConnectionApproval;
			connectionRequestMessage.ConnectionData = NetworkManager.NetworkConfig.ConnectionData;
			connectionRequestMessage.MessageVersions = new NativeArray<MessageVersionData>(MessageManager.MessageHandlers.Length, Allocator.Temp);
			ConnectionRequestMessage message = connectionRequestMessage;
			for (int i = 0; i < MessageManager.MessageHandlers.Length; i++)
			{
				if (MessageManager.MessageTypes[i] != null)
				{
					Type type = MessageManager.MessageTypes[i];
					message.MessageVersions[i] = new MessageVersionData
					{
						Hash = type.FullName.Hash32(),
						Version = MessageManager.GetLocalVersion(type)
					};
				}
			}
			SendMessage(ref message, NetworkDelivery.ReliableSequenced, 0uL);
			message.MessageVersions.Dispose();
		}

		private IEnumerator ApprovalTimeout(ulong clientId)
		{
			float num = (LocalClient.IsServer ? NetworkManager.LocalTime.TimeAsFloat : NetworkManager.RealTimeProvider.RealTimeSinceStartup);
			bool flag = false;
			bool flag2 = false;
			bool connectionNotApproved = false;
			float timeoutMarker = num + (float)NetworkManager.NetworkConfig.ClientConnectionBufferTimeout;
			while (NetworkManager.IsListening && !NetworkManager.ShutdownInProgress && !flag && !flag2)
			{
				yield return null;
				flag = timeoutMarker < (LocalClient.IsServer ? NetworkManager.LocalTime.TimeAsFloat : NetworkManager.RealTimeProvider.RealTimeSinceStartup);
				if (LocalClient.IsServer)
				{
					flag2 = !PendingClients.ContainsKey(clientId) && ConnectedClients.ContainsKey(clientId);
					connectionNotApproved = !PendingClients.ContainsKey(clientId) && !ConnectedClients.ContainsKey(clientId);
				}
				else
				{
					flag2 = NetworkManager.LocalClient.IsApproved;
				}
			}
			if (!NetworkManager.IsListening || NetworkManager.ShutdownInProgress || !(flag || connectionNotApproved))
			{
				yield break;
			}
			if (NetworkLog.CurrentLogLevel <= LogLevel.Developer)
			{
				if (flag)
				{
					if (LocalClient.IsServer)
					{
						NetworkLog.LogWarning($"Server detected a transport connection from Client-{clientId}, but timed out waiting for the connection request message.");
					}
					else
					{
						NetworkLog.LogInfo("Timed out waiting for the server to approve the connection request.");
					}
				}
				else if (connectionNotApproved)
				{
					NetworkLog.LogInfo($"Client-{clientId} was either denied approval or disconnected while being approved.");
				}
			}
			if (LocalClient.IsServer)
			{
				DisconnectClient(clientId);
			}
			else
			{
				NetworkManager.Shutdown(discardMessageQueue: true);
			}
		}

		internal void ApproveConnection(ref ConnectionRequestMessage connectionRequestMessage, ref NetworkContext context)
		{
			NetworkManager.ConnectionApprovalResponse connectionApprovalResponse = new NetworkManager.ConnectionApprovalResponse();
			ClientsToApprove[context.SenderId] = connectionApprovalResponse;
			ConnectionApprovalCallback(new NetworkManager.ConnectionApprovalRequest
			{
				Payload = connectionRequestMessage.ConnectionData,
				ClientNetworkId = context.SenderId
			}, connectionApprovalResponse);
		}

		internal void ProcessPendingApprovals()
		{
			List<ulong> list = null;
			foreach (KeyValuePair<ulong, NetworkManager.ConnectionApprovalResponse> item in ClientsToApprove)
			{
				NetworkManager.ConnectionApprovalResponse value = item.Value;
				ulong key = item.Key;
				if (value.Pending)
				{
					continue;
				}
				try
				{
					HandleConnectionApproval(key, value);
					if (list == null)
					{
						list = new List<ulong>();
					}
					list.Add(key);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
			if (list == null)
			{
				return;
			}
			foreach (ulong item2 in list)
			{
				ClientsToApprove.Remove(item2);
			}
		}

		internal void HandleConnectionApproval(ulong ownerClientId, NetworkManager.ConnectionApprovalResponse response)
		{
			LocalClient.IsApproved = response.Approved;
			if (response.Approved)
			{
				RemovePendingClient(ownerClientId);
				NetworkClient networkClient = AddClient(ownerClientId);
				if (response.CreatePlayerObject)
				{
					NetworkObject component = NetworkManager.NetworkConfig.PlayerPrefab.GetComponent<NetworkObject>();
					uint hash = response.PlayerPrefabHash ?? component.GlobalObjectIdHash;
					NetworkObject.SceneObject sceneObject = default(NetworkObject.SceneObject);
					sceneObject.OwnerClientId = ownerClientId;
					sceneObject.IsPlayerObject = true;
					sceneObject.IsSceneObject = false;
					sceneObject.HasTransform = component.SynchronizeTransform;
					sceneObject.Hash = hash;
					sceneObject.TargetClientId = ownerClientId;
					sceneObject.Transform = new NetworkObject.SceneObject.TransformData
					{
						Position = response.Position.GetValueOrDefault(),
						Rotation = response.Rotation.GetValueOrDefault()
					};
					NetworkObject.SceneObject sceneObject2 = sceneObject;
					NetworkObject networkObject = NetworkManager.SpawnManager.CreateLocalNetworkObject(sceneObject2);
					NetworkManager.SpawnManager.SpawnNetworkObjectLocally(networkObject, NetworkManager.SpawnManager.GetNetworkObjectId(), sceneObject: false, playerObject: true, ownerClientId, destroyWithScene: false);
					networkClient.AssignPlayerObject(ref networkObject);
				}
				if (ownerClientId != 0L)
				{
					ConnectionApprovedMessage connectionApprovedMessage = default(ConnectionApprovedMessage);
					connectionApprovedMessage.OwnerClientId = ownerClientId;
					connectionApprovedMessage.NetworkTick = NetworkManager.LocalTime.Tick;
					ConnectionApprovedMessage message = connectionApprovedMessage;
					if (!NetworkManager.NetworkConfig.EnableSceneManagement)
					{
						NetworkManager.SpawnManager.UpdateObservedNetworkObjects(ownerClientId);
						if (NetworkManager.SpawnManager.SpawnedObjectsList.Count != 0)
						{
							message.SpawnedObjectsList = NetworkManager.SpawnManager.SpawnedObjectsList;
						}
					}
					message.MessageVersions = new NativeArray<MessageVersionData>(MessageManager.MessageHandlers.Length, Allocator.Temp);
					for (int i = 0; i < MessageManager.MessageHandlers.Length; i++)
					{
						if (MessageManager.MessageTypes[i] != null)
						{
							Type type = MessageManager.MessageTypes[i];
							message.MessageVersions[i] = new MessageVersionData
							{
								Hash = type.FullName.Hash32(),
								Version = MessageManager.GetLocalVersion(type)
							};
						}
					}
					SendMessage(ref message, NetworkDelivery.ReliableFragmentedSequenced, ownerClientId);
					message.MessageVersions.Dispose();
					if (!NetworkManager.NetworkConfig.EnableSceneManagement)
					{
						NetworkManager.ConnectedClients[ownerClientId].IsConnected = true;
						InvokeOnClientConnectedCallback(ownerClientId);
					}
					else
					{
						NetworkManager.SceneManager.SynchronizeNetworkObjects(ownerClientId);
					}
				}
				else
				{
					LocalClient = networkClient;
					NetworkManager.SpawnManager.UpdateObservedNetworkObjects(ownerClientId);
					LocalClient.IsConnected = true;
				}
				if (response.CreatePlayerObject && (response.PlayerPrefabHash.HasValue || !(NetworkManager.NetworkConfig.PlayerPrefab == null)))
				{
					ApprovedPlayerSpawn(ownerClientId, response.PlayerPrefabHash ?? NetworkManager.NetworkConfig.PlayerPrefab.GetComponent<NetworkObject>().GlobalObjectIdHash);
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(response.Reason))
				{
					DisconnectReasonMessage disconnectReasonMessage = default(DisconnectReasonMessage);
					disconnectReasonMessage.Reason = response.Reason;
					DisconnectReasonMessage message2 = disconnectReasonMessage;
					SendMessage(ref message2, NetworkDelivery.Reliable, ownerClientId);
					MessageManager.ProcessSendQueues();
				}
				DisconnectRemoteClient(ownerClientId);
			}
		}

		internal void ApprovedPlayerSpawn(ulong clientId, uint playerPrefabHash)
		{
			foreach (KeyValuePair<ulong, NetworkClient> connectedClient in ConnectedClients)
			{
				if (connectedClient.Key != clientId && connectedClient.Key != 0L && !(ConnectedClients[clientId].PlayerObject == null) && ConnectedClients[clientId].PlayerObject.Observers.Contains(connectedClient.Key))
				{
					CreateObjectMessage createObjectMessage = default(CreateObjectMessage);
					createObjectMessage.ObjectInfo = ConnectedClients[clientId].PlayerObject.GetMessageSceneObject(connectedClient.Key);
					CreateObjectMessage message = createObjectMessage;
					message.ObjectInfo.Hash = playerPrefabHash;
					message.ObjectInfo.IsSceneObject = false;
					message.ObjectInfo.HasParent = false;
					message.ObjectInfo.IsPlayerObject = true;
					message.ObjectInfo.OwnerClientId = clientId;
					int num = SendMessage(ref message, NetworkDelivery.ReliableFragmentedSequenced, connectedClient.Key);
					NetworkManager.NetworkMetrics.TrackObjectSpawnSent(connectedClient.Key, ConnectedClients[clientId].PlayerObject, num);
				}
			}
		}

		internal NetworkClient AddClient(ulong clientId)
		{
			NetworkClient localClient = LocalClient;
			localClient = new NetworkClient();
			localClient.SetRole(clientId == 0, isClient: true, NetworkManager);
			localClient.ClientId = clientId;
			ConnectedClients.Add(clientId, localClient);
			ConnectedClientsList.Add(localClient);
			ConnectedClientIds.Add(clientId);
			return localClient;
		}

		internal void OnClientDisconnectFromServer(ulong clientId)
		{
			if (!LocalClient.IsServer)
			{
				throw new Exception("[OnClientDisconnectFromServer] Was invoked by non-server instance!");
			}
			if (NetworkManager.ShutdownInProgress && clientId == 0L)
			{
				return;
			}
			if (ConnectedClients.TryGetValue(clientId, out var value))
			{
				NetworkObject playerObject = value.PlayerObject;
				if (playerObject != null)
				{
					if (!playerObject.DontDestroyWithOwner)
					{
						if (NetworkManager.PrefabHandler.ContainsHandler(ConnectedClients[clientId].PlayerObject.GlobalObjectIdHash))
						{
							NetworkManager.PrefabHandler.HandleNetworkPrefabDestroy(ConnectedClients[clientId].PlayerObject);
						}
						else if (playerObject.IsSpawned)
						{
							NetworkManager.SpawnManager.DespawnObject(playerObject, destroyObject: true);
						}
					}
					else
					{
						playerObject.RemoveOwnership();
					}
				}
				List<NetworkObject> clientOwnedObjects = NetworkManager.SpawnManager.GetClientOwnedObjects(clientId);
				if (clientOwnedObjects == null)
				{
					if (NetworkManager.LogLevel == LogLevel.Developer)
					{
						NetworkLog.LogWarning($"ClientID {clientId} disconnected with (0) zero owned objects!  Was a player prefab not assigned?");
					}
				}
				else
				{
					for (int num = clientOwnedObjects.Count - 1; num >= 0; num--)
					{
						NetworkObject networkObject = clientOwnedObjects[num];
						if (networkObject != null)
						{
							if (!networkObject.DontDestroyWithOwner)
							{
								if (NetworkManager.PrefabHandler.ContainsHandler(clientOwnedObjects[num].GlobalObjectIdHash))
								{
									NetworkManager.PrefabHandler.HandleNetworkPrefabDestroy(clientOwnedObjects[num]);
								}
								else
								{
									UnityEngine.Object.Destroy(networkObject.gameObject);
								}
							}
							else
							{
								networkObject.RemoveOwnership();
							}
						}
					}
				}
				foreach (NetworkObject spawnedObjects in NetworkManager.SpawnManager.SpawnedObjectsList)
				{
					spawnedObjects.Observers.Remove(clientId);
				}
				if (ConnectedClients.ContainsKey(clientId))
				{
					ConnectedClientsList.Remove(ConnectedClients[clientId]);
					ConnectedClients.Remove(clientId);
				}
				ConnectedClientIds.Remove(clientId);
			}
			if (ClientIdToTransportIdMap.ContainsKey(clientId))
			{
				ulong num2 = ClientIdToTransportId(clientId);
				NetworkManager.NetworkConfig.NetworkTransport.DisconnectRemoteClient(num2);
				try
				{
					this.OnClientDisconnectCallback?.Invoke(clientId);
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
				TransportIdCleanUp(num2);
			}
			RemovePendingClient(clientId);
			MessageManager.ClientDisconnected(clientId);
		}

		internal void DisconnectRemoteClient(ulong clientId)
		{
			MessageManager.ProcessSendQueues();
			OnClientDisconnectFromServer(clientId);
		}

		internal void DisconnectClient(ulong clientId, string reason = null)
		{
			if (!LocalClient.IsServer)
			{
				throw new NotServerException("Only server can disconnect remote clients. Please use `Shutdown()` instead.");
			}
			if (!string.IsNullOrEmpty(reason))
			{
				DisconnectReasonMessage disconnectReasonMessage = default(DisconnectReasonMessage);
				disconnectReasonMessage.Reason = reason;
				DisconnectReasonMessage message = disconnectReasonMessage;
				SendMessage(ref message, NetworkDelivery.Reliable, clientId);
			}
			DisconnectRemoteClient(clientId);
		}

		internal void Initialize(NetworkManager networkManager)
		{
			LocalClient.IsApproved = false;
			m_PendingClients.Clear();
			ConnectedClients.Clear();
			ConnectedClientsList.Clear();
			ConnectedClientIds.Clear();
			ClientIdToTransportIdMap.Clear();
			TransportIdToClientIdMap.Clear();
			ClientsToApprove.Clear();
			NetworkObject.OrphanChildren.Clear();
			DisconnectReason = string.Empty;
			NetworkManager = networkManager;
			MessageManager = networkManager.MessageManager;
			NetworkManager.NetworkConfig.NetworkTransport.NetworkMetrics = NetworkManager.MetricsManager.NetworkMetrics;
			NetworkManager.NetworkConfig.NetworkTransport.OnTransportEvent += HandleNetworkEvent;
			NetworkManager.NetworkConfig.NetworkTransport.Initialize(networkManager);
		}

		internal void Shutdown()
		{
			LocalClient.IsApproved = false;
			LocalClient.IsConnected = false;
			if (LocalClient.IsServer)
			{
				MessageManager?.ProcessSendQueues();
				HashSet<ulong> hashSet = new HashSet<ulong>();
				ulong serverClientId = NetworkManager.NetworkConfig.NetworkTransport.ServerClientId;
				foreach (KeyValuePair<ulong, NetworkClient> connectedClient in ConnectedClients)
				{
					if (!hashSet.Contains(connectedClient.Key) && connectedClient.Key != serverClientId)
					{
						hashSet.Add(connectedClient.Key);
					}
				}
				foreach (KeyValuePair<ulong, PendingClient> pendingClient in PendingClients)
				{
					if (!hashSet.Contains(pendingClient.Key) && pendingClient.Key != serverClientId)
					{
						hashSet.Add(pendingClient.Key);
					}
				}
				foreach (ulong item in hashSet)
				{
					DisconnectRemoteClient(item);
				}
			}
			else if (NetworkManager != null && NetworkManager.IsListening && LocalClient.IsClient)
			{
				try
				{
					NetworkManager.NetworkConfig.NetworkTransport.DisconnectLocalClient();
				}
				catch (Exception exception)
				{
					Debug.LogException(exception);
				}
			}
			if (NetworkManager != null && NetworkManager.NetworkConfig?.NetworkTransport != null)
			{
				NetworkManager.NetworkConfig.NetworkTransport.OnTransportEvent -= HandleNetworkEvent;
			}
			if (!IsListening)
			{
				return;
			}
			NetworkTransport networkTransport = NetworkManager.NetworkConfig?.NetworkTransport;
			if (networkTransport != null)
			{
				networkTransport.Shutdown();
				if (NetworkManager.LogLevel <= LogLevel.Developer)
				{
					NetworkLog.LogInfo("NetworkConnectionManager.Shutdown() -> IsListening && NetworkTransport != null -> NetworkTransport.Shutdown()");
				}
			}
		}

		internal unsafe int SendMessage<TMessageType, TClientIdListType>(ref TMessageType message, NetworkDelivery delivery, in TClientIdListType clientIds) where TMessageType : INetworkMessage where TClientIdListType : IReadOnlyList<ulong>
		{
			if (LocalClient.IsServer)
			{
				ulong* ptr = stackalloc ulong[clientIds.Count];
				int num = 0;
				for (int i = 0; i < clientIds.Count; i++)
				{
					if (clientIds[i] != 0L)
					{
						ptr[num++] = clientIds[i];
					}
				}
				if (num == 0)
				{
					return 0;
				}
				return MessageManager.SendMessage(ref message, delivery, ptr, num);
			}
			if (clientIds.Count != 1 || clientIds[0] != 0L)
			{
				throw new ArgumentException("Clients may only send messages to ServerClientId");
			}
			return MessageManager.SendMessage(ref message, delivery, in clientIds);
		}

		internal unsafe int SendMessage<T>(ref T message, NetworkDelivery delivery, ulong* clientIds, int numClientIds) where T : INetworkMessage
		{
			if (LocalClient.IsServer)
			{
				ulong* ptr = stackalloc ulong[numClientIds];
				int num = 0;
				for (int i = 0; i < numClientIds; i++)
				{
					if (clientIds[i] != 0L)
					{
						ptr[num++] = clientIds[i];
					}
				}
				if (num == 0)
				{
					return 0;
				}
				return MessageManager.SendMessage(ref message, delivery, ptr, num);
			}
			if (numClientIds != 1 || *clientIds != 0L)
			{
				throw new ArgumentException("Clients may only send messages to ServerClientId");
			}
			return MessageManager.SendMessage(ref message, delivery, clientIds, numClientIds);
		}

		internal unsafe int SendMessage<T>(ref T message, NetworkDelivery delivery, in NativeArray<ulong> clientIds) where T : INetworkMessage
		{
			return SendMessage(ref message, delivery, (ulong*)clientIds.GetUnsafePtr(), clientIds.Length);
		}

		internal int SendMessage<T>(ref T message, NetworkDelivery delivery, ulong clientId) where T : INetworkMessage
		{
			if (LocalClient.IsServer && clientId == 0L)
			{
				return 0;
			}
			if (!LocalClient.IsServer && clientId != 0L)
			{
				throw new ArgumentException("Clients may only send messages to ServerClientId");
			}
			return MessageManager.SendMessage(ref message, delivery, clientId);
		}
	}
}
