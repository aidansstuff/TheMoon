using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct UserFavoriteItemsListChanged_t : ICallbackData
	{
		internal PublishedFileId PublishedFileId;

		internal Result Result;

		[MarshalAs(UnmanagedType.I1)]
		internal bool WasAddRequest;

		public static int _datasize = Marshal.SizeOf(typeof(UserFavoriteItemsListChanged_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.UserFavoriteItemsListChanged;
	}
}
