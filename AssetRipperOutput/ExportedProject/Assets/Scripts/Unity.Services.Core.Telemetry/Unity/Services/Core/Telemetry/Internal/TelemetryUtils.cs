using System;
using System.Collections.Generic;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Environments.Internal;
using Unity.Services.Core.Internal;
using Unity.Services.Core.Internal.Serialization;
using Unity.Services.Core.Scheduler.Internal;
using UnityEngine;

namespace Unity.Services.Core.Telemetry.Internal
{
	internal static class TelemetryUtils
	{
		internal const string TelemetryDisabledKey = "com.unity.services.core.telemetry-disabled";

		public static IMetricsFactory CreateMetricsFactory(IActionScheduler scheduler, IProjectConfiguration projectConfiguration, ICloudProjectId cloudProjectId, IEnvironments environments)
		{
			if (IsTelemetryDisabled(projectConfiguration))
			{
				return new DisabledMetricsFactory();
			}
			TelemetryConfig telemetryConfig = CreateTelemetryConfig(projectConfiguration);
			CachedPayload<MetricsPayload> cache = new CachedPayload<MetricsPayload>
			{
				Payload = new MetricsPayload
				{
					Metrics = new List<Metric>(),
					CommonTags = new Dictionary<string, string>(),
					MetricsCommonTags = new Dictionary<string, string>()
				}
			};
			ICachePersister<MetricsPayload> cachePersister = CreateCachePersister<MetricsPayload>("UnityServicesCachedMetrics", Application.platform);
			ExponentialBackOffRetryPolicy retryPolicy = new ExponentialBackOffRetryPolicy();
			UnityWebRequestSender requestSender = new UnityWebRequestSender();
			TelemetrySender sender = new TelemetrySender(telemetryConfig.TargetUrl, telemetryConfig.ServicePath, scheduler, retryPolicy, requestSender, new NewtonsoftSerializer());
			MetricsHandler metricsHandler = new MetricsHandler(telemetryConfig, cache, scheduler, cachePersister, sender);
			metricsHandler.Initialize(cloudProjectId, environments);
			return new MetricsFactory(metricsHandler, projectConfiguration);
		}

		public static IDiagnosticsFactory CreateDiagnosticsFactory(IActionScheduler scheduler, IProjectConfiguration projectConfiguration, ICloudProjectId cloudProjectId, IEnvironments environments)
		{
			if (IsTelemetryDisabled(projectConfiguration))
			{
				return new DisabledDiagnosticsFactory();
			}
			TelemetryConfig telemetryConfig = CreateTelemetryConfig(projectConfiguration);
			CachedPayload<DiagnosticsPayload> cache = new CachedPayload<DiagnosticsPayload>
			{
				Payload = new DiagnosticsPayload
				{
					Diagnostics = new List<Diagnostic>(),
					CommonTags = new Dictionary<string, string>(),
					DiagnosticsCommonTags = new Dictionary<string, string>()
				}
			};
			ICachePersister<DiagnosticsPayload> cachePersister = CreateCachePersister<DiagnosticsPayload>("UnityServicesCachedDiagnostics", Application.platform);
			ExponentialBackOffRetryPolicy retryPolicy = new ExponentialBackOffRetryPolicy();
			UnityWebRequestSender requestSender = new UnityWebRequestSender();
			TelemetrySender sender = new TelemetrySender(telemetryConfig.TargetUrl, telemetryConfig.ServicePath, scheduler, retryPolicy, requestSender, new NewtonsoftSerializer());
			DiagnosticsHandler diagnosticsHandler = new DiagnosticsHandler(telemetryConfig, cache, scheduler, cachePersister, sender);
			diagnosticsHandler.Initialize(cloudProjectId, environments);
			return new DiagnosticsFactory(diagnosticsHandler, projectConfiguration);
		}

		private static bool IsTelemetryDisabled(IProjectConfiguration projectConfiguration)
		{
			return projectConfiguration.GetBool("com.unity.services.core.telemetry-disabled");
		}

		internal static ICachePersister<TPayload> CreateCachePersister<TPayload>(string fileName, RuntimePlatform platform) where TPayload : ITelemetryPayload
		{
			if (platform == RuntimePlatform.Switch)
			{
				return new DisabledCachePersister<TPayload>();
			}
			return new FileCachePersister<TPayload>(fileName, new NewtonsoftSerializer(), CoreDiagnostics.Instance);
		}

		internal static TelemetryConfig CreateTelemetryConfig(IProjectConfiguration projectConfiguration)
		{
			return new TelemetryConfig
			{
				TargetUrl = projectConfiguration.GetString("com.unity.services.core.telemetry-target-url", "https://operate-sdk-telemetry.unity3d.com"),
				ServicePath = projectConfiguration.GetString("com.unity.services.core.telemetry-service-path", "v1/record"),
				PayloadExpirationSeconds = projectConfiguration.GetInt("com.unity.services.core.telemetry-payload-expiration-seconds", 3600),
				PayloadSendingMaxIntervalSeconds = projectConfiguration.GetInt("com.unity.services.core.telemetry-payload-sending-max-interval-seconds", 600),
				SafetyPersistenceIntervalSeconds = projectConfiguration.GetInt("com.unity.services.core.telemetry-safety-persistence-interval-seconds", 300),
				MaxMetricCountPerPayload = Math.Min(295, projectConfiguration.GetInt("com.unity.services.core.telemetry-max-metric-count-per-payload", 295))
			};
		}

		public static bool LogTelemetryException(Exception e, bool predicateValue = false)
		{
			return predicateValue;
		}
	}
}
