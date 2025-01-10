using System;
using System.Collections.Generic;
using Unity.Profiling;

namespace Unity.Netcode
{
	internal class ProfilingHooks : INetworkHooks
	{
		private Dictionary<Type, ProfilerMarker> m_HandlerProfilerMarkers = new Dictionary<Type, ProfilerMarker>();

		private Dictionary<Type, ProfilerMarker> m_SenderProfilerMarkers = new Dictionary<Type, ProfilerMarker>();

		private readonly ProfilerMarker m_SendBatch = new ProfilerMarker("NetworkMessageManager.SendBatch");

		private readonly ProfilerMarker m_ReceiveBatch = new ProfilerMarker("NetworkMessageManager.ReceiveBatchBatch");

		private ProfilerMarker GetHandlerProfilerMarker(Type type)
		{
			if (m_HandlerProfilerMarkers.TryGetValue(type, out var value))
			{
				return value;
			}
			value = new ProfilerMarker("NetworkMessageManager.DeserializeAndHandle." + type.Name);
			m_HandlerProfilerMarkers[type] = value;
			return value;
		}

		private ProfilerMarker GetSenderProfilerMarker(Type type)
		{
			if (m_SenderProfilerMarkers.TryGetValue(type, out var value))
			{
				return value;
			}
			value = new ProfilerMarker("NetworkMessageManager.SerializeAndEnqueue." + type.Name);
			m_SenderProfilerMarkers[type] = value;
			return value;
		}

		public void OnBeforeSendMessage<T>(ulong clientId, ref T message, NetworkDelivery delivery) where T : INetworkMessage
		{
		}

		public void OnAfterSendMessage<T>(ulong clientId, ref T message, NetworkDelivery delivery, int messageSizeBytes) where T : INetworkMessage
		{
		}

		public void OnBeforeReceiveMessage(ulong senderId, Type messageType, int messageSizeBytes)
		{
		}

		public void OnAfterReceiveMessage(ulong senderId, Type messageType, int messageSizeBytes)
		{
		}

		public void OnBeforeSendBatch(ulong clientId, int messageCount, int batchSizeInBytes, NetworkDelivery delivery)
		{
		}

		public void OnAfterSendBatch(ulong clientId, int messageCount, int batchSizeInBytes, NetworkDelivery delivery)
		{
		}

		public void OnBeforeReceiveBatch(ulong senderId, int messageCount, int batchSizeInBytes)
		{
		}

		public void OnAfterReceiveBatch(ulong senderId, int messageCount, int batchSizeInBytes)
		{
		}

		public bool OnVerifyCanSend(ulong destinationId, Type messageType, NetworkDelivery delivery)
		{
			return true;
		}

		public bool OnVerifyCanReceive(ulong senderId, Type messageType, FastBufferReader messageContent, ref NetworkContext context)
		{
			return true;
		}

		public void OnBeforeHandleMessage<T>(ref T message, ref NetworkContext context) where T : INetworkMessage
		{
		}

		public void OnAfterHandleMessage<T>(ref T message, ref NetworkContext context) where T : INetworkMessage
		{
		}
	}
}
