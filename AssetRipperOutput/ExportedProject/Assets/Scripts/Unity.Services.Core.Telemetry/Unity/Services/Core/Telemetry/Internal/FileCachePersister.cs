using System;
using System.IO;
using Unity.Services.Core.Internal;
using Unity.Services.Core.Internal.Serialization;
using UnityEngine;

namespace Unity.Services.Core.Telemetry.Internal
{
	internal abstract class FileCachePersister
	{
		internal static bool IsAvailableFor(RuntimePlatform platform)
		{
			return !string.IsNullOrEmpty(GetPersistentDataPathFor(platform));
		}

		internal static string GetPersistentDataPathFor(RuntimePlatform platform)
		{
			if (platform == RuntimePlatform.Switch)
			{
				return string.Empty;
			}
			return Application.persistentDataPath;
		}
	}
	internal class FileCachePersister<TPayload> : FileCachePersister, ICachePersister<TPayload> where TPayload : ITelemetryPayload
	{
		private const string k_MultipleInstanceDiagnosticsName = "telemetry_cache_file_multiple_instances_exception";

		private const string k_CacheFileException = "telemetry_cache_file_exception";

		private const string k_MultipleInstanceError = "This exception is most likely caused by a multiple instance file sharing violation.";

		private readonly IJsonSerializer m_Serializer;

		private readonly CoreDiagnostics m_Diagnostics;

		public string FilePath { get; }

		public bool CanPersist { get; } = FileCachePersister.IsAvailableFor(Application.platform);


		public FileCachePersister(string fileName, IJsonSerializer serializer, CoreDiagnostics diagnostics)
		{
			FilePath = Path.Combine(FileCachePersister.GetPersistentDataPathFor(Application.platform), fileName);
			m_Serializer = serializer;
			m_Diagnostics = diagnostics;
		}

		public void Persist(CachedPayload<TPayload> cache)
		{
			if (cache.IsEmpty())
			{
				return;
			}
			try
			{
				string contents = m_Serializer.SerializeObject(cache);
				File.WriteAllText(FilePath, contents);
			}
			catch (IOException ex) when (TelemetryUtils.LogTelemetryException(ex))
			{
				IOException exception = new IOException("This exception is most likely caused by a multiple instance file sharing violation.", ex);
				m_Diagnostics.SendCoreDiagnosticsAsync("telemetry_cache_file_multiple_instances_exception", exception);
			}
			catch (Exception ex2) when (TelemetryUtils.LogTelemetryException(ex2, predicateValue: true))
			{
				m_Diagnostics.SendCoreDiagnosticsAsync("telemetry_cache_file_exception", ex2);
			}
		}

		public bool TryFetch(out CachedPayload<TPayload> persistedCache)
		{
			persistedCache = null;
			if (!File.Exists(FilePath))
			{
				return false;
			}
			try
			{
				string value = File.ReadAllText(FilePath);
				persistedCache = m_Serializer.DeserializeObject<CachedPayload<TPayload>>(value);
				return persistedCache != null;
			}
			catch (IOException innerException)
			{
				IOException exception = new IOException("This exception is most likely caused by a multiple instance file sharing violation.", innerException);
				m_Diagnostics.SendCoreDiagnosticsAsync("telemetry_cache_file_multiple_instances_exception", exception);
				return false;
			}
			catch (Exception ex) when (TelemetryUtils.LogTelemetryException(ex, predicateValue: true))
			{
				m_Diagnostics.SendCoreDiagnosticsAsync("telemetry_cache_file_exception", ex);
				return false;
			}
		}

		public void Delete()
		{
			if (!File.Exists(FilePath))
			{
				return;
			}
			try
			{
				File.Delete(FilePath);
			}
			catch (IOException innerException)
			{
				IOException exception = new IOException("This exception is most likely caused by a multiple instance file sharing violation.", innerException);
				m_Diagnostics.SendCoreDiagnosticsAsync("telemetry_cache_file_multiple_instances_exception", exception);
			}
			catch (Exception ex) when (TelemetryUtils.LogTelemetryException(ex, predicateValue: true))
			{
				m_Diagnostics.SendCoreDiagnosticsAsync("telemetry_cache_file_exception", ex);
			}
		}
	}
}
