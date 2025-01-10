using UnityEngine;

namespace Unity.Netcode
{
	public interface INetworkPrefabInstanceHandler
	{
		NetworkObject Instantiate(ulong ownerClientId, Vector3 position, Quaternion rotation);

		void Destroy(NetworkObject networkObject);
	}
}
