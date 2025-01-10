using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct HTML_FinishedRequest_t : ICallbackData
	{
		internal uint UnBrowserHandle;

		internal string PchURL;

		internal string PchPageTitle;

		public static int _datasize = Marshal.SizeOf(typeof(HTML_FinishedRequest_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.HTML_FinishedRequest;
	}
}
