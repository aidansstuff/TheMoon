using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct HTML_CloseBrowser_t : ICallbackData
	{
		internal uint UnBrowserHandle;

		public static int _datasize = Marshal.SizeOf(typeof(HTML_CloseBrowser_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.HTML_CloseBrowser;
	}
}
