using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct GameConnectedFriendChatMsg_t : ICallbackData
	{
		internal ulong SteamIDUser;

		internal int MessageID;

		public static int _datasize = Marshal.SizeOf(typeof(GameConnectedFriendChatMsg_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.GameConnectedFriendChatMsg;
	}
}
