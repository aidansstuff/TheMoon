using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct ActiveBeaconsUpdated_t : ICallbackData
	{
		public static int _datasize = Marshal.SizeOf(typeof(ActiveBeaconsUpdated_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.ActiveBeaconsUpdated;
	}
}
