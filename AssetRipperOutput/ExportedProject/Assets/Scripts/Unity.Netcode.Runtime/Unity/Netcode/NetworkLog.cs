using UnityEngine;

namespace Unity.Netcode
{
	public static class NetworkLog
	{
		internal enum LogType : byte
		{
			Info = 0,
			Warning = 1,
			Error = 2,
			None = 3
		}

		internal static NetworkManager NetworkManagerOverride;

		public static LogLevel CurrentLogLevel
		{
			get
			{
				if (!(NetworkManager.Singleton == null))
				{
					return NetworkManager.Singleton.LogLevel;
				}
				return LogLevel.Normal;
			}
		}

		public static void LogInfo(string message)
		{
			Debug.Log("[Netcode] " + message);
		}

		public static void LogWarning(string message)
		{
			Debug.LogWarning("[Netcode] " + message);
		}

		public static void LogError(string message)
		{
			Debug.LogError("[Netcode] " + message);
		}

		public static void LogInfoServer(string message)
		{
			LogServer(message, LogType.Info);
		}

		public static void LogWarningServer(string message)
		{
			LogServer(message, LogType.Warning);
		}

		public static void LogErrorServer(string message)
		{
			LogServer(message, LogType.Error);
		}

		private static void LogServer(string message, LogType logType)
		{
			NetworkManager networkManager = NetworkManagerOverride ?? (NetworkManagerOverride = NetworkManager.Singleton);
			ulong sender = networkManager?.LocalClientId ?? 0;
			bool flag = networkManager?.IsServer ?? true;
			switch (logType)
			{
			case LogType.Info:
				if (flag)
				{
					LogInfoServerLocal(message, sender);
				}
				else
				{
					LogInfo(message);
				}
				break;
			case LogType.Warning:
				if (flag)
				{
					LogWarningServerLocal(message, sender);
				}
				else
				{
					LogWarning(message);
				}
				break;
			case LogType.Error:
				if (flag)
				{
					LogErrorServerLocal(message, sender);
				}
				else
				{
					LogError(message);
				}
				break;
			}
			if (!flag && networkManager.NetworkConfig.EnableNetworkLogs)
			{
				ServerLogMessage serverLogMessage = default(ServerLogMessage);
				serverLogMessage.LogType = logType;
				serverLogMessage.Message = message;
				ServerLogMessage message2 = serverLogMessage;
				int num = networkManager.ConnectionManager.SendMessage(ref message2, NetworkDelivery.ReliableFragmentedSequenced, 0uL);
				networkManager.NetworkMetrics.TrackServerLogSent(0uL, (uint)logType, num);
			}
		}

		internal static void LogInfoServerLocal(string message, ulong sender)
		{
			Debug.Log($"[Netcode-Server Sender={sender}] {message}");
		}

		internal static void LogWarningServerLocal(string message, ulong sender)
		{
			Debug.LogWarning($"[Netcode-Server Sender={sender}] {message}");
		}

		internal static void LogErrorServerLocal(string message, ulong sender)
		{
			Debug.LogError($"[Netcode-Server Sender={sender}] {message}");
		}
	}
}
