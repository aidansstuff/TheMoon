using Unity.Netcode;
using UnityEngine;

namespace __GEN
{
	internal class INetworkMessageHelper
	{
		[RuntimeInitializeOnLoadMethod]
		internal static void InitializeMessages()
		{
			ILPPMessageProvider.__network_message_types.Add(new NetworkMessageManager.MessageWithHandler
			{
				MessageType = typeof(DisconnectReasonMessage),
				Handler = NetworkMessageManager.ReceiveMessage<DisconnectReasonMessage>,
				GetVersion = NetworkMessageManager.CreateMessageAndGetVersion<DisconnectReasonMessage>
			});
			ILPPMessageProvider.__network_message_types.Add(new NetworkMessageManager.MessageWithHandler
			{
				MessageType = typeof(ChangeOwnershipMessage),
				Handler = NetworkMessageManager.ReceiveMessage<ChangeOwnershipMessage>,
				GetVersion = NetworkMessageManager.CreateMessageAndGetVersion<ChangeOwnershipMessage>
			});
			ILPPMessageProvider.__network_message_types.Add(new NetworkMessageManager.MessageWithHandler
			{
				MessageType = typeof(ConnectionApprovedMessage),
				Handler = NetworkMessageManager.ReceiveMessage<ConnectionApprovedMessage>,
				GetVersion = NetworkMessageManager.CreateMessageAndGetVersion<ConnectionApprovedMessage>
			});
			ILPPMessageProvider.__network_message_types.Add(new NetworkMessageManager.MessageWithHandler
			{
				MessageType = typeof(ConnectionRequestMessage),
				Handler = NetworkMessageManager.ReceiveMessage<ConnectionRequestMessage>,
				GetVersion = NetworkMessageManager.CreateMessageAndGetVersion<ConnectionRequestMessage>
			});
			ILPPMessageProvider.__network_message_types.Add(new NetworkMessageManager.MessageWithHandler
			{
				MessageType = typeof(CreateObjectMessage),
				Handler = NetworkMessageManager.ReceiveMessage<CreateObjectMessage>,
				GetVersion = NetworkMessageManager.CreateMessageAndGetVersion<CreateObjectMessage>
			});
			ILPPMessageProvider.__network_message_types.Add(new NetworkMessageManager.MessageWithHandler
			{
				MessageType = typeof(DestroyObjectMessage),
				Handler = NetworkMessageManager.ReceiveMessage<DestroyObjectMessage>,
				GetVersion = NetworkMessageManager.CreateMessageAndGetVersion<DestroyObjectMessage>
			});
			ILPPMessageProvider.__network_message_types.Add(new NetworkMessageManager.MessageWithHandler
			{
				MessageType = typeof(NamedMessage),
				Handler = NetworkMessageManager.ReceiveMessage<NamedMessage>,
				GetVersion = NetworkMessageManager.CreateMessageAndGetVersion<NamedMessage>
			});
			ILPPMessageProvider.__network_message_types.Add(new NetworkMessageManager.MessageWithHandler
			{
				MessageType = typeof(NetworkVariableDeltaMessage),
				Handler = NetworkMessageManager.ReceiveMessage<NetworkVariableDeltaMessage>,
				GetVersion = NetworkMessageManager.CreateMessageAndGetVersion<NetworkVariableDeltaMessage>
			});
			ILPPMessageProvider.__network_message_types.Add(new NetworkMessageManager.MessageWithHandler
			{
				MessageType = typeof(ParentSyncMessage),
				Handler = NetworkMessageManager.ReceiveMessage<ParentSyncMessage>,
				GetVersion = NetworkMessageManager.CreateMessageAndGetVersion<ParentSyncMessage>
			});
			ILPPMessageProvider.__network_message_types.Add(new NetworkMessageManager.MessageWithHandler
			{
				MessageType = typeof(ServerRpcMessage),
				Handler = NetworkMessageManager.ReceiveMessage<ServerRpcMessage>,
				GetVersion = NetworkMessageManager.CreateMessageAndGetVersion<ServerRpcMessage>
			});
			ILPPMessageProvider.__network_message_types.Add(new NetworkMessageManager.MessageWithHandler
			{
				MessageType = typeof(ClientRpcMessage),
				Handler = NetworkMessageManager.ReceiveMessage<ClientRpcMessage>,
				GetVersion = NetworkMessageManager.CreateMessageAndGetVersion<ClientRpcMessage>
			});
			ILPPMessageProvider.__network_message_types.Add(new NetworkMessageManager.MessageWithHandler
			{
				MessageType = typeof(SceneEventMessage),
				Handler = NetworkMessageManager.ReceiveMessage<SceneEventMessage>,
				GetVersion = NetworkMessageManager.CreateMessageAndGetVersion<SceneEventMessage>
			});
			ILPPMessageProvider.__network_message_types.Add(new NetworkMessageManager.MessageWithHandler
			{
				MessageType = typeof(ServerLogMessage),
				Handler = NetworkMessageManager.ReceiveMessage<ServerLogMessage>,
				GetVersion = NetworkMessageManager.CreateMessageAndGetVersion<ServerLogMessage>
			});
			ILPPMessageProvider.__network_message_types.Add(new NetworkMessageManager.MessageWithHandler
			{
				MessageType = typeof(TimeSyncMessage),
				Handler = NetworkMessageManager.ReceiveMessage<TimeSyncMessage>,
				GetVersion = NetworkMessageManager.CreateMessageAndGetVersion<TimeSyncMessage>
			});
			ILPPMessageProvider.__network_message_types.Add(new NetworkMessageManager.MessageWithHandler
			{
				MessageType = typeof(UnnamedMessage),
				Handler = NetworkMessageManager.ReceiveMessage<UnnamedMessage>,
				GetVersion = NetworkMessageManager.CreateMessageAndGetVersion<UnnamedMessage>
			});
		}
	}
}
