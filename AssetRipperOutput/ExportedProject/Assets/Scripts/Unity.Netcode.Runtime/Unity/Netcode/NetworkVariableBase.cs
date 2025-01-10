using System;
using UnityEngine;

namespace Unity.Netcode
{
	public abstract class NetworkVariableBase : IDisposable
	{
		internal const NetworkDelivery Delivery = NetworkDelivery.ReliableFragmentedSequenced;

		private protected NetworkBehaviour m_NetworkBehaviour;

		public const NetworkVariableReadPermission DefaultReadPerm = NetworkVariableReadPermission.Everyone;

		public const NetworkVariableWritePermission DefaultWritePerm = NetworkVariableWritePermission.Server;

		private bool m_IsDirty;

		public readonly NetworkVariableReadPermission ReadPerm;

		public readonly NetworkVariableWritePermission WritePerm;

		public string Name { get; internal set; }

		public NetworkBehaviour GetBehaviour()
		{
			return m_NetworkBehaviour;
		}

		public void Initialize(NetworkBehaviour networkBehaviour)
		{
			m_NetworkBehaviour = networkBehaviour;
		}

		protected NetworkVariableBase(NetworkVariableReadPermission readPerm = NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission writePerm = NetworkVariableWritePermission.Server)
		{
			ReadPerm = readPerm;
			WritePerm = writePerm;
		}

		public virtual void SetDirty(bool isDirty)
		{
			m_IsDirty = isDirty;
			if (m_IsDirty)
			{
				if (m_NetworkBehaviour == null)
				{
					Debug.LogWarning("NetworkVariable is written to, but doesn't know its NetworkBehaviour yet. Are you modifying a NetworkVariable before the NetworkObject is spawned?");
				}
				else
				{
					m_NetworkBehaviour.NetworkManager.BehaviourUpdater.AddForUpdate(m_NetworkBehaviour.NetworkObject);
				}
			}
		}

		public virtual void ResetDirty()
		{
			m_IsDirty = false;
		}

		public virtual bool IsDirty()
		{
			return m_IsDirty;
		}

		public bool CanClientRead(ulong clientId)
		{
			NetworkVariableReadPermission readPerm = ReadPerm;
			if (readPerm == NetworkVariableReadPermission.Everyone || readPerm != NetworkVariableReadPermission.Owner)
			{
				return true;
			}
			if (clientId != m_NetworkBehaviour.NetworkObject.OwnerClientId)
			{
				return clientId == 0;
			}
			return true;
		}

		public bool CanClientWrite(ulong clientId)
		{
			NetworkVariableWritePermission writePerm = WritePerm;
			if (writePerm == NetworkVariableWritePermission.Server || writePerm != NetworkVariableWritePermission.Owner)
			{
				return clientId == 0;
			}
			return clientId == m_NetworkBehaviour.NetworkObject.OwnerClientId;
		}

		internal ulong OwnerClientId()
		{
			return m_NetworkBehaviour.NetworkObject.OwnerClientId;
		}

		public abstract void WriteDelta(FastBufferWriter writer);

		public abstract void WriteField(FastBufferWriter writer);

		public abstract void ReadField(FastBufferReader reader);

		public abstract void ReadDelta(FastBufferReader reader, bool keepDirtyDelta);

		public virtual void Dispose()
		{
		}
	}
}
