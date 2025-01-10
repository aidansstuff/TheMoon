using Unity.Services.Core.Telemetry.Internal;

namespace Unity.Services.Authentication
{
	internal class AuthenticationMetrics : IAuthenticationMetrics
	{
		private const string k_PackageName = "com.unity.services.authentication";

		private const string k_NetworkErrorKey = "network_error_event";

		private const string k_ExpiredSessionKey = "expired_session_event";

		private const string k_ClientInvalidStateExceptionKey = "client_invalid_state_exception_event";

		private const string k_UnlinkExternalIdNotFoundExceptionKey = "unlink_external_id_not_found_exception_event";

		private const string k_ClientSessionTokenNotExistsExceptionKey = "client_session_token_not_exists_exception_event";

		private readonly IMetrics m_Metrics;

		internal AuthenticationMetrics(IMetricsFactory metricsFactory)
		{
			m_Metrics = metricsFactory.Create("com.unity.services.authentication");
		}

		public void SendNetworkErrorMetric()
		{
			m_Metrics.SendSumMetric("network_error_event");
		}

		public void SendExpiredSessionMetric()
		{
			m_Metrics.SendSumMetric("expired_session_event");
		}

		public void SendClientInvalidStateExceptionMetric()
		{
			m_Metrics.SendSumMetric("client_invalid_state_exception_event");
		}

		public void SendUnlinkExternalIdNotFoundExceptionMetric()
		{
			m_Metrics.SendSumMetric("unlink_external_id_not_found_exception_event");
		}

		public void SendClientSessionTokenNotExistsExceptionMetric()
		{
			m_Metrics.SendSumMetric("client_session_token_not_exists_exception_event");
		}
	}
}
