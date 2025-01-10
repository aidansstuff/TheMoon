using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct HTML_FileOpenDialog_t : ICallbackData
	{
		internal uint UnBrowserHandle;

		internal string PchTitle;

		internal string PchInitialFile;

		public static int _datasize = Marshal.SizeOf(typeof(HTML_FileOpenDialog_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.HTML_FileOpenDialog;
	}
}
