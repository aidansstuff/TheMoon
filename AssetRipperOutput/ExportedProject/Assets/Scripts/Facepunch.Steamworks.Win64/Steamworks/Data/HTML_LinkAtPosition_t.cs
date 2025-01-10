using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct HTML_LinkAtPosition_t : ICallbackData
	{
		internal uint UnBrowserHandle;

		internal uint X;

		internal uint Y;

		internal string PchURL;

		[MarshalAs(UnmanagedType.I1)]
		internal bool BInput;

		[MarshalAs(UnmanagedType.I1)]
		internal bool BLiveLink;

		public static int _datasize = Marshal.SizeOf(typeof(HTML_LinkAtPosition_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.HTML_LinkAtPosition;
	}
}
