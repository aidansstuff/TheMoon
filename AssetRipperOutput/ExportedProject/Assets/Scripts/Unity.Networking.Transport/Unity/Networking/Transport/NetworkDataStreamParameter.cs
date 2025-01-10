using UnityEngine;

namespace Unity.Networking.Transport
{
	public struct NetworkDataStreamParameter : INetworkParameter
	{
		internal const int k_DefaultSize = 0;

		public int size;

		public bool Validate()
		{
			bool result = true;
			if (size < 0)
			{
				result = false;
				Debug.LogError(string.Format("{0} value ({1}) must be greater or equal to 0", "size", size));
			}
			return result;
		}
	}
}
