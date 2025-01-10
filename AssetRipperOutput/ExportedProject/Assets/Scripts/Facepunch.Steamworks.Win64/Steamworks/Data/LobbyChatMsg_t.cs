using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LobbyChatMsg_t : ICallbackData
	{
		internal ulong SteamIDLobby;

		internal ulong SteamIDUser;

		internal byte ChatEntryType;

		internal uint ChatID;

		public static int _datasize = Marshal.SizeOf(typeof(LobbyChatMsg_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.LobbyChatMsg;
	}
}
