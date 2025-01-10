using System;

namespace Unity.Netcode
{
	internal interface INetworkHooks
	{
		void OnBeforeSendMessage<T>(ulong clientId, ref T message, NetworkDelivery delivery) where T : INetworkMessage;

		void OnAfterSendMessage<T>(ulong clientId, ref T message, NetworkDelivery delivery, int messageSizeBytes) where T : INetworkMessage;

		void OnBeforeReceiveMessage(ulong senderId, Type messageType, int messageSizeBytes);

		void OnAfterReceiveMessage(ulong senderId, Type messageType, int messageSizeBytes);

		void OnBeforeSendBatch(ulong clientId, int messageCount, int batchSizeInBytes, NetworkDelivery delivery);

		void OnAfterSendBatch(ulong clientId, int messageCount, int batchSizeInBytes, NetworkDelivery delivery);

		void OnBeforeReceiveBatch(ulong senderId, int messageCount, int batchSizeInBytes);

		void OnAfterReceiveBatch(ulong senderId, int messageCount, int batchSizeInBytes);

		bool OnVerifyCanSend(ulong destinationId, Type messageType, NetworkDelivery delivery);

		bool OnVerifyCanReceive(ulong senderId, Type messageType, FastBufferReader messageContent, ref NetworkContext context);

		void OnBeforeHandleMessage<T>(ref T message, ref NetworkContext context) where T : INetworkMessage;

		void OnAfterHandleMessage<T>(ref T message, ref NetworkContext context) where T : INetworkMessage;
	}
}
