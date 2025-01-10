using UnityEngine;

namespace Unity.Networking.Transport.Relay
{
	public struct RelayNetworkParameter : INetworkParameter
	{
		internal const int k_DefaultConnectionTimeMS = 3000;

		public RelayServerData ServerData;

		public int RelayConnectionTimeMS;

		public bool Validate()
		{
			bool result = true;
			if (ServerData.Endpoint == default(NetworkEndPoint))
			{
				result = false;
				Debug.LogError(string.Format("{0} value ({1}) must be a valid value", "Endpoint", ServerData.Endpoint));
			}
			if (ServerData.AllocationId == default(RelayAllocationId))
			{
				result = false;
				Debug.LogError(string.Format("{0} value ({1}) must be a valid value", "AllocationId", ServerData.AllocationId));
			}
			if (RelayConnectionTimeMS < 0)
			{
				result = false;
				Debug.LogError(string.Format("{0} value({1}) must be greater or equal to 0", "RelayConnectionTimeMS", RelayConnectionTimeMS));
			}
			return result;
		}
	}
}
