using System;

namespace Unity.Netcode
{
	public class NetworkConfigurationException : Exception
	{
		public NetworkConfigurationException()
		{
		}

		public NetworkConfigurationException(string message)
			: base(message)
		{
		}

		public NetworkConfigurationException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
