using System;
using System.Collections.Generic;
using Unity.Collections;

namespace Unity.Netcode
{
	public class CustomMessagingManager
	{
		public delegate void UnnamedMessageDelegate(ulong clientId, FastBufferReader reader);

		public delegate void HandleNamedMessageDelegate(ulong senderClientId, FastBufferReader messagePayload);

		private readonly NetworkManager m_NetworkManager;

		private Dictionary<ulong, HandleNamedMessageDelegate> m_NamedMessageHandlers32 = new Dictionary<ulong, HandleNamedMessageDelegate>();

		private Dictionary<ulong, HandleNamedMessageDelegate> m_NamedMessageHandlers64 = new Dictionary<ulong, HandleNamedMessageDelegate>();

		private Dictionary<ulong, string> m_MessageHandlerNameLookup32 = new Dictionary<ulong, string>();

		private Dictionary<ulong, string> m_MessageHandlerNameLookup64 = new Dictionary<ulong, string>();

		public event UnnamedMessageDelegate OnUnnamedMessage;

		internal CustomMessagingManager(NetworkManager networkManager)
		{
			m_NetworkManager = networkManager;
		}

		internal void InvokeUnnamedMessage(ulong clientId, FastBufferReader reader, int serializedHeaderSize)
		{
			if (this.OnUnnamedMessage != null)
			{
				int position = reader.Position;
				Delegate[] invocationList = this.OnUnnamedMessage.GetInvocationList();
				foreach (Delegate obj in invocationList)
				{
					reader.Seek(position);
					((UnnamedMessageDelegate)obj)(clientId, reader);
				}
			}
			m_NetworkManager.NetworkMetrics.TrackUnnamedMessageReceived(clientId, reader.Length + serializedHeaderSize);
		}

		public void SendUnnamedMessageToAll(FastBufferWriter messageBuffer, NetworkDelivery networkDelivery = NetworkDelivery.ReliableSequenced)
		{
			SendUnnamedMessage(m_NetworkManager.ConnectedClientsIds, messageBuffer, networkDelivery);
		}

		public void SendUnnamedMessage(IReadOnlyList<ulong> clientIds, FastBufferWriter messageBuffer, NetworkDelivery networkDelivery = NetworkDelivery.ReliableSequenced)
		{
			if (!m_NetworkManager.IsServer)
			{
				throw new InvalidOperationException("Can not send unnamed messages to multiple users as a client");
			}
			if (clientIds == null)
			{
				throw new ArgumentNullException("clientIds", "You must pass in a valid clientId List");
			}
			if (m_NetworkManager.IsHost)
			{
				for (int i = 0; i < clientIds.Count; i++)
				{
					if (clientIds[i] == m_NetworkManager.LocalClientId)
					{
						InvokeUnnamedMessage(m_NetworkManager.LocalClientId, new FastBufferReader(messageBuffer, Allocator.None), 0);
					}
				}
			}
			UnnamedMessage unnamedMessage = default(UnnamedMessage);
			unnamedMessage.SendData = messageBuffer;
			UnnamedMessage message = unnamedMessage;
			int num = m_NetworkManager.ConnectionManager.SendMessage(ref message, networkDelivery, in clientIds);
			if (num != 0)
			{
				m_NetworkManager.NetworkMetrics.TrackUnnamedMessageSent(clientIds, num);
			}
		}

		public void SendUnnamedMessage(ulong clientId, FastBufferWriter messageBuffer, NetworkDelivery networkDelivery = NetworkDelivery.ReliableSequenced)
		{
			if (m_NetworkManager.IsHost && clientId == m_NetworkManager.LocalClientId)
			{
				InvokeUnnamedMessage(m_NetworkManager.LocalClientId, new FastBufferReader(messageBuffer, Allocator.None), 0);
				return;
			}
			UnnamedMessage unnamedMessage = default(UnnamedMessage);
			unnamedMessage.SendData = messageBuffer;
			UnnamedMessage message = unnamedMessage;
			int num = m_NetworkManager.ConnectionManager.SendMessage(ref message, networkDelivery, clientId);
			if (num != 0)
			{
				m_NetworkManager.NetworkMetrics.TrackUnnamedMessageSent(clientId, num);
			}
		}

		internal void InvokeNamedMessage(ulong hash, ulong sender, FastBufferReader reader, int serializedHeaderSize)
		{
			int num = reader.Length + serializedHeaderSize;
			if (m_NetworkManager == null)
			{
				if (m_NamedMessageHandlers32.TryGetValue(hash, out var value))
				{
					string messageName = m_MessageHandlerNameLookup32[hash];
					value(sender, reader);
					m_NetworkManager.NetworkMetrics.TrackNamedMessageReceived(sender, messageName, num);
				}
				if (m_NamedMessageHandlers64.TryGetValue(hash, out var value2))
				{
					string messageName2 = m_MessageHandlerNameLookup64[hash];
					value2(sender, reader);
					m_NetworkManager.NetworkMetrics.TrackNamedMessageReceived(sender, messageName2, num);
				}
				return;
			}
			switch (m_NetworkManager.NetworkConfig.RpcHashSize)
			{
			case HashSize.VarIntFourBytes:
			{
				if (m_NamedMessageHandlers32.TryGetValue(hash, out var value4))
				{
					string messageName4 = m_MessageHandlerNameLookup32[hash];
					value4(sender, reader);
					m_NetworkManager.NetworkMetrics.TrackNamedMessageReceived(sender, messageName4, num);
				}
				break;
			}
			case HashSize.VarIntEightBytes:
			{
				if (m_NamedMessageHandlers64.TryGetValue(hash, out var value3))
				{
					string messageName3 = m_MessageHandlerNameLookup64[hash];
					value3(sender, reader);
					m_NetworkManager.NetworkMetrics.TrackNamedMessageReceived(sender, messageName3, num);
				}
				break;
			}
			}
		}

		public void RegisterNamedMessageHandler(string name, HandleNamedMessageDelegate callback)
		{
			uint num = name.Hash32();
			ulong key = name.Hash64();
			m_NamedMessageHandlers32[num] = callback;
			m_NamedMessageHandlers64[key] = callback;
			m_MessageHandlerNameLookup32[num] = name;
			m_MessageHandlerNameLookup64[key] = name;
		}

		public void UnregisterNamedMessageHandler(string name)
		{
			uint num = name.Hash32();
			ulong key = name.Hash64();
			m_NamedMessageHandlers32.Remove(num);
			m_NamedMessageHandlers64.Remove(key);
			m_MessageHandlerNameLookup32.Remove(num);
			m_MessageHandlerNameLookup64.Remove(key);
		}

		public void SendNamedMessageToAll(string messageName, FastBufferWriter messageStream, NetworkDelivery networkDelivery = NetworkDelivery.ReliableSequenced)
		{
			SendNamedMessage(messageName, m_NetworkManager.ConnectedClientsIds, messageStream, networkDelivery);
		}

		public void SendNamedMessage(string messageName, ulong clientId, FastBufferWriter messageStream, NetworkDelivery networkDelivery = NetworkDelivery.ReliableSequenced)
		{
			ulong hash = 0uL;
			switch (m_NetworkManager.NetworkConfig.RpcHashSize)
			{
			case HashSize.VarIntFourBytes:
				hash = messageName.Hash32();
				break;
			case HashSize.VarIntEightBytes:
				hash = messageName.Hash64();
				break;
			}
			if (m_NetworkManager.IsHost && clientId == m_NetworkManager.LocalClientId)
			{
				InvokeNamedMessage(hash, m_NetworkManager.LocalClientId, new FastBufferReader(messageStream, Allocator.None), 0);
				return;
			}
			NamedMessage namedMessage = default(NamedMessage);
			namedMessage.Hash = hash;
			namedMessage.SendData = messageStream;
			NamedMessage message = namedMessage;
			int num = m_NetworkManager.ConnectionManager.SendMessage(ref message, networkDelivery, clientId);
			if (num != 0)
			{
				m_NetworkManager.NetworkMetrics.TrackNamedMessageSent(clientId, messageName, num);
			}
		}

		public void SendNamedMessage(string messageName, IReadOnlyList<ulong> clientIds, FastBufferWriter messageStream, NetworkDelivery networkDelivery = NetworkDelivery.ReliableSequenced)
		{
			if (!m_NetworkManager.IsServer)
			{
				throw new InvalidOperationException("Can not send unnamed messages to multiple users as a client");
			}
			if (clientIds == null)
			{
				throw new ArgumentNullException("clientIds", "You must pass in a valid clientId List");
			}
			ulong hash = 0uL;
			switch (m_NetworkManager.NetworkConfig.RpcHashSize)
			{
			case HashSize.VarIntFourBytes:
				hash = messageName.Hash32();
				break;
			case HashSize.VarIntEightBytes:
				hash = messageName.Hash64();
				break;
			}
			if (m_NetworkManager.IsHost)
			{
				for (int i = 0; i < clientIds.Count; i++)
				{
					if (clientIds[i] == m_NetworkManager.LocalClientId)
					{
						InvokeNamedMessage(hash, m_NetworkManager.LocalClientId, new FastBufferReader(messageStream, Allocator.None), 0);
					}
				}
			}
			NamedMessage namedMessage = default(NamedMessage);
			namedMessage.Hash = hash;
			namedMessage.SendData = messageStream;
			NamedMessage message = namedMessage;
			int num = m_NetworkManager.ConnectionManager.SendMessage(ref message, networkDelivery, in clientIds);
			if (num != 0)
			{
				m_NetworkManager.NetworkMetrics.TrackNamedMessageSent(clientIds, messageName, num);
			}
		}
	}
}
