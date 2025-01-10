using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct AddUGCDependencyResult_t : ICallbackData
	{
		internal Result Result;

		internal PublishedFileId PublishedFileId;

		internal PublishedFileId ChildPublishedFileId;

		public static int _datasize = Marshal.SizeOf(typeof(AddUGCDependencyResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.AddUGCDependencyResult;
	}
}
