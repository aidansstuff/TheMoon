using System;

namespace Unity.Services.Core
{
	public class ServicesInitializationException : Exception
	{
		public ServicesInitializationException()
		{
		}

		public ServicesInitializationException(string message)
			: base(message)
		{
		}

		public ServicesInitializationException(string message, Exception innerException)
			: base(message, innerException)
		{
		}
	}
}
