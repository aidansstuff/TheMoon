using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct SearchForGameResultCallback_t : ICallbackData
	{
		internal ulong LSearchID;

		internal Result Result;

		internal int CountPlayersInGame;

		internal int CountAcceptedGame;

		internal ulong SteamIDHost;

		[MarshalAs(UnmanagedType.I1)]
		internal bool FinalCallback;

		public static int _datasize = Marshal.SizeOf(typeof(SearchForGameResultCallback_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.SearchForGameResultCallback;
	}
}
