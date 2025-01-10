using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct SearchForGameProgressCallback_t : ICallbackData
	{
		internal ulong LSearchID;

		internal Result Result;

		internal ulong LobbyID;

		internal ulong SteamIDEndedSearch;

		internal int SecondsRemainingEstimate;

		internal int CPlayersSearching;

		public static int _datasize = Marshal.SizeOf(typeof(SearchForGameProgressCallback_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.SearchForGameProgressCallback;
	}
}
