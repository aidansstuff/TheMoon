using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Unity.Netcode
{
	[AddComponentMenu("Netcode/Network Manager", -100)]
	public class NetworkManager : MonoBehaviour, INetworkUpdateSystem
	{
		public delegate void RpcReceiveHandler(NetworkBehaviour behaviour, FastBufferReader reader, __RpcParams parameters);

		public class ConnectionApprovalResponse
		{
			public bool Approved;

			public bool CreatePlayerObject;

			public uint? PlayerPrefabHash;

			public Vector3? Position;

			public Quaternion? Rotation;

			public bool Pending;

			public string Reason;
		}

		public struct ConnectionApprovalRequest
		{
			public byte[] Payload;

			public ulong ClientNetworkId;
		}

		private enum StartType
		{
			Server = 0,
			Host = 1,
			Client = 2
		}

		public static readonly Dictionary<uint, RpcReceiveHandler> __rpc_func_table = new Dictionary<uint, RpcReceiveHandler>();

		public const ulong ServerClientId = 0uL;

		public readonly Dictionary<ulong, PendingClient> PendingClients = new Dictionary<ulong, PendingClient>();

		private bool m_ShuttingDown;

		[HideInInspector]
		public NetworkConfig NetworkConfig;

		[HideInInspector]
		public bool RunInBackground = true;

		[HideInInspector]
		public LogLevel LogLevel = LogLevel.Normal;

		private NetworkPrefabHandler m_PrefabHandler;

		internal NetworkMetricsManager MetricsManager = new NetworkMetricsManager();

		internal NetworkConnectionManager ConnectionManager = new NetworkConnectionManager();

		internal NetworkMessageManager MessageManager;

		public ulong LocalClientId
		{
			get
			{
				return ConnectionManager.LocalClient.ClientId;
			}
			internal set
			{
				ConnectionManager.LocalClient.ClientId = value;
			}
		}

		public IReadOnlyDictionary<ulong, NetworkClient> ConnectedClients
		{
			get
			{
				if (!IsServer)
				{
					throw new NotServerException("ConnectedClients should only be accessed on server.");
				}
				return ConnectionManager.ConnectedClients;
			}
		}

		public IReadOnlyList<NetworkClient> ConnectedClientsList
		{
			get
			{
				if (!IsServer)
				{
					throw new NotServerException("ConnectedClientsList should only be accessed on server.");
				}
				return ConnectionManager.ConnectedClientsList;
			}
		}

		public IReadOnlyList<ulong> ConnectedClientsIds
		{
			get
			{
				if (!IsServer)
				{
					throw new NotServerException("ConnectedClientIds should only be accessed on server.");
				}
				return ConnectionManager.ConnectedClientIds;
			}
		}

		public NetworkClient LocalClient => ConnectionManager.LocalClient;

		public bool IsServer => ConnectionManager.LocalClient.IsServer;

		public bool IsClient => ConnectionManager.LocalClient.IsClient;

		public bool IsHost => ConnectionManager.LocalClient.IsHost;

		public string DisconnectReason => ConnectionManager.DisconnectReason;

		public bool IsListening
		{
			get
			{
				return ConnectionManager.IsListening;
			}
			internal set
			{
				ConnectionManager.IsListening = value;
			}
		}

		public bool IsConnectedClient
		{
			get
			{
				return ConnectionManager.LocalClient.IsConnected;
			}
			internal set
			{
				ConnectionManager.LocalClient.IsConnected = value;
			}
		}

		public bool IsApproved
		{
			get
			{
				return ConnectionManager.LocalClient.IsApproved;
			}
			internal set
			{
				ConnectionManager.LocalClient.IsApproved = value;
			}
		}

		public Action<ConnectionApprovalRequest, ConnectionApprovalResponse> ConnectionApprovalCallback
		{
			get
			{
				return ConnectionManager.ConnectionApprovalCallback;
			}
			set
			{
				if (value != null && value.GetInvocationList().Length > 1)
				{
					throw new InvalidOperationException("Only one ConnectionApprovalCallback can be registered at a time.");
				}
				ConnectionManager.ConnectionApprovalCallback = value;
			}
		}

		public string ConnectedHostname => string.Empty;

		public bool ShutdownInProgress => m_ShuttingDown;

		public NetworkTime LocalTime => NetworkTickSystem?.LocalTime ?? default(NetworkTime);

		public NetworkTime ServerTime => NetworkTickSystem?.ServerTime ?? default(NetworkTime);

		public static NetworkManager Singleton { get; private set; }

		public NetworkPrefabHandler PrefabHandler
		{
			get
			{
				if (m_PrefabHandler == null)
				{
					m_PrefabHandler = new NetworkPrefabHandler();
					m_PrefabHandler.Initialize(this);
				}
				return m_PrefabHandler;
			}
		}

		public NetworkSpawnManager SpawnManager { get; private set; }

		internal IDeferredNetworkMessageManager DeferredMessageManager { get; private set; }

		public CustomMessagingManager CustomMessagingManager { get; private set; }

		public NetworkSceneManager SceneManager { get; private set; }

		internal NetworkBehaviourUpdater BehaviourUpdater { get; set; }

		public NetworkTimeSystem NetworkTimeSystem { get; private set; }

		public NetworkTickSystem NetworkTickSystem { get; private set; }

		internal IRealTimeProvider RealTimeProvider { get; private set; }

		internal INetworkMetrics NetworkMetrics => MetricsManager.NetworkMetrics;

		public int MaximumTransmissionUnitSize
		{
			get
			{
				return MessageManager.NonFragmentedMessageMaxSize;
			}
			set
			{
				MessageManager.NonFragmentedMessageMaxSize = value & -8;
			}
		}

		public int MaximumFragmentedMessageSize
		{
			get
			{
				return MessageManager.FragmentedMessageMaxSize;
			}
			set
			{
				MessageManager.FragmentedMessageMaxSize = value;
			}
		}

		public event Action OnTransportFailure
		{
			add
			{
				ConnectionManager.OnTransportFailure += value;
			}
			remove
			{
				ConnectionManager.OnTransportFailure -= value;
			}
		}

		public event Action<ulong> OnClientConnectedCallback
		{
			add
			{
				ConnectionManager.OnClientConnectedCallback += value;
			}
			remove
			{
				ConnectionManager.OnClientConnectedCallback -= value;
			}
		}

		public event Action<ulong> OnClientDisconnectCallback
		{
			add
			{
				ConnectionManager.OnClientDisconnectCallback += value;
			}
			remove
			{
				ConnectionManager.OnClientDisconnectCallback -= value;
			}
		}

		internal static event Action OnSingletonReady;

		public event Action OnServerStarted;

		public event Action OnClientStarted;

		public event Action<bool> OnServerStopped;

		public event Action<bool> OnClientStopped;

		public void NetworkUpdate(NetworkUpdateStage updateStage)
		{
			switch (updateStage)
			{
			case NetworkUpdateStage.EarlyUpdate:
				ConnectionManager.ProcessPendingApprovals();
				ConnectionManager.PollAndHandleNetworkEvents();
				MessageManager.ProcessIncomingMessageQueue();
				MessageManager.CleanupDisconnectedClients();
				break;
			case NetworkUpdateStage.PreUpdate:
				NetworkTimeSystem.UpdateTime();
				break;
			case NetworkUpdateStage.PostLateUpdate:
				SceneManager.CheckForAndSendNetworkObjectSceneChanged();
				MessageManager.ProcessSendQueues();
				MetricsManager.UpdateMetrics();
				NetworkObject.VerifyParentingStatus();
				DeferredMessageManager.CleanupStaleTriggers();
				if (m_ShuttingDown)
				{
					ShutdownInternal();
				}
				break;
			}
		}

		internal bool NetworkManagerCheckForParent(bool ignoreNetworkManagerCache = false)
		{
			bool num = base.transform.root != base.transform;
			if (num)
			{
				throw new Exception(GenerateNestedNetworkManagerMessage(base.transform));
			}
			return num;
		}

		internal static string GenerateNestedNetworkManagerMessage(Transform transform)
		{
			return transform.name + " is nested under " + transform.root.name + ". NetworkManager cannot be nested.\n";
		}

		private void OnTransformParentChanged()
		{
			NetworkManagerCheckForParent();
		}

		public void SetSingleton()
		{
			Singleton = this;
			NetworkManager.OnSingletonReady?.Invoke();
		}

		private void Awake()
		{
			NetworkConfig?.InitializePrefabs();
			UnityEngine.SceneManagement.SceneManager.sceneUnloaded += OnSceneUnloaded;
		}

		private void OnEnable()
		{
			if (RunInBackground)
			{
				Application.runInBackground = true;
			}
			if (Singleton == null)
			{
				SetSingleton();
			}
			if (!NetworkManagerCheckForParent())
			{
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
			}
		}

		public GameObject GetNetworkPrefabOverride(GameObject gameObject)
		{
			return PrefabHandler.GetNetworkPrefabOverride(gameObject);
		}

		public void AddNetworkPrefab(GameObject prefab)
		{
			PrefabHandler.AddNetworkPrefab(prefab);
		}

		public void RemoveNetworkPrefab(GameObject prefab)
		{
			PrefabHandler.RemoveNetworkPrefab(prefab);
		}

		internal void Initialize(bool server)
		{
			if (NetworkManagerCheckForParent(ignoreNetworkManagerCache: true))
			{
				return;
			}
			if (NetworkConfig.NetworkTransport == null)
			{
				if (NetworkLog.CurrentLogLevel <= LogLevel.Error)
				{
					NetworkLog.LogError("No transport has been selected!");
				}
				return;
			}
			if (NetworkLog.CurrentLogLevel <= LogLevel.Developer)
			{
				NetworkLog.LogInfo("Initialize");
			}
			this.RegisterNetworkUpdate(NetworkUpdateStage.EarlyUpdate);
			this.RegisterNetworkUpdate(NetworkUpdateStage.PreUpdate);
			this.RegisterNetworkUpdate(NetworkUpdateStage.PostLateUpdate);
			ComponentFactory.SetDefaults();
			RealTimeProvider = ComponentFactory.Create<IRealTimeProvider>(this);
			MetricsManager.Initialize(this);
			MessageManager = new NetworkMessageManager(new DefaultMessageSender(this), this);
			MessageManager.Hook(new NetworkManagerHooks(this));
			MessageManager.Hook(new MetricHooks(this));
			MessageManager.ClientConnected(0uL);
			ConnectionManager.Initialize(this);
			NetworkTimeSystem = (server ? NetworkTimeSystem.ServerTimeSystem() : new NetworkTimeSystem(1.0 / (double)NetworkConfig.TickRate));
			NetworkTickSystem = NetworkTimeSystem.Initialize(this);
			SpawnManager = new NetworkSpawnManager(this);
			DeferredMessageManager = ComponentFactory.Create<IDeferredNetworkMessageManager>(this);
			CustomMessagingManager = new CustomMessagingManager(this);
			SceneManager = new NetworkSceneManager(this);
			BehaviourUpdater = new NetworkBehaviourUpdater();
			BehaviourUpdater.Initialize(this);
			NetworkConfig.InitializePrefabs();
			PrefabHandler.RegisterPlayerPrefab();
		}

		private bool CanStart(StartType type)
		{
			if (IsListening)
			{
				if (NetworkLog.CurrentLogLevel <= LogLevel.Normal)
				{
					NetworkLog.LogWarning("Cannot start " + type.ToString() + " while an instance is already running");
				}
				return false;
			}
			if (NetworkConfig.ConnectionApproval && type != StartType.Client && ConnectionApprovalCallback == null && NetworkLog.CurrentLogLevel <= LogLevel.Normal)
			{
				NetworkLog.LogWarning("No ConnectionApproval callback defined. Connection approval will timeout");
			}
			if (ConnectionApprovalCallback != null && !NetworkConfig.ConnectionApproval && NetworkLog.CurrentLogLevel <= LogLevel.Normal)
			{
				NetworkLog.LogWarning("A ConnectionApproval callback is defined but ConnectionApproval is disabled. In order to use ConnectionApproval it has to be explicitly enabled ");
			}
			return true;
		}

		public bool StartServer()
		{
			if (NetworkLog.CurrentLogLevel <= LogLevel.Developer)
			{
				NetworkLog.LogInfo("StartServer");
			}
			if (!CanStart(StartType.Server))
			{
				return false;
			}
			ConnectionManager.LocalClient.SetRole(isServer: true, isClient: false, this);
			ConnectionManager.LocalClient.ClientId = 0uL;
			Initialize(server: true);
			try
			{
				IsListening = NetworkConfig.NetworkTransport.StartServer();
				if (IsListening)
				{
					SpawnManager.ServerSpawnSceneObjectsOnStartSweep();
					this.OnServerStarted?.Invoke();
					ConnectionManager.LocalClient.IsApproved = true;
					return true;
				}
				ConnectionManager.TransportFailureEventHandler(duringStart: true);
			}
			catch (Exception)
			{
				ConnectionManager.LocalClient.SetRole(isServer: false, isClient: false);
				IsListening = false;
				throw;
			}
			return IsListening;
		}

		public bool StartClient()
		{
			if (NetworkLog.CurrentLogLevel <= LogLevel.Developer)
			{
				NetworkLog.LogInfo("StartClient");
			}
			if (!CanStart(StartType.Client))
			{
				return false;
			}
			ConnectionManager.LocalClient.SetRole(isServer: false, isClient: true, this);
			Initialize(server: false);
			try
			{
				IsListening = NetworkConfig.NetworkTransport.StartClient();
				if (!IsListening)
				{
					ConnectionManager.TransportFailureEventHandler(duringStart: true);
				}
				else
				{
					this.OnClientStarted?.Invoke();
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				ConnectionManager.LocalClient.SetRole(isServer: false, isClient: false);
				IsListening = false;
			}
			return IsListening;
		}

		public bool StartHost()
		{
			if (NetworkLog.CurrentLogLevel <= LogLevel.Developer)
			{
				NetworkLog.LogInfo("StartHost");
			}
			if (!CanStart(StartType.Host))
			{
				return false;
			}
			ConnectionManager.LocalClient.SetRole(isServer: true, isClient: true, this);
			Initialize(server: true);
			try
			{
				IsListening = NetworkConfig.NetworkTransport.StartServer();
				if (!IsListening)
				{
					ConnectionManager.TransportFailureEventHandler(duringStart: true);
				}
				else
				{
					HostServerInitialize();
				}
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
				ConnectionManager.LocalClient.SetRole(isServer: false, isClient: false);
				IsListening = false;
			}
			return IsListening;
		}

		private void HostServerInitialize()
		{
			LocalClientId = 0uL;
			NetworkMetrics.SetConnectionId(LocalClientId);
			if (NetworkConfig.ConnectionApproval && ConnectionApprovalCallback != null)
			{
				ConnectionApprovalResponse connectionApprovalResponse = new ConnectionApprovalResponse();
				ConnectionApprovalCallback(new ConnectionApprovalRequest
				{
					Payload = NetworkConfig.ConnectionData,
					ClientNetworkId = 0uL
				}, connectionApprovalResponse);
				if (!connectionApprovalResponse.Approved && NetworkLog.CurrentLogLevel <= LogLevel.Normal)
				{
					NetworkLog.LogWarning("You cannot decline the host connection. The connection was automatically approved.");
				}
				connectionApprovalResponse.Approved = true;
				ConnectionManager.HandleConnectionApproval(0uL, connectionApprovalResponse);
			}
			else
			{
				ConnectionApprovalResponse response = new ConnectionApprovalResponse
				{
					Approved = true,
					CreatePlayerObject = (NetworkConfig.PlayerPrefab != null)
				};
				ConnectionManager.HandleConnectionApproval(0uL, response);
			}
			SpawnManager.ServerSpawnSceneObjectsOnStartSweep();
			this.OnServerStarted?.Invoke();
			this.OnClientStarted?.Invoke();
			ConnectionManager.InvokeOnClientConnectedCallback(LocalClientId);
		}

		public void DisconnectClient(ulong clientId)
		{
			ConnectionManager.DisconnectClient(clientId);
		}

		public void DisconnectClient(ulong clientId, string reason = null)
		{
			ConnectionManager.DisconnectClient(clientId, reason);
		}

		public void Shutdown(bool discardMessageQueue = false)
		{
			if (NetworkLog.CurrentLogLevel <= LogLevel.Developer)
			{
				NetworkLog.LogInfo("Shutdown");
			}
			if (IsServer || IsClient)
			{
				m_ShuttingDown = true;
				if (MessageManager != null)
				{
					MessageManager.StopProcessing = discardMessageQueue;
				}
			}
			if (NetworkConfig != null && NetworkConfig.NetworkTransport != null)
			{
				NetworkConfig.NetworkTransport.OnTransportEvent -= ConnectionManager.HandleNetworkEvent;
			}
		}

		private void OnSceneUnloaded(Scene scene)
		{
			if (base.gameObject != null && scene == base.gameObject.scene)
			{
				OnDestroy();
			}
		}

		internal void ShutdownInternal()
		{
			if (NetworkLog.CurrentLogLevel <= LogLevel.Developer)
			{
				NetworkLog.LogInfo("ShutdownInternal");
			}
			this.UnregisterAllNetworkUpdates();
			DeferredMessageManager?.CleanupAllTriggers();
			CustomMessagingManager = null;
			BehaviourUpdater?.Shutdown();
			BehaviourUpdater = null;
			ConnectionManager.Shutdown();
			if (MessageManager != null)
			{
				MessageManager.Dispose();
				MessageManager = null;
			}
			SpawnManager?.DespawnAndDestroyNetworkObjects();
			SpawnManager?.ServerResetShudownStateForSceneObjects();
			SpawnManager = null;
			SceneManager?.Dispose();
			SceneManager = null;
			IsListening = false;
			m_ShuttingDown = false;
			if (ConnectionManager.LocalClient.IsClient)
			{
				this.OnClientStopped?.Invoke(ConnectionManager.LocalClient.IsServer);
			}
			if (ConnectionManager.LocalClient.IsServer)
			{
				this.OnServerStopped?.Invoke(ConnectionManager.LocalClient.IsClient);
			}
			m_ShuttingDown = false;
			ConnectionManager.LocalClient.SetRole(isServer: false, isClient: false);
			NetworkConfig?.Prefabs?.Shutdown();
			NetworkConfig?.ClearConfigHash();
			NetworkTimeSystem?.Shutdown();
			NetworkTickSystem = null;
		}

		private void OnApplicationQuit()
		{
			m_ShuttingDown = true;
			OnDestroy();
		}

		private void OnDestroy()
		{
			ShutdownInternal();
			UnityEngine.SceneManagement.SceneManager.sceneUnloaded -= OnSceneUnloaded;
			if (Singleton == this)
			{
				Singleton = null;
			}
		}
	}
}
