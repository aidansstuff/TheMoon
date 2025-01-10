using System.Runtime.InteropServices;

namespace Steamworks.Data
{
	[StructLayout(LayoutKind.Sequential, Pack = 8)]
	internal struct FileDetailsResult_t : ICallbackData
	{
		internal Result Result;

		internal ulong FileSize;

		[MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
		internal byte[] FileSHA;

		internal uint Flags;

		public static int _datasize = Marshal.SizeOf(typeof(FileDetailsResult_t));

		public int DataSize => _datasize;

		public CallbackType CallbackType => CallbackType.FileDetailsResult;
	}
}
