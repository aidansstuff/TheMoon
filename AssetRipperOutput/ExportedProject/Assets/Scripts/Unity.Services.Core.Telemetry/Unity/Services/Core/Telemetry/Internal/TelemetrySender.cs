using System;
using System.Text;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Unity.Services.Core.Internal.Serialization;
using Unity.Services.Core.Scheduler.Internal;
using UnityEngine.Networking;

namespace Unity.Services.Core.Telemetry.Internal
{
	internal class TelemetrySender
	{
		private readonly ExponentialBackOffRetryPolicy m_RetryPolicy;

		private readonly IActionScheduler m_Scheduler;

		private readonly IUnityWebRequestSender m_RequestSender;

		public string TargetUrl { get; }

		internal IJsonSerializer Serializer { get; }

		public TelemetrySender([NotNull] string targetUrl, [NotNull] string servicePath, [NotNull] IActionScheduler scheduler, [NotNull] ExponentialBackOffRetryPolicy retryPolicy, [NotNull] IUnityWebRequestSender requestSender, [NotNull] IJsonSerializer serializer)
		{
			TargetUrl = targetUrl + "/" + servicePath;
			m_RetryPolicy = retryPolicy;
			m_Scheduler = scheduler;
			m_RequestSender = requestSender;
			Serializer = serializer;
		}

		public Task SendAsync<TPayload>(TPayload payload) where TPayload : ITelemetryPayload
		{
			TaskCompletionSource<object> completionSource = new TaskCompletionSource<object>();
			int sendCount = 0;
			byte[] serializedPayload;
			try
			{
				serializedPayload = SerializePayload(payload);
				SendWebRequest();
			}
			catch (Exception exception)
			{
				completionSource.TrySetException(exception);
			}
			return completionSource.Task;
			void OnRequestCompleted(WebRequest webRequest)
			{
				try
				{
					if (webRequest.IsSuccess)
					{
						completionSource.TrySetResult(null);
					}
					else if (m_RetryPolicy.CanRetry(webRequest, sendCount))
					{
						float delayBeforeSendingSeconds = m_RetryPolicy.GetDelayBeforeSendingSeconds(sendCount);
						m_Scheduler.ScheduleAction(SendWebRequest, delayBeforeSendingSeconds);
					}
					else
					{
						string message = "Error: " + webRequest.ErrorMessage + "\nBody: " + webRequest.ErrorBody;
						completionSource.TrySetException(new Exception(message));
					}
				}
				catch (Exception ex) when (TelemetryUtils.LogTelemetryException(ex, predicateValue: true))
				{
					completionSource.TrySetException(ex);
				}
			}
			void SendWebRequest()
			{
				UnityWebRequest request = CreateRequest(serializedPayload);
				sendCount++;
				m_RequestSender.SendRequest(request, OnRequestCompleted);
			}
		}

		internal byte[] SerializePayload<TPayload>(TPayload payload) where TPayload : ITelemetryPayload
		{
			string s = Serializer.SerializeObject(payload);
			return Encoding.UTF8.GetBytes(s);
		}

		internal UnityWebRequest CreateRequest(byte[] serializedPayload)
		{
			UnityWebRequest unityWebRequest = new UnityWebRequest(TargetUrl, "POST");
			unityWebRequest.uploadHandler = new UploadHandlerRaw(serializedPayload)
			{
				contentType = "application/json"
			};
			unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
			unityWebRequest.SetRequestHeader("Content-Type", "application/json");
			return unityWebRequest;
		}
	}
}
