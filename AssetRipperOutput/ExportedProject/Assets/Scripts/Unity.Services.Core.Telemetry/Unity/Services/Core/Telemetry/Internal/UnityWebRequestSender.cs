using System;
using Unity.Services.Core.Internal;
using UnityEngine;
using UnityEngine.Networking;

namespace Unity.Services.Core.Telemetry.Internal
{
	internal class UnityWebRequestSender : IUnityWebRequestSender
	{
		public void SendRequest(UnityWebRequest request, Action<WebRequest> callback)
		{
			request.SendWebRequest().completed += OnSendingRequestCompleted;
			void OnSendingRequestCompleted(UnityEngine.AsyncOperation operation)
			{
				try
				{
					using UnityWebRequest webRequest = ((UnityWebRequestAsyncOperation)operation).webRequest;
					if (callback != null)
					{
						WebRequest obj = Simplify(webRequest);
						callback(obj);
					}
				}
				catch (Exception e) when (TelemetryUtils.LogTelemetryException(e))
				{
				}
			}
		}

		private static WebRequest Simplify(UnityWebRequest webRequest)
		{
			WebRequest webRequest2 = default(WebRequest);
			webRequest2.ResponseCode = webRequest.responseCode;
			WebRequest result = webRequest2;
			if (webRequest.HasSucceeded())
			{
				result.Result = WebRequestResult.Success;
			}
			else
			{
				switch (webRequest.result)
				{
				case UnityWebRequest.Result.ConnectionError:
					result.Result = WebRequestResult.ConnectionError;
					break;
				case UnityWebRequest.Result.ProtocolError:
					result.Result = WebRequestResult.ProtocolError;
					break;
				default:
					result.Result = WebRequestResult.UnknownError;
					break;
				}
				result.ErrorMessage = webRequest.error;
				result.ErrorBody = webRequest.downloadHandler.text;
			}
			return result;
		}
	}
}
