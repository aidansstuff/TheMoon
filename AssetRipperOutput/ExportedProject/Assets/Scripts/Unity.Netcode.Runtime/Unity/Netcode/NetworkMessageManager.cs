using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;

namespace Unity.Netcode
{
	internal class NetworkMessageManager : IDisposable
	{
		private struct ReceiveQueueItem
		{
			public FastBufferReader Reader;

			public NetworkMessageHeader Header;

			public ulong SenderId;

			public float Timestamp;

			public int MessageHeaderSerializedSize;
		}

		private struct SendQueueItem
		{
			public NetworkBatchHeader BatchHeader;

			public FastBufferWriter Writer;

			public readonly NetworkDelivery NetworkDelivery;

			public SendQueueItem(NetworkDelivery delivery, int writerSize, Allocator writerAllocator, int maxWriterSize = -1)
			{
				Writer = new FastBufferWriter(writerSize, writerAllocator, maxWriterSize);
				NetworkDelivery = delivery;
				BatchHeader = new NetworkBatchHeader
				{
					Magic = 4448
				};
			}
		}

		internal delegate void MessageHandler(FastBufferReader reader, ref NetworkContext context, NetworkMessageManager manager);

		internal delegate int VersionGetter();

		internal struct MessageWithHandler
		{
			public Type MessageType;

			public MessageHandler Handler;

			public VersionGetter GetVersion;
		}

		private struct PointerListWrapper<T> : IReadOnlyList<T>, IEnumerable<T>, IEnumerable, IReadOnlyCollection<T> where T : unmanaged
		{
			private unsafe T* m_Value;

			private int m_Length;

			public int Count
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return m_Length;
				}
			}

			public unsafe T this[int index]
			{
				[MethodImpl(MethodImplOptions.AggressiveInlining)]
				get
				{
					return m_Value[index];
				}
			}

			internal unsafe PointerListWrapper(T* ptr, int length)
			{
				m_Value = ptr;
				m_Length = length;
			}

			public IEnumerator<T> GetEnumerator()
			{
				throw new NotImplementedException();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return GetEnumerator();
			}
		}

		public bool StopProcessing;

		private NativeList<ReceiveQueueItem> m_IncomingMessageQueue = new NativeList<ReceiveQueueItem>(16, Allocator.Persistent);

		private MessageHandler[] m_MessageHandlers = new MessageHandler[4];

		private Type[] m_ReverseTypeMap = new Type[4];

		private Dictionary<Type, uint> m_MessageTypes = new Dictionary<Type, uint>();

		private Dictionary<ulong, NativeList<SendQueueItem>> m_SendQueues = new Dictionary<ulong, NativeList<SendQueueItem>>();

		private HashSet<ulong> m_DisconnectedClients = new HashSet<ulong>();

		private Dictionary<ulong, Dictionary<Type, int>> m_PerClientMessageVersions = new Dictionary<ulong, Dictionary<Type, int>>();

		private Dictionary<uint, Type> m_MessagesByHash = new Dictionary<uint, Type>();

		private Dictionary<Type, int> m_LocalVersions = new Dictionary<Type, int>();

		private List<INetworkHooks> m_Hooks = new List<INetworkHooks>();

		private uint m_HighMessageType;

		private object m_Owner;

		private INetworkMessageSender m_Sender;

		private bool m_Disposed;

		public const int DefaultNonFragmentedMessageMaxSize = 1296;

		public int NonFragmentedMessageMaxSize = 1296;

		public int FragmentedMessageMaxSize = int.MaxValue;

		internal Type[] MessageTypes => m_ReverseTypeMap;

		internal MessageHandler[] MessageHandlers => m_MessageHandlers;

		internal uint MessageHandlerCount => m_HighMessageType;

		internal uint GetMessageType(Type t)
		{
			return m_MessageTypes[t];
		}

		internal List<MessageWithHandler> PrioritizeMessageOrder(List<MessageWithHandler> allowedTypes)
		{
			List<MessageWithHandler> list = new List<MessageWithHandler>();
			foreach (MessageWithHandler allowedType in allowedTypes)
			{
				if (allowedType.MessageType.FullName == typeof(ConnectionRequestMessage).FullName || allowedType.MessageType.FullName == typeof(ConnectionApprovedMessage).FullName)
				{
					list.Add(allowedType);
				}
			}
			foreach (MessageWithHandler allowedType2 in allowedTypes)
			{
				if (allowedType2.MessageType.FullName != typeof(ConnectionRequestMessage).FullName && allowedType2.MessageType.FullName != typeof(ConnectionApprovedMessage).FullName)
				{
					list.Add(allowedType2);
				}
			}
			return list;
		}

		public NetworkMessageManager(INetworkMessageSender sender, object owner, INetworkMessageProvider provider = null)
		{
			try
			{
				m_Sender = sender;
				m_Owner = owner;
				if (provider == null)
				{
					provider = default(ILPPMessageProvider);
				}
				List<MessageWithHandler> messages = provider.GetMessages();
				messages.Sort((MessageWithHandler a, MessageWithHandler b) => string.CompareOrdinal(a.MessageType.FullName, b.MessageType.FullName));
				messages = PrioritizeMessageOrder(messages);
				foreach (MessageWithHandler item in messages)
				{
					RegisterMessageType(item);
				}
			}
			catch (Exception)
			{
				Dispose();
				throw;
			}
		}

		public void Dispose()
		{
			if (m_Disposed)
			{
				return;
			}
			foreach (KeyValuePair<ulong, NativeList<SendQueueItem>> sendQueue in m_SendQueues)
			{
				ClientDisconnected(sendQueue.Key);
			}
			CleanupDisconnectedClients();
			for (int i = 0; i < m_IncomingMessageQueue.Length; i++)
			{
				m_IncomingMessageQueue.ElementAt(i).Reader.Dispose();
			}
			m_IncomingMessageQueue.Dispose();
			m_Disposed = true;
		}

		~NetworkMessageManager()
		{
			Dispose();
		}

		public void Hook(INetworkHooks hooks)
		{
			m_Hooks.Add(hooks);
		}

		public void Unhook(INetworkHooks hooks)
		{
			m_Hooks.Remove(hooks);
		}

		private void RegisterMessageType(MessageWithHandler messageWithHandler)
		{
			if (m_HighMessageType == m_MessageHandlers.Length)
			{
				Array.Resize(ref m_MessageHandlers, 2 * m_MessageHandlers.Length);
				Array.Resize(ref m_ReverseTypeMap, 2 * m_ReverseTypeMap.Length);
			}
			m_MessageHandlers[m_HighMessageType] = messageWithHandler.Handler;
			m_ReverseTypeMap[m_HighMessageType] = messageWithHandler.MessageType;
			m_MessagesByHash[messageWithHandler.MessageType.FullName.Hash32()] = messageWithHandler.MessageType;
			m_MessageTypes[messageWithHandler.MessageType] = m_HighMessageType++;
			m_LocalVersions[messageWithHandler.MessageType] = messageWithHandler.GetVersion();
		}

		public int GetLocalVersion(Type messageType)
		{
			return m_LocalVersions[messageType];
		}

		internal static string ByteArrayToString(byte[] ba, int offset, int count)
		{
			StringBuilder stringBuilder = new StringBuilder(ba.Length * 2);
			for (int i = offset; i < offset + count; i++)
			{
				stringBuilder.AppendFormat("{0:x2} ", ba[i]);
			}
			return stringBuilder.ToString();
		}

		internal unsafe void HandleIncomingData(ulong clientId, ArraySegment<byte> data, float receiveTime)
		{
			fixed (byte* ptr = data.Array)
			{
				FastBufferReader reader = new FastBufferReader(ptr + data.Offset, Allocator.None, data.Count);
				if (!reader.TryBeginRead(sizeof(NetworkBatchHeader)))
				{
					NetworkLog.LogError("Received a packet too small to contain a BatchHeader. Ignoring it.");
					return;
				}
				reader.ReadValue(out NetworkBatchHeader value, default(FastBufferWriter.ForStructs));
				if (value.Magic != 4448)
				{
					NetworkLog.LogError($"Received a packet with an invalid Magic Value. Please report this to the Netcode for GameObjects team at https://github.com/Unity-Technologies/com.unity.netcode.gameobjects/issues and include the following data: Offset: {data.Offset}, Size: {data.Count}, Full receive array: {ByteArrayToString(data.Array, 0, data.Array.Length)}");
					return;
				}
				if (value.BatchSize != data.Count)
				{
					NetworkLog.LogError($"Received a packet with an invalid Batch Size Value. Please report this to the Netcode for GameObjects team at https://github.com/Unity-Technologies/com.unity.netcode.gameobjects/issues and include the following data: Offset: {data.Offset}, Size: {data.Count}, Expected Size: {value.BatchSize}, Full receive array: {ByteArrayToString(data.Array, 0, data.Array.Length)}");
					return;
				}
				ulong num = XXHash.Hash64(reader.GetUnsafePtrAtCurrentPosition(), reader.Length - reader.Position);
				if (num != value.BatchHash)
				{
					NetworkLog.LogError($"Received a packet with an invalid Hash Value. Please report this to the Netcode for GameObjects team at https://github.com/Unity-Technologies/com.unity.netcode.gameobjects/issues and include the following data: Received Hash: {value.BatchHash}, Calculated Hash: {num}, Offset: {data.Offset}, Size: {data.Count}, Full receive array: {ByteArrayToString(data.Array, 0, data.Array.Length)}");
					return;
				}
				for (int i = 0; i < m_Hooks.Count; i++)
				{
					m_Hooks[i].OnBeforeReceiveBatch(clientId, value.BatchCount, reader.Length);
				}
				for (int j = 0; j < value.BatchCount; j++)
				{
					NetworkMessageHeader header = default(NetworkMessageHeader);
					int position = reader.Position;
					try
					{
						ByteUnpacker.ReadValueBitPacked(reader, out header.MessageType);
						ByteUnpacker.ReadValueBitPacked(reader, out header.MessageSize);
					}
					catch (OverflowException)
					{
						NetworkLog.LogError("Received a batch that didn't have enough data for all of its batches, ending early!");
						throw;
					}
					int messageHeaderSerializedSize = reader.Position - position;
					if (!reader.TryBeginRead((int)header.MessageSize))
					{
						NetworkLog.LogError("Received a message that claimed a size larger than the packet, ending early!");
						return;
					}
					ref NativeList<ReceiveQueueItem> incomingMessageQueue = ref m_IncomingMessageQueue;
					ReceiveQueueItem value2 = new ReceiveQueueItem
					{
						Header = header,
						SenderId = clientId,
						Timestamp = receiveTime,
						Reader = new FastBufferReader(reader.GetUnsafePtrAtCurrentPosition(), Allocator.TempJob, (int)header.MessageSize),
						MessageHeaderSerializedSize = messageHeaderSerializedSize
					};
					incomingMessageQueue.Add(in value2);
					reader.Seek(reader.Position + (int)header.MessageSize);
				}
				for (int k = 0; k < m_Hooks.Count; k++)
				{
					m_Hooks[k].OnAfterReceiveBatch(clientId, value.BatchCount, reader.Length);
				}
			}
		}

		private bool CanReceive(ulong clientId, Type messageType, FastBufferReader messageContent, ref NetworkContext context)
		{
			for (int i = 0; i < m_Hooks.Count; i++)
			{
				if (!m_Hooks[i].OnVerifyCanReceive(clientId, messageType, messageContent, ref context))
				{
					return false;
				}
			}
			return true;
		}

		internal Type GetMessageForHash(uint messageHash)
		{
			if (!m_MessagesByHash.ContainsKey(messageHash))
			{
				return null;
			}
			return m_MessagesByHash[messageHash];
		}

		internal void SetVersion(ulong clientId, uint messageHash, int version)
		{
			if (m_MessagesByHash.ContainsKey(messageHash))
			{
				Type key = m_MessagesByHash[messageHash];
				if (!m_PerClientMessageVersions.ContainsKey(clientId))
				{
					m_PerClientMessageVersions[clientId] = new Dictionary<Type, int>();
				}
				m_PerClientMessageVersions[clientId][key] = version;
			}
		}

		internal void SetServerMessageOrder(NativeArray<uint> messagesInIdOrder)
		{
			MessageHandler[] messageHandlers = m_MessageHandlers;
			Dictionary<Type, uint> messageTypes = m_MessageTypes;
			m_ReverseTypeMap = new Type[messagesInIdOrder.Length];
			m_MessageHandlers = new MessageHandler[messagesInIdOrder.Length];
			m_MessageTypes = new Dictionary<Type, uint>();
			for (int i = 0; i < messagesInIdOrder.Length; i++)
			{
				if (m_MessagesByHash.ContainsKey(messagesInIdOrder[i]))
				{
					Type type = m_MessagesByHash[messagesInIdOrder[i]];
					uint num = messageTypes[type];
					MessageHandler messageHandler = messageHandlers[num];
					uint num2 = (uint)i;
					m_MessageTypes[type] = num2;
					m_MessageHandlers[num2] = messageHandler;
					m_ReverseTypeMap[num2] = type;
				}
			}
		}

		public void HandleMessage(in NetworkMessageHeader header, FastBufferReader reader, ulong senderId, float timestamp, int serializedHeaderSize)
		{
			using (reader)
			{
				if (header.MessageType >= m_HighMessageType)
				{
					Debug.LogWarning($"Received a message with invalid message type value {header.MessageType}");
					return;
				}
				NetworkContext networkContext = default(NetworkContext);
				networkContext.SystemOwner = m_Owner;
				networkContext.SenderId = senderId;
				networkContext.Timestamp = timestamp;
				networkContext.Header = header;
				networkContext.SerializedHeaderSize = serializedHeaderSize;
				networkContext.MessageSize = header.MessageSize;
				NetworkContext context = networkContext;
				Type messageType = m_ReverseTypeMap[header.MessageType];
				if (!CanReceive(senderId, messageType, reader, ref context))
				{
					return;
				}
				MessageHandler messageHandler = m_MessageHandlers[header.MessageType];
				for (int i = 0; i < m_Hooks.Count; i++)
				{
					m_Hooks[i].OnBeforeReceiveMessage(senderId, messageType, reader.Length + FastBufferWriter.GetWriteSize<NetworkMessageHeader>());
				}
				if (messageHandler == null)
				{
					Debug.LogException(new HandlerNotRegisteredException(header.MessageType.ToString()));
				}
				else
				{
					try
					{
						messageHandler(reader, ref context, this);
					}
					catch (Exception exception)
					{
						Debug.LogException(exception);
					}
				}
				for (int j = 0; j < m_Hooks.Count; j++)
				{
					m_Hooks[j].OnAfterReceiveMessage(senderId, messageType, reader.Length + FastBufferWriter.GetWriteSize<NetworkMessageHeader>());
				}
			}
		}

		internal void ProcessIncomingMessageQueue()
		{
			if (StopProcessing)
			{
				return;
			}
			for (int i = 0; i < m_IncomingMessageQueue.Length; i++)
			{
				ref ReceiveQueueItem reference = ref m_IncomingMessageQueue.ElementAt(i);
				HandleMessage(in reference.Header, reference.Reader, reference.SenderId, reference.Timestamp, reference.MessageHeaderSerializedSize);
				if (m_Disposed)
				{
					return;
				}
			}
			m_IncomingMessageQueue.Clear();
		}

		internal void ClientConnected(ulong clientId)
		{
			if (!m_SendQueues.ContainsKey(clientId))
			{
				m_SendQueues[clientId] = new NativeList<SendQueueItem>(16, Allocator.Persistent);
			}
		}

		internal void ClientDisconnected(ulong clientId)
		{
			m_DisconnectedClients.Add(clientId);
		}

		private void CleanupDisconnectedClient(ulong clientId)
		{
			if (m_SendQueues.ContainsKey(clientId))
			{
				NativeList<SendQueueItem> nativeList = m_SendQueues[clientId];
				for (int i = 0; i < nativeList.Length; i++)
				{
					nativeList.ElementAt(i).Writer.Dispose();
				}
				nativeList.Dispose();
				m_SendQueues.Remove(clientId);
				m_PerClientMessageVersions.Remove(clientId);
			}
		}

		internal void CleanupDisconnectedClients()
		{
			foreach (ulong disconnectedClient in m_DisconnectedClients)
			{
				CleanupDisconnectedClient(disconnectedClient);
			}
			m_DisconnectedClients.Clear();
		}

		public static int CreateMessageAndGetVersion<T>() where T : INetworkMessage, new()
		{
			return new T().Version;
		}

		internal int GetMessageVersion(Type type, ulong clientId, bool forReceive = false)
		{
			if (!m_PerClientMessageVersions.TryGetValue(clientId, out var value))
			{
				NetworkManager singleton = NetworkManager.Singleton;
				if (singleton != null && singleton.LogLevel == LogLevel.Developer)
				{
					if (forReceive)
					{
						NetworkLog.LogWarning($"Trying to receive {type.Name} from client {clientId} which is not in a connected state.");
					}
					else
					{
						NetworkLog.LogWarning($"Trying to send {type.Name} to client {clientId} which is not in a connected state.");
					}
				}
				return -1;
			}
			if (!value.TryGetValue(type, out var value2))
			{
				return -1;
			}
			return value2;
		}

		public static void ReceiveMessage<T>(FastBufferReader reader, ref NetworkContext context, NetworkMessageManager manager) where T : INetworkMessage, new()
		{
			T message = new T();
			int num = 0;
			if (typeof(T) != typeof(ConnectionRequestMessage) && typeof(T) != typeof(ConnectionApprovedMessage) && typeof(T) != typeof(DisconnectReasonMessage))
			{
				num = manager.GetMessageVersion(typeof(T), context.SenderId, forReceive: true);
				if (num < 0)
				{
					return;
				}
			}
			if (message.Deserialize(reader, ref context, num))
			{
				for (int i = 0; i < manager.m_Hooks.Count; i++)
				{
					manager.m_Hooks[i].OnBeforeHandleMessage(ref message, ref context);
				}
				message.Handle(ref context);
				for (int j = 0; j < manager.m_Hooks.Count; j++)
				{
					manager.m_Hooks[j].OnAfterHandleMessage(ref message, ref context);
				}
			}
		}

		private bool CanSend(ulong clientId, Type messageType, NetworkDelivery delivery)
		{
			for (int i = 0; i < m_Hooks.Count; i++)
			{
				if (!m_Hooks[i].OnVerifyCanSend(clientId, messageType, delivery))
				{
					return false;
				}
			}
			return true;
		}

		internal int SendMessage<TMessageType, TClientIdListType>(ref TMessageType message, NetworkDelivery delivery, in TClientIdListType clientIds) where TMessageType : INetworkMessage where TClientIdListType : IReadOnlyList<ulong>
		{
			if (clientIds.Count == 0)
			{
				return 0;
			}
			int num = 0;
			NativeHashSet<int> nativeHashSet = new NativeHashSet<int>(clientIds.Count, Allocator.Temp);
			for (int i = 0; i < clientIds.Count; i++)
			{
				int num2 = 0;
				if (typeof(TMessageType) != typeof(ConnectionRequestMessage))
				{
					num2 = GetMessageVersion(typeof(TMessageType), clientIds[i]);
					if (num2 < 0)
					{
						continue;
					}
				}
				if (!nativeHashSet.Contains(num2))
				{
					nativeHashSet.Add(num2);
					int num3 = ((delivery == NetworkDelivery.ReliableFragmentedSequenced) ? FragmentedMessageMaxSize : NonFragmentedMessageMaxSize);
					FastBufferWriter tmpSerializer = new FastBufferWriter(NonFragmentedMessageMaxSize - FastBufferWriter.GetWriteSize<NetworkMessageHeader>(), Allocator.Temp, num3 - FastBufferWriter.GetWriteSize<NetworkMessageHeader>());
					try
					{
						message.Serialize(tmpSerializer, num2);
						IReadOnlyList<ulong> clientIds2 = clientIds;
						int num4 = SendPreSerializedMessage(in tmpSerializer, num3, ref message, delivery, in clientIds2, num2);
						num = ((num4 > num) ? num4 : num);
					}
					finally
					{
						((IDisposable)tmpSerializer).Dispose();
					}
				}
			}
			nativeHashSet.Dispose();
			return num;
		}

		internal unsafe int SendPreSerializedMessage<TMessageType>(in FastBufferWriter tmpSerializer, int maxSize, ref TMessageType message, NetworkDelivery delivery, in IReadOnlyList<ulong> clientIds, int messageVersionFilter) where TMessageType : INetworkMessage
		{
			using FastBufferWriter writer = new FastBufferWriter(FastBufferWriter.GetWriteSize<NetworkMessageHeader>(), Allocator.Temp);
			NetworkMessageHeader networkMessageHeader = default(NetworkMessageHeader);
			networkMessageHeader.MessageSize = (uint)tmpSerializer.Length;
			networkMessageHeader.MessageType = m_MessageTypes[typeof(TMessageType)];
			NetworkMessageHeader networkMessageHeader2 = networkMessageHeader;
			BytePacker.WriteValueBitPacked(writer, networkMessageHeader2.MessageType);
			BytePacker.WriteValueBitPacked(writer, networkMessageHeader2.MessageSize);
			for (int i = 0; i < clientIds.Count; i++)
			{
				if (m_DisconnectedClients.Contains(clientIds[i]))
				{
					continue;
				}
				if (typeof(TMessageType) != typeof(ConnectionRequestMessage))
				{
					int messageVersion = GetMessageVersion(typeof(TMessageType), clientIds[i]);
					if (messageVersion < 0 || messageVersion != messageVersionFilter)
					{
						continue;
					}
				}
				ulong num = clientIds[i];
				if (!CanSend(num, typeof(TMessageType), delivery))
				{
					continue;
				}
				for (int j = 0; j < m_Hooks.Count; j++)
				{
					m_Hooks[j].OnBeforeSendMessage(num, ref message, delivery);
				}
				NativeList<SendQueueItem> nativeList = m_SendQueues[num];
				if (nativeList.Length == 0)
				{
					SendQueueItem value = new SendQueueItem(delivery, NonFragmentedMessageMaxSize, Allocator.TempJob, maxSize);
					nativeList.Add(in value);
					nativeList.ElementAt(0).Writer.Seek(sizeof(NetworkBatchHeader));
				}
				else
				{
					ref SendQueueItem reference = ref nativeList.ElementAt(nativeList.Length - 1);
					if (reference.NetworkDelivery != delivery || reference.Writer.MaxCapacity - reference.Writer.Position < tmpSerializer.Length + writer.Length)
					{
						SendQueueItem value = new SendQueueItem(delivery, NonFragmentedMessageMaxSize, Allocator.TempJob, maxSize);
						nativeList.Add(in value);
						nativeList.ElementAt(nativeList.Length - 1).Writer.Seek(sizeof(NetworkBatchHeader));
					}
				}
				ref SendQueueItem reference2 = ref nativeList.ElementAt(nativeList.Length - 1);
				reference2.Writer.TryBeginWrite(tmpSerializer.Length + writer.Length);
				reference2.Writer.WriteBytes(writer.GetUnsafePtr(), writer.Length);
				reference2.Writer.WriteBytes(tmpSerializer.GetUnsafePtr(), tmpSerializer.Length);
				reference2.BatchHeader.BatchCount++;
				for (int k = 0; k < m_Hooks.Count; k++)
				{
					m_Hooks[k].OnAfterSendMessage(num, ref message, delivery, tmpSerializer.Length + writer.Length);
				}
			}
			return tmpSerializer.Length + writer.Length;
		}

		internal unsafe int SendPreSerializedMessage<TMessageType>(in FastBufferWriter tmpSerializer, int maxSize, ref TMessageType message, NetworkDelivery delivery, ulong clientId) where TMessageType : INetworkMessage
		{
			int num = 0;
			if (typeof(TMessageType) != typeof(ConnectionRequestMessage))
			{
				num = GetMessageVersion(typeof(TMessageType), clientId);
				if (num < 0)
				{
					return 0;
				}
			}
			ulong* ptr = stackalloc ulong[1] { clientId };
			IReadOnlyList<ulong> clientIds = new PointerListWrapper<ulong>(ptr, 1);
			return SendPreSerializedMessage(in tmpSerializer, maxSize, ref message, delivery, in clientIds, num);
		}

		internal unsafe int SendMessage<T>(ref T message, NetworkDelivery delivery, ulong* clientIds, int numClientIds) where T : INetworkMessage
		{
			PointerListWrapper<ulong> clientIds2 = new PointerListWrapper<ulong>(clientIds, numClientIds);
			return SendMessage(ref message, delivery, in clientIds2);
		}

		internal unsafe int SendMessage<T>(ref T message, NetworkDelivery delivery, ulong clientId) where T : INetworkMessage
		{
			ulong* ptr = stackalloc ulong[1] { clientId };
			PointerListWrapper<ulong> clientIds = new PointerListWrapper<ulong>(ptr, 1);
			return SendMessage(ref message, delivery, in clientIds);
		}

		internal unsafe int SendMessage<T>(ref T message, NetworkDelivery delivery, in NativeArray<ulong> clientIds) where T : INetworkMessage
		{
			PointerListWrapper<ulong> clientIds2 = new PointerListWrapper<ulong>((ulong*)clientIds.GetUnsafePtr(), clientIds.Length);
			return SendMessage(ref message, delivery, in clientIds2);
		}

		internal unsafe void ProcessSendQueues()
		{
			if (StopProcessing)
			{
				return;
			}
			foreach (KeyValuePair<ulong, NativeList<SendQueueItem>> sendQueue in m_SendQueues)
			{
				ulong key = sendQueue.Key;
				NativeList<SendQueueItem> value = sendQueue.Value;
				for (int i = 0; i < value.Length; i++)
				{
					ref SendQueueItem reference = ref value.ElementAt(i);
					if (m_DisconnectedClients.Contains(key))
					{
						reference.Writer.Dispose();
						continue;
					}
					if (reference.BatchHeader.BatchCount == 0)
					{
						reference.Writer.Dispose();
						continue;
					}
					for (int j = 0; j < m_Hooks.Count; j++)
					{
						m_Hooks[j].OnBeforeSendBatch(key, reference.BatchHeader.BatchCount, reference.Writer.Length, reference.NetworkDelivery);
					}
					reference.Writer.Seek(0);
					int num = (reference.Writer.Length + 7) & -8;
					reference.Writer.TryBeginWrite(num);
					reference.BatchHeader.BatchHash = XXHash.Hash64(reference.Writer.GetUnsafePtr() + sizeof(NetworkBatchHeader), num - sizeof(NetworkBatchHeader));
					reference.BatchHeader.BatchSize = num;
					reference.Writer.WriteValue(in reference.BatchHeader, default(FastBufferWriter.ForStructs));
					reference.Writer.Seek(num);
					try
					{
						m_Sender.Send(key, reference.NetworkDelivery, reference.Writer);
						for (int k = 0; k < m_Hooks.Count; k++)
						{
							m_Hooks[k].OnAfterSendBatch(key, reference.BatchHeader.BatchCount, reference.Writer.Length, reference.NetworkDelivery);
						}
					}
					finally
					{
						reference.Writer.Dispose();
					}
				}
				value.Clear();
			}
		}
	}
}
