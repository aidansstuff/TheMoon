using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Authentication.Shared;
using UnityEngine.Networking;

namespace Unity.Services.Authentication
{
	internal class AuthenticationApiClient : IApiClient
	{
		private INetworkConfiguration Configuration { get; }

		public AuthenticationApiClient(INetworkConfiguration configuration)
		{
			Configuration = configuration;
		}

		public Task<ApiResponse> GetAsync(string path, ApiRequestOptions options, IApiConfiguration configuration, CancellationToken cancellationToken = default(CancellationToken))
		{
			return SendAsync(path, WebRequestVerb.Get, options, configuration, cancellationToken);
		}

		public Task<ApiResponse<T>> GetAsync<T>(string path, ApiRequestOptions options, IApiConfiguration configuration, CancellationToken cancellationToken = default(CancellationToken))
		{
			return SendAsync<T>(path, WebRequestVerb.Get, options, configuration, cancellationToken);
		}

		public Task<ApiResponse> PostAsync(string path, ApiRequestOptions options, IApiConfiguration configuration, CancellationToken cancellationToken = default(CancellationToken))
		{
			return SendAsync(path, WebRequestVerb.Post, options, configuration, cancellationToken);
		}

		public Task<ApiResponse<T>> PostAsync<T>(string path, ApiRequestOptions options, IApiConfiguration configuration, CancellationToken cancellationToken = default(CancellationToken))
		{
			return SendAsync<T>(path, WebRequestVerb.Post, options, configuration, cancellationToken);
		}

		public Task<ApiResponse> PutAsync(string path, ApiRequestOptions options, IApiConfiguration configuration, CancellationToken cancellationToken = default(CancellationToken))
		{
			return SendAsync(path, WebRequestVerb.Put, options, configuration, cancellationToken);
		}

		public Task<ApiResponse<T>> PutAsync<T>(string path, ApiRequestOptions options, IApiConfiguration configuration, CancellationToken cancellationToken = default(CancellationToken))
		{
			return SendAsync<T>(path, WebRequestVerb.Put, options, configuration, cancellationToken);
		}

		public Task<ApiResponse> DeleteAsync(string path, ApiRequestOptions options, IApiConfiguration configuration, CancellationToken cancellationToken = default(CancellationToken))
		{
			return SendAsync(path, WebRequestVerb.Delete, options, configuration, cancellationToken);
		}

		public Task<ApiResponse<T>> DeleteAsync<T>(string path, ApiRequestOptions options, IApiConfiguration configuration, CancellationToken cancellationToken = default(CancellationToken))
		{
			return SendAsync<T>(path, WebRequestVerb.Delete, options, configuration, cancellationToken);
		}

		public Task<ApiResponse> HeadAsync(string path, ApiRequestOptions options, IApiConfiguration configuration, CancellationToken cancellationToken = default(CancellationToken))
		{
			return SendAsync(path, WebRequestVerb.Head, options, configuration, cancellationToken);
		}

		public Task<ApiResponse<T>> HeadAsync<T>(string path, ApiRequestOptions options, IApiConfiguration configuration, CancellationToken cancellationToken = default(CancellationToken))
		{
			return SendAsync<T>(path, WebRequestVerb.Head, options, configuration, cancellationToken);
		}

		public Task<ApiResponse> OptionsAsync(string path, ApiRequestOptions options, IApiConfiguration configuration, CancellationToken cancellationToken = default(CancellationToken))
		{
			return SendAsync(path, WebRequestVerb.Options, options, configuration, cancellationToken);
		}

		public Task<ApiResponse<T>> OptionsAsync<T>(string path, ApiRequestOptions options, IApiConfiguration configuration, CancellationToken cancellationToken = default(CancellationToken))
		{
			return SendAsync<T>(path, WebRequestVerb.Options, options, configuration, cancellationToken);
		}

		public Task<ApiResponse> PatchAsync(string path, ApiRequestOptions options, IApiConfiguration configuration, CancellationToken cancellationToken = default(CancellationToken))
		{
			return SendAsync(path, WebRequestVerb.Patch, options, configuration, cancellationToken);
		}

		public Task<ApiResponse<T>> PatchAsync<T>(string path, ApiRequestOptions options, IApiConfiguration configuration, CancellationToken cancellationToken = default(CancellationToken))
		{
			return SendAsync<T>(path, WebRequestVerb.Patch, options, configuration, cancellationToken);
		}

		private async Task<ApiResponse> SendAsync(string path, WebRequestVerb method, ApiRequestOptions options, IApiConfiguration configuration, CancellationToken cancellationToken = default(CancellationToken))
		{
			int attempts = 0;
			bool flag;
			do
			{
				try
				{
					using UnityWebRequest request = BuildWebRequest(path, method, options, configuration);
					return await request.SendWebRequestAsync();
				}
				catch (ApiException ex)
				{
					flag = ex.Type == ApiExceptionType.Network;
					attempts++;
					if (attempts >= Configuration.Retries || !flag)
					{
						throw;
					}
				}
			}
			while (flag);
			return null;
		}

		private async Task<ApiResponse<T>> SendAsync<T>(string path, WebRequestVerb method, ApiRequestOptions options, IApiConfiguration configuration, CancellationToken cancellationToken = default(CancellationToken))
		{
			int attempts = 0;
			bool flag;
			do
			{
				try
				{
					using UnityWebRequest request = BuildWebRequest(path, method, options, configuration);
					return await request.SendWebRequestAsync<T>(cancellationToken);
				}
				catch (ApiException ex)
				{
					flag = ex.Type == ApiExceptionType.Network;
					if (attempts >= Configuration.Retries || !flag)
					{
						throw;
					}
					attempts++;
				}
			}
			while (flag);
			return null;
		}

		internal UnityWebRequest BuildWebRequest(string path, WebRequestVerb method, ApiRequestOptions options, IApiConfiguration configuration)
		{
			ApiRequestPathBuilder apiRequestPathBuilder = new ApiRequestPathBuilder(configuration.BasePath, path);
			apiRequestPathBuilder.AddPathParameters(options.PathParameters);
			apiRequestPathBuilder.AddQueryParameters(options.QueryParameters);
			UnityWebRequest unityWebRequest = new UnityWebRequest(apiRequestPathBuilder.GetFullUri(), method.ToString());
			if (configuration.UserAgent != null)
			{
				unityWebRequest.SetRequestHeader("User-Agent", configuration.UserAgent);
			}
			if (configuration.DefaultHeaders != null)
			{
				foreach (KeyValuePair<string, string> defaultHeader in configuration.DefaultHeaders)
				{
					unityWebRequest.SetRequestHeader(defaultHeader.Key, defaultHeader.Value);
				}
			}
			if (options.HeaderParameters != null)
			{
				foreach (KeyValuePair<string, IList<string>> headerParameter in options.HeaderParameters)
				{
					foreach (string item in headerParameter.Value)
					{
						unityWebRequest.SetRequestHeader(headerParameter.Key, item);
					}
				}
			}
			unityWebRequest.timeout = configuration.Timeout;
			if (options.Data != null)
			{
				JsonSerializerSettings settings = new JsonSerializerSettings
				{
					ReferenceLoopHandling = ReferenceLoopHandling.Ignore
				};
				string s = IsolatedJsonConvert.SerializeObject(options.Data, settings);
				unityWebRequest.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(s));
			}
			unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
			return unityWebRequest;
		}
	}
}
