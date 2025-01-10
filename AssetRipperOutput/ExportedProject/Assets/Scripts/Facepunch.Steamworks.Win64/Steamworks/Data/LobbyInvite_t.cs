using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LobbyInvite_t : ICallbackData
	{
		internal ulong SteamIDUser;

		internal ulong SteamIDLobby;

		internal ulong GameID;

		public static int _datasize = Marshal.SizeOf(typeof(LobbyInvite_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.LobbyInvite;
	}
}
