using System;
using System.Collections.Generic;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Environments.Internal;
using Unity.Services.Core.Scheduler.Internal;
using UnityEngine;

namespace Unity.Services.Core.Telemetry.Internal
{
	internal abstract class TelemetryHandler
	{
		internal static string FormatOperatingSystemInfo(string rawOsInfo)
		{
			return rawOsInfo;
		}
	}
	internal abstract class TelemetryHandler<TPayload, TEvent> : TelemetryHandler where TPayload : ITelemetryPayload where TEvent : ITelemetryEvent
	{
		private readonly IActionScheduler m_Scheduler;

		protected readonly ICachePersister<TPayload> m_CachePersister;

		protected readonly TelemetrySender m_Sender;

		internal long SendingLoopScheduleId;

		internal long PersistenceLoopScheduleId;

		public TelemetryConfig Config { get; }

		public CachedPayload<TPayload> Cache { get; }

		protected object Lock { get; } = new object();


		protected TelemetryHandler(TelemetryConfig config, CachedPayload<TPayload> cache, IActionScheduler scheduler, ICachePersister<TPayload> cachePersister, TelemetrySender sender)
		{
			Config = config;
			Cache = cache;
			m_Scheduler = scheduler;
			m_CachePersister = cachePersister;
			m_Sender = sender;
		}

		public void Initialize(ICloudProjectId cloudProjectId, IEnvironments environments)
		{
			HandlePersistedCache();
			FetchAllCommonTags(cloudProjectId, environments);
			ScheduleSendingLoop();
			if (m_CachePersister.CanPersist)
			{
				SchedulePersistenceLoop();
			}
		}

		internal void HandlePersistedCache()
		{
			lock (Lock)
			{
				if (m_CachePersister.CanPersist && m_CachePersister.TryFetch(out var persistedCache))
				{
					if (persistedCache.IsEmpty())
					{
						m_CachePersister.Delete();
					}
					else
					{
						SendPersistedCache(persistedCache);
					}
				}
			}
		}

		internal abstract void SendPersistedCache(CachedPayload<TPayload> persistedCache);

		private void FetchAllCommonTags(ICloudProjectId cloudProjectId, IEnvironments environments)
		{
			FetchTelemetryCommonTags();
			FetchSpecificCommonTags(cloudProjectId, environments);
		}

		internal abstract void FetchSpecificCommonTags(ICloudProjectId cloudProjectId, IEnvironments environments);

		internal void FetchTelemetryCommonTags()
		{
			Dictionary<string, string> commonTags = Cache.Payload.CommonTags;
			commonTags.Clear();
			commonTags["application_install_mode"] = Application.installMode.ToString();
			commonTags["operating_system"] = TelemetryHandler.FormatOperatingSystemInfo(SystemInfo.operatingSystem);
			commonTags["platform"] = Application.platform.ToString();
			commonTags["engine"] = "Unity";
			commonTags["unity_version"] = Application.unityVersion;
		}

		internal void ScheduleSendingLoop()
		{
			try
			{
				SendingLoopScheduleId = m_Scheduler.ScheduleAction(SendingLoop, Config.PayloadSendingMaxIntervalSeconds);
			}
			catch (Exception e) when (TelemetryUtils.LogTelemetryException(e))
			{
			}
			void SendingLoop()
			{
				ScheduleSendingLoop();
				lock (Lock)
				{
					try
					{
						SendCachedPayload();
					}
					catch (Exception e2) when (TelemetryUtils.LogTelemetryException(e2))
					{
					}
				}
			}
		}

		internal abstract void SendCachedPayload();

		internal void SchedulePersistenceLoop()
		{
			try
			{
				PersistenceLoopScheduleId = m_Scheduler.ScheduleAction(PersistenceLoop, Config.SafetyPersistenceIntervalSeconds);
			}
			catch (Exception e) when (TelemetryUtils.LogTelemetryException(e))
			{
			}
			void PersistenceLoop()
			{
				SchedulePersistenceLoop();
				try
				{
					PersistCache();
				}
				catch (Exception e2) when (TelemetryUtils.LogTelemetryException(e2))
				{
				}
			}
		}

		internal void PersistCache()
		{
			lock (Lock)
			{
				if (m_CachePersister.CanPersist && Cache.TimeOfOccurenceTicks > 0 && Cache.Payload.Count > 0)
				{
					m_CachePersister.Persist(Cache);
				}
			}
		}

		public void Register(TEvent telemetryEvent)
		{
			try
			{
				lock (Lock)
				{
					Cache.Add(telemetryEvent);
					if (!IsCacheFull())
					{
						return;
					}
					SendCachedPayload();
				}
				m_Scheduler.CancelAction(SendingLoopScheduleId);
				ScheduleSendingLoop();
			}
			catch (Exception e) when (TelemetryUtils.LogTelemetryException(e))
			{
			}
			bool IsCacheFull()
			{
				return Cache.Payload.Count >= Config.MaxMetricCountPerPayload;
			}
		}
	}
}
