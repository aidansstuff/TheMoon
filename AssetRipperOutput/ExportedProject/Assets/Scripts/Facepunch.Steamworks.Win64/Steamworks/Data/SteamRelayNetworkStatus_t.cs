using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SteamRelayNetworkStatus_t : ICallbackData
	{
		internal SteamNetworkingAvailability Avail;

		internal int PingMeasurementInProgress;

		internal SteamNetworkingAvailability AvailNetworkConfig;

		internal SteamNetworkingAvailability AvailAnyRelay;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		internal byte[] DebugMsg;

		public static int _datasize = Marshal.SizeOf(typeof(SteamRelayNetworkStatus_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.SteamRelayNetworkStatus;

		internal string DebugMsgUTF8()
		{
			return Encoding.UTF8.GetString(DebugMsg, 0, Array.IndexOf(DebugMsg, (byte)0));
		}
	}
}
