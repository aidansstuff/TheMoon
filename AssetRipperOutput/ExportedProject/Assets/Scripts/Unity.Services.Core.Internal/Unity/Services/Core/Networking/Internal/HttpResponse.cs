using System.Collections.Generic;

namespace Unity.Services.Core.Networking.Internal
{
	internal class HttpResponse
	{
		public ReadOnlyHttpRequest Request;

		public Dictionary<string, string> Headers;

		public byte[] Data;

		public long StatusCode;

		public string ErrorMessage;

		public bool IsHttpError;

		public bool IsNetworkError;

		public HttpResponse SetRequest(HttpRequest request)
		{
			Request = new ReadOnlyHttpRequest(request);
			return this;
		}

		public HttpResponse SetRequest(ReadOnlyHttpRequest request)
		{
			Request = request;
			return this;
		}

		public HttpResponse SetHeader(string key, string value)
		{
			Headers[key] = value;
			return this;
		}

		public HttpResponse SetHeaders(Dictionary<string, string> headers)
		{
			Headers = headers;
			return this;
		}

		public HttpResponse SetData(byte[] data)
		{
			Data = data;
			return this;
		}

		public HttpResponse SetStatusCode(long statusCode)
		{
			StatusCode = statusCode;
			return this;
		}

		public HttpResponse SetErrorMessage(string errorMessage)
		{
			ErrorMessage = errorMessage;
			return this;
		}

		public HttpResponse SetIsHttpError(bool isHttpError)
		{
			IsHttpError = isHttpError;
			return this;
		}

		public HttpResponse SetIsNetworkError(bool isNetworkError)
		{
			IsNetworkError = isNetworkError;
			return this;
		}
	}
}
