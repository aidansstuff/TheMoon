using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct GetUserItemVoteResult_t : ICallbackData
	{
		internal PublishedFileId PublishedFileId;

		internal Result Result;

		[MarshalAs(UnmanagedType.I1)]
		internal bool VotedUp;

		[MarshalAs(UnmanagedType.I1)]
		internal bool VotedDown;

		[MarshalAs(UnmanagedType.I1)]
		internal bool VoteSkipped;

		public static int _datasize = Marshal.SizeOf(typeof(GetUserItemVoteResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.GetUserItemVoteResult;
	}
}
