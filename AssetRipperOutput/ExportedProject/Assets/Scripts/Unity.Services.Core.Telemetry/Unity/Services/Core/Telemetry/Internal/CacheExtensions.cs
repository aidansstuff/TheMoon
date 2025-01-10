using System;

namespace Unity.Services.Core.Telemetry.Internal
{
	internal static class CacheExtensions
	{
		public static bool IsEmpty<TPayload>(this CachedPayload<TPayload> self) where TPayload : ITelemetryPayload
		{
			return (self?.Payload?.Count).GetValueOrDefault() <= 0;
		}

		public static void AddRangeFrom(this CachedPayload<DiagnosticsPayload> self, CachedPayload<DiagnosticsPayload> payload)
		{
			if ((payload?.Payload.Diagnostics?.Count).GetValueOrDefault() > 0)
			{
				self.Payload.Diagnostics.AddRange(payload.Payload.Diagnostics);
				if (self.TimeOfOccurenceTicks <= 0)
				{
					self.TimeOfOccurenceTicks = payload.TimeOfOccurenceTicks;
				}
			}
		}

		public static void Add<TPayload>(this CachedPayload<TPayload> self, ITelemetryEvent telemetryEvent) where TPayload : ITelemetryPayload
		{
			if (self.TimeOfOccurenceTicks == 0L)
			{
				self.TimeOfOccurenceTicks = DateTime.UtcNow.Ticks;
			}
			self.Payload.Add(telemetryEvent);
		}
	}
}
