using System.Collections.Generic;
using System.Threading.Tasks;

namespace Unity.Services.Authentication
{
	internal class NetworkHandler : INetworkHandler
	{
		public static class ContentType
		{
			public const string Json = "application/json";
		}

		private INetworkConfiguration Configuration { get; }

		public NetworkHandler(INetworkConfiguration configuration)
		{
			Configuration = configuration;
		}

		public Task<T> GetAsync<T>(string url, IDictionary<string, string> headers = null)
		{
			return new WebRequest(Configuration, WebRequestVerb.Get, url, headers, null, "application/json").SendAsync<T>();
		}

		public Task<T> PostAsync<T>(string url, IDictionary<string, string> headers = null)
		{
			return new WebRequest(Configuration, WebRequestVerb.Post, url, headers, null, "application/json").SendAsync<T>();
		}

		public Task<T> PostAsync<T>(string url, object payload, IDictionary<string, string> headers = null)
		{
			string payload2 = ((payload != null) ? IsolatedJsonConvert.SerializeObject(payload, SerializerSettings.DefaultSerializerSettings) : null);
			return new WebRequest(Configuration, WebRequestVerb.Post, url, headers, payload2, "application/json").SendAsync<T>();
		}

		public Task<T> PutAsync<T>(string url, object payload, IDictionary<string, string> headers = null)
		{
			string payload2 = ((payload != null) ? IsolatedJsonConvert.SerializeObject(payload, SerializerSettings.DefaultSerializerSettings) : null);
			return new WebRequest(Configuration, WebRequestVerb.Put, url, headers, payload2, "application/json").SendAsync<T>();
		}

		public Task DeleteAsync(string url, IDictionary<string, string> headers = null)
		{
			return new WebRequest(Configuration, WebRequestVerb.Delete, url, headers, null, "application/json").SendAsync();
		}

		public Task<T> DeleteAsync<T>(string url, IDictionary<string, string> headers = null)
		{
			return new WebRequest(Configuration, WebRequestVerb.Delete, url, headers, null, "application/json").SendAsync<T>();
		}
	}
}
