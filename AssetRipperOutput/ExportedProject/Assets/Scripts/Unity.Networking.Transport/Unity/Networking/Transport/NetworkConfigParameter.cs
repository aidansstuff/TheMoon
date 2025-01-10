using UnityEngine;

namespace Unity.Networking.Transport
{
	public struct NetworkConfigParameter : INetworkParameter
	{
		public int connectTimeoutMS;

		public int maxConnectAttempts;

		public int disconnectTimeoutMS;

		public int heartbeatTimeoutMS;

		public int maxFrameTimeMS;

		public int fixedFrameTimeMS;

		public int maxMessageSize;

		public bool Validate()
		{
			bool flag = true;
			if (connectTimeoutMS < 0)
			{
				flag = false;
				Debug.LogError(string.Format("{0} value ({1}) must be greater or equal to 0", "connectTimeoutMS", connectTimeoutMS));
			}
			if (maxConnectAttempts < 0)
			{
				flag = false;
				Debug.LogError(string.Format("{0} value ({1}) must be greater or equal to 0", "maxConnectAttempts", maxConnectAttempts));
			}
			if (disconnectTimeoutMS < 0)
			{
				flag = false;
				Debug.LogError(string.Format("{0} value ({1}) must be greater or equal to 0", "disconnectTimeoutMS", disconnectTimeoutMS));
			}
			if (heartbeatTimeoutMS < 0)
			{
				flag = false;
				Debug.LogError(string.Format("{0} value ({1}) must be greater or equal to 0", "heartbeatTimeoutMS", heartbeatTimeoutMS));
			}
			if (maxFrameTimeMS < 0)
			{
				flag = false;
				Debug.LogError(string.Format("{0} value ({1}) must be greater or equal to 0", "maxFrameTimeMS", maxFrameTimeMS));
			}
			if (fixedFrameTimeMS < 0)
			{
				flag = false;
				Debug.LogError(string.Format("{0} value ({1}) must be greater or equal to 0", "fixedFrameTimeMS", fixedFrameTimeMS));
			}
			if (maxMessageSize <= 0 || maxMessageSize > 1472)
			{
				flag = false;
				Debug.LogError(string.Format("{0} value ({1}) must be greater than 0 and less than or equal to {2}", "maxMessageSize", maxMessageSize, 1472));
			}
			if (flag && maxMessageSize < 548)
			{
				Debug.LogWarning(string.Format("{0} value ({1}) is unnecessarily low. 548 should be safe in all circumstances.", "maxMessageSize", maxMessageSize));
			}
			return flag;
		}
	}
}
