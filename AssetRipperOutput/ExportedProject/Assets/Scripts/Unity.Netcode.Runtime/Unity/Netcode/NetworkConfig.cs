using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Netcode
{
	[Serializable]
	public class NetworkConfig
	{
		[Tooltip("Use this to make two builds incompatible with each other")]
		public ushort ProtocolVersion;

		[Tooltip("The NetworkTransport to use")]
		public NetworkTransport NetworkTransport;

		[Tooltip("When set, NetworkManager will automatically create and spawn the assigned player prefab. This can be overridden by adding it to the NetworkPrefabs list and selecting override.")]
		public GameObject PlayerPrefab;

		[SerializeField]
		public NetworkPrefabs Prefabs = new NetworkPrefabs();

		[Tooltip("The tickrate. This value controls how often netcode runs user code and sends out data. The value is in 'ticks per seconds' which means a value of 50 will result in 50 ticks being executed per second or a fixed delta time of 0.02.")]
		public uint TickRate = 30u;

		[Tooltip("The amount of seconds for the server to wait for the connection approval handshake to complete before the client is disconnected")]
		public int ClientConnectionBufferTimeout = 10;

		[Tooltip("Whether or not to force clients to be approved before they connect")]
		public bool ConnectionApproval;

		[Tooltip("The connection data sent along with connection requests")]
		public byte[] ConnectionData = new byte[0];

		[Tooltip("Enable this to re-sync the NetworkTime after the initial sync")]
		public bool EnableTimeResync;

		[Tooltip("The amount of seconds between re-syncs of NetworkTime, if enabled")]
		public int TimeResyncInterval = 30;

		[Tooltip("Ensures that NetworkVariables can be read even if a client accidental writes where its not allowed to. This will cost some CPU time and bandwidth")]
		public bool EnsureNetworkVariableLengthSafety;

		[Tooltip("Enables scene management. This will allow network scene switches and automatic scene difference corrections upon connect.\nSoftSynced scene objects wont work with this disabled. That means that disabling SceneManagement also enables PrefabSync.")]
		public bool EnableSceneManagement = true;

		[Tooltip("Whether or not the netcode should check for differences in the prefab lists at connection")]
		public bool ForceSamePrefabs = true;

		[Tooltip("If true, NetworkIds will be reused after the NetworkIdRecycleDelay")]
		public bool RecycleNetworkIds = true;

		[Tooltip("The amount of seconds a NetworkId has to unused in order for it to be reused")]
		public float NetworkIdRecycleDelay = 120f;

		[Tooltip("The maximum amount of bytes to use for RPC messages.")]
		public HashSize RpcHashSize;

		[Tooltip("The amount of seconds to wait for all clients to load or unload a requested scene (only when EnableSceneManagement is enabled)")]
		public int LoadSceneTimeOut = 120;

		[Tooltip("The amount of time a message should be buffered if the asset or object needed to process it doesn't exist yet. If the asset is not added/object is not spawned within this time, it will be dropped")]
		public float SpawnTimeout = 1f;

		public bool EnableNetworkLogs = true;

		public const int RttAverageSamples = 5;

		public const int RttWindowSize = 64;

		private ulong? m_ConfigHash;

		[NonSerialized]
		private bool m_DidWarnOldPrefabList;

		[FormerlySerializedAs("NetworkPrefabs")]
		[SerializeField]
		internal List<NetworkPrefab> OldPrefabList;

		public string ToBase64()
		{
			FastBufferWriter fastBufferWriter = new FastBufferWriter(1024, Allocator.Temp);
			using (fastBufferWriter)
			{
				fastBufferWriter.WriteValueSafe(in ProtocolVersion, default(FastBufferWriter.ForPrimitives));
				fastBufferWriter.WriteValueSafe(in TickRate, default(FastBufferWriter.ForPrimitives));
				fastBufferWriter.WriteValueSafe(in ClientConnectionBufferTimeout, default(FastBufferWriter.ForPrimitives));
				fastBufferWriter.WriteValueSafe(in ConnectionApproval, default(FastBufferWriter.ForPrimitives));
				fastBufferWriter.WriteValueSafe(in LoadSceneTimeOut, default(FastBufferWriter.ForPrimitives));
				fastBufferWriter.WriteValueSafe(in EnableTimeResync, default(FastBufferWriter.ForPrimitives));
				fastBufferWriter.WriteValueSafe(in EnsureNetworkVariableLengthSafety, default(FastBufferWriter.ForPrimitives));
				fastBufferWriter.WriteValueSafe(in RpcHashSize, default(FastBufferWriter.ForEnums));
				fastBufferWriter.WriteValueSafe(in ForceSamePrefabs, default(FastBufferWriter.ForPrimitives));
				fastBufferWriter.WriteValueSafe(in EnableSceneManagement, default(FastBufferWriter.ForPrimitives));
				fastBufferWriter.WriteValueSafe(in RecycleNetworkIds, default(FastBufferWriter.ForPrimitives));
				fastBufferWriter.WriteValueSafe(in NetworkIdRecycleDelay, default(FastBufferWriter.ForPrimitives));
				fastBufferWriter.WriteValueSafe(in EnableNetworkLogs, default(FastBufferWriter.ForPrimitives));
				return Convert.ToBase64String(fastBufferWriter.ToArray());
			}
		}

		public void FromBase64(string base64)
		{
			byte[] buffer = Convert.FromBase64String(base64);
			using FastBufferReader fastBufferReader = new FastBufferReader(buffer, Allocator.Temp);
			using (fastBufferReader)
			{
				fastBufferReader.ReadValueSafe(out ProtocolVersion, default(FastBufferWriter.ForPrimitives));
				fastBufferReader.ReadValueSafe(out TickRate, default(FastBufferWriter.ForPrimitives));
				fastBufferReader.ReadValueSafe(out ClientConnectionBufferTimeout, default(FastBufferWriter.ForPrimitives));
				fastBufferReader.ReadValueSafe(out ConnectionApproval, default(FastBufferWriter.ForPrimitives));
				fastBufferReader.ReadValueSafe(out LoadSceneTimeOut, default(FastBufferWriter.ForPrimitives));
				fastBufferReader.ReadValueSafe(out EnableTimeResync, default(FastBufferWriter.ForPrimitives));
				fastBufferReader.ReadValueSafe(out EnsureNetworkVariableLengthSafety, default(FastBufferWriter.ForPrimitives));
				fastBufferReader.ReadValueSafe(out RpcHashSize, default(FastBufferWriter.ForEnums));
				fastBufferReader.ReadValueSafe(out ForceSamePrefabs, default(FastBufferWriter.ForPrimitives));
				fastBufferReader.ReadValueSafe(out EnableSceneManagement, default(FastBufferWriter.ForPrimitives));
				fastBufferReader.ReadValueSafe(out RecycleNetworkIds, default(FastBufferWriter.ForPrimitives));
				fastBufferReader.ReadValueSafe(out NetworkIdRecycleDelay, default(FastBufferWriter.ForPrimitives));
				fastBufferReader.ReadValueSafe(out EnableNetworkLogs, default(FastBufferWriter.ForPrimitives));
			}
		}

		internal void ClearConfigHash()
		{
			m_ConfigHash = null;
		}

		public ulong GetConfig(bool cache = true)
		{
			if (m_ConfigHash.HasValue && cache)
			{
				return m_ConfigHash.Value;
			}
			FastBufferWriter fastBufferWriter = new FastBufferWriter(1024, Allocator.Temp, int.MaxValue);
			using (fastBufferWriter)
			{
				fastBufferWriter.WriteValueSafe(in ProtocolVersion, default(FastBufferWriter.ForPrimitives));
				fastBufferWriter.WriteValueSafe("15.0.0");
				if (ForceSamePrefabs)
				{
					foreach (KeyValuePair<uint, NetworkPrefab> item in Prefabs.NetworkPrefabOverrideLinks.OrderBy((KeyValuePair<uint, NetworkPrefab> x) => x.Key))
					{
						uint value = item.Key;
						fastBufferWriter.WriteValueSafe(in value, default(FastBufferWriter.ForPrimitives));
					}
				}
				fastBufferWriter.WriteValueSafe(in TickRate, default(FastBufferWriter.ForPrimitives));
				fastBufferWriter.WriteValueSafe(in ConnectionApproval, default(FastBufferWriter.ForPrimitives));
				fastBufferWriter.WriteValueSafe(in ForceSamePrefabs, default(FastBufferWriter.ForPrimitives));
				fastBufferWriter.WriteValueSafe(in EnableSceneManagement, default(FastBufferWriter.ForPrimitives));
				fastBufferWriter.WriteValueSafe(in EnsureNetworkVariableLengthSafety, default(FastBufferWriter.ForPrimitives));
				fastBufferWriter.WriteValueSafe(in RpcHashSize, default(FastBufferWriter.ForEnums));
				if (cache)
				{
					m_ConfigHash = fastBufferWriter.ToArray().Hash64();
					return m_ConfigHash.Value;
				}
				return fastBufferWriter.ToArray().Hash64();
			}
		}

		public bool CompareConfig(ulong hash)
		{
			return hash == GetConfig();
		}

		internal void InitializePrefabs()
		{
			if (HasOldPrefabList())
			{
				MigrateOldNetworkPrefabsToNetworkPrefabsList();
			}
			Prefabs.Initialize();
		}

		private void WarnOldPrefabList()
		{
			if (!m_DidWarnOldPrefabList)
			{
				Debug.LogWarning("Using Legacy Network Prefab List. Consider Migrating.");
				m_DidWarnOldPrefabList = true;
			}
		}

		internal bool HasOldPrefabList()
		{
			List<NetworkPrefab> oldPrefabList = OldPrefabList;
			if (oldPrefabList == null)
			{
				return false;
			}
			return oldPrefabList.Count > 0;
		}

		internal NetworkPrefabsList MigrateOldNetworkPrefabsToNetworkPrefabsList()
		{
			if (OldPrefabList == null || OldPrefabList.Count == 0)
			{
				return null;
			}
			if (Prefabs == null)
			{
				throw new Exception("Prefabs field is null.");
			}
			Prefabs.NetworkPrefabsLists.Add(ScriptableObject.CreateInstance<NetworkPrefabsList>());
			List<NetworkPrefab> oldPrefabList = OldPrefabList;
			if (oldPrefabList != null && oldPrefabList.Count > 0)
			{
				foreach (NetworkPrefab oldPrefab in OldPrefabList)
				{
					Prefabs.NetworkPrefabsLists[Prefabs.NetworkPrefabsLists.Count - 1].Add(oldPrefab);
				}
			}
			OldPrefabList = null;
			return Prefabs.NetworkPrefabsLists[Prefabs.NetworkPrefabsLists.Count - 1];
		}
	}
}
