using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct HTML_NeedsPaint_t : ICallbackData
	{
		internal uint UnBrowserHandle;

		internal string PBGRA;

		internal uint UnWide;

		internal uint UnTall;

		internal uint UnUpdateX;

		internal uint UnUpdateY;

		internal uint UnUpdateWide;

		internal uint UnUpdateTall;

		internal uint UnScrollX;

		internal uint UnScrollY;

		internal float FlPageScale;

		internal uint UnPageSerial;

		public static int _datasize = Marshal.SizeOf(typeof(HTML_NeedsPaint_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.HTML_NeedsPaint;
	}
}
