using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Qos.Scheduler;
using UnityEngine.Networking;

namespace Unity.Services.Qos.Http
{
	internal class HttpClient : IHttpClient
	{
		public async Task<HttpClientResponse> MakeRequestAsync(string method, string url, byte[] body, Dictionary<string, string> headers, int requestTimeout)
		{
			return await CreateWebRequestAsync(method.ToUpper(), url, body, headers, requestTimeout);
		}

		public async Task<HttpClientResponse> MakeRequestAsync(string method, string url, List<IMultipartFormSection> body, Dictionary<string, string> headers, int requestTimeout, string boundary = null)
		{
			return await CreateWebRequestAsync(method.ToUpper(), url, body, headers, requestTimeout, boundary);
		}

		private async Task<HttpClientResponse> CreateWebRequestAsync(string method, string url, byte[] body, IDictionary<string, string> headers, int requestTimeout)
		{
			return await CreateHttpClientResponse(method, url, body, headers, requestTimeout);
		}

		private async Task<HttpClientResponse> CreateHttpClientResponse(string method, string url, byte[] body, IDictionary<string, string> headers, int requestTimeout)
		{
			return await (await Task.Factory.StartNew(async delegate
			{
				using UnityWebRequest request = new UnityWebRequest(url, method);
				foreach (KeyValuePair<string, string> header in headers)
				{
					request.SetRequestHeader(header.Key, header.Value);
				}
				request.timeout = requestTimeout;
				if (body != null && (method == "POST" || method == "PUT" || method == "PATCH"))
				{
					request.uploadHandler = new UploadHandlerRaw(body);
				}
				request.downloadHandler = new DownloadHandlerBuffer();
				return await SendWebRequest(request);
			}, CancellationToken.None, TaskCreationOptions.None, ThreadHelper.TaskScheduler));
		}

		private async Task<HttpClientResponse> CreateWebRequestAsync(string method, string url, List<IMultipartFormSection> body, IDictionary<string, string> headers, int requestTimeout, string boundary = null)
		{
			return await (await Task.Factory.StartNew(async delegate
			{
				byte[] boundary2 = (string.IsNullOrEmpty(boundary) ? UnityWebRequest.GenerateBoundary() : Encoding.Default.GetBytes(boundary));
				UnityWebRequest unityWebRequest = new UnityWebRequest(url, method);
				foreach (KeyValuePair<string, string> header in headers)
				{
					unityWebRequest.SetRequestHeader(header.Key, header.Value);
				}
				unityWebRequest.timeout = requestTimeout;
				unityWebRequest = SetupMultipartRequest(unityWebRequest, body, boundary2);
				unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
				return await SendWebRequest(unityWebRequest);
			}, CancellationToken.None, TaskCreationOptions.None, ThreadHelper.TaskScheduler));
		}

		private static UnityWebRequest SetupMultipartRequest(UnityWebRequest request, List<IMultipartFormSection> multipartFormSections, byte[] boundary)
		{
			byte[] data = null;
			if (multipartFormSections != null && multipartFormSections.Count != 0)
			{
				data = UnityWebRequest.SerializeFormSections(multipartFormSections, boundary);
			}
			UploadHandler uploadHandler = new UploadHandlerRaw(data);
			uploadHandler.contentType = "multipart/form-data; boundary=" + Encoding.UTF8.GetString(boundary, 0, boundary.Length);
			request.uploadHandler = uploadHandler;
			request.downloadHandler = new DownloadHandlerBuffer();
			return request;
		}

		private UnityWebRequestAsyncOperation SendWebRequest(UnityWebRequest request)
		{
			return request.SendWebRequest();
		}
	}
}
