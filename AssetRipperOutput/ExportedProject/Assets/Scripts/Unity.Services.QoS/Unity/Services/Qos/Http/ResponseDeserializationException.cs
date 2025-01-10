using System;

namespace Unity.Services.Qos.Http
{
	[Serializable]
	internal class ResponseDeserializationException : Exception
	{
		public HttpClientResponse response;

		public ResponseDeserializationException()
		{
		}

		public ResponseDeserializationException(string message)
			: base(message)
		{
		}

		private ResponseDeserializationException(Exception inner, string message)
			: base(message, inner)
		{
		}

		public ResponseDeserializationException(HttpClientResponse httpClientResponse)
			: base("Unable to Deserialize Http Client Response")
		{
			response = httpClientResponse;
		}

		public ResponseDeserializationException(HttpClientResponse httpClientResponse, string message)
			: base(message)
		{
			response = httpClientResponse;
		}

		public ResponseDeserializationException(HttpClientResponse httpClientResponse, Exception inner, string message)
			: base(message, inner)
		{
			response = httpClientResponse;
		}
	}
}
