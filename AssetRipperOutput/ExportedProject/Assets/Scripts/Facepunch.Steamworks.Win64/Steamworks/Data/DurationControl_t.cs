using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct DurationControl_t : ICallbackData
	{
		internal Result Result;

		internal AppId Appid;

		[MarshalAs(UnmanagedType.I1)]
		internal bool Applicable;

		internal int CsecsLast5h;

		internal DurationControlProgress Progress;

		internal DurationControlNotification Otification;

		internal int CsecsToday;

		internal int CsecsRemaining;

		public static int _datasize = Marshal.SizeOf(typeof(DurationControl_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.DurationControl;
	}
}
