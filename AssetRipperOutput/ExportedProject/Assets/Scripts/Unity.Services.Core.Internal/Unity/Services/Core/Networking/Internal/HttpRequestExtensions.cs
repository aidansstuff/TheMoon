namespace Unity.Services.Core.Networking.Internal
{
	internal static class HttpRequestExtensions
	{
		public static HttpRequest AsGet(this HttpRequest self)
		{
			return self.SetMethod("GET");
		}

		public static HttpRequest AsPost(this HttpRequest self)
		{
			return self.SetMethod("POST");
		}

		public static HttpRequest AsPut(this HttpRequest self)
		{
			return self.SetMethod("PUT");
		}

		public static HttpRequest AsDelete(this HttpRequest self)
		{
			return self.SetMethod("DELETE");
		}

		public static HttpRequest AsPatch(this HttpRequest self)
		{
			return self.SetMethod("PATCH");
		}

		public static HttpRequest AsHead(this HttpRequest self)
		{
			return self.SetMethod("HEAD");
		}

		public static HttpRequest AsConnect(this HttpRequest self)
		{
			return self.SetMethod("CONNECT");
		}

		public static HttpRequest AsOptions(this HttpRequest self)
		{
			return self.SetMethod("OPTIONS");
		}

		public static HttpRequest AsTrace(this HttpRequest self)
		{
			return self.SetMethod("TRACE");
		}
	}
}
