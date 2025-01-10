using System;

namespace Unity.Netcode
{
	public class NotListeningException : Exception
	{
		public NotListeningException()
		{
		}

		public NotListeningException(string message)
			: base(message)
		{
		}

		public NotListeningException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
