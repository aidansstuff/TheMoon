using System;

namespace Unity.Netcode
{
	public class InvalidChannelException : Exception
	{
		public InvalidChannelException(string message)
			: base(message)
		{
		}
	}
}
