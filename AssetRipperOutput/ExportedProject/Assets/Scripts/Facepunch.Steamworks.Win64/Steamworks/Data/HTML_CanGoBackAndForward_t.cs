using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct HTML_CanGoBackAndForward_t : ICallbackData
	{
		internal uint UnBrowserHandle;

		[MarshalAs(UnmanagedType.I1)]
		internal bool BCanGoBack;

		[MarshalAs(UnmanagedType.I1)]
		internal bool BCanGoForward;

		public static int _datasize = Marshal.SizeOf(typeof(HTML_CanGoBackAndForward_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.HTML_CanGoBackAndForward;
	}
}
