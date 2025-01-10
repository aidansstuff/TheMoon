using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct GameConnectedClanChatMsg_t : ICallbackData
	{
		internal ulong SteamIDClanChat;

		internal ulong SteamIDUser;

		internal int MessageID;

		public static int _datasize = Marshal.SizeOf(typeof(GameConnectedClanChatMsg_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.GameConnectedClanChatMsg;
	}
}
