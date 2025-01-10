using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct SteamNetAuthenticationStatus_t : ICallbackData
	{
		internal SteamNetworkingAvailability Avail;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 256)]
		internal byte[] DebugMsg;

		public static int _datasize = Marshal.SizeOf(typeof(SteamNetAuthenticationStatus_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.SteamNetAuthenticationStatus;

		internal string DebugMsgUTF8()
		{
			return Encoding.UTF8.GetString(DebugMsg, 0, Array.IndexOf(DebugMsg, (byte)0));
		}
	}
}
