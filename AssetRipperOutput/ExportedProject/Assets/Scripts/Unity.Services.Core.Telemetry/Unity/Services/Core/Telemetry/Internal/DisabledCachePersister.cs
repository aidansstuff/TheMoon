using System;

namespace Unity.Services.Core.Telemetry.Internal
{
	internal class DisabledCachePersister<TPayload> : ICachePersister<TPayload> where TPayload : ITelemetryPayload
	{
		private const string k_ErrorMessage = "Cache persistence isn't supported on the current platform.";

		public bool CanPersist => false;

		public void Persist(CachedPayload<TPayload> cache)
		{
			throw new NotSupportedException("Cache persistence isn't supported on the current platform.");
		}

		public bool TryFetch(out CachedPayload<TPayload> persistedCache)
		{
			throw new NotSupportedException("Cache persistence isn't supported on the current platform.");
		}

		public void Delete()
		{
			throw new NotSupportedException("Cache persistence isn't supported on the current platform.");
		}
	}
}
