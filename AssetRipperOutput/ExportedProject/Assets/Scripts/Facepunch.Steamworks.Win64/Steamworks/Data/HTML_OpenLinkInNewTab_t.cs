using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct HTML_OpenLinkInNewTab_t : ICallbackData
	{
		internal uint UnBrowserHandle;

		internal string PchURL;

		public static int _datasize = Marshal.SizeOf(typeof(HTML_OpenLinkInNewTab_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.HTML_OpenLinkInNewTab;
	}
}
