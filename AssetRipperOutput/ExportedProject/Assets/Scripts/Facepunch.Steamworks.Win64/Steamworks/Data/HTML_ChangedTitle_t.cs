using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct HTML_ChangedTitle_t : ICallbackData
	{
		internal uint UnBrowserHandle;

		internal string PchTitle;

		public static int _datasize = Marshal.SizeOf(typeof(HTML_ChangedTitle_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.HTML_ChangedTitle;
	}
}
