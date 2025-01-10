using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct RemoteStorageGetPublishedItemVoteDetailsResult_t : ICallbackData
	{
		internal Result Result;

		internal PublishedFileId PublishedFileId;

		internal int VotesFor;

		internal int VotesAgainst;

		internal int Reports;

		internal float FScore;

		public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageGetPublishedItemVoteDetailsResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.RemoteStorageGetPublishedItemVoteDetailsResult;
	}
}
