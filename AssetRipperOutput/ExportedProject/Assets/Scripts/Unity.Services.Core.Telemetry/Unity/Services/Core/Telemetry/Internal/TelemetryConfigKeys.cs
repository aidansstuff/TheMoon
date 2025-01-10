namespace Unity.Services.Core.Telemetry.Internal
{
	internal static class TelemetryConfigKeys
	{
		private const string k_BaseKey = "com.unity.services.core.telemetry-";

		public const string TargetUrl = "com.unity.services.core.telemetry-target-url";

		public const string ServicePath = "com.unity.services.core.telemetry-service-path";

		public const string PayloadExpirationSeconds = "com.unity.services.core.telemetry-payload-expiration-seconds";

		public const string PayloadSendingMaxIntervalSeconds = "com.unity.services.core.telemetry-payload-sending-max-interval-seconds";

		public const string SafetyPersistenceIntervalSeconds = "com.unity.services.core.telemetry-safety-persistence-interval-seconds";

		public const string MaxMetricCountPerPayload = "com.unity.services.core.telemetry-max-metric-count-per-payload";
	}
}
