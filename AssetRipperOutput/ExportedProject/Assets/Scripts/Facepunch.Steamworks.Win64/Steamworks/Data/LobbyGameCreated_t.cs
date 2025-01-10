using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LobbyGameCreated_t : ICallbackData
	{
		internal ulong SteamIDLobby;

		internal ulong SteamIDGameServer;

		internal uint IP;

		internal ushort Port;

		public static int _datasize = Marshal.SizeOf(typeof(LobbyGameCreated_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.LobbyGameCreated;
	}
}
