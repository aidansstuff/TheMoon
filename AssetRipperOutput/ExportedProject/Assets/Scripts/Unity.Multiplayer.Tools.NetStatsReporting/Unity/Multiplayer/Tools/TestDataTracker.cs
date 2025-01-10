using Unity.Multiplayer.Tools.MetricTypes;
using Unity.Multiplayer.Tools.NetStats;

namespace Unity.Multiplayer.Tools
{
	internal class TestDataTracker : ITestDataTracker
	{
		private readonly Counter m_TransportBytesSent = new Counter(DirectedMetricType.TotalBytesSent.GetId(), 0L)
		{
			ShouldResetOnDispatch = true
		};

		private readonly Counter m_TransportBytesReceived = new Counter(DirectedMetricType.TotalBytesReceived.GetId(), 0L)
		{
			ShouldResetOnDispatch = true
		};

		private readonly EventMetric<NetworkMessageEvent> m_NetworkMessageSentEvent = new EventMetric<NetworkMessageEvent>(DirectedMetricType.NetworkMessageSent.GetId());

		private readonly EventMetric<NetworkMessageEvent> m_NetworkMessageReceivedEvent = new EventMetric<NetworkMessageEvent>(DirectedMetricType.NetworkMessageReceived.GetId());

		private readonly EventMetric<NamedMessageEvent> m_NamedMessageSentEvent = new EventMetric<NamedMessageEvent>(DirectedMetricType.NamedMessageSent.GetId());

		private readonly EventMetric<NamedMessageEvent> m_NamedMessageReceivedEvent = new EventMetric<NamedMessageEvent>(DirectedMetricType.NamedMessageReceived.GetId());

		private readonly EventMetric<UnnamedMessageEvent> m_UnnamedMessageSentEvent = new EventMetric<UnnamedMessageEvent>(DirectedMetricType.UnnamedMessageSent.GetId());

		private readonly EventMetric<UnnamedMessageEvent> m_UnnamedMessageReceivedEvent = new EventMetric<UnnamedMessageEvent>(DirectedMetricType.UnnamedMessageReceived.GetId());

		private readonly EventMetric<NetworkVariableEvent> m_NetworkVariableDeltaSentEvent = new EventMetric<NetworkVariableEvent>(DirectedMetricType.NetworkVariableDeltaSent.GetId());

		private readonly EventMetric<NetworkVariableEvent> m_NetworkVariableDeltaReceivedEvent = new EventMetric<NetworkVariableEvent>(DirectedMetricType.NetworkVariableDeltaReceived.GetId());

		private readonly EventMetric<OwnershipChangeEvent> m_OwnershipChangeSentEvent = new EventMetric<OwnershipChangeEvent>(DirectedMetricType.OwnershipChangeSent.GetId());

		private readonly EventMetric<OwnershipChangeEvent> m_OwnershipChangeReceivedEvent = new EventMetric<OwnershipChangeEvent>(DirectedMetricType.OwnershipChangeReceived.GetId());

		private readonly EventMetric<ObjectSpawnedEvent> m_ObjectSpawnSentEvent = new EventMetric<ObjectSpawnedEvent>(DirectedMetricType.ObjectSpawnedSent.GetId());

		private readonly EventMetric<ObjectSpawnedEvent> m_ObjectSpawnReceivedEvent = new EventMetric<ObjectSpawnedEvent>(DirectedMetricType.ObjectSpawnedReceived.GetId());

		private readonly EventMetric<ObjectDestroyedEvent> m_ObjectDestroySentEvent = new EventMetric<ObjectDestroyedEvent>(DirectedMetricType.ObjectDestroyedSent.GetId());

		private readonly EventMetric<ObjectDestroyedEvent> m_ObjectDestroyReceivedEvent = new EventMetric<ObjectDestroyedEvent>(DirectedMetricType.ObjectDestroyedReceived.GetId());

		private readonly EventMetric<RpcEvent> m_RpcSentEvent = new EventMetric<RpcEvent>(DirectedMetricType.RpcSent.GetId());

		private readonly EventMetric<RpcEvent> m_RpcReceivedEvent = new EventMetric<RpcEvent>(DirectedMetricType.RpcReceived.GetId());

		private readonly EventMetric<ServerLogEvent> m_ServerLogSentEvent = new EventMetric<ServerLogEvent>(DirectedMetricType.ServerLogSent.GetId());

		private readonly EventMetric<ServerLogEvent> m_ServerLogReceivedEvent = new EventMetric<ServerLogEvent>(DirectedMetricType.ServerLogReceived.GetId());

		private readonly EventMetric<SceneEventMetric> m_SceneEventSentEvent = new EventMetric<SceneEventMetric>(DirectedMetricType.SceneEventSent.GetId());

		private readonly EventMetric<SceneEventMetric> m_SceneEventReceivedEvent = new EventMetric<SceneEventMetric>(DirectedMetricType.SceneEventReceived.GetId());

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

		private readonly Gauge m_PacketLoss = new Gauge(NetworkMetricTypes.PacketLoss.Id)
		{
			ShouldResetOnDispatch = true
		};

		public IMetricDispatcher Dispatcher { get; }

		public TestDataTracker()
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
				.WithGauges(m_PacketLoss)
				.Build();
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

		public void TrackNetworkMessageSent(NetworkMessageEvent networkMessageEvent)
		{
			m_NetworkMessageSentEvent.Mark(networkMessageEvent);
		}

		public void TrackNetworkMessageReceived(NetworkMessageEvent networkMessageEvent)
		{
			m_NetworkMessageReceivedEvent.Mark(networkMessageEvent);
		}

		public void TrackNamedMessageSent(NamedMessageEvent namedMessageEvent)
		{
			m_NamedMessageSentEvent.Mark(namedMessageEvent);
		}

		public void TrackNamedMessageReceived(NamedMessageEvent namedMessageEvent)
		{
			m_NamedMessageReceivedEvent.Mark(namedMessageEvent);
		}

		public void TrackUnnamedMessageSent(UnnamedMessageEvent unnamedMessageEvent)
		{
			m_UnnamedMessageSentEvent.Mark(unnamedMessageEvent);
		}

		public void TrackUnnamedMessageReceived(UnnamedMessageEvent unnamedMessageEvent)
		{
			m_UnnamedMessageReceivedEvent.Mark(unnamedMessageEvent);
		}

		public void TrackNetworkVariableDeltaSent(NetworkVariableEvent networkVariableEvent)
		{
			m_NetworkVariableDeltaSentEvent.Mark(networkVariableEvent);
		}

		public void TrackNetworkVariableDeltaReceived(NetworkVariableEvent networkVariableEvent)
		{
			m_NetworkVariableDeltaReceivedEvent.Mark(networkVariableEvent);
		}

		public void TrackOwnershipChangeSent(OwnershipChangeEvent ownershipChangeEvent)
		{
			m_OwnershipChangeSentEvent.Mark(ownershipChangeEvent);
		}

		public void TrackOwnershipChangeReceived(OwnershipChangeEvent ownershipChangeEvent)
		{
			m_OwnershipChangeReceivedEvent.Mark(ownershipChangeEvent);
		}

		public void TrackObjectSpawnSent(ObjectSpawnedEvent objectSpawnedEvent)
		{
			m_ObjectSpawnSentEvent.Mark(objectSpawnedEvent);
		}

		public void TrackObjectSpawnReceived(ObjectSpawnedEvent objectSpawnedEvent)
		{
			m_ObjectSpawnReceivedEvent.Mark(objectSpawnedEvent);
		}

		public void TrackObjectDestroySent(ObjectDestroyedEvent objectDestroyedEvent)
		{
			m_ObjectDestroySentEvent.Mark(objectDestroyedEvent);
		}

		public void TrackObjectDestroyReceived(ObjectDestroyedEvent objectDestroyedEvent)
		{
			m_ObjectDestroyReceivedEvent.Mark(objectDestroyedEvent);
		}

		public void TrackRpcSent(RpcEvent rpcEvent)
		{
			m_RpcSentEvent.Mark(rpcEvent);
		}

		public void TrackRpcReceived(RpcEvent rpcEvent)
		{
			m_RpcReceivedEvent.Mark(rpcEvent);
		}

		public void TrackServerLogSent(ServerLogEvent serverLogEvent)
		{
			m_ServerLogSentEvent.Mark(serverLogEvent);
		}

		public void TrackServerLogReceived(ServerLogEvent serverLogEvent)
		{
			m_ServerLogReceivedEvent.Mark(serverLogEvent);
		}

		public void TrackSceneEventSent(SceneEventMetric sceneEvent)
		{
			m_SceneEventSentEvent.Mark(sceneEvent);
		}

		public void TrackSceneEventReceived(SceneEventMetric sceneEvent)
		{
			m_SceneEventReceivedEvent.Mark(sceneEvent);
		}

		public void TrackPacketSent(int packetCount)
		{
			m_PacketSentCounter.Increment(packetCount);
		}

		public void TrackPacketReceived(int packetCount)
		{
			m_PacketReceivedCounter.Increment(packetCount);
		}

		public void TrackRttToServer(int rtt)
		{
			m_RttToServerGauge.Set(rtt);
		}

		public void UpdateNetworkObjectsCount(int count)
		{
			m_NetworkObjectsGauge.Set(count);
		}

		public void UpdateConnectionsCount(int count)
		{
			m_ConnectionsGauge.Set(count);
		}

		public void UpdatePacketLoss(float count)
		{
			m_PacketLoss.Set(count);
		}
	}
}
