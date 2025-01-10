using System;
using System.Threading;
using System.Threading.Tasks;
using Unity.Services.Authentication.Shared;
using UnityEngine.Networking;

namespace Unity.Services.Authentication
{
	internal static class AuthenticationWebRequestUtils
	{
		public static Task<ApiResponse> SendWebRequestAsync(this UnityWebRequest request)
		{
			TaskCompletionSource<ApiResponse> tcs = new TaskCompletionSource<ApiResponse>();
			UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = request.SendWebRequest();
			if (unityWebRequestAsyncOperation.isDone)
			{
				ProcessResponse(tcs, request);
			}
			else
			{
				unityWebRequestAsyncOperation.completed += delegate
				{
					ProcessResponse(tcs, request);
				};
			}
			return tcs.Task;
		}

		public static Task<ApiResponse<T>> SendWebRequestAsync<T>(this UnityWebRequest request, CancellationToken cancellationToken)
		{
			TaskCompletionSource<ApiResponse<T>> tcs = new TaskCompletionSource<ApiResponse<T>>();
			cancellationToken.Register(delegate
			{
				tcs.SetCanceled();
			});
			UnityWebRequestAsyncOperation unityWebRequestAsyncOperation = request.SendWebRequest();
			if (unityWebRequestAsyncOperation.isDone)
			{
				ProcessResponse(tcs, request);
			}
			else
			{
				unityWebRequestAsyncOperation.completed += delegate
				{
					ProcessResponse(tcs, request);
				};
			}
			return tcs.Task;
		}

		private static void ProcessResponse(TaskCompletionSource<ApiResponse> tcs, UnityWebRequest request)
		{
			ApiResponse apiResponse = new ApiResponse
			{
				StatusCode = (int)request.responseCode,
				ErrorText = request.error,
				RawContent = request.downloadHandler?.text
			};
			string message = request.error + "\n" + request.downloadHandler?.text;
			if (IsNetworkError(request))
			{
				tcs.SetException(new ApiException(ApiExceptionType.Network, message, apiResponse));
			}
			else if (IsHttpError(request))
			{
				tcs.SetException(new ApiException(ApiExceptionType.Http, message, apiResponse));
			}
			else
			{
				tcs.SetResult(apiResponse);
			}
		}

		private static void ProcessResponse<T>(TaskCompletionSource<ApiResponse<T>> tcs, UnityWebRequest request)
		{
			ApiResponse<T> apiResponse = new ApiResponse<T>
			{
				StatusCode = (int)request.responseCode,
				ErrorText = request.error,
				RawContent = request.downloadHandler?.text
			};
			string message = request.error + "\n" + request.downloadHandler?.text;
			if (IsNetworkError(request))
			{
				tcs.SetException(new ApiException(ApiExceptionType.Network, message, apiResponse));
				return;
			}
			if (IsHttpError(request))
			{
				tcs.SetException(new ApiException(ApiExceptionType.Http, message, apiResponse));
				return;
			}
			try
			{
				if (!string.IsNullOrEmpty(request.downloadHandler?.text))
				{
					apiResponse.Data = IsolatedJsonConvert.DeserializeObject<T>(request.downloadHandler?.text, SerializerSettings.DefaultSerializerSettings);
				}
			}
			catch (Exception)
			{
				tcs.SetException(new ApiException(ApiExceptionType.Deserialization, $"Deserialization of type '{typeof(T)}' failed.", apiResponse));
				return;
			}
			tcs.SetResult(apiResponse);
		}

		public static bool IsNetworkError(UnityWebRequest request)
		{
			return request.responseCode >= 500;
		}

		public static bool IsHttpError(UnityWebRequest request)
		{
			if (request.responseCode >= 400)
			{
				return request.responseCode < 500;
			}
			return false;
		}
	}
}
