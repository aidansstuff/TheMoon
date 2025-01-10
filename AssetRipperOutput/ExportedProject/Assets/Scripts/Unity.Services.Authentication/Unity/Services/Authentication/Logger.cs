using System;
using System.Diagnostics;
using UnityEngine;

namespace Unity.Services.Authentication
{
	internal static class Logger
	{
		private const string k_Tag = "[Authentication]";

		internal const string k_GlobalVerboseLoggingDefine = "ENABLE_UNITY_SERVICES_VERBOSE_LOGGING";

		internal const string k_AuthenticationVerboseLoggingDefine = "ENABLE_UNITY_AUTHENTICATION_VERBOSE_LOGGING";

		public static void Log(object message)
		{
			UnityEngine.Debug.unityLogger.Log("[Authentication]", message);
		}

		public static void LogWarning(object message)
		{
			UnityEngine.Debug.unityLogger.LogWarning("[Authentication]", message);
		}

		public static void LogError(object message)
		{
			UnityEngine.Debug.unityLogger.LogError("[Authentication]", message);
		}

		public static void LogException(Exception exception)
		{
			UnityEngine.Debug.unityLogger.Log(LogType.Exception, "[Authentication]", exception);
		}

		[Conditional("UNITY_ASSERTIONS")]
		public static void LogAssertion(object message)
		{
			UnityEngine.Debug.unityLogger.Log(LogType.Assert, "[Authentication]", message);
		}

		[Conditional("ENABLE_UNITY_SERVICES_VERBOSE_LOGGING")]
		[Conditional("ENABLE_UNITY_AUTHENTICATION_VERBOSE_LOGGING")]
		public static void LogVerbose(object message)
		{
			UnityEngine.Debug.unityLogger.Log("[Authentication]", message);
		}
	}
}
