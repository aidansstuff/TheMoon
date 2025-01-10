using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Unity.Netcode
{
	[CreateAssetMenu(fileName = "NetworkPrefabsList", menuName = "Netcode/Network Prefabs List")]
	public class NetworkPrefabsList : ScriptableObject
	{
		internal delegate void OnAddDelegate(NetworkPrefab prefab);

		internal delegate void OnRemoveDelegate(NetworkPrefab prefab);

		internal OnAddDelegate OnAdd;

		internal OnRemoveDelegate OnRemove;

		[SerializeField]
		internal bool IsDefault;

		[FormerlySerializedAs("Prefabs")]
		[SerializeField]
		internal List<NetworkPrefab> List = new List<NetworkPrefab>();

		public IReadOnlyList<NetworkPrefab> PrefabList => List;

		public void Add(NetworkPrefab prefab)
		{
			List.Add(prefab);
			OnAdd?.Invoke(prefab);
		}

		public void Remove(NetworkPrefab prefab)
		{
			List.Remove(prefab);
			OnRemove?.Invoke(prefab);
		}

		public bool Contains(GameObject prefab)
		{
			for (int i = 0; i < List.Count; i++)
			{
				if (List[i].Prefab == prefab)
				{
					return true;
				}
			}
			return false;
		}

		public bool Contains(NetworkPrefab prefab)
		{
			for (int i = 0; i < List.Count; i++)
			{
				if (List[i].Equals(prefab))
				{
					return true;
				}
			}
			return false;
		}
	}
}
