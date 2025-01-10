using System;
using UnityEngine;

namespace Unity.Services.Core.Telemetry.Internal
{
	internal class ExponentialBackOffRetryPolicy
	{
		private int m_MaxTryCount = 10;

		private float m_BaseDelaySeconds = 2f;

		public int MaxTryCount
		{
			get
			{
				return m_MaxTryCount;
			}
			set
			{
				m_MaxTryCount = Math.Max(value, 0);
			}
		}

		public float BaseDelaySeconds
		{
			get
			{
				return m_BaseDelaySeconds;
			}
			set
			{
				m_BaseDelaySeconds = Math.Max(value, 0f);
			}
		}

		public bool CanRetry(WebRequest webRequest, int sendCount)
		{
			if (sendCount < MaxTryCount)
			{
				return IsTransientError(webRequest);
			}
			return false;
		}

		public static bool IsTransientError(WebRequest webRequest)
		{
			if (webRequest.Result != WebRequestResult.ConnectionError)
			{
				if (webRequest.Result == WebRequestResult.ProtocolError)
				{
					return IsServerErrorCode(webRequest.ResponseCode);
				}
				return false;
			}
			return true;
			static bool IsServerErrorCode(long responseCode)
			{
				if (responseCode >= 500)
				{
					return responseCode < 600;
				}
				return false;
			}
		}

		public float GetDelayBeforeSendingSeconds(int sendCount)
		{
			if (sendCount <= 0)
			{
				return BaseDelaySeconds;
			}
			return Mathf.Pow(BaseDelaySeconds, sendCount);
		}
	}
}
