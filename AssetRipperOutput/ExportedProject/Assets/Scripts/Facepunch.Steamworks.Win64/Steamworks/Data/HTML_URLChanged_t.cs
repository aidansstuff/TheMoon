using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct HTML_URLChanged_t : ICallbackData
	{
		internal uint UnBrowserHandle;

		internal string PchURL;

		internal string PchPostData;

		[MarshalAs(UnmanagedType.I1)]
		internal bool BIsRedirect;

		internal string PchPageTitle;

		[MarshalAs(UnmanagedType.I1)]
		internal bool BNewNavigation;

		public static int _datasize = Marshal.SizeOf(typeof(HTML_URLChanged_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.HTML_URLChanged;
	}
}
