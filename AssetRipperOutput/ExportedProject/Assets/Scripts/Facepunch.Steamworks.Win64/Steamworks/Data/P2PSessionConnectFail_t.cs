using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct P2PSessionConnectFail_t : ICallbackData
	{
		internal ulong SteamIDRemote;

		internal byte P2PSessionError;

		public static int _datasize = Marshal.SizeOf(typeof(P2PSessionConnectFail_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.P2PSessionConnectFail;
	}
}
