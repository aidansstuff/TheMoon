using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LobbyEnter_t : ICallbackData
	{
		internal ulong SteamIDLobby;

		internal uint GfChatPermissions;

		[MarshalAs(UnmanagedType.I1)]
		internal bool Locked;

		internal uint EChatRoomEnterResponse;

		public static int _datasize = Marshal.SizeOf(typeof(LobbyEnter_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.LobbyEnter;
	}
}
