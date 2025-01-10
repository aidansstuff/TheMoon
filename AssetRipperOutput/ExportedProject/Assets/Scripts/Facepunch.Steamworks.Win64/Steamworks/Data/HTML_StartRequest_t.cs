using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct HTML_StartRequest_t : ICallbackData
	{
		internal uint UnBrowserHandle;

		internal string PchURL;

		internal string PchTarget;

		internal string PchPostData;

		[MarshalAs(UnmanagedType.I1)]
		internal bool BIsRedirect;

		public static int _datasize = Marshal.SizeOf(typeof(HTML_StartRequest_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.HTML_StartRequest;
	}
}
