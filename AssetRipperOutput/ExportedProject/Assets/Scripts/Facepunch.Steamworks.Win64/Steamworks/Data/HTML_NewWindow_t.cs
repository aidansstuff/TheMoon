using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct HTML_NewWindow_t : ICallbackData
	{
		internal uint UnBrowserHandle;

		internal string PchURL;

		internal uint UnX;

		internal uint UnY;

		internal uint UnWide;

		internal uint UnTall;

		internal uint UnNewWindow_BrowserHandle_IGNORE;

		public static int _datasize = Marshal.SizeOf(typeof(HTML_NewWindow_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.HTML_NewWindow;
	}
}
