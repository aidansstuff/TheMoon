using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct BroadcastUploadStop_t : ICallbackData
	{
		internal BroadcastUploadResult Result;

		public static int _datasize = Marshal.SizeOf(typeof(BroadcastUploadStop_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.BroadcastUploadStop;
	}
}
