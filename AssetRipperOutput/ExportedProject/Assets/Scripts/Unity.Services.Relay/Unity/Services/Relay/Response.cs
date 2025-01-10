using System.Collections.Generic;
using Unity.Services.Relay.Http;

namespace Unity.Services.Relay
{
	internal class Response
	{
		public Dictionary<string, string> Headers { get; }

		public long Status { get; set; }

		public Response(HttpClientResponse httpResponse)
		{
			Headers = httpResponse.Headers;
			Status = httpResponse.StatusCode;
		}
	}
	internal class Response<T> : Response
	{
		public T Result { get; }

		public Response(HttpClientResponse httpResponse, T result)
			: base(httpResponse)
		{
			Result = result;
		}
	}
}
