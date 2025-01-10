using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct ScreenshotReady_t : ICallbackData
	{
		internal uint Local;

		internal Result Result;

		public static int _datasize = Marshal.SizeOf(typeof(ScreenshotReady_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.ScreenshotReady;
	}
}
