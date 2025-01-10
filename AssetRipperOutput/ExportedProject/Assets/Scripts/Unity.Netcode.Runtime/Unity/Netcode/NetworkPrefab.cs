using System;
using UnityEngine;

namespace Unity.Netcode
{
	[Serializable]
	public class NetworkPrefab
	{
		public NetworkPrefabOverride Override;

		public GameObject Prefab;

		public GameObject SourcePrefabToOverride;

		public uint SourceHashToOverride;

		public GameObject OverridingTargetPrefab;

		public uint SourcePrefabGlobalObjectIdHash
		{
			get
			{
				switch (Override)
				{
				case NetworkPrefabOverride.None:
				{
					if (Prefab != null && Prefab.TryGetComponent<NetworkObject>(out var component2))
					{
						return component2.GlobalObjectIdHash;
					}
					throw new InvalidOperationException("Prefab field is not set or is not a NetworkObject");
				}
				case NetworkPrefabOverride.Prefab:
				{
					if (SourcePrefabToOverride != null && SourcePrefabToOverride.TryGetComponent<NetworkObject>(out var component))
					{
						return component.GlobalObjectIdHash;
					}
					throw new InvalidOperationException("Source Prefab field is not set or is not a NetworkObject");
				}
				case NetworkPrefabOverride.Hash:
					return SourceHashToOverride;
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}

		public uint TargetPrefabGlobalObjectIdHash
		{
			get
			{
				switch (Override)
				{
				case NetworkPrefabOverride.None:
					return 0u;
				case NetworkPrefabOverride.Prefab:
				case NetworkPrefabOverride.Hash:
				{
					if (OverridingTargetPrefab != null && OverridingTargetPrefab.TryGetComponent<NetworkObject>(out var component))
					{
						return component.GlobalObjectIdHash;
					}
					throw new InvalidOperationException("Target Prefab field is not set or is not a NetworkObject");
				}
				default:
					throw new ArgumentOutOfRangeException();
				}
			}
		}

		public bool Equals(NetworkPrefab other)
		{
			if (Override == other.Override && Prefab == other.Prefab && SourcePrefabToOverride == other.SourcePrefabToOverride && SourceHashToOverride == other.SourceHashToOverride)
			{
				return OverridingTargetPrefab == other.OverridingTargetPrefab;
			}
			return false;
		}

		public bool Validate(int index = -1)
		{
			if (Override == NetworkPrefabOverride.None)
			{
				if (Prefab == null)
				{
					NetworkLog.LogWarning(string.Format("{0} cannot be null ({1} at index: {2})", "NetworkPrefab", "NetworkPrefab", index));
					return false;
				}
				NetworkObject component = Prefab.GetComponent<NetworkObject>();
				if (component == null)
				{
					if (NetworkLog.CurrentLogLevel <= LogLevel.Error)
					{
						NetworkLog.LogWarning(NetworkPrefabHandler.PrefabDebugHelper(this) + " is missing a NetworkObject component (entry will be ignored).");
					}
					return false;
				}
				return true;
			}
			switch (Override)
			{
			case NetworkPrefabOverride.Hash:
				if (SourceHashToOverride == 0)
				{
					if (NetworkLog.CurrentLogLevel <= LogLevel.Error)
					{
						NetworkLog.LogWarning("NetworkPrefab SourceHashToOverride is zero (entry will be ignored).");
					}
					return false;
				}
				break;
			case NetworkPrefabOverride.Prefab:
			{
				if (SourcePrefabToOverride == null)
				{
					if (Prefab != null)
					{
						SourcePrefabToOverride = Prefab;
					}
					else if (NetworkLog.CurrentLogLevel <= LogLevel.Error)
					{
						NetworkLog.LogWarning("NetworkPrefab SourcePrefabToOverride is null (entry will be ignored).");
						return false;
					}
				}
				if (!SourcePrefabToOverride.TryGetComponent<NetworkObject>(out var _))
				{
					if (NetworkLog.CurrentLogLevel <= LogLevel.Error)
					{
						NetworkLog.LogWarning("NetworkPrefab (" + SourcePrefabToOverride.name + ") is missing a NetworkObject component (entry will be ignored).");
					}
					return false;
				}
				break;
			}
			}
			if (OverridingTargetPrefab == null)
			{
				if (NetworkLog.CurrentLogLevel <= LogLevel.Error)
				{
					NetworkLog.LogWarning("NetworkPrefab OverridingTargetPrefab is null!");
				}
				switch (Override)
				{
				case NetworkPrefabOverride.Hash:
					Debug.LogWarning(string.Format("{0} override entry {1} will be removed and ignored.", "NetworkPrefab", SourceHashToOverride));
					break;
				case NetworkPrefabOverride.Prefab:
					Debug.LogWarning("NetworkPrefab override entry (" + SourcePrefabToOverride.name + ") will be removed and ignored.");
					break;
				}
				return false;
			}
			return true;
		}

		public override string ToString()
		{
			return $"{{SourceHash: {SourceHashToOverride}, TargetHash: {TargetPrefabGlobalObjectIdHash}}}";
		}
	}
}
