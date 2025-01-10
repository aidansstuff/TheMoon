namespace Unity.Services.Authentication
{
	internal interface IAuthenticationMetrics
	{
		void SendNetworkErrorMetric();

		void SendExpiredSessionMetric();

		void SendClientInvalidStateExceptionMetric();

		void SendUnlinkExternalIdNotFoundExceptionMetric();

		void SendClientSessionTokenNotExistsExceptionMetric();
	}
}
