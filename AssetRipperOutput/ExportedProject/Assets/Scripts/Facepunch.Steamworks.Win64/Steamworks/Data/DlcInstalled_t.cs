using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct DlcInstalled_t : ICallbackData
	{
		internal AppId AppID;

		public static int _datasize = Marshal.SizeOf(typeof(DlcInstalled_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.DlcInstalled;
	}
}
