using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct MusicPlayerSelectsPlaylistEntry_t : ICallbackData
	{
		internal int NID;

		public static int _datasize = Marshal.SizeOf(typeof(MusicPlayerSelectsPlaylistEntry_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.MusicPlayerSelectsPlaylistEntry;
	}
}
