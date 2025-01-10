using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity.Services.Core.Internal
{
	internal static class UnityWebRequestUtils
	{
		public const string JsonContentType = "application/json";

		public static bool HasSucceeded(this UnityWebRequest self)
		{
			return self.result == UnityWebRequest.Result.Success;
		}

		public static Task<string> GetTextAsync(string uri)
		{
			TaskCompletionSource<string> completionSource = new TaskCompletionSource<string>();
			UnityWebRequest.Get(uri).SendWebRequest().completed += CompleteFetchTaskOnRequestCompleted;
			return completionSource.Task;
			void CompleteFetchTaskOnRequestCompleted(UnityEngine.AsyncOperation rawOperation)
			{
				try
				{
					using UnityWebRequest unityWebRequest = ((UnityWebRequestAsyncOperation)rawOperation).webRequest;
					if (unityWebRequest.HasSucceeded())
					{
						completionSource.TrySetResult(unityWebRequest.downloadHandler.text);
					}
					else
					{
						string message = "Couldn't fetch config file.\nURL: " + unityWebRequest.url + "\nReason: " + unityWebRequest.error;
						completionSource.TrySetException(new Exception(message));
					}
				}
				catch (Exception exception)
				{
					completionSource.TrySetException(exception);
				}
			}
		}
	}
}
