using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

namespace Unity.Services.Core.Configuration
{
	internal static class StreamingAssetsUtils
	{
		public static Task<string> GetFileTextFromStreamingAssetsAsync(string path)
		{
			string path2 = Path.Combine(Application.streamingAssetsPath, path);
			TaskCompletionSource<string> taskCompletionSource = new TaskCompletionSource<string>();
			try
			{
				string result = File.ReadAllText(path2);
				taskCompletionSource.SetResult(result);
			}
			catch (Exception exception)
			{
				taskCompletionSource.SetException(exception);
			}
			return taskCompletionSource.Task;
		}
	}
}
