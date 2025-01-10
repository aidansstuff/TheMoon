using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct GSClientGroupStatus_t : ICallbackData
	{
		internal ulong SteamIDUser;

		internal ulong SteamIDGroup;

		[MarshalAs(UnmanagedType.I1)]
		internal bool Member;

		[MarshalAs(UnmanagedType.I1)]
		internal bool Officer;

		public static int _datasize = Marshal.SizeOf(typeof(GSClientGroupStatus_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.GSClientGroupStatus;
	}
}
