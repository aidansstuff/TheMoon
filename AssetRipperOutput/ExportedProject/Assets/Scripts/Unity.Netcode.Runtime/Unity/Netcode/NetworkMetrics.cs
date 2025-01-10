using System;
using System.Collections.Generic;
using Unity.Multiplayer.Tools.MetricTypes;
using Unity.Multiplayer.Tools.NetStats;
using Unity.Profiling;

namespace Unity.Netcode
{
	internal class NetworkMetrics : INetworkMetrics
	{
		private const ulong k_MaxMetricsPerFrame = 1000uL;

		private static Dictionary<uint, string> s_SceneEventTypeNames;

		private static ProfilerMarker s_FrameDispatch;

		private readonly Counter m_TransportBytesSent = new Counter(NetworkMetricTypes.TotalBytesSent.Id, 0L)
		{
			ShouldResetOnDispatch = true
		};

		private readonly Counter m_TransportBytesReceived = new Counter(NetworkMetricTypes.TotalBytesReceived.Id, 0L)
		{
			ShouldResetOnDispatch = true
		};

		private readonly EventMetric<NetworkMessageEvent> m_NetworkMessageSentEvent = new EventMetric<NetworkMessageEvent>(NetworkMetricTypes.NetworkMessageSent.Id);

		private readonly EventMetric<NetworkMessageEvent> m_NetworkMessageReceivedEvent = new EventMetric<NetworkMessageEvent>(NetworkMetricTypes.NetworkMessageReceived.Id);

		private readonly EventMetric<NamedMessageEvent> m_NamedMessageSentEvent = new EventMetric<NamedMessageEvent>(NetworkMetricTypes.NamedMessageSent.Id);

		private readonly EventMetric<NamedMessageEvent> m_NamedMessageReceivedEvent = new EventMetric<NamedMessageEvent>(NetworkMetricTypes.NamedMessageReceived.Id);

		private readonly EventMetric<UnnamedMessageEvent> m_UnnamedMessageSentEvent = new EventMetric<UnnamedMessageEvent>(NetworkMetricTypes.UnnamedMessageSent.Id);

		private readonly EventMetric<UnnamedMessageEvent> m_UnnamedMessageReceivedEvent = new EventMetric<UnnamedMessageEvent>(NetworkMetricTypes.UnnamedMessageReceived.Id);

		private readonly EventMetric<NetworkVariableEvent> m_NetworkVariableDeltaSentEvent = new EventMetric<NetworkVariableEvent>(NetworkMetricTypes.NetworkVariableDeltaSent.Id);

		private readonly EventMetric<NetworkVariableEvent> m_NetworkVariableDeltaReceivedEvent = new EventMetric<NetworkVariableEvent>(NetworkMetricTypes.NetworkVariableDeltaReceived.Id);

		private readonly EventMetric<OwnershipChangeEvent> m_OwnershipChangeSentEvent = new EventMetric<OwnershipChangeEvent>(NetworkMetricTypes.OwnershipChangeSent.Id);

		private readonly EventMetric<OwnershipChangeEvent> m_OwnershipChangeReceivedEvent = new EventMetric<OwnershipChangeEvent>(NetworkMetricTypes.OwnershipChangeReceived.Id);

		private readonly EventMetric<ObjectSpawnedEvent> m_ObjectSpawnSentEvent = new EventMetric<ObjectSpawnedEvent>(NetworkMetricTypes.ObjectSpawnedSent.Id);

		private readonly EventMetric<ObjectSpawnedEvent> m_ObjectSpawnReceivedEvent = new EventMetric<ObjectSpawnedEvent>(NetworkMetricTypes.ObjectSpawnedReceived.Id);

		private readonly EventMetric<ObjectDestroyedEvent> m_ObjectDestroySentEvent = new EventMetric<ObjectDestroyedEvent>(NetworkMetricTypes.ObjectDestroyedSent.Id);

		private readonly EventMetric<ObjectDestroyedEvent> m_ObjectDestroyReceivedEvent = new EventMetric<ObjectDestroyedEvent>(NetworkMetricTypes.ObjectDestroyedReceived.Id);

		private readonly EventMetric<RpcEvent> m_RpcSentEvent = new EventMetric<RpcEvent>(NetworkMetricTypes.RpcSent.Id);

		private readonly EventMetric<RpcEvent> m_RpcReceivedEvent = new EventMetric<RpcEvent>(NetworkMetricTypes.RpcReceived.Id);

		private readonly EventMetric<ServerLogEvent> m_ServerLogSentEvent = new EventMetric<ServerLogEvent>(NetworkMetricTypes.ServerLogSent.Id);

		private readonly EventMetric<ServerLogEvent> m_ServerLogReceivedEvent = new EventMetric<ServerLogEvent>(NetworkMetricTypes.ServerLogReceived.Id);

		private readonly EventMetric<SceneEventMetric> m_SceneEventSentEvent = new EventMetric<SceneEventMetric>(NetworkMetricTypes.SceneEventSent.Id);

		private readonly EventMetric<SceneEventMetric> m_SceneEventReceivedEvent = new EventMetric<SceneEventMetric>(NetworkMetricTypes.SceneEventReceived.Id);

		private readonly Counter m_PacketSentCounter = new Counter(NetworkMetricTypes.PacketsSent.Id, 0L)
		{
			ShouldResetOnDispatch = true
		};

		private readonly Counter m_PacketReceivedCounter = new Counter(NetworkMetricTypes.PacketsReceived.Id, 0L)
		{
			ShouldResetOnDispatch = true
		};

		private readonly Gauge m_RttToServerGauge = new Gauge(NetworkMetricTypes.RttToServer.Id)
		{
			ShouldResetOnDispatch = true
		};

		private readonly Gauge m_NetworkObjectsGauge = new Gauge(NetworkMetricTypes.NetworkObjects.Id)
		{
			ShouldResetOnDispatch = true
		};

		private readonly Gauge m_ConnectionsGauge = new Gauge(NetworkMetricTypes.ConnectedClients.Id)
		{
			ShouldResetOnDispatch = true
		};

		private readonly Gauge m_PacketLossGauge = new Gauge(NetworkMetricTypes.PacketLoss.Id);

		private ulong m_NumberOfMetricsThisFrame;

		internal IMetricDispatcher Dispatcher { get; }

		private bool CanSendMetrics => m_NumberOfMetricsThisFrame < 1000;

		static NetworkMetrics()
		{
			s_FrameDispatch = new ProfilerMarker("NetworkMetrics.DispatchFrame");
			s_SceneEventTypeNames = new Dictionary<uint, string>();
			foreach (SceneEventType value in Enum.GetValues(typeof(SceneEventType)))
			{
				s_SceneEventTypeNames[(uint)value] = value.ToString();
			}
		}

		private static string GetSceneEventTypeName(uint typeCode)
		{
			if (!s_SceneEventTypeNames.TryGetValue(typeCode, out var value))
			{
				return "Unknown";
			}
			return value;
		}

		public NetworkMetrics()
		{
			Dispatcher = new MetricDispatcherBuilder().WithCounters(m_TransportBytesSent, m_TransportBytesReceived).WithMetricEvents<NetworkMessageEvent>(m_NetworkMessageSentEvent, m_NetworkMessageReceivedEvent).WithMetricEvents<NamedMessageEvent>(m_NamedMessageSentEvent, m_NamedMessageReceivedEvent)
				.WithMetricEvents<UnnamedMessageEvent>(m_UnnamedMessageSentEvent, m_UnnamedMessageReceivedEvent)
				.WithMetricEvents<NetworkVariableEvent>(m_NetworkVariableDeltaSentEvent, m_NetworkVariableDeltaReceivedEvent)
				.WithMetricEvents<OwnershipChangeEvent>(m_OwnershipChangeSentEvent, m_OwnershipChangeReceivedEvent)
				.WithMetricEvents<ObjectSpawnedEvent>(m_ObjectSpawnSentEvent, m_ObjectSpawnReceivedEvent)
				.WithMetricEvents<ObjectDestroyedEvent>(m_ObjectDestroySentEvent, m_ObjectDestroyReceivedEvent)
				.WithMetricEvents<RpcEvent>(m_RpcSentEvent, m_RpcReceivedEvent)
				.WithMetricEvents<ServerLogEvent>(m_ServerLogSentEvent, m_ServerLogReceivedEvent)
				.WithMetricEvents<SceneEventMetric>(m_SceneEventSentEvent, m_SceneEventReceivedEvent)
				.WithCounters(m_PacketSentCounter, m_PacketReceivedCounter)
				.WithGauges(m_RttToServerGauge)
				.WithGauges(m_NetworkObjectsGauge)
				.WithGauges(m_ConnectionsGauge)
				.WithGauges(m_PacketLossGauge)
				.Build();
			Dispatcher.RegisterObserver(NetcodeObserver.Observer);
		}

		public void SetConnectionId(ulong connectionId)
		{
			Dispatcher.SetConnectionId(connectionId);
		}

		public void TrackTransportBytesSent(long bytesCount)
		{
			m_TransportBytesSent.Increment(bytesCount);
		}

		public void TrackTransportBytesReceived(long bytesCount)
		{
			m_TransportBytesReceived.Increment(bytesCount);
		}

		public void TrackNetworkMessageSent(ulong receivedClientId, string messageType, long bytesCount)
		{
			if (CanSendMetrics)
			{
				m_NetworkMessageSentEvent.Mark(new NetworkMessageEvent(new ConnectionInfo(receivedClientId), messageType, bytesCount));
				IncrementMetricCount();
			}
		}

		public void TrackNetworkMessageReceived(ulong senderClientId, string messageType, long bytesCount)
		{
			if (CanSendMetrics)
			{
				m_NetworkMessageReceivedEvent.Mark(new NetworkMessageEvent(new ConnectionInfo(senderClientId), messageType, bytesCount));
				IncrementMetricCount();
			}
		}

		public void TrackNamedMessageSent(ulong receiverClientId, string messageName, long bytesCount)
		{
			if (CanSendMetrics)
			{
				m_NamedMessageSentEvent.Mark(new NamedMessageEvent(new ConnectionInfo(receiverClientId), messageName, bytesCount));
				IncrementMetricCount();
			}
		}

		public void TrackNamedMessageSent(IReadOnlyCollection<ulong> receiverClientIds, string messageName, long bytesCount)
		{
			foreach (ulong receiverClientId in receiverClientIds)
			{
				TrackNamedMessageSent(receiverClientId, messageName, bytesCount);
			}
		}

		public void TrackNamedMessageReceived(ulong senderClientId, string messageName, long bytesCount)
		{
			if (CanSendMetrics)
			{
				m_NamedMessageReceivedEvent.Mark(new NamedMessageEvent(new ConnectionInfo(senderClientId), messageName, bytesCount));
				IncrementMetricCount();
			}
		}

		public void TrackUnnamedMessageSent(ulong receiverClientId, long bytesCount)
		{
			if (CanSendMetrics)
			{
				m_UnnamedMessageSentEvent.Mark(new UnnamedMessageEvent(new ConnectionInfo(receiverClientId), bytesCount));
				IncrementMetricCount();
			}
		}

		public void TrackUnnamedMessageSent(IReadOnlyCollection<ulong> receiverClientIds, long bytesCount)
		{
			foreach (ulong receiverClientId in receiverClientIds)
			{
				TrackUnnamedMessageSent(receiverClientId, bytesCount);
			}
		}

		public void TrackUnnamedMessageReceived(ulong senderClientId, long bytesCount)
		{
			if (CanSendMetrics)
			{
				m_UnnamedMessageReceivedEvent.Mark(new UnnamedMessageEvent(new ConnectionInfo(senderClientId), bytesCount));
				IncrementMetricCount();
			}
		}

		public void TrackNetworkVariableDeltaSent(ulong receiverClientId, NetworkObject networkObject, string variableName, string networkBehaviourName, long bytesCount)
		{
			if (CanSendMetrics)
			{
				m_NetworkVariableDeltaSentEvent.Mark(new NetworkVariableEvent(new ConnectionInfo(receiverClientId), GetObjectIdentifier(networkObject), variableName, networkBehaviourName, bytesCount));
				IncrementMetricCount();
			}
		}

		public void TrackNetworkVariableDeltaReceived(ulong senderClientId, NetworkObject networkObject, string variableName, string networkBehaviourName, long bytesCount)
		{
			if (CanSendMetrics)
			{
				m_NetworkVariableDeltaReceivedEvent.Mark(new NetworkVariableEvent(new ConnectionInfo(senderClientId), GetObjectIdentifier(networkObject), variableName, networkBehaviourName, bytesCount));
				IncrementMetricCount();
			}
		}

		public void TrackOwnershipChangeSent(ulong receiverClientId, NetworkObject networkObject, long bytesCount)
		{
			if (CanSendMetrics)
			{
				m_OwnershipChangeSentEvent.Mark(new OwnershipChangeEvent(new ConnectionInfo(receiverClientId), GetObjectIdentifier(networkObject), bytesCount));
				IncrementMetricCount();
			}
		}

		public void TrackOwnershipChangeReceived(ulong senderClientId, NetworkObject networkObject, long bytesCount)
		{
			if (CanSendMetrics)
			{
				m_OwnershipChangeReceivedEvent.Mark(new OwnershipChangeEvent(new ConnectionInfo(senderClientId), GetObjectIdentifier(networkObject), bytesCount));
				IncrementMetricCount();
			}
		}

		public void TrackObjectSpawnSent(ulong receiverClientId, NetworkObject networkObject, long bytesCount)
		{
			if (CanSendMetrics)
			{
				m_ObjectSpawnSentEvent.Mark(new ObjectSpawnedEvent(new ConnectionInfo(receiverClientId), GetObjectIdentifier(networkObject), bytesCount));
				IncrementMetricCount();
			}
		}

		public void TrackObjectSpawnReceived(ulong senderClientId, NetworkObject networkObject, long bytesCount)
		{
			if (CanSendMetrics)
			{
				m_ObjectSpawnReceivedEvent.Mark(new ObjectSpawnedEvent(new ConnectionInfo(senderClientId), GetObjectIdentifier(networkObject), bytesCount));
				IncrementMetricCount();
			}
		}

		public void TrackObjectDestroySent(ulong receiverClientId, NetworkObject networkObject, long bytesCount)
		{
			if (CanSendMetrics)
			{
				m_ObjectDestroySentEvent.Mark(new ObjectDestroyedEvent(new ConnectionInfo(receiverClientId), GetObjectIdentifier(networkObject), bytesCount));
				IncrementMetricCount();
			}
		}

		public void TrackObjectDestroyReceived(ulong senderClientId, NetworkObject networkObject, long bytesCount)
		{
			if (CanSendMetrics)
			{
				m_ObjectDestroyReceivedEvent.Mark(new ObjectDestroyedEvent(new ConnectionInfo(senderClientId), GetObjectIdentifier(networkObject), bytesCount));
				IncrementMetricCount();
			}
		}

		public void TrackRpcSent(ulong receiverClientId, NetworkObject networkObject, string rpcName, string networkBehaviourName, long bytesCount)
		{
			if (CanSendMetrics)
			{
				m_RpcSentEvent.Mark(new RpcEvent(new ConnectionInfo(receiverClientId), GetObjectIdentifier(networkObject), rpcName, networkBehaviourName, bytesCount));
				IncrementMetricCount();
			}
		}

		public void TrackRpcSent(ulong[] receiverClientIds, NetworkObject networkObject, string rpcName, string networkBehaviourName, long bytesCount)
		{
			foreach (ulong receiverClientId in receiverClientIds)
			{
				TrackRpcSent(receiverClientId, networkObject, rpcName, networkBehaviourName, bytesCount);
			}
		}

		public void TrackRpcReceived(ulong senderClientId, NetworkObject networkObject, string rpcName, string networkBehaviourName, long bytesCount)
		{
			if (CanSendMetrics)
			{
				m_RpcReceivedEvent.Mark(new RpcEvent(new ConnectionInfo(senderClientId), GetObjectIdentifier(networkObject), rpcName, networkBehaviourName, bytesCount));
				IncrementMetricCount();
			}
		}

		public void TrackServerLogSent(ulong receiverClientId, uint logType, long bytesCount)
		{
			if (CanSendMetrics)
			{
				m_ServerLogSentEvent.Mark(new ServerLogEvent(new ConnectionInfo(receiverClientId), (Unity.Multiplayer.Tools.MetricTypes.LogLevel)logType, bytesCount));
				IncrementMetricCount();
			}
		}

		public void TrackServerLogReceived(ulong senderClientId, uint logType, long bytesCount)
		{
			if (CanSendMetrics)
			{
				m_ServerLogReceivedEvent.Mark(new ServerLogEvent(new ConnectionInfo(senderClientId), (Unity.Multiplayer.Tools.MetricTypes.LogLevel)logType, bytesCount));
				IncrementMetricCount();
			}
		}

		public void TrackSceneEventSent(IReadOnlyList<ulong> receiverClientIds, uint sceneEventType, string sceneName, long bytesCount)
		{
			foreach (ulong receiverClientId in receiverClientIds)
			{
				TrackSceneEventSent(receiverClientId, sceneEventType, sceneName, bytesCount);
			}
		}

		public void TrackSceneEventSent(ulong receiverClientId, uint sceneEventType, string sceneName, long bytesCount)
		{
			if (CanSendMetrics)
			{
				m_SceneEventSentEvent.Mark(new SceneEventMetric(new ConnectionInfo(receiverClientId), GetSceneEventTypeName(sceneEventType), sceneName, bytesCount));
				IncrementMetricCount();
			}
		}

		public void TrackSceneEventReceived(ulong senderClientId, uint sceneEventType, string sceneName, long bytesCount)
		{
			if (CanSendMetrics)
			{
				m_SceneEventReceivedEvent.Mark(new SceneEventMetric(new ConnectionInfo(senderClientId), GetSceneEventTypeName(sceneEventType), sceneName, bytesCount));
				IncrementMetricCount();
			}
		}

		public void TrackPacketSent(uint packetCount)
		{
			if (CanSendMetrics)
			{
				m_PacketSentCounter.Increment(packetCount);
				IncrementMetricCount();
			}
		}

		public void TrackPacketReceived(uint packetCount)
		{
			if (CanSendMetrics)
			{
				m_PacketReceivedCounter.Increment(packetCount);
				IncrementMetricCount();
			}
		}

		public void UpdateRttToServer(int rttMilliseconds)
		{
			if (CanSendMetrics)
			{
				double value = (double)rttMilliseconds * 0.001;
				m_RttToServerGauge.Set(value);
			}
		}

		public void UpdateNetworkObjectsCount(int count)
		{
			if (CanSendMetrics)
			{
				m_NetworkObjectsGauge.Set(count);
			}
		}

		public void UpdateConnectionsCount(int count)
		{
			if (CanSendMetrics)
			{
				m_ConnectionsGauge.Set(count);
			}
		}

		public void UpdatePacketLoss(float packetLoss)
		{
			if (CanSendMetrics)
			{
				m_PacketLossGauge.Set(packetLoss);
			}
		}

		public void DispatchFrame()
		{
			Dispatcher.Dispatch();
			m_NumberOfMetricsThisFrame = 0uL;
		}

		private void IncrementMetricCount()
		{
			m_NumberOfMetricsThisFrame++;
		}

		private static NetworkObjectIdentifier GetObjectIdentifier(NetworkObject networkObject)
		{
			return new NetworkObjectIdentifier(networkObject.GetNameForMetrics(), networkObject.NetworkObjectId);
		}
	}
}
