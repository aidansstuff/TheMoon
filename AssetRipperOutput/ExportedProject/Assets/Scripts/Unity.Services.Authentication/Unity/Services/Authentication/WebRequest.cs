using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine.Networking;

namespace Unity.Services.Authentication
{
	internal class WebRequest
	{
		private readonly WebRequestVerb m_Verb;

		private readonly string m_Url;

		private readonly IDictionary<string, string> m_Headers;

		private readonly string m_Payload;

		private readonly string m_PayloadContentType;

		private readonly JsonSerializerSettings m_JsonSerializerSettings = new JsonSerializerSettings
		{
			NullValueHandling = NullValueHandling.Ignore
		};

		internal INetworkConfiguration Configuration { get; }

		internal int Retries { get; private set; }

		internal WebRequest(INetworkConfiguration configuration, WebRequestVerb verb, string url, IDictionary<string, string> headers, string payload, string payloadContentType)
		{
			Configuration = configuration;
			m_Verb = verb;
			m_Url = url;
			m_Headers = headers;
			m_Payload = payload;
			m_PayloadContentType = payloadContentType;
		}

		internal Task SendAsync()
		{
			return SendAttemptAsync(new TaskCompletionSource<string>());
		}

		internal async Task<T> SendAsync<T>()
		{
			string value = await SendAttemptAsync(new TaskCompletionSource<string>());
			if (string.IsNullOrEmpty(value))
			{
				return default(T);
			}
			try
			{
				return IsolatedJsonConvert.DeserializeObject<T>(value, m_JsonSerializerSettings);
			}
			catch (Exception ex)
			{
				string text = "Failed to deserialize object!";
				Logger.Log(text + " " + ex.Message);
				throw new WebRequestException(isNetworkError: false, isServerError: false, isDeserializationError: true, 0L, text);
			}
		}

		private Task<string> SendAttemptAsync(TaskCompletionSource<string> tcs)
		{
			try
			{
				UnityWebRequest request = Build();
				request.SendWebRequest().completed += delegate
				{
					RequestCompleted(tcs, request.responseCode, RequestHasNetworkError(request), RequestHasServerError(request), request.error, request.downloadHandler?.text, request.GetResponseHeaders());
					request.Dispose();
				};
			}
			catch (Exception exception)
			{
				tcs.SetException(exception);
			}
			return tcs.Task;
		}

		internal UnityWebRequest Build()
		{
			UnityWebRequest unityWebRequest;
			switch (m_Verb)
			{
			case WebRequestVerb.Post:
				unityWebRequest = ((!string.IsNullOrEmpty(m_Payload)) ? new UnityWebRequest(m_Url, "POST")
				{
					uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(m_Payload)),
					downloadHandler = new DownloadHandlerBuffer()
				} : UnityWebRequest.PostWwwForm(m_Url, string.Empty));
				break;
			case WebRequestVerb.Get:
				unityWebRequest = UnityWebRequest.Get(m_Url);
				break;
			case WebRequestVerb.Put:
				if (string.IsNullOrEmpty(m_Payload))
				{
					throw new ArgumentException("PUT payload cannot be empty.");
				}
				unityWebRequest = new UnityWebRequest(m_Url, "PUT")
				{
					uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(m_Payload)),
					downloadHandler = new DownloadHandlerBuffer()
				};
				break;
			case WebRequestVerb.Delete:
				unityWebRequest = UnityWebRequest.Delete(m_Url);
				unityWebRequest.downloadHandler = new DownloadHandlerBuffer();
				break;
			default:
				throw new ArgumentException("Unknown verb " + m_Verb);
			}
			if (!string.IsNullOrEmpty(m_PayloadContentType))
			{
				unityWebRequest.SetRequestHeader("Content-Type", m_PayloadContentType);
			}
			if (m_Headers != null)
			{
				foreach (KeyValuePair<string, string> header in m_Headers)
				{
					unityWebRequest.SetRequestHeader(header.Key, header.Value);
				}
			}
			unityWebRequest.timeout = Configuration.Timeout;
			return unityWebRequest;
		}

		internal void RequestCompleted(TaskCompletionSource<string> tcs, long responseCode, bool isNetworkError, bool isServerError, string errorText, string bodyText, IDictionary<string, string> headers)
		{
			if (isNetworkError && Retries < Configuration.Retries)
			{
				Logger.LogWarning("Network error detected, retrying...");
				Retries++;
				SendAttemptAsync(tcs);
			}
			else if (isNetworkError || isServerError)
			{
				string text = ((isServerError && !string.IsNullOrEmpty(bodyText)) ? bodyText : errorText);
				WebRequestException exception = new WebRequestException(isNetworkError, isServerError, isDeserializationError: false, responseCode, text, headers);
				tcs.SetException(exception);
				Logger.LogWarning("Request completed with error: " + text);
			}
			else
			{
				tcs.SetResult(bodyText);
			}
		}

		private bool RequestHasServerError(UnityWebRequest request)
		{
			return request.responseCode >= 400;
		}

		private bool RequestHasNetworkError(UnityWebRequest request)
		{
			if (request.result == UnityWebRequest.Result.ConnectionError)
			{
				return request.error != "Redirect limit exceeded";
			}
			return false;
		}
	}
}
