using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Environments.Internal;
using Unity.Services.Core.Scheduler.Internal;
using UnityEngine;

namespace Unity.Services.Core.Telemetry.Internal
{
	internal class DiagnosticsHandler : TelemetryHandler<DiagnosticsPayload, Diagnostic>
	{
		private class SendState
		{
			public DiagnosticsHandler Self;

			public CachedPayload<DiagnosticsPayload> Payload;
		}

		public DiagnosticsHandler(TelemetryConfig config, CachedPayload<DiagnosticsPayload> cache, IActionScheduler scheduler, ICachePersister<DiagnosticsPayload> cachePersister, TelemetrySender sender)
			: base(config, cache, scheduler, cachePersister, sender)
		{
		}

		internal override void SendPersistedCache(CachedPayload<DiagnosticsPayload> persistedCache)
		{
			Task task = m_Sender.SendAsync(persistedCache.Payload);
			m_CachePersister.Delete();
			task.ContinueWith(state: new SendState
			{
				Self = this,
				Payload = new CachedPayload<DiagnosticsPayload>
				{
					TimeOfOccurenceTicks = persistedCache.TimeOfOccurenceTicks,
					Payload = persistedCache.Payload
				}
			}, continuationAction: OnSendAsyncCompleted, continuationOptions: TaskContinuationOptions.ExecuteSynchronously);
		}

		private static void OnSendAsyncCompleted(Task sendOperation, object state)
		{
			try
			{
				if (!(state is SendState sendState))
				{
					throw new ArgumentException("The given state is invalid.");
				}
				switch (sendOperation.Status)
				{
				case TaskStatus.Canceled:
				case TaskStatus.Faulted:
					sendState.Self.ThreadSafeCache(sendState.Payload);
					break;
				default:
					throw new ArgumentOutOfRangeException("Status", "Can't continue without the send operation being completed.");
				case TaskStatus.RanToCompletion:
					break;
				}
			}
			catch (Exception e) when (TelemetryUtils.LogTelemetryException(e))
			{
			}
		}

		private void ThreadSafeCache(CachedPayload<DiagnosticsPayload> payload)
		{
			lock (base.Lock)
			{
				base.Cache.AddRangeFrom(payload);
			}
		}

		internal override void FetchSpecificCommonTags(ICloudProjectId cloudProjectId, IEnvironments environments)
		{
			Dictionary<string, string> diagnosticsCommonTags = base.Cache.Payload.DiagnosticsCommonTags;
			diagnosticsCommonTags.Clear();
			diagnosticsCommonTags["application_version"] = Application.version;
			diagnosticsCommonTags["product_name"] = Application.productName;
			diagnosticsCommonTags["cloud_project_id"] = cloudProjectId.GetCloudProjectId();
			diagnosticsCommonTags["environment_name"] = environments.Current;
			diagnosticsCommonTags["application_genuine"] = (Application.genuineCheckAvailable ? Application.genuine.ToString(CultureInfo.InvariantCulture) : "unavailable");
			diagnosticsCommonTags["internet_reachability"] = Application.internetReachability.ToString();
		}

		internal override void SendCachedPayload()
		{
			if (!base.Cache.IsEmpty())
			{
				Task task = m_Sender.SendAsync(base.Cache.Payload);
				SendState state = new SendState
				{
					Self = this,
					Payload = new CachedPayload<DiagnosticsPayload>
					{
						TimeOfOccurenceTicks = base.Cache.TimeOfOccurenceTicks,
						Payload = new DiagnosticsPayload
						{
							Diagnostics = new List<Diagnostic>(base.Cache.Payload.Diagnostics),
							CommonTags = new Dictionary<string, string>(base.Cache.Payload.CommonTags),
							DiagnosticsCommonTags = new Dictionary<string, string>(base.Cache.Payload.DiagnosticsCommonTags)
						}
					}
				};
				base.Cache.TimeOfOccurenceTicks = 0L;
				base.Cache.Payload.Diagnostics.Clear();
				if (m_CachePersister.CanPersist)
				{
					m_CachePersister.Delete();
				}
				task.ContinueWith(OnSendAsyncCompleted, state, TaskContinuationOptions.ExecuteSynchronously);
			}
		}
	}
}
