using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct HTML_BrowserRestarted_t : ICallbackData
	{
		internal uint UnBrowserHandle;

		internal uint UnOldBrowserHandle;

		public static int _datasize = Marshal.SizeOf(typeof(HTML_BrowserRestarted_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.HTML_BrowserRestarted;
	}
}
