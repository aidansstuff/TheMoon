using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct ReservationNotificationCallback_t : ICallbackData
	{
		internal ulong BeaconID;

		internal ulong SteamIDJoiner;

		public static int _datasize = Marshal.SizeOf(typeof(ReservationNotificationCallback_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.ReservationNotificationCallback;
	}
}
