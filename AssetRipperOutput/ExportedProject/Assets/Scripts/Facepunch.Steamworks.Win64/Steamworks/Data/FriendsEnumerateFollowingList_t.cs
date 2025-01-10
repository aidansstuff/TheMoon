using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 4)]
	internal struct FriendsEnumerateFollowingList_t : ICallbackData
	{
		internal Result Result;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 50, ArraySubType = UnmanagedType.U8)]
		internal ulong[] GSteamID;

		internal int ResultsReturned;

		internal int TotalResultCount;

		public static int _datasize = Marshal.SizeOf(typeof(FriendsEnumerateFollowingList_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.FriendsEnumerateFollowingList;
	}
}
