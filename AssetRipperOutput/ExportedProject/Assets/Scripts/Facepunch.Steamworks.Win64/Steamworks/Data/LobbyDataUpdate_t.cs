using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LobbyDataUpdate_t : ICallbackData
	{
		internal ulong SteamIDLobby;

		internal ulong SteamIDMember;

		internal byte Success;

		public static int _datasize = Marshal.SizeOf(typeof(LobbyDataUpdate_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.LobbyDataUpdate;
	}
}
