using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Unity.Netcode
{
	public struct NetworkObjectReference : INetworkSerializable, IEquatable<NetworkObjectReference>
	{
		private ulong m_NetworkObjectId;

		public ulong NetworkObjectId
		{
			get
			{
				return m_NetworkObjectId;
			}
			internal set
			{
				m_NetworkObjectId = value;
			}
		}

		public NetworkObjectReference(NetworkObject networkObject)
		{
			if (networkObject == null)
			{
				throw new ArgumentNullException("networkObject");
			}
			if (!networkObject.IsSpawned)
			{
				throw new ArgumentException("NetworkObjectReference can only be created from spawned NetworkObjects.");
			}
			m_NetworkObjectId = networkObject.NetworkObjectId;
		}

		public NetworkObjectReference(GameObject gameObject)
		{
			if (gameObject == null)
			{
				throw new ArgumentNullException("gameObject");
			}
			NetworkObject networkObject = gameObject.GetComponent<NetworkObject>() ?? throw new ArgumentException("Cannot create NetworkObjectReference from GameObject without a NetworkObject component.");
			if (!networkObject.IsSpawned)
			{
				throw new ArgumentException("NetworkObjectReference can only be created from spawned NetworkObjects.");
			}
			m_NetworkObjectId = networkObject.NetworkObjectId;
		}

		public bool TryGet(out NetworkObject networkObject, NetworkManager networkManager = null)
		{
			networkObject = Resolve(this, networkManager);
			return networkObject != null;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static NetworkObject Resolve(NetworkObjectReference networkObjectRef, NetworkManager networkManager = null)
		{
			networkManager = networkManager ?? NetworkManager.Singleton;
			networkManager.SpawnManager.SpawnedObjects.TryGetValue(networkObjectRef.m_NetworkObjectId, out var value);
			return value;
		}

		public bool Equals(NetworkObjectReference other)
		{
			return m_NetworkObjectId == other.m_NetworkObjectId;
		}

		public override bool Equals(object obj)
		{
			if (obj is NetworkObjectReference other)
			{
				return Equals(other);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return m_NetworkObjectId.GetHashCode();
		}

		public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
		{
			serializer.SerializeValue(ref m_NetworkObjectId, default(FastBufferWriter.ForPrimitives));
		}

		public static implicit operator NetworkObject(NetworkObjectReference networkObjectRef)
		{
			return Resolve(networkObjectRef);
		}

		public static implicit operator NetworkObjectReference(NetworkObject networkObject)
		{
			return new NetworkObjectReference(networkObject);
		}

		public static implicit operator GameObject(NetworkObjectReference networkObjectRef)
		{
			NetworkObject networkObject = Resolve(networkObjectRef);
			if (networkObject != null)
			{
				return networkObject.gameObject;
			}
			return null;
		}

		public static implicit operator NetworkObjectReference(GameObject gameObject)
		{
			return new NetworkObjectReference(gameObject);
		}
	}
}
