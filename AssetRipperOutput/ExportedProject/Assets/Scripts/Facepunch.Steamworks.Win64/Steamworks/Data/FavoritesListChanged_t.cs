using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct FavoritesListChanged_t : ICallbackData
	{
		internal uint IP;

		internal uint QueryPort;

		internal uint ConnPort;

		internal uint AppID;

		internal uint Flags;

		[MarshalAs(UnmanagedType.I1)]
		internal bool Add;

		internal uint AccountId;

		public static int _datasize = Marshal.SizeOf(typeof(FavoritesListChanged_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.FavoritesListChanged;
	}
}
