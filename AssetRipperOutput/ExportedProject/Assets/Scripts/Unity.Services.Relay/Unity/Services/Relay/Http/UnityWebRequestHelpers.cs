using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity.Services.Relay.Http
{
	internal static class UnityWebRequestHelpers
	{
		public static TaskAwaiter<HttpClientResponse> GetAwaiter(this UnityWebRequestAsyncOperation asyncOp)
		{
			TaskCompletionSource<HttpClientResponse> tcs = new TaskCompletionSource<HttpClientResponse>();
			asyncOp.completed += delegate(AsyncOperation obj)
			{
				HttpClientResponse result = CreateHttpClientResponse((UnityWebRequestAsyncOperation)obj);
				tcs.SetResult(result);
			};
			return tcs.Task.GetAwaiter();
		}

		internal static HttpClientResponse CreateHttpClientResponse(UnityWebRequestAsyncOperation unityResponse)
		{
			UnityWebRequest webRequest = unityResponse.webRequest;
			return new HttpClientResponse(webRequest.GetResponseHeaders(), webRequest.responseCode, webRequest.result == UnityWebRequest.Result.ProtocolError, webRequest.result == UnityWebRequest.Result.ConnectionError, webRequest.downloadHandler.data, webRequest.error);
		}
	}
}
