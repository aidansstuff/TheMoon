using System.Collections.Generic;

namespace Unity.Services.Core.Networking.Internal
{
	internal class HttpRequest
	{
		public string Method;

		public string Url;

		public Dictionary<string, string> Headers;

		public byte[] Body;

		public HttpOptions Options;

		public HttpRequest()
		{
		}

		public HttpRequest(string method, string url, Dictionary<string, string> headers, byte[] body)
		{
			Method = method;
			Url = url;
			Headers = headers;
			Body = body;
		}

		public HttpRequest SetMethod(string method)
		{
			Method = method;
			return this;
		}

		public HttpRequest SetUrl(string url)
		{
			Url = url;
			return this;
		}

		public HttpRequest SetHeader(string key, string value)
		{
			if (Headers == null)
			{
				Headers = new Dictionary<string, string>(1);
			}
			Headers[key] = value;
			return this;
		}

		public HttpRequest SetHeaders(Dictionary<string, string> headers)
		{
			Headers = headers;
			return this;
		}

		public HttpRequest SetBody(byte[] body)
		{
			Body = body;
			return this;
		}

		public HttpRequest SetOptions(HttpOptions options)
		{
			Options = options;
			return this;
		}

		public HttpRequest SetRedirectLimit(int redirectLimit)
		{
			Options.RedirectLimit = redirectLimit;
			return this;
		}

		public HttpRequest SetTimeOutInSeconds(int timeout)
		{
			Options.RequestTimeoutInSeconds = timeout;
			return this;
		}
	}
}
