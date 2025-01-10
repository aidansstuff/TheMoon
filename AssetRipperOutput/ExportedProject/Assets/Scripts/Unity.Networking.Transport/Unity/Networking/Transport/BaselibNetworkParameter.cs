using UnityEngine;

namespace Unity.Networking.Transport
{
	public struct BaselibNetworkParameter : INetworkParameter
	{
		public int receiveQueueCapacity;

		public int sendQueueCapacity;

		public uint maximumPayloadSize;

		public bool Validate()
		{
			bool result = true;
			if (receiveQueueCapacity <= 0)
			{
				result = false;
				Debug.LogError(string.Format("{0} value ({1}) must be greater than 0", "receiveQueueCapacity", receiveQueueCapacity));
			}
			if (sendQueueCapacity <= 0)
			{
				result = false;
				Debug.LogError(string.Format("{0} value ({1}) must be greater than 0", "sendQueueCapacity", sendQueueCapacity));
			}
			if (maximumPayloadSize == 0)
			{
				result = false;
				Debug.LogError(string.Format("{0} value ({1}) must be greater than 0", "maximumPayloadSize", maximumPayloadSize));
			}
			return result;
		}
	}
}
