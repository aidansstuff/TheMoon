using UnityEngine;

namespace Unity.Networking.Transport
{
	public struct NetworkPipelineParams : INetworkParameter
	{
		internal const int k_DefaultInitialCapacity = 0;

		public int initialCapacity;

		public bool Validate()
		{
			bool result = true;
			if (initialCapacity < 0)
			{
				result = false;
				Debug.LogError(string.Format("{0} value ({1}) must be greater or equal to 0", "initialCapacity", initialCapacity));
			}
			return result;
		}
	}
}
