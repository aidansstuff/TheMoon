using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct GameLobbyJoinRequested_t : ICallbackData
	{
		internal ulong SteamIDLobby;

		internal ulong SteamIDFriend;

		public static int _datasize = Marshal.SizeOf(typeof(GameLobbyJoinRequested_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.GameLobbyJoinRequested;
	}
}
