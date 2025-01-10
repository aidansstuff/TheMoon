using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct RemoteStorageFileShareResult_t : ICallbackData
	{
		internal Result Result;

		internal ulong File;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 260)]
		internal byte[] Filename;

		public static int _datasize = Marshal.SizeOf(typeof(RemoteStorageFileShareResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.RemoteStorageFileShareResult;

		internal string FilenameUTF8()
		{
			return Encoding.UTF8.GetString(Filename, 0, Array.IndexOf(Filename, (byte)0));
		}
	}
}
