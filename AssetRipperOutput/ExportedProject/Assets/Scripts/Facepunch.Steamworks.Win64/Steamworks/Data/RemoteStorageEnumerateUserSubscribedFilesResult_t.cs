using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct RemoteStorageEnumerateUserSubscribedFilesResult_t : ICallbackData
	{
		internal Result Result;

		internal int ResultsReturned;

		internal int TotalResultCount;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 50, ArraySubType = UnmanagedType.U8)]
		internal PublishedFileId[] GPublishedFileId;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 50, ArraySubType = UnmanagedType.U4)]
		internal uint[] GRTimeSubscribed;

		public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageEnumerateUserSubscribedFilesResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.RemoteStorageEnumerateUserSubscribedFilesResult;
	}
}
