using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct GSClientKick_t : ICallbackData
	{
		internal ulong SteamID;

		internal DenyReason DenyReason;

		public static int _datasize = Marshal.SizeOf(typeof(GSClientKick_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.GSClientKick;
	}
}
