using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct HTML_JSConfirm_t : ICallbackData
	{
		internal uint UnBrowserHandle;

		internal string PchMessage;

		public static int _datasize = Marshal.SizeOf(typeof(HTML_JSConfirm_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.HTML_JSConfirm;
	}
}
