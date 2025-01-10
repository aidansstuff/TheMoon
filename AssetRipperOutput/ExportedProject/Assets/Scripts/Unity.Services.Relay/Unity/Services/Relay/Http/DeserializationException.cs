using System;

namespace Unity.Services.Relay.Http
{
	[Serializable]
	internal class DeserializationException : Exception
	{
		public DeserializationException()
		{
		}

		public DeserializationException(string message)
			: base(message)
		{
		}

		private DeserializationException(string message, Exception inner)
			: base(message, inner)
		{
		}
	}
}
