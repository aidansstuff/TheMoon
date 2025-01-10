using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct HTML_HorizontalScroll_t : ICallbackData
	{
		internal uint UnBrowserHandle;

		internal uint UnScrollMax;

		internal uint UnScrollCurrent;

		internal float FlPageScale;

		[MarshalAs(UnmanagedType.I1)]
		internal bool BVisible;

		internal uint UnPageSize;

		public static int _datasize = Marshal.SizeOf(typeof(HTML_HorizontalScroll_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.HTML_HorizontalScroll;
	}
}
