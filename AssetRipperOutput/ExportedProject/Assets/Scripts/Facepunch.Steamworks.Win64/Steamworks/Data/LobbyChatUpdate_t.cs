using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LobbyChatUpdate_t : ICallbackData
	{
		internal ulong SteamIDLobby;

		internal ulong SteamIDUserChanged;

		internal ulong SteamIDMakingChange;

		internal uint GfChatMemberStateChange;

		public static int _datasize = Marshal.SizeOf(typeof(LobbyChatUpdate_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.LobbyChatUpdate;
	}
}
