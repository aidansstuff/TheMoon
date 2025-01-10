using System.Collections.Generic;

namespace Unity.Services.Core.Networking.Internal
{
	internal struct ReadOnlyHttpResponse
	{
		private HttpResponse m_Response;

		public ReadOnlyHttpRequest Request => m_Response.Request;

		public IReadOnlyDictionary<string, string> Headers => m_Response.Headers;

		public byte[] Data => m_Response.Data;

		public long StatusCode => m_Response.StatusCode;

		public string ErrorMessage => m_Response.ErrorMessage;

		public bool IsHttpError => m_Response.IsHttpError;

		public bool IsNetworkError => m_Response.IsNetworkError;

		public ReadOnlyHttpResponse(HttpResponse response)
		{
			m_Response = response;
		}
	}
}
