using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LobbyKicked_t : ICallbackData
	{
		internal ulong SteamIDLobby;

		internal ulong SteamIDAdmin;

		internal byte KickedDueToDisconnect;

		public static int _datasize = Marshal.SizeOf(typeof(LobbyKicked_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.LobbyKicked;
	}
}
