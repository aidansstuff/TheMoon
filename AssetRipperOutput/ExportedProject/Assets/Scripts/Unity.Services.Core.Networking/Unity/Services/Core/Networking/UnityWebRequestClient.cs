using System;
using System.Collections.Generic;
using System.IO;
using Unity.Services.Core.Internal;
using Unity.Services.Core.Networking.Internal;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity.Services.Core.Networking
{
	internal class UnityWebRequestClient : IHttpClient, IServiceComponent
	{
		private readonly Dictionary<string, HttpServiceConfig> m_ServiceIdToConfig = new Dictionary<string, HttpServiceConfig>();

		public string GetBaseUrlFor(string serviceId)
		{
			return m_ServiceIdToConfig[serviceId].BaseUrl;
		}

		public HttpOptions GetDefaultOptionsFor(string serviceId)
		{
			return m_ServiceIdToConfig[serviceId].DefaultOptions;
		}

		public HttpRequest CreateRequestForService(string serviceId, string resourcePath)
		{
			HttpServiceConfig httpServiceConfig = m_ServiceIdToConfig[serviceId];
			string url = CombinePaths(httpServiceConfig.BaseUrl, resourcePath);
			return new HttpRequest().SetUrl(url).SetOptions(httpServiceConfig.DefaultOptions);
		}

		internal static string CombinePaths(string path1, string path2)
		{
			return Path.Combine(path1, path2).Replace('\\', '/');
		}

		public IAsyncOperation<ReadOnlyHttpResponse> Send(HttpRequest request)
		{
			AsyncOperation<ReadOnlyHttpResponse> operation = new AsyncOperation<ReadOnlyHttpResponse>();
			operation.SetInProgress();
			try
			{
				ConvertToWebRequest(request).SendWebRequest().completed += OnWebRequestCompleted;
			}
			catch (Exception reason)
			{
				operation.Fail(reason);
			}
			return operation;
			void OnWebRequestCompleted(UnityEngine.AsyncOperation unityOperation)
			{
				UnityWebRequest webRequest = ((UnityWebRequestAsyncOperation)unityOperation).webRequest;
				HttpResponse response = ConvertToResponse(webRequest).SetRequest(request);
				ReadOnlyHttpResponse result = new ReadOnlyHttpResponse(response);
				webRequest.Dispose();
				operation.Succeed(result);
			}
		}

		private static UnityWebRequest ConvertToWebRequest(HttpRequest request)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(request.Url, request.Method)
			{
				downloadHandler = new DownloadHandlerBuffer(),
				redirectLimit = request.Options.RedirectLimit,
				timeout = request.Options.RequestTimeoutInSeconds
			};
			if (request.Body != null && request.Body.Length != 0)
			{
				unityWebRequest.uploadHandler = new UploadHandlerRaw(request.Body);
			}
			if (request.Headers != null)
			{
				foreach (KeyValuePair<string, string> header in request.Headers)
				{
					unityWebRequest.SetRequestHeader(header.Key, header.Value);
				}
			}
			return unityWebRequest;
		}

		private static HttpResponse ConvertToResponse(UnityWebRequest webRequest)
		{
			return new HttpResponse().SetHeaders(webRequest.GetResponseHeaders()).SetData(webRequest.downloadHandler?.data).SetStatusCode(webRequest.responseCode)
				.SetErrorMessage(webRequest.error)
				.SetIsHttpError(webRequest.result == UnityWebRequest.Result.ProtocolError)
				.SetIsNetworkError(webRequest.result == UnityWebRequest.Result.ConnectionError);
		}

		internal void SetServiceConfig(HttpServiceConfig config)
		{
			m_ServiceIdToConfig[config.ServiceId] = config;
		}
	}
}
