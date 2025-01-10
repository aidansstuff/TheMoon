using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct GameConnectedChatLeave_t : ICallbackData
	{
		internal ulong SteamIDClanChat;

		internal ulong SteamIDUser;

		[MarshalAs(UnmanagedType.I1)]
		internal bool Kicked;

		[MarshalAs(UnmanagedType.I1)]
		internal bool Dropped;

		public static int _datasize = Marshal.SizeOf(typeof(GameConnectedChatLeave_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.GameConnectedChatLeave;
	}
}
