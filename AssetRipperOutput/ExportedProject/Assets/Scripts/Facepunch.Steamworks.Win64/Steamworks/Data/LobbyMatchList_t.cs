using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct LobbyMatchList_t : ICallbackData
	{
		internal uint LobbiesMatching;

		public static int _datasize = Marshal.SizeOf(typeof(LobbyMatchList_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.LobbyMatchList;
	}
}
