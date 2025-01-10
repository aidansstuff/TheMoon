using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct RemoteStorageDownloadUGCResult_t : ICallbackData
	{
		internal Result Result;

		internal ulong File;

		internal AppId AppID;

		internal int SizeInBytes;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
		internal byte[] PchFileName;

		internal ulong SteamIDOwner;

		public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageDownloadUGCResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.RemoteStorageDownloadUGCResult;

		internal string PchFileNameUTF8()
		{
			return Encoding.UTF8.GetString(PchFileName, 0, Array.IndexOf(PchFileName, (byte)0));
		}
	}
}
