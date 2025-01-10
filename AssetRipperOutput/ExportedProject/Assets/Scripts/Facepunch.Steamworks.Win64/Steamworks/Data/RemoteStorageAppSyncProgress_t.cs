using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct RemoteStorageAppSyncProgress_t : ICallbackData
	{
		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
		internal byte[] CurrentFile;

		internal AppId AppID;

		internal uint BytesTransferredThisChunk;

		internal double DAppPercentComplete;

		[MarshalAs(UnmanagedType.I1)]
		internal bool Uploading;

		public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageAppSyncProgress_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.RemoteStorageAppSyncProgress;

		internal string CurrentFileUTF8()
		{
			return Encoding.UTF8.GetString(CurrentFile, 0, Array.IndexOf(CurrentFile, (byte)0));
		}
	}
}
