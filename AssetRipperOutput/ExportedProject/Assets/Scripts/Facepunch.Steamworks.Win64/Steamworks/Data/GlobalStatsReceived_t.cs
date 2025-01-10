using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct GlobalStatsReceived_t : ICallbackData
	{
		internal ulong GameID;

		internal Result Result;

		public static int _datasize = Marshal.SizeOf(typeof(GlobalStatsReceived_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.GlobalStatsReceived;
	}
}
