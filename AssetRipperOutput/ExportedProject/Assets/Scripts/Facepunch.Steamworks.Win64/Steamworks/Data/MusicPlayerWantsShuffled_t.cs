using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct MusicPlayerWantsShuffled_t : ICallbackData
	{
		[MarshalAs(UnmanagedType.I1)]
		internal bool Shuffled;

		public static int _datasize = Marshal.SizeOf(typeof(MusicPlayerWantsShuffled_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.MusicPlayerWantsShuffled;
	}
}
