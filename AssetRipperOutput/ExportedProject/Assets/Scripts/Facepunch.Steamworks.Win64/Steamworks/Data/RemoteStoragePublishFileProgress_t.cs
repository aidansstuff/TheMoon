using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct RemoteStoragePublishFileProgress_t : ICallbackData
	{
		internal double DPercentFile;

		[MarshalAs(UnmanagedType.I1)]
		internal bool Preview;

		public static int _datasize = Marshal.SizeOf(typeof(RemoteStoragePublishFileProgress_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.RemoteStoragePublishFileProgress;
	}
}
