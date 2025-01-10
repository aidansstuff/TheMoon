using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace Unity.Netcode
{
	public abstract class NetworkBehaviour : MonoBehaviour
	{
		protected enum __RpcExecStage
		{
			None = 0,
			Server = 1,
			Client = 2
		}

		[NonSerialized]
		protected internal __RpcExecStage __rpc_exec_stage;

		private const int k_RpcMessageDefaultSize = 1024;

		private const int k_RpcMessageMaximumSize = 65536;

		private NetworkObject m_NetworkObject;

		internal ushort NetworkBehaviourIdCache;

		private bool m_VarInit;

		private readonly List<HashSet<int>> m_DeliveryMappedNetworkVariableIndices = new List<HashSet<int>>();

		private readonly List<NetworkDelivery> m_DeliveryTypesForNetworkVariableGroups = new List<NetworkDelivery>();

		protected internal readonly List<NetworkVariableBase> NetworkVariableFields = new List<NetworkVariableBase>();

		internal readonly List<int> NetworkVariableIndexesToReset = new List<int>();

		internal readonly HashSet<int> NetworkVariableIndexesToResetSet = new HashSet<int>();

		public NetworkManager NetworkManager
		{
			get
			{
				if (NetworkObject?.NetworkManager != null)
				{
					return NetworkObject?.NetworkManager;
				}
				return NetworkManager.Singleton;
			}
		}

		public bool IsLocalPlayer { get; private set; }

		public bool IsOwner { get; internal set; }

		public bool IsServer { get; private set; }

		public bool IsClient { get; private set; }

		public bool IsHost { get; private set; }

		public bool IsOwnedByServer { get; internal set; }

		public bool IsSpawned { get; internal set; }

		public NetworkObject NetworkObject
		{
			get
			{
				try
				{
					if (m_NetworkObject == null)
					{
						m_NetworkObject = GetComponentInParent<NetworkObject>();
					}
				}
				catch (Exception)
				{
					return null;
				}
				if (IsSpawned && m_NetworkObject == null && (NetworkManager.Singleton == null || !NetworkManager.Singleton.ShutdownInProgress) && NetworkLog.CurrentLogLevel <= LogLevel.Normal)
				{
					NetworkLog.LogWarning("Could not get NetworkObject for the NetworkBehaviour. Are you missing a NetworkObject component?");
				}
				return m_NetworkObject;
			}
		}

		public bool HasNetworkObject => NetworkObject != null;

		public ulong NetworkObjectId { get; internal set; }

		public ushort NetworkBehaviourId { get; internal set; }

		public ulong OwnerClientId { get; internal set; }

		protected ulong m_TargetIdBeingSynchronized { get; private set; }

		protected internal virtual string __getTypeName()
		{
			return "NetworkBehaviour";
		}

		protected FastBufferWriter __beginSendServerRpc(uint rpcMethodId, ServerRpcParams serverRpcParams, RpcDelivery rpcDelivery)
		{
			return new FastBufferWriter(1024, Allocator.Temp, 65536);
		}

		protected void __endSendServerRpc(ref FastBufferWriter bufferWriter, uint rpcMethodId, ServerRpcParams serverRpcParams, RpcDelivery rpcDelivery)
		{
			ServerRpcMessage serverRpcMessage = default(ServerRpcMessage);
			serverRpcMessage.Metadata = new RpcMetadata
			{
				NetworkObjectId = NetworkObjectId,
				NetworkBehaviourId = NetworkBehaviourId,
				NetworkRpcMethodId = rpcMethodId
			};
			serverRpcMessage.WriteBuffer = bufferWriter;
			ServerRpcMessage message = serverRpcMessage;
			NetworkDelivery delivery;
			if (rpcDelivery == RpcDelivery.Reliable || rpcDelivery != RpcDelivery.Unreliable)
			{
				delivery = NetworkDelivery.ReliableFragmentedSequenced;
			}
			else
			{
				if (bufferWriter.Length > NetworkManager.MessageManager.NonFragmentedMessageMaxSize)
				{
					throw new OverflowException("RPC parameters are too large for unreliable delivery.");
				}
				delivery = NetworkDelivery.Unreliable;
			}
			if (IsHost || IsServer)
			{
				using FastBufferReader readBuffer = new FastBufferReader(bufferWriter, Allocator.Temp);
				NetworkContext networkContext = default(NetworkContext);
				networkContext.SenderId = 0uL;
				networkContext.Timestamp = NetworkManager.RealTimeProvider.RealTimeSinceStartup;
				networkContext.SystemOwner = NetworkManager;
				networkContext.Header = default(NetworkMessageHeader);
				networkContext.SerializedHeaderSize = 0;
				networkContext.MessageSize = 0u;
				NetworkContext context = networkContext;
				message.ReadBuffer = readBuffer;
				message.Handle(ref context);
				_ = readBuffer.Length;
			}
			else
			{
				NetworkManager.ConnectionManager.SendMessage(ref message, delivery, 0uL);
			}
			bufferWriter.Dispose();
		}

		protected FastBufferWriter __beginSendClientRpc(uint rpcMethodId, ClientRpcParams clientRpcParams, RpcDelivery rpcDelivery)
		{
			return new FastBufferWriter(1024, Allocator.Temp, 65536);
		}

		protected void __endSendClientRpc(ref FastBufferWriter bufferWriter, uint rpcMethodId, ClientRpcParams clientRpcParams, RpcDelivery rpcDelivery)
		{
			ClientRpcMessage clientRpcMessage = default(ClientRpcMessage);
			clientRpcMessage.Metadata = new RpcMetadata
			{
				NetworkObjectId = NetworkObjectId,
				NetworkBehaviourId = NetworkBehaviourId,
				NetworkRpcMethodId = rpcMethodId
			};
			clientRpcMessage.WriteBuffer = bufferWriter;
			ClientRpcMessage message = clientRpcMessage;
			NetworkDelivery networkDelivery;
			if (rpcDelivery == RpcDelivery.Reliable || rpcDelivery != RpcDelivery.Unreliable)
			{
				networkDelivery = NetworkDelivery.ReliableFragmentedSequenced;
			}
			else
			{
				if (bufferWriter.Length > NetworkManager.MessageManager.NonFragmentedMessageMaxSize)
				{
					throw new OverflowException("RPC parameters are too large for unreliable delivery.");
				}
				networkDelivery = NetworkDelivery.Unreliable;
			}
			bool flag = false;
			if (clientRpcParams.Send.TargetClientIds != null)
			{
				foreach (ulong targetClientId in clientRpcParams.Send.TargetClientIds)
				{
					if (targetClientId == 0L)
					{
						flag = true;
						break;
					}
					if (NetworkManager.LogLevel >= LogLevel.Error && !NetworkObject.Observers.Contains(targetClientId))
					{
						NetworkLog.LogError(GenerateObserverErrorMessage(clientRpcParams, targetClientId));
					}
				}
				NetworkManager.ConnectionManager.SendMessage(ref message, networkDelivery, in clientRpcParams.Send.TargetClientIds);
			}
			else if (clientRpcParams.Send.TargetClientIdsNativeArray.HasValue)
			{
				foreach (ulong item in clientRpcParams.Send.TargetClientIdsNativeArray.Value)
				{
					if (item == 0L)
					{
						flag = true;
						break;
					}
					if (NetworkManager.LogLevel >= LogLevel.Error && !NetworkObject.Observers.Contains(item))
					{
						NetworkLog.LogError(GenerateObserverErrorMessage(clientRpcParams, item));
					}
				}
				NetworkConnectionManager connectionManager = NetworkManager.ConnectionManager;
				NetworkDelivery delivery = networkDelivery;
				NativeArray<ulong> clientIds = clientRpcParams.Send.TargetClientIdsNativeArray.Value;
				connectionManager.SendMessage(ref message, delivery, in clientIds);
			}
			else
			{
				HashSet<ulong>.Enumerator enumerator3 = NetworkObject.Observers.GetEnumerator();
				while (enumerator3.MoveNext())
				{
					if (IsHost && enumerator3.Current == NetworkManager.LocalClientId)
					{
						flag = true;
					}
					else
					{
						NetworkManager.ConnectionManager.SendMessage(ref message, networkDelivery, enumerator3.Current);
					}
				}
			}
			if (flag)
			{
				using FastBufferReader readBuffer = new FastBufferReader(bufferWriter, Allocator.Temp);
				NetworkContext networkContext = default(NetworkContext);
				networkContext.SenderId = 0uL;
				networkContext.Timestamp = NetworkManager.RealTimeProvider.RealTimeSinceStartup;
				networkContext.SystemOwner = NetworkManager;
				networkContext.Header = default(NetworkMessageHeader);
				networkContext.SerializedHeaderSize = 0;
				networkContext.MessageSize = 0u;
				NetworkContext context = networkContext;
				message.ReadBuffer = readBuffer;
				message.Handle(ref context);
			}
			bufferWriter.Dispose();
		}

		protected static NativeList<T> __createNativeList<T>() where T : unmanaged
		{
			return new NativeList<T>(Allocator.Temp);
		}

		internal string GenerateObserverErrorMessage(ClientRpcParams clientRpcParams, ulong targetClientId)
		{
			string arg = ((clientRpcParams.Send.TargetClientIds != null) ? "TargetClientIds" : "TargetClientIdsNativeArray");
			return $"Sending ClientRpc to non-observer! {arg} contains clientId {targetClientId} that is not an observer!";
		}

		internal bool IsBehaviourEditable()
		{
			if ((bool)m_NetworkObject && !(m_NetworkObject.NetworkManager == null) && m_NetworkObject.NetworkManager.IsListening)
			{
				return m_NetworkObject.NetworkManager.IsServer;
			}
			return true;
		}

		protected NetworkBehaviour GetNetworkBehaviour(ushort behaviourId)
		{
			return NetworkObject.GetNetworkBehaviourAtOrderIndex(behaviourId);
		}

		internal void UpdateNetworkProperties()
		{
			if (NetworkObject != null)
			{
				NetworkObjectId = NetworkObject.NetworkObjectId;
				IsLocalPlayer = NetworkObject.IsLocalPlayer;
				NetworkBehaviourId = NetworkObject.GetNetworkBehaviourOrderIndex(this);
				IsOwnedByServer = NetworkObject.IsOwnedByServer;
				IsOwner = NetworkObject.IsOwner;
				OwnerClientId = NetworkObject.OwnerClientId;
				if (NetworkManager != null)
				{
					IsHost = NetworkManager.IsListening && NetworkManager.IsHost;
					IsClient = NetworkManager.IsListening && NetworkManager.IsClient;
					IsServer = NetworkManager.IsListening && NetworkManager.IsServer;
				}
			}
			else
			{
				ulong ownerClientId = (NetworkObjectId = 0uL);
				OwnerClientId = ownerClientId;
				bool flag2 = (IsServer = false);
				bool flag4 = (IsClient = flag2);
				bool flag6 = (IsHost = flag4);
				bool isOwnedByServer = (IsOwner = flag6);
				IsOwnedByServer = isOwnedByServer;
				NetworkBehaviourId = 0;
			}
		}

		public virtual void OnNetworkSpawn()
		{
		}

		public virtual void OnNetworkDespawn()
		{
		}

		internal void InternalOnNetworkSpawn()
		{
			IsSpawned = true;
			InitializeVariables();
			UpdateNetworkProperties();
		}

		internal void VisibleOnNetworkSpawn()
		{
			try
			{
				OnNetworkSpawn();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
			InitializeVariables();
			if (IsServer)
			{
				PostNetworkVariableWrite(forced: true);
			}
		}

		internal void InternalOnNetworkDespawn()
		{
			IsSpawned = false;
			UpdateNetworkProperties();
			try
			{
				OnNetworkDespawn();
			}
			catch (Exception exception)
			{
				Debug.LogException(exception);
			}
		}

		public virtual void OnGainedOwnership()
		{
		}

		internal void InternalOnGainedOwnership()
		{
			UpdateNetworkProperties();
			OnGainedOwnership();
		}

		public virtual void OnLostOwnership()
		{
		}

		internal void InternalOnLostOwnership()
		{
			UpdateNetworkProperties();
			OnLostOwnership();
		}

		public virtual void OnNetworkObjectParentChanged(NetworkObject parentNetworkObject)
		{
		}

		protected virtual void __initializeVariables()
		{
		}

		protected void __nameNetworkVariable(NetworkVariableBase variable, string varName)
		{
			variable.Name = varName;
		}

		internal void InitializeVariables()
		{
			if (m_VarInit)
			{
				return;
			}
			m_VarInit = true;
			__initializeVariables();
			Dictionary<NetworkDelivery, int> dictionary = new Dictionary<NetworkDelivery, int>();
			int num = 0;
			for (int i = 0; i < NetworkVariableFields.Count; i++)
			{
				NetworkDelivery networkDelivery = NetworkDelivery.ReliableFragmentedSequenced;
				if (!dictionary.ContainsKey(networkDelivery))
				{
					dictionary.Add(networkDelivery, num);
					m_DeliveryTypesForNetworkVariableGroups.Add(networkDelivery);
					num++;
				}
				if (dictionary[networkDelivery] >= m_DeliveryMappedNetworkVariableIndices.Count)
				{
					m_DeliveryMappedNetworkVariableIndices.Add(new HashSet<int>());
				}
				m_DeliveryMappedNetworkVariableIndices[dictionary[networkDelivery]].Add(i);
			}
		}

		internal void PreNetworkVariableWrite()
		{
			NetworkVariableIndexesToReset.Clear();
			NetworkVariableIndexesToResetSet.Clear();
		}

		internal void PostNetworkVariableWrite(bool forced = false)
		{
			if (forced)
			{
				for (int i = 0; i < NetworkVariableFields.Count; i++)
				{
					NetworkVariableFields[i].ResetDirty();
				}
			}
			else
			{
				for (int j = 0; j < NetworkVariableIndexesToReset.Count; j++)
				{
					NetworkVariableFields[NetworkVariableIndexesToReset[j]].ResetDirty();
				}
			}
			MarkVariablesDirty(dirty: false);
		}

		internal void PreVariableUpdate()
		{
			if (!m_VarInit)
			{
				InitializeVariables();
			}
			PreNetworkVariableWrite();
		}

		internal void VariableUpdate(ulong targetClientId)
		{
			NetworkVariableUpdate(targetClientId, NetworkBehaviourId);
		}

		private void NetworkVariableUpdate(ulong targetClientId, int behaviourIndex)
		{
			if (!CouldHaveDirtyNetworkVariables())
			{
				return;
			}
			for (int i = 0; i < m_DeliveryMappedNetworkVariableIndices.Count; i++)
			{
				bool flag = false;
				for (int j = 0; j < NetworkVariableFields.Count; j++)
				{
					NetworkVariableBase networkVariableBase = NetworkVariableFields[j];
					if (networkVariableBase.IsDirty() && networkVariableBase.CanClientRead(targetClientId))
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					continue;
				}
				NetworkVariableDeltaMessage networkVariableDeltaMessage = default(NetworkVariableDeltaMessage);
				networkVariableDeltaMessage.NetworkObjectId = NetworkObjectId;
				networkVariableDeltaMessage.NetworkBehaviourIndex = NetworkObject.GetNetworkBehaviourOrderIndex(this);
				networkVariableDeltaMessage.NetworkBehaviour = this;
				networkVariableDeltaMessage.TargetClientId = targetClientId;
				networkVariableDeltaMessage.DeliveryMappedNetworkVariableIndex = m_DeliveryMappedNetworkVariableIndices[i];
				NetworkVariableDeltaMessage message = networkVariableDeltaMessage;
				if (IsServer && targetClientId == 0L)
				{
					FastBufferWriter fastBufferWriter = new FastBufferWriter(NetworkManager.MessageManager.NonFragmentedMessageMaxSize, Allocator.Temp, NetworkManager.MessageManager.FragmentedMessageMaxSize);
					using (fastBufferWriter)
					{
						message.Serialize(fastBufferWriter, message.Version);
					}
				}
				else
				{
					NetworkManager.ConnectionManager.SendMessage(ref message, m_DeliveryTypesForNetworkVariableGroups[i], targetClientId);
				}
			}
		}

		private bool CouldHaveDirtyNetworkVariables()
		{
			for (int i = 0; i < NetworkVariableFields.Count; i++)
			{
				if (NetworkVariableFields[i].IsDirty())
				{
					return true;
				}
			}
			return false;
		}

		internal void MarkVariablesDirty(bool dirty)
		{
			for (int i = 0; i < NetworkVariableFields.Count; i++)
			{
				NetworkVariableFields[i].SetDirty(dirty);
			}
		}

		internal void WriteNetworkVariableData(FastBufferWriter writer, ulong targetClientId)
		{
			if (NetworkVariableFields.Count == 0)
			{
				return;
			}
			for (int i = 0; i < NetworkVariableFields.Count; i++)
			{
				if (NetworkVariableFields[i].CanClientRead(targetClientId))
				{
					if (NetworkManager.NetworkConfig.EnsureNetworkVariableLengthSafety)
					{
						int position = writer.Position;
						ushort value = 0;
						writer.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
						int position2 = writer.Position;
						NetworkVariableFields[i].WriteField(writer);
						int num = writer.Position - position2;
						writer.Seek(position);
						value = (ushort)num;
						writer.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
						writer.Seek(position2 + num);
					}
					else
					{
						NetworkVariableFields[i].WriteField(writer);
					}
				}
				else if (NetworkManager.NetworkConfig.EnsureNetworkVariableLengthSafety)
				{
					ushort value = 0;
					writer.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
				}
			}
		}

		internal void SetNetworkVariableData(FastBufferReader reader, ulong clientId)
		{
			if (NetworkVariableFields.Count == 0)
			{
				return;
			}
			for (int i = 0; i < NetworkVariableFields.Count; i++)
			{
				ushort value = 0;
				int num = 0;
				if (NetworkManager.NetworkConfig.EnsureNetworkVariableLengthSafety)
				{
					reader.ReadValueSafe(out value, default(FastBufferWriter.ForPrimitives));
					if (value == 0)
					{
						continue;
					}
					num = reader.Position;
				}
				else if (!NetworkVariableFields[i].CanClientRead(clientId))
				{
					continue;
				}
				NetworkVariableFields[i].ReadField(reader);
				if (!NetworkManager.NetworkConfig.EnsureNetworkVariableLengthSafety)
				{
					continue;
				}
				if (reader.Position > num + value)
				{
					if (NetworkLog.CurrentLogLevel <= LogLevel.Normal)
					{
						NetworkLog.LogWarning($"Var data read too far. {reader.Position - (num + value)} bytes.");
					}
					reader.Seek(num + value);
				}
				else if (reader.Position < num + value)
				{
					if (NetworkLog.CurrentLogLevel <= LogLevel.Normal)
					{
						NetworkLog.LogWarning($"Var data read too little. {num + value - reader.Position} bytes.");
					}
					reader.Seek(num + value);
				}
			}
		}

		protected NetworkObject GetNetworkObject(ulong networkId)
		{
			if (!NetworkManager.SpawnManager.SpawnedObjects.TryGetValue(networkId, out var value))
			{
				return null;
			}
			return value;
		}

		protected virtual void OnSynchronize<T>(ref BufferSerializer<T> serializer) where T : IReaderWriter
		{
		}

		internal bool Synchronize<T>(ref BufferSerializer<T> serializer, ulong targetClientId = 0uL) where T : IReaderWriter
		{
			m_TargetIdBeingSynchronized = targetClientId;
			if (serializer.IsWriter)
			{
				FastBufferWriter fastBufferWriter = serializer.GetFastBufferWriter();
				int position = fastBufferWriter.Position;
				ushort value = NetworkBehaviourId;
				fastBufferWriter.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
				int position2 = fastBufferWriter.Position;
				value = 0;
				fastBufferWriter.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
				int position3 = fastBufferWriter.Position;
				bool flag = false;
				try
				{
					OnSynchronize(ref serializer);
				}
				catch (Exception ex)
				{
					flag = true;
					if (NetworkManager.LogLevel <= LogLevel.Normal)
					{
						NetworkLog.LogWarning(base.name + " threw an exception during synchronization serialization, this NetworkBehaviour is being skipped and will not be synchronized!");
						if (NetworkManager.LogLevel == LogLevel.Developer)
						{
							NetworkLog.LogError(ex.Message + "\n " + ex.StackTrace);
						}
					}
				}
				int position4 = fastBufferWriter.Position;
				m_TargetIdBeingSynchronized = 0uL;
				if (position4 == position3 || flag)
				{
					fastBufferWriter.Seek(position);
					return false;
				}
				int num = position4 - position3;
				fastBufferWriter.Seek(position2);
				value = (ushort)num;
				fastBufferWriter.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
				fastBufferWriter.Seek(position4);
				return true;
			}
			FastBufferReader fastBufferReader = serializer.GetFastBufferReader();
			fastBufferReader.ReadValueSafe(out ushort value2, default(FastBufferWriter.ForPrimitives));
			int position5 = fastBufferReader.Position;
			bool flag2 = false;
			try
			{
				OnSynchronize(ref serializer);
			}
			catch (Exception ex2)
			{
				if (NetworkManager.LogLevel <= LogLevel.Normal)
				{
					NetworkLog.LogWarning(base.name + " threw an exception during synchronization deserialization, this NetworkBehaviour is being skipped and will not be synchronized!");
					if (NetworkManager.LogLevel == LogLevel.Developer)
					{
						NetworkLog.LogError(ex2.Message + "\n " + ex2.StackTrace);
					}
				}
				flag2 = true;
			}
			int num2 = fastBufferReader.Position - position5;
			if (num2 != value2)
			{
				if (NetworkManager.LogLevel <= LogLevel.Normal)
				{
					NetworkLog.LogWarning(string.Format("{0} read {1} bytes but was expected to read {2} bytes during synchronization deserialization! This {3} is being skipped and will not be synchronized!", base.name, num2, value2, "NetworkBehaviour"));
				}
				flag2 = true;
			}
			m_TargetIdBeingSynchronized = 0uL;
			if (flag2)
			{
				int where = position5 + value2;
				fastBufferReader.Seek(where);
				return false;
			}
			return true;
		}

		public virtual void OnDestroy()
		{
			if (NetworkObject != null && NetworkObject.IsSpawned && IsSpawned)
			{
				NetworkObject.OnNetworkBehaviourDestroyed(this);
			}
			if (!m_VarInit)
			{
				InitializeVariables();
			}
			for (int i = 0; i < NetworkVariableFields.Count; i++)
			{
				NetworkVariableFields[i].Dispose();
			}
		}
	}
}
