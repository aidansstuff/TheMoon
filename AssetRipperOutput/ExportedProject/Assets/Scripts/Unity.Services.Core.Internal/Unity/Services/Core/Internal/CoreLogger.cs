using System;
using System.Diagnostics;
using UnityEngine;

namespace Unity.Services.Core.Internal
{
	internal static class CoreLogger
	{
		internal const string Tag = "[ServicesCore]";

		internal const string VerboseLoggingDefine = "ENABLE_UNITY_SERVICES_CORE_VERBOSE_LOGGING";

		private const string k_TelemetryLoggingDefine = "ENABLE_UNITY_SERVICES_CORE_TELEMETRY_LOGGING";

		public static void Log(object message)
		{
			UnityEngine.Debug.unityLogger.Log("[ServicesCore]", message);
		}

		public static void LogWarning(object message)
		{
			UnityEngine.Debug.unityLogger.LogWarning("[ServicesCore]", message);
		}

		public static void LogError(object message)
		{
			UnityEngine.Debug.unityLogger.LogError("[ServicesCore]", message);
		}

		public static void LogException(Exception exception)
		{
			UnityEngine.Debug.unityLogger.Log(LogType.Exception, "[ServicesCore]", exception);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void LogAssertion(object message)
		{
			UnityEngine.Debug.unityLogger.Log(LogType.Assert, "[ServicesCore]", message);
		}

		[Conditional("ENABLE_UNITY_SERVICES_CORE_VERBOSE_LOGGING")]
		public static void LogVerbose(object message)
		{
			UnityEngine.Debug.unityLogger.Log("[ServicesCore]", message);
		}

		[Conditional("ENABLE_UNITY_SERVICES_CORE_TELEMETRY_LOGGING")]
		public static void LogTelemetry(object message)
		{
			UnityEngine.Debug.unityLogger.Log("[ServicesCore]", message);
		}
	}
}
