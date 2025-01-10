using System;
using System.Runtime.CompilerServices;

namespace Unity.Netcode
{
	public struct NetworkBehaviourReference : INetworkSerializable, IEquatable<NetworkBehaviourReference>
	{
		private NetworkObjectReference m_NetworkObjectReference;

		private ushort m_NetworkBehaviourId;

		public NetworkBehaviourReference(NetworkBehaviour networkBehaviour)
		{
			if (networkBehaviour == null)
			{
				throw new ArgumentNullException("networkBehaviour");
			}
			if (networkBehaviour.NetworkObject == null)
			{
				throw new ArgumentException("Cannot create NetworkBehaviourReference from NetworkBehaviour without a NetworkObject.");
			}
			m_NetworkObjectReference = networkBehaviour.NetworkObject;
			m_NetworkBehaviourId = networkBehaviour.NetworkBehaviourId;
		}

		public bool TryGet(out NetworkBehaviour networkBehaviour, NetworkManager networkManager = null)
		{
			networkBehaviour = GetInternal(this);
			return networkBehaviour != null;
		}

		public bool TryGet<T>(out T networkBehaviour, NetworkManager networkManager = null) where T : NetworkBehaviour
		{
			networkBehaviour = GetInternal(this) as T;
			return networkBehaviour != null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static NetworkBehaviour GetInternal(NetworkBehaviourReference networkBehaviourRef, NetworkManager networkManager = null)
		{
			if (networkBehaviourRef.m_NetworkObjectReference.TryGet(out var networkObject, networkManager))
			{
				return networkObject.GetNetworkBehaviourAtOrderIndex(networkBehaviourRef.m_NetworkBehaviourId);
			}
			return null;
		}

		public bool Equals(NetworkBehaviourReference other)
		{
			if (m_NetworkObjectReference.Equals(other.m_NetworkObjectReference))
			{
				return m_NetworkBehaviourId == other.m_NetworkBehaviourId;
			}
			return false;
		}

		public override bool Equals(object obj)
		{
			if (obj is NetworkBehaviourReference other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return (m_NetworkObjectReference.GetHashCode() * 397) ^ m_NetworkBehaviourId.GetHashCode();
		}

		public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
		{
			m_NetworkObjectReference.NetworkSerialize(serializer);
			serializer.SerializeValue(ref m_NetworkBehaviourId, default(FastBufferWriter.ForPrimitives));
		}

		public static implicit operator NetworkBehaviour(NetworkBehaviourReference networkBehaviourRef)
		{
			return GetInternal(networkBehaviourRef);
		}

		public static implicit operator NetworkBehaviourReference(NetworkBehaviour networkBehaviour)
		{
			return new NetworkBehaviourReference(networkBehaviour);
		}
	}
}
