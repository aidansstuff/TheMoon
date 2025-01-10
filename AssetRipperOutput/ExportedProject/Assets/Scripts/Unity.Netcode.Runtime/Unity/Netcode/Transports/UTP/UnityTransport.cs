using System;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;
using Unity.Networking.Transport.Utilities;
using UnityEngine;

namespace Unity.Netcode.Transports.UTP
{
	[AddComponentMenu("Netcode/Unity Transport")]
	public class UnityTransport : NetworkTransport, INetworkStreamDriverConstructor
	{
		public enum ProtocolType
		{
			UnityTransport = 0,
			RelayUnityTransport = 1
		}

		private enum State
		{
			Disconnected = 0,
			Listening = 1,
			Connected = 2
		}

		[Serializable]
		public struct ConnectionAddressData
		{
			[Tooltip("IP address of the server (address to which clients will connect to).")]
			[SerializeField]
			public string Address;

			[Tooltip("UDP port of the server.")]
			[SerializeField]
			public ushort Port;

			[Tooltip("IP address the server will listen on. If not provided, will use localhost.")]
			[SerializeField]
			public string ServerListenAddress;

			public NetworkEndPoint ServerEndPoint => ParseNetworkEndpoint(Address, Port);

			public NetworkEndPoint ListenEndPoint
			{
				get
				{
					if (string.IsNullOrEmpty(ServerListenAddress))
					{
						NetworkEndPoint networkEndPoint = NetworkEndPoint.LoopbackIpv4;
						if (!string.IsNullOrEmpty(Address) && ServerEndPoint.Family == NetworkFamily.Ipv6)
						{
							networkEndPoint = NetworkEndPoint.LoopbackIpv6;
						}
						return networkEndPoint.WithPort(Port);
					}
					return ParseNetworkEndpoint(ServerListenAddress, Port);
				}
			}

			public bool IsIpv6
			{
				get
				{
					if (!string.IsNullOrEmpty(Address))
					{
						return ParseNetworkEndpoint(Address, Port, silent: true).Family == NetworkFamily.Ipv6;
					}
					return false;
				}
			}

			private static NetworkEndPoint ParseNetworkEndpoint(string ip, ushort port, bool silent = false)
			{
				NetworkEndPoint endpoint = default(NetworkEndPoint);
				if (!NetworkEndPoint.TryParse(ip, port, out endpoint) && !NetworkEndPoint.TryParse(ip, port, out endpoint, NetworkFamily.Ipv6) && !silent)
				{
					UnityEngine.Debug.LogError($"Invalid network endpoint: {ip}:{port}.");
				}
				return endpoint;
			}
		}

		[Serializable]
		public struct SimulatorParameters
		{
			[Tooltip("Delay to add to every send and received packet (in milliseconds). Only applies in the editor and in development builds. The value is ignored in production builds.")]
			[SerializeField]
			public int PacketDelayMS;

			[Tooltip("Jitter (random variation) to add/substract to the packet delay (in milliseconds). Only applies in the editor and in development builds. The value is ignored in production builds.")]
			[SerializeField]
			public int PacketJitterMS;

			[Tooltip("Percentage of sent and received packets to drop. Only applies in the editor and in the editor and in developments builds.")]
			[SerializeField]
			public int PacketDropRate;
		}

		private struct PacketLossCache
		{
			public int PacketsReceived;

			public int PacketsDropped;

			public float PacketLoss;
		}

		[BurstCompile]
		private struct SendBatchedMessagesJob : IJob
		{
			public NetworkDriver.Concurrent Driver;

			public SendTarget Target;

			public BatchedSendQueue Queue;

			public NetworkPipeline ReliablePipeline;

			public void Execute()
			{
				ulong clientId = Target.ClientId;
				NetworkConnection id = ParseClientId(clientId);
				NetworkPipeline networkPipeline = Target.NetworkPipeline;
				while (!Queue.IsEmpty)
				{
					int num = Driver.BeginSend(networkPipeline, id, out var writer);
					if (num != 0)
					{
						UnityEngine.Debug.LogError($"Error sending message: {ErrorUtilities.ErrorToFixedString(num, clientId)}");
						break;
					}
					int num2 = ((networkPipeline == ReliablePipeline) ? Queue.FillWriterWithBytes(ref writer) : Queue.FillWriterWithMessages(ref writer));
					num = Driver.EndSend(writer);
					if (num == num2)
					{
						Queue.Consume(num2);
						continue;
					}
					if (num != -5)
					{
						UnityEngine.Debug.LogError($"Error sending the message: {ErrorUtilities.ErrorToFixedString(num, clientId)}");
						Queue.Consume(num2);
					}
					break;
				}
			}
		}

		private struct SendTarget : IEquatable<SendTarget>
		{
			public readonly ulong ClientId;

			public readonly NetworkPipeline NetworkPipeline;

			public SendTarget(ulong clientId, NetworkPipeline networkPipeline)
			{
				ClientId = clientId;
				NetworkPipeline = networkPipeline;
			}

			public bool Equals(SendTarget other)
			{
				if (ClientId == other.ClientId)
				{
					return NetworkPipeline.Equals(other.NetworkPipeline);
				}
				return false;
			}

			public override bool Equals(object obj)
			{
				if (obj is SendTarget other)
				{
					return Equals(other);
				}
				return false;
			}

			public override int GetHashCode()
			{
				return (ClientId.GetHashCode() * 397) ^ NetworkPipeline.GetHashCode();
			}
		}

		public const int InitialMaxPacketQueueSize = 128;

		public const int InitialMaxPayloadSize = 6144;

		[Obsolete("MaxSendQueueSize is now determined dynamically (can still be set programmatically using the MaxSendQueueSize property). This initial value is not used anymore.", false)]
		public const int InitialMaxSendQueueSize = 98304;

		private const int k_MaxReliableThroughput = 5376;

		private static ConnectionAddressData s_DefaultConnectionAddressData = new ConnectionAddressData
		{
			Address = "127.0.0.1",
			Port = 7777,
			ServerListenAddress = string.Empty
		};

		public static INetworkStreamDriverConstructor s_DriverConstructor;

		[Tooltip("Which protocol should be selected (Relay/Non-Relay).")]
		[SerializeField]
		private ProtocolType m_ProtocolType;

		[Tooltip("The maximum amount of packets that can be in the internal send/receive queues. Basically this is how many packets can be sent/received in a single update/frame.")]
		[SerializeField]
		private int m_MaxPacketQueueSize = 128;

		[Tooltip("The maximum size of an unreliable payload that can be handled by the transport.")]
		[SerializeField]
		private int m_MaxPayloadSize = 6144;

		private int m_MaxSendQueueSize;

		[Tooltip("Timeout in milliseconds after which a heartbeat is sent if there is no activity.")]
		[SerializeField]
		private int m_HeartbeatTimeoutMS = 500;

		[Tooltip("Timeout in milliseconds indicating how long we will wait until we send a new connection attempt.")]
		[SerializeField]
		private int m_ConnectTimeoutMS = 1000;

		[Tooltip("The maximum amount of connection attempts we will try before disconnecting.")]
		[SerializeField]
		private int m_MaxConnectAttempts = 60;

		[Tooltip("Inactivity timeout after which a connection will be disconnected. The connection needs to receive data from the connected endpoint within this timeout. Note that with heartbeats enabled, simply not sending any data will not be enough to trigger this timeout (since heartbeats count as connection events).")]
		[SerializeField]
		private int m_DisconnectTimeoutMS = 30000;

		public ConnectionAddressData ConnectionData = s_DefaultConnectionAddressData;

		public SimulatorParameters DebugSimulator = new SimulatorParameters
		{
			PacketDelayMS = 0,
			PacketJitterMS = 0,
			PacketDropRate = 0
		};

		private PacketLossCache m_PacketLossCache;

		private State m_State;

		private NetworkDriver m_Driver;

		private NetworkSettings m_NetworkSettings;

		private ulong m_ServerClientId;

		private NetworkPipeline m_UnreliableFragmentedPipeline;

		private NetworkPipeline m_UnreliableSequencedFragmentedPipeline;

		private NetworkPipeline m_ReliableSequencedPipeline;

		private RelayServerData m_RelayServerData;

		internal NetworkManager NetworkManager;

		private IRealTimeProvider m_RealTimeProvider;

		private readonly Dictionary<SendTarget, BatchedSendQueue> m_SendQueue = new Dictionary<SendTarget, BatchedSendQueue>();

		private readonly Dictionary<ulong, BatchedReceiveQueue> m_ReliableReceiveQueues = new Dictionary<ulong, BatchedReceiveQueue>();

		private string m_ServerPrivateKey;

		private string m_ServerCertificate;

		private string m_ServerCommonName;

		private string m_ClientCaCertificate;

		public INetworkStreamDriverConstructor DriverConstructor => s_DriverConstructor ?? this;

		public int MaxPacketQueueSize
		{
			get
			{
				return m_MaxPacketQueueSize;
			}
			set
			{
				m_MaxPacketQueueSize = value;
			}
		}

		public int MaxPayloadSize
		{
			get
			{
				return m_MaxPayloadSize;
			}
			set
			{
				m_MaxPayloadSize = value;
			}
		}

		public int MaxSendQueueSize
		{
			get
			{
				return m_MaxSendQueueSize;
			}
			set
			{
				m_MaxSendQueueSize = value;
			}
		}

		public int HeartbeatTimeoutMS
		{
			get
			{
				return m_HeartbeatTimeoutMS;
			}
			set
			{
				m_HeartbeatTimeoutMS = value;
			}
		}

		public int ConnectTimeoutMS
		{
			get
			{
				return m_ConnectTimeoutMS;
			}
			set
			{
				m_ConnectTimeoutMS = value;
			}
		}

		public int MaxConnectAttempts
		{
			get
			{
				return m_MaxConnectAttempts;
			}
			set
			{
				m_MaxConnectAttempts = value;
			}
		}

		public int DisconnectTimeoutMS
		{
			get
			{
				return m_DisconnectTimeoutMS;
			}
			set
			{
				m_DisconnectTimeoutMS = value;
			}
		}

		internal uint? DebugSimulatorRandomSeed { get; set; }

		internal NetworkDriver NetworkDriver => m_Driver;

		public override ulong ServerClientId => m_ServerClientId;

		public ProtocolType Protocol => m_ProtocolType;

		internal static event Action<int, NetworkDriver> TransportInitialized;

		internal static event Action<int> TransportDisposed;

		private void InitDriver()
		{
			DriverConstructor.CreateDriver(this, out m_Driver, out m_UnreliableFragmentedPipeline, out m_UnreliableSequencedFragmentedPipeline, out m_ReliableSequencedPipeline);
			UnityTransport.TransportInitialized?.Invoke(GetInstanceID(), NetworkDriver);
		}

		private void DisposeInternals()
		{
			if (m_Driver.IsCreated)
			{
				m_Driver.Dispose();
			}
			m_NetworkSettings.Dispose();
			foreach (BatchedSendQueue value in m_SendQueue.Values)
			{
				value.Dispose();
			}
			m_SendQueue.Clear();
			UnityTransport.TransportDisposed?.Invoke(GetInstanceID());
		}

		private NetworkPipeline SelectSendPipeline(NetworkDelivery delivery)
		{
			switch (delivery)
			{
			case NetworkDelivery.Unreliable:
				return m_UnreliableFragmentedPipeline;
			case NetworkDelivery.UnreliableSequenced:
				return m_UnreliableSequencedFragmentedPipeline;
			case NetworkDelivery.Reliable:
			case NetworkDelivery.ReliableSequenced:
			case NetworkDelivery.ReliableFragmentedSequenced:
				return m_ReliableSequencedPipeline;
			default:
				UnityEngine.Debug.LogError(string.Format("Unknown {0} value: {1}", "NetworkDelivery", delivery));
				return NetworkPipeline.Null;
			}
		}

		private bool ClientBindAndConnect()
		{
			NetworkEndPoint networkEndPoint = default(NetworkEndPoint);
			if (m_ProtocolType == ProtocolType.RelayUnityTransport)
			{
				if (m_RelayServerData.Equals(default(RelayServerData)))
				{
					UnityEngine.Debug.LogError("You must call SetRelayServerData() at least once before calling StartClient.");
					return false;
				}
				_ = ref m_NetworkSettings.WithRelayParameters(ref m_RelayServerData, m_HeartbeatTimeoutMS);
				networkEndPoint = m_RelayServerData.Endpoint;
			}
			else
			{
				networkEndPoint = ConnectionData.ServerEndPoint;
			}
			if (networkEndPoint.Family == NetworkFamily.Invalid)
			{
				UnityEngine.Debug.LogError("Target server network address (" + ConnectionData.Address + ") is Invalid!");
				return false;
			}
			InitDriver();
			NetworkEndPoint endpoint = ((networkEndPoint.Family == NetworkFamily.Ipv6) ? NetworkEndPoint.AnyIpv6 : NetworkEndPoint.AnyIpv4);
			if (m_Driver.Bind(endpoint) != 0)
			{
				UnityEngine.Debug.LogError("Client failed to bind");
				return false;
			}
			NetworkConnection utpConnectionId = m_Driver.Connect(networkEndPoint);
			m_ServerClientId = ParseClientId(utpConnectionId);
			return true;
		}

		private bool ServerBindAndListen(NetworkEndPoint endPoint)
		{
			if (endPoint.Family == NetworkFamily.Invalid)
			{
				UnityEngine.Debug.LogError("Network listen address (" + ConnectionData.Address + ") is Invalid!");
				return false;
			}
			InitDriver();
			if (m_Driver.Bind(endPoint) != 0)
			{
				UnityEngine.Debug.LogError("Server failed to bind. This is usually caused by another process being bound to the same port.");
				return false;
			}
			if (m_Driver.Listen() != 0)
			{
				UnityEngine.Debug.LogError("Server failed to listen.");
				return false;
			}
			m_State = State.Listening;
			return true;
		}

		private void SetProtocol(ProtocolType inProtocol)
		{
			m_ProtocolType = inProtocol;
		}

		public void SetRelayServerData(string ipv4Address, ushort port, byte[] allocationIdBytes, byte[] keyBytes, byte[] connectionDataBytes, byte[] hostConnectionDataBytes = null, bool isSecure = false)
		{
			byte[] hostConnectionData = hostConnectionDataBytes ?? connectionDataBytes;
			m_RelayServerData = new RelayServerData(ipv4Address, port, allocationIdBytes, connectionDataBytes, hostConnectionData, keyBytes, isSecure);
			SetProtocol(ProtocolType.RelayUnityTransport);
		}

		public void SetRelayServerData(RelayServerData serverData)
		{
			m_RelayServerData = serverData;
			SetProtocol(ProtocolType.RelayUnityTransport);
		}

		public void SetHostRelayData(string ipAddress, ushort port, byte[] allocationId, byte[] key, byte[] connectionData, bool isSecure = false)
		{
			SetRelayServerData(ipAddress, port, allocationId, key, connectionData, null, isSecure);
		}

		public void SetClientRelayData(string ipAddress, ushort port, byte[] allocationId, byte[] key, byte[] connectionData, byte[] hostConnectionData, bool isSecure = false)
		{
			SetRelayServerData(ipAddress, port, allocationId, key, connectionData, hostConnectionData, isSecure);
		}

		public void SetConnectionData(string ipv4Address, ushort port, string listenAddress = null)
		{
			ConnectionData = new ConnectionAddressData
			{
				Address = ipv4Address,
				Port = port,
				ServerListenAddress = (listenAddress ?? ipv4Address)
			};
			SetProtocol(ProtocolType.UnityTransport);
		}

		public void SetConnectionData(NetworkEndPoint endPoint, NetworkEndPoint listenEndPoint = default(NetworkEndPoint))
		{
			string ipv4Address = endPoint.Address.Split(':')[0];
			string listenAddress = string.Empty;
			if (listenEndPoint != default(NetworkEndPoint))
			{
				listenAddress = listenEndPoint.Address.Split(':')[0];
				if (endPoint.Port != listenEndPoint.Port)
				{
					UnityEngine.Debug.LogError($"Port mismatch between server and listen endpoints ({endPoint.Port} vs {listenEndPoint.Port}).");
				}
			}
			SetConnectionData(ipv4Address, endPoint.Port, listenAddress);
		}

		public void SetDebugSimulatorParameters(int packetDelay, int packetJitter, int dropRate)
		{
			if (m_Driver.IsCreated)
			{
				UnityEngine.Debug.LogError("SetDebugSimulatorParameters() must be called before StartClient() or StartServer().");
				return;
			}
			DebugSimulator = new SimulatorParameters
			{
				PacketDelayMS = packetDelay,
				PacketJitterMS = packetJitter,
				PacketDropRate = dropRate
			};
		}

		private bool StartRelayServer()
		{
			if (m_RelayServerData.Equals(default(RelayServerData)))
			{
				UnityEngine.Debug.LogError("You must call SetRelayServerData() at least once before calling StartServer.");
				return false;
			}
			_ = ref m_NetworkSettings.WithRelayParameters(ref m_RelayServerData, m_HeartbeatTimeoutMS);
			return ServerBindAndListen(NetworkEndPoint.AnyIpv4);
		}

		private void SendBatchedMessages(SendTarget sendTarget, BatchedSendQueue queue)
		{
			if (m_Driver.IsCreated)
			{
				SendBatchedMessagesJob jobData = default(SendBatchedMessagesJob);
				jobData.Driver = m_Driver.ToConcurrent();
				jobData.Target = sendTarget;
				jobData.Queue = queue;
				jobData.ReliablePipeline = m_ReliableSequencedPipeline;
				IJobExtensions.Run(jobData);
			}
		}

		private bool AcceptConnection()
		{
			NetworkConnection networkConnection = m_Driver.Accept();
			if (networkConnection == default(NetworkConnection))
			{
				return false;
			}
			InvokeOnTransportEvent(NetworkEvent.Connect, ParseClientId(networkConnection), default(ArraySegment<byte>), m_RealTimeProvider.RealTimeSinceStartup);
			return true;
		}

		private void ReceiveMessages(ulong clientId, NetworkPipeline pipeline, DataStreamReader dataReader)
		{
			BatchedReceiveQueue value;
			if (pipeline == m_ReliableSequencedPipeline)
			{
				if (m_ReliableReceiveQueues.TryGetValue(clientId, out value))
				{
					value.PushReader(dataReader);
				}
				else
				{
					value = new BatchedReceiveQueue(dataReader);
					m_ReliableReceiveQueues[clientId] = value;
				}
			}
			else
			{
				value = new BatchedReceiveQueue(dataReader);
			}
			while (!value.IsEmpty)
			{
				ArraySegment<byte> arraySegment = value.PopMessage();
				if (!(arraySegment == default(ArraySegment<byte>)))
				{
					InvokeOnTransportEvent(NetworkEvent.Data, clientId, arraySegment, m_RealTimeProvider.RealTimeSinceStartup);
					continue;
				}
				break;
			}
		}

		private bool ProcessEvent()
		{
			NetworkConnection con;
			DataStreamReader reader;
			NetworkPipeline pipeline;
			Unity.Networking.Transport.NetworkEvent.Type type = m_Driver.PopEvent(out con, out reader, out pipeline);
			ulong num = ParseClientId(con);
			switch (type)
			{
			case Unity.Networking.Transport.NetworkEvent.Type.Connect:
				InvokeOnTransportEvent(NetworkEvent.Connect, num, default(ArraySegment<byte>), m_RealTimeProvider.RealTimeSinceStartup);
				m_State = State.Connected;
				return true;
			case Unity.Networking.Transport.NetworkEvent.Type.Disconnect:
				if (m_State == State.Connected)
				{
					m_State = State.Disconnected;
					m_ServerClientId = 0uL;
				}
				else if (m_State == State.Disconnected)
				{
					UnityEngine.Debug.LogError("Failed to connect to server.");
					m_ServerClientId = 0uL;
				}
				m_ReliableReceiveQueues.Remove(num);
				ClearSendQueuesForClientId(num);
				InvokeOnTransportEvent(NetworkEvent.Disconnect, num, default(ArraySegment<byte>), m_RealTimeProvider.RealTimeSinceStartup);
				return true;
			case Unity.Networking.Transport.NetworkEvent.Type.Data:
				ReceiveMessages(num, pipeline, reader);
				return true;
			default:
				return false;
			}
		}

		private void Update()
		{
			if (!m_Driver.IsCreated)
			{
				return;
			}
			foreach (KeyValuePair<SendTarget, BatchedSendQueue> item in m_SendQueue)
			{
				SendBatchedMessages(item.Key, item.Value);
			}
			m_Driver.ScheduleUpdate().Complete();
			if (m_ProtocolType == ProtocolType.RelayUnityTransport && m_Driver.GetRelayConnectionStatus() == RelayConnectionStatus.AllocationInvalid)
			{
				UnityEngine.Debug.LogError("Transport failure! Relay allocation needs to be recreated, and NetworkManager restarted. Use NetworkManager.OnTransportFailure to be notified of such events programmatically.");
				InvokeOnTransportEvent(NetworkEvent.TransportFailure, 0uL, default(ArraySegment<byte>), m_RealTimeProvider.RealTimeSinceStartup);
				return;
			}
			while (AcceptConnection() && m_Driver.IsCreated)
			{
			}
			while (ProcessEvent() && m_Driver.IsCreated)
			{
			}
			if ((bool)NetworkManager)
			{
				ExtractNetworkMetrics();
			}
		}

		private void OnDestroy()
		{
			DisposeInternals();
		}

		private void ExtractNetworkMetrics()
		{
			if (NetworkManager.IsServer)
			{
				foreach (ulong key in NetworkManager.ConnectedClients.Keys)
				{
					if (key != 0L || !NetworkManager.IsHost)
					{
						ulong transportClientId = NetworkManager.ConnectionManager.ClientIdToTransportId(key);
						ExtractNetworkMetricsForClient(transportClientId);
					}
				}
				return;
			}
			if (m_ServerClientId != 0L)
			{
				ExtractNetworkMetricsForClient(m_ServerClientId);
			}
		}

		private void ExtractNetworkMetricsForClient(ulong transportClientId)
		{
			NetworkConnection networkConnection = ParseClientId(transportClientId);
			ExtractNetworkMetricsFromPipeline(m_UnreliableFragmentedPipeline, networkConnection);
			ExtractNetworkMetricsFromPipeline(m_UnreliableSequencedFragmentedPipeline, networkConnection);
			ExtractNetworkMetricsFromPipeline(m_ReliableSequencedPipeline, networkConnection);
			int rtt = ((!NetworkManager.IsServer) ? ExtractRtt(networkConnection) : 0);
			NetworkMetrics.UpdateRttToServer(rtt);
			float packetLoss = (NetworkManager.IsServer ? 0f : ExtractPacketLoss(networkConnection));
			NetworkMetrics.UpdatePacketLoss(packetLoss);
		}

		private unsafe void ExtractNetworkMetricsFromPipeline(NetworkPipeline pipeline, NetworkConnection networkConnection)
		{
			m_Driver.GetPipelineBuffers(pipeline, NetworkPipelineStageCollection.GetStageId(typeof(NetworkMetricsPipelineStage)), networkConnection, out var _, out var _, out var sharedBuffer);
			NetworkMetricsContext* unsafePtr = (NetworkMetricsContext*)sharedBuffer.GetUnsafePtr();
			NetworkMetrics.TrackPacketSent(unsafePtr->PacketSentCount);
			NetworkMetrics.TrackPacketReceived(unsafePtr->PacketReceivedCount);
			unsafePtr->PacketSentCount = 0u;
			unsafePtr->PacketReceivedCount = 0u;
		}

		private unsafe int ExtractRtt(NetworkConnection networkConnection)
		{
			if (m_Driver.GetConnectionState(networkConnection) != NetworkConnection.State.Connected)
			{
				return 0;
			}
			m_Driver.GetPipelineBuffers(m_ReliableSequencedPipeline, NetworkPipelineStageCollection.GetStageId(typeof(ReliableSequencedPipelineStage)), networkConnection, out var _, out var _, out var sharedBuffer);
			ReliableUtility.SharedContext* unsafePtr = (ReliableUtility.SharedContext*)sharedBuffer.GetUnsafePtr();
			return unsafePtr->RttInfo.LastRtt;
		}

		private unsafe float ExtractPacketLoss(NetworkConnection networkConnection)
		{
			if (m_Driver.GetConnectionState(networkConnection) != NetworkConnection.State.Connected)
			{
				return 0f;
			}
			m_Driver.GetPipelineBuffers(m_ReliableSequencedPipeline, NetworkPipelineStageCollection.GetStageId(typeof(ReliableSequencedPipelineStage)), networkConnection, out var _, out var _, out var sharedBuffer);
			ReliableUtility.SharedContext* unsafePtr = (ReliableUtility.SharedContext*)sharedBuffer.GetUnsafePtr();
			float num = unsafePtr->stats.PacketsReceived - m_PacketLossCache.PacketsReceived;
			float num2 = unsafePtr->stats.PacketsDropped - m_PacketLossCache.PacketsDropped;
			if (num2 == 0f && num == 0f)
			{
				return m_PacketLossCache.PacketLoss;
			}
			m_PacketLossCache.PacketsReceived = unsafePtr->stats.PacketsReceived;
			m_PacketLossCache.PacketsDropped = unsafePtr->stats.PacketsDropped;
			m_PacketLossCache.PacketLoss = ((num > 0f) ? (num2 / num) : 0f);
			return m_PacketLossCache.PacketLoss;
		}

		private unsafe static ulong ParseClientId(NetworkConnection utpConnectionId)
		{
			return *(ulong*)(&utpConnectionId);
		}

		private unsafe static NetworkConnection ParseClientId(ulong netcodeConnectionId)
		{
			return *(NetworkConnection*)(&netcodeConnectionId);
		}

		private void ClearSendQueuesForClientId(ulong clientId)
		{
			using NativeList<SendTarget> nativeList = new NativeList<SendTarget>(16, Allocator.Temp);
			foreach (SendTarget key in m_SendQueue.Keys)
			{
				SendTarget value = key;
				if (value.ClientId == clientId)
				{
					nativeList.Add(in value);
				}
			}
			foreach (SendTarget item in nativeList)
			{
				m_SendQueue[item].Dispose();
				m_SendQueue.Remove(item);
			}
		}

		private void FlushSendQueuesForClientId(ulong clientId)
		{
			foreach (KeyValuePair<SendTarget, BatchedSendQueue> item in m_SendQueue)
			{
				if (item.Key.ClientId == clientId)
				{
					SendBatchedMessages(item.Key, item.Value);
				}
			}
		}

		public override void DisconnectLocalClient()
		{
			if (m_State == State.Connected)
			{
				FlushSendQueuesForClientId(m_ServerClientId);
				if (m_Driver.Disconnect(ParseClientId(m_ServerClientId)) == 0)
				{
					m_State = State.Disconnected;
					m_ReliableReceiveQueues.Remove(m_ServerClientId);
					ClearSendQueuesForClientId(m_ServerClientId);
					InvokeOnTransportEvent(NetworkEvent.Disconnect, m_ServerClientId, default(ArraySegment<byte>), m_RealTimeProvider.RealTimeSinceStartup);
				}
			}
		}

		public override void DisconnectRemoteClient(ulong clientId)
		{
			if (m_State == State.Listening)
			{
				FlushSendQueuesForClientId(clientId);
				m_ReliableReceiveQueues.Remove(clientId);
				ClearSendQueuesForClientId(clientId);
				NetworkConnection networkConnection = ParseClientId(clientId);
				if (m_Driver.GetConnectionState(networkConnection) != 0)
				{
					m_Driver.Disconnect(networkConnection);
				}
			}
		}

		public override ulong GetCurrentRtt(ulong clientId)
		{
			if (NetworkManager != null)
			{
				ulong netcodeConnectionId = NetworkManager.ConnectionManager.ClientIdToTransportId(clientId);
				int num = ExtractRtt(ParseClientId(netcodeConnectionId));
				if (num > 0)
				{
					return (ulong)num;
				}
			}
			return (ulong)ExtractRtt(ParseClientId(clientId));
		}

		public override void Initialize(NetworkManager networkManager = null)
		{
			NetworkManager = networkManager;
			IRealTimeProvider realTimeProvider2;
			if (!NetworkManager)
			{
				IRealTimeProvider realTimeProvider = new RealTimeProvider();
				realTimeProvider2 = realTimeProvider;
			}
			else
			{
				realTimeProvider2 = NetworkManager.RealTimeProvider;
			}
			m_RealTimeProvider = realTimeProvider2;
			m_NetworkSettings = new NetworkSettings(Allocator.Persistent);
			int payloadCapacity = m_MaxPayloadSize + 4;
			_ = ref m_NetworkSettings.WithFragmentationStageParameters(payloadCapacity);
			_ = ref m_NetworkSettings.WithReliableStageParameters(64);
			_ = ref m_NetworkSettings.WithBaselibNetworkInterfaceParameters(m_MaxPacketQueueSize, m_MaxPacketQueueSize);
		}

		public override NetworkEvent PollEvent(out ulong clientId, out ArraySegment<byte> payload, out float receiveTime)
		{
			clientId = 0uL;
			payload = default(ArraySegment<byte>);
			receiveTime = 0f;
			return NetworkEvent.Nothing;
		}

		public override void Send(ulong clientId, ArraySegment<byte> payload, NetworkDelivery networkDelivery)
		{
			NetworkPipeline networkPipeline = SelectSendPipeline(networkDelivery);
			if (networkPipeline != m_ReliableSequencedPipeline && payload.Count > m_MaxPayloadSize)
			{
				UnityEngine.Debug.LogError($"Unreliable payload of size {payload.Count} larger than configured 'Max Payload Size' ({m_MaxPayloadSize}).");
				return;
			}
			SendTarget sendTarget = new SendTarget(clientId, networkPipeline);
			if (!m_SendQueue.TryGetValue(sendTarget, out var value))
			{
				int val = ((m_MaxSendQueueSize > 0) ? m_MaxSendQueueSize : (m_DisconnectTimeoutMS * 5376));
				value = new BatchedSendQueue(Math.Max(val, m_MaxPayloadSize));
				m_SendQueue.Add(sendTarget, value);
			}
			if (value.PushMessage(payload))
			{
				return;
			}
			if (networkPipeline == m_ReliableSequencedPipeline)
			{
				ulong num = NetworkManager?.ConnectionManager.TransportIdToClientId(clientId) ?? clientId;
				UnityEngine.Debug.LogError($"Couldn't add payload of size {payload.Count} to reliable send queue. " + $"Closing connection {num} as reliability guarantees can't be maintained.");
				if (clientId == m_ServerClientId)
				{
					DisconnectLocalClient();
					return;
				}
				DisconnectRemoteClient(clientId);
				InvokeOnTransportEvent(NetworkEvent.Disconnect, clientId, default(ArraySegment<byte>), m_RealTimeProvider.RealTimeSinceStartup);
			}
			else
			{
				m_Driver.ScheduleFlushSend(default(JobHandle)).Complete();
				SendBatchedMessages(sendTarget, value);
				value.PushMessage(payload);
			}
		}

		public override bool StartClient()
		{
			if (m_Driver.IsCreated)
			{
				return false;
			}
			bool num = ClientBindAndConnect();
			if (!num && m_Driver.IsCreated)
			{
				m_Driver.Dispose();
			}
			return num;
		}

		public override bool StartServer()
		{
			if (m_Driver.IsCreated)
			{
				return false;
			}
			switch (m_ProtocolType)
			{
			case ProtocolType.UnityTransport:
			{
				bool num2 = ServerBindAndListen(ConnectionData.ListenEndPoint);
				if (!num2 && m_Driver.IsCreated)
				{
					m_Driver.Dispose();
				}
				return num2;
			}
			case ProtocolType.RelayUnityTransport:
			{
				bool num = StartRelayServer();
				if (!num && m_Driver.IsCreated)
				{
					m_Driver.Dispose();
				}
				return num;
			}
			default:
				return false;
			}
		}

		public override void Shutdown()
		{
			if (m_Driver.IsCreated)
			{
				foreach (KeyValuePair<SendTarget, BatchedSendQueue> item in m_SendQueue)
				{
					SendBatchedMessages(item.Key, item.Value);
				}
				m_Driver.ScheduleUpdate().Complete();
			}
			DisposeInternals();
			m_ReliableReceiveQueues.Clear();
			m_ServerClientId = 0uL;
		}

		private void ConfigureSimulatorForUtp1()
		{
			_ = ref m_NetworkSettings.WithSimulatorStageParameters(300, 1400, DebugSimulator.PacketDelayMS, DebugSimulator.PacketJitterMS, 0, DebugSimulator.PacketDropRate, 0, 0, (uint)(((int?)DebugSimulatorRandomSeed) ?? ((int)Stopwatch.GetTimestamp())));
		}

		public void SetServerSecrets(string serverCertificate, string serverPrivateKey)
		{
			m_ServerPrivateKey = serverPrivateKey;
			m_ServerCertificate = serverCertificate;
		}

		public void SetClientSecrets(string serverCommonName, string caCertificate = null)
		{
			m_ServerCommonName = serverCommonName;
			m_ClientCaCertificate = caCertificate;
		}

		public void CreateDriver(UnityTransport transport, out NetworkDriver driver, out NetworkPipeline unreliableFragmentedPipeline, out NetworkPipeline unreliableSequencedFragmentedPipeline, out NetworkPipeline reliableSequencedPipeline)
		{
			NetworkPipelineStageCollection.RegisterPipelineStage(default(NetworkMetricsPipelineStage));
			_ = ref m_NetworkSettings.WithNetworkConfigParameters(maxConnectAttempts: transport.m_MaxConnectAttempts, connectTimeoutMS: transport.m_ConnectTimeoutMS, disconnectTimeoutMS: transport.m_DisconnectTimeoutMS, heartbeatTimeoutMS: transport.m_HeartbeatTimeoutMS);
			driver = NetworkDriver.Create(m_NetworkSettings);
			SetupPipelinesForUtp1(driver, out unreliableFragmentedPipeline, out unreliableSequencedFragmentedPipeline, out reliableSequencedPipeline);
		}

		private void SetupPipelinesForUtp1(NetworkDriver driver, out NetworkPipeline unreliableFragmentedPipeline, out NetworkPipeline unreliableSequencedFragmentedPipeline, out NetworkPipeline reliableSequencedPipeline)
		{
			unreliableFragmentedPipeline = driver.CreatePipeline(typeof(FragmentationPipelineStage), typeof(NetworkMetricsPipelineStage));
			unreliableSequencedFragmentedPipeline = driver.CreatePipeline(typeof(FragmentationPipelineStage), typeof(UnreliableSequencedPipelineStage), typeof(NetworkMetricsPipelineStage));
			reliableSequencedPipeline = driver.CreatePipeline(typeof(ReliableSequencedPipelineStage), typeof(NetworkMetricsPipelineStage));
		}
	}
}
