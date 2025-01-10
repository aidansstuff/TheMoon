using UnityEngine;

namespace Unity.Netcode
{
	internal struct ParentSyncMessage : INetworkMessage
	{
		public ulong NetworkObjectId;

		private byte m_BitField;

		public ulong? LatestParent;

		public Vector3 Position;

		public Quaternion Rotation;

		public Vector3 Scale;

		public int Version => 0;

		public bool WorldPositionStays
		{
			get
			{
				return ByteUtility.GetBit(m_BitField, 0);
			}
			set
			{
				ByteUtility.SetBit(ref m_BitField, 0, value);
			}
		}

		public bool IsLatestParentSet
		{
			get
			{
				return ByteUtility.GetBit(m_BitField, 1);
			}
			set
			{
				ByteUtility.SetBit(ref m_BitField, 1, value);
			}
		}

		public bool RemoveParent
		{
			get
			{
				return ByteUtility.GetBit(m_BitField, 2);
			}
			set
			{
				ByteUtility.SetBit(ref m_BitField, 2, value);
			}
		}

		public void Serialize(FastBufferWriter writer, int targetVersion)
		{
			BytePacker.WriteValueBitPacked(writer, NetworkObjectId);
			writer.WriteValueSafe(in m_BitField, default(FastBufferWriter.ForPrimitives));
			if (!RemoveParent && IsLatestParentSet)
			{
				BytePacker.WriteValueBitPacked(writer, LatestParent.Value);
			}
			writer.WriteValueSafe(in Position);
			writer.WriteValueSafe(in Rotation);
			writer.WriteValueSafe(in Scale);
		}

		public bool Deserialize(FastBufferReader reader, ref NetworkContext context, int receivedMessageVersion)
		{
			NetworkManager networkManager = (NetworkManager)context.SystemOwner;
			if (!networkManager.IsClient)
			{
				return false;
			}
			ByteUnpacker.ReadValueBitPacked(reader, out NetworkObjectId);
			reader.ReadValueSafe(out m_BitField, default(FastBufferWriter.ForPrimitives));
			if (!RemoveParent && IsLatestParentSet)
			{
				ByteUnpacker.ReadValueBitPacked(reader, out ulong value);
				LatestParent = value;
			}
			reader.ReadValueSafe(out Position);
			reader.ReadValueSafe(out Rotation);
			reader.ReadValueSafe(out Scale);
			if (!networkManager.SpawnManager.SpawnedObjects.ContainsKey(NetworkObjectId))
			{
				networkManager.DeferredMessageManager.DeferMessage(IDeferredNetworkMessageManager.TriggerType.OnSpawn, NetworkObjectId, reader, ref context);
				return false;
			}
			return true;
		}

		public void Handle(ref NetworkContext context)
		{
			NetworkObject networkObject = ((NetworkManager)context.SystemOwner).SpawnManager.SpawnedObjects[NetworkObjectId];
			networkObject.SetNetworkParenting(LatestParent, WorldPositionStays);
			networkObject.ApplyNetworkParenting(RemoveParent);
			if (!WorldPositionStays)
			{
				networkObject.transform.localPosition = Position;
				networkObject.transform.localRotation = Rotation;
			}
			else
			{
				networkObject.transform.position = Position;
				networkObject.transform.rotation = Rotation;
			}
			networkObject.transform.localScale = Scale;
		}
	}
}
