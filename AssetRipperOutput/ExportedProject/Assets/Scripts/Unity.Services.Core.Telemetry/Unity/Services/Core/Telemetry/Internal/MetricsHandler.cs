using System;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Utilities;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Environments.Internal;
using Unity.Services.Core.Scheduler.Internal;

namespace Unity.Services.Core.Telemetry.Internal
{
	internal class MetricsHandler : TelemetryHandler<MetricsPayload, Metric>
	{
		public MetricsHandler(TelemetryConfig config, CachedPayload<MetricsPayload> cache, IActionScheduler scheduler, ICachePersister<MetricsPayload> cachePersister, TelemetrySender sender)
			: base(config, cache, scheduler, cachePersister, sender)
		{
			AotHelper.EnsureType<StringEnumConverter>();
		}

		internal override void SendPersistedCache(CachedPayload<MetricsPayload> persistedCache)
		{
			if (!AreMetricsOutdated())
			{
				m_Sender.SendAsync(persistedCache.Payload);
			}
			m_CachePersister.Delete();
			bool AreMetricsOutdated()
			{
				return (DateTime.UtcNow - new DateTime(persistedCache.TimeOfOccurenceTicks)).TotalSeconds > base.Config.PayloadExpirationSeconds;
			}
		}

		internal override void FetchSpecificCommonTags(ICloudProjectId cloudProjectId, IEnvironments environments)
		{
			base.Cache.Payload.MetricsCommonTags.Clear();
		}

		internal override void SendCachedPayload()
		{
			if (base.Cache.Payload.Metrics.Count > 0)
			{
				m_Sender.SendAsync(base.Cache.Payload);
				base.Cache.Payload.Metrics.Clear();
				base.Cache.TimeOfOccurenceTicks = 0L;
				if (m_CachePersister.CanPersist)
				{
					m_CachePersister.Delete();
				}
			}
		}
	}
}
