using System;
using System.Collections.Concurrent;
using System.Text;
using System.Threading.Tasks;
using Steamworks.Data;

namespace Steamworks
{
	public class SteamNetworkingUtils : SteamSharedClass<SteamNetworkingUtils>
	{
		private struct DebugMessage
		{
			public NetDebugOutput Type;

			public string Msg;
		}

		private static NetDebugOutput _debugLevel;

		private static NetDebugFunc _debugFunc;

		private static ConcurrentQueue<DebugMessage> debugMessages = new ConcurrentQueue<DebugMessage>();

		internal static ISteamNetworkingUtils Internal => SteamSharedClass<SteamNetworkingUtils>.Interface as ISteamNetworkingUtils;

		public static SteamNetworkingAvailability Status { get; private set; }

		public static NetPingLocation? LocalPingLocation
		{
			get
			{
				NetPingLocation result = default(NetPingLocation);
				float localPingLocation = Internal.GetLocalPingLocation(ref result);
				if (localPingLocation < 0f)
				{
					return null;
				}
				return result;
			}
		}

		public static long LocalTimestamp => Internal.GetLocalTimestamp();

		public static float FakeSendPacketLoss
		{
			get
			{
				return GetConfigFloat(NetConfig.FakePacketLoss_Send);
			}
			set
			{
				SetConfigFloat(NetConfig.FakePacketLoss_Send, value);
			}
		}

		public static float FakeRecvPacketLoss
		{
			get
			{
				return GetConfigFloat(NetConfig.FakePacketLoss_Recv);
			}
			set
			{
				SetConfigFloat(NetConfig.FakePacketLoss_Recv, value);
			}
		}

		public static float FakeSendPacketLag
		{
			get
			{
				return GetConfigFloat(NetConfig.FakePacketLag_Send);
			}
			set
			{
				SetConfigFloat(NetConfig.FakePacketLag_Send, value);
			}
		}

		public static float FakeRecvPacketLag
		{
			get
			{
				return GetConfigFloat(NetConfig.FakePacketLag_Recv);
			}
			set
			{
				SetConfigFloat(NetConfig.FakePacketLag_Recv, value);
			}
		}

		public static int ConnectionTimeout
		{
			get
			{
				return GetConfigInt(NetConfig.TimeoutInitial);
			}
			set
			{
				SetConfigInt(NetConfig.TimeoutInitial, value);
			}
		}

		public static int Timeout
		{
			get
			{
				return GetConfigInt(NetConfig.TimeoutConnected);
			}
			set
			{
				SetConfigInt(NetConfig.TimeoutConnected, value);
			}
		}

		public static int SendBufferSize
		{
			get
			{
				return GetConfigInt(NetConfig.SendBufferSize);
			}
			set
			{
				SetConfigInt(NetConfig.SendBufferSize, value);
			}
		}

		public static NetDebugOutput DebugLevel
		{
			get
			{
				return _debugLevel;
			}
			set
			{
				_debugLevel = value;
				_debugFunc = OnDebugMessage;
				Internal.SetDebugOutputFunction(value, _debugFunc);
			}
		}

		public static event Action<NetDebugOutput, string> OnDebugOutput;

		internal override void InitializeInterface(bool server)
		{
			SetInterface(server, new ISteamNetworkingUtils(server));
			InstallCallbacks(server);
		}

		private static void InstallCallbacks(bool server)
		{
			Dispatch.Install(delegate(SteamRelayNetworkStatus_t x)
			{
				Status = x.Avail;
			}, server);
		}

		public static void InitRelayNetworkAccess()
		{
			Internal.InitRelayNetworkAccess();
		}

		public static int EstimatePingTo(NetPingLocation target)
		{
			return Internal.EstimatePingTimeFromLocalHost(ref target);
		}

		public static async Task WaitForPingDataAsync(float maxAgeInSeconds = 300f)
		{
			if (!Internal.CheckPingDataUpToDate(maxAgeInSeconds))
			{
				SteamRelayNetworkStatus_t status = default(SteamRelayNetworkStatus_t);
				while (Internal.GetRelayNetworkStatus(ref status) != SteamNetworkingAvailability.Current)
				{
					await Task.Delay(10);
				}
			}
		}

		[MonoPInvokeCallback]
		private static void OnDebugMessage(NetDebugOutput nType, IntPtr str)
		{
			debugMessages.Enqueue(new DebugMessage
			{
				Type = nType,
				Msg = Helpers.MemoryToString(str)
			});
		}

		internal static void OutputDebugMessages()
		{
			if (!debugMessages.IsEmpty)
			{
				DebugMessage result;
				while (debugMessages.TryDequeue(out result))
				{
					SteamNetworkingUtils.OnDebugOutput?.Invoke(result.Type, result.Msg);
				}
			}
		}

		internal unsafe static bool SetConfigInt(NetConfig type, int value)
		{
			int* ptr = &value;
			return Internal.SetConfigValue(type, NetConfigScope.Global, IntPtr.Zero, NetConfigType.Int32, (IntPtr)ptr);
		}

		internal unsafe static int GetConfigInt(NetConfig type)
		{
			int result = 0;
			NetConfigType pOutDataType = NetConfigType.Int32;
			int* ptr = &result;
			UIntPtr cbResult = new UIntPtr(4u);
			NetConfigResult configValue = Internal.GetConfigValue(type, NetConfigScope.Global, IntPtr.Zero, ref pOutDataType, (IntPtr)ptr, ref cbResult);
			if (configValue != NetConfigResult.OK)
			{
				return 0;
			}
			return result;
		}

		internal unsafe static bool SetConfigFloat(NetConfig type, float value)
		{
			float* ptr = &value;
			return Internal.SetConfigValue(type, NetConfigScope.Global, IntPtr.Zero, NetConfigType.Float, (IntPtr)ptr);
		}

		internal unsafe static float GetConfigFloat(NetConfig type)
		{
			float result = 0f;
			NetConfigType pOutDataType = NetConfigType.Float;
			float* ptr = &result;
			UIntPtr cbResult = new UIntPtr(4u);
			NetConfigResult configValue = Internal.GetConfigValue(type, NetConfigScope.Global, IntPtr.Zero, ref pOutDataType, (IntPtr)ptr, ref cbResult);
			if (configValue != NetConfigResult.OK)
			{
				return 0f;
			}
			return result;
		}

		internal unsafe static bool SetConfigString(NetConfig type, string value)
		{
			fixed (byte* ptr = Encoding.UTF8.GetBytes(value))
			{
				return Internal.SetConfigValue(type, NetConfigScope.Global, IntPtr.Zero, NetConfigType.String, (IntPtr)ptr);
			}
		}
	}
}
